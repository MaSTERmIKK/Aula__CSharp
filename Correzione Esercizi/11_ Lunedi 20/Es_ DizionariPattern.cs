using System;
using System.Collections.Generic;

// =========================
// DOMAIN & STORAGE (Singleton)
// =========================
public sealed class BankContext
{
    // Singleton thread-safe (lazy)
    private static readonly Lazy<BankContext> _lazy = new(() => new BankContext());
    public static BankContext Instance => _lazy.Value;

    // "DB" in memoria con dizionari
    public readonly Dictionary<int, Cliente> Clienti = new();
    public readonly Dictionary<int, IConto> Conti = new();
    public readonly Dictionary<int, List<Operazione>> OperazioniPerConto = new();

    // Generatori ID incrementali
    private int _nextClienteId = 1;
    private int _nextContoId = 100;

    // Configurazioni globali
    public string Valuta { get; set; } = "EUR";

    // Strategy attiva (calcolo interessi/commissioni)
    public ICalcoloInteressi Strategy { get; private set; } = new StandardStrategy();

    // Observer: elenco sottoscrittori
    private readonly List<IObserver> _observers = new();

    private BankContext() { } // costruttore privato

    // ==== API di "sistema" ====
    public int NuovoClienteId() => _nextClienteId++;
    public int NuovoContoId() => _nextContoId++;

    public void SetStrategy(ICalcoloInteressi strategy)
    {
        Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        NotifyAll(new EventoSistema(EventType.StrategyChanged, $"Strategy attiva: {strategy.GetType().Name}"));
    }

    public void Subscribe(IObserver obs)
    {
        if (obs == null) return;
        _observers.Add(obs);
    }

    public void NotifyAll(EventoSistema ev)
    {
        foreach (var obs in _observers)
            obs.OnNotify(ev);
    }
}

// =========================
// ENTITÀ DI DOMINIO
// =========================
public class Cliente
{
    public int Id { get; }
    public string Nome { get; }
    public string Email { get; }

    public Cliente(int id, string nome, string email)
    {
        Id = id; Nome = nome; Email = email;
    }
}

public interface IConto
{
    int Id { get; }
    int ClienteId { get; }
    string Tipo { get; }
    decimal Saldo { get; }
    void Deposita(decimal importo);
    bool Preleva(decimal importo);
}

public abstract class ContoBaseAstratto : IConto
{
    public int Id { get; }
    public int ClienteId { get; }
    public string Tipo { get; protected set; }
    public decimal Saldo { get; protected set; }

    protected ContoBaseAstratto(int id, int clienteId, string tipo, decimal saldoIniziale = 0m)
    {
        Id = id; ClienteId = clienteId; Tipo = tipo; Saldo = saldoIniziale;
    }

    public virtual void Deposita(decimal importo)
    {
        if (importo <= 0) throw new ArgumentException("Importo non valido.");
        Saldo += importo;
    }

    public virtual bool Preleva(decimal importo)
    {
        if (importo <= 0) return false;
        if (Saldo < importo) return false;
        Saldo -= importo;
        return true;
    }
}

public class ContoBase : ContoBaseAstratto
{
    public ContoBase(int id, int clienteId) : base(id, clienteId, "Base") { }
}

public class ContoPremium : ContoBaseAstratto
{
    public ContoPremium(int id, int clienteId) : base(id, clienteId, "Premium") { }
}

public class ContoStudent : ContoBaseAstratto
{
    public ContoStudent(int id, int clienteId) : base(id, clienteId, "Student") { }
}

// =========================
// FACTORY (creazione conti)
// =========================
public static class ContoFactory
{
    public static IConto Crea(string tipo, int clienteId)
    {
        var ctx = BankContext.Instance;
        int id = ctx.NuovoContoId();

        return tipo.ToUpper() switch
        {
            "BASE" => new ContoBase(id, clienteId),
            "PREMIUM" => new ContoPremium(id, clienteId),
            "STUDENT" => new ContoStudent(id, clienteId),
            _ => throw new ArgumentException("Tipo conto non supportato.")
        };
    }
}

// =========================
// STRATEGY (interessi/commissioni)
// =========================
public interface ICalcoloInteressi
{
    // Ritorna la "variazione" (positiva o negativa) da applicare al saldo
    decimal CalcolaDeltaInteressi(IConto conto);
    // Commissione per trasferimento (fissa o percentuale, a discrezione)
    decimal CalcolaCommissioneTrasferimento(IConto from, decimal importo);
}

public class StandardStrategy : ICalcoloInteressi
{
    public decimal CalcolaDeltaInteressi(IConto conto)
    {
        // +0.1% su tutti i conti
        return Math.Round(conto.Saldo * 0.001m, 2);
    }
    public decimal CalcolaCommissioneTrasferimento(IConto from, decimal importo)
    {
        // commissione fissa 0.50
        return 0.50m;
    }
}

public class PromoStrategy : ICalcoloInteressi
{
    public decimal CalcolaDeltaInteressi(IConto conto)
    {
        // Base 0.15%, Premium 0.25%, Student 0.20%
        decimal rate = conto.Tipo switch
        {
            "Premium" => 0.0025m,
            "Student" => 0.0020m,
            _ => 0.0015m
        };
        return Math.Round(conto.Saldo * rate, 2);
    }

    public decimal CalcolaCommissioneTrasferimento(IConto from, decimal importo)
    {
        // Nessuna commissione sotto 100 EUR, altrimenti 0.3% minimo 0.20
        if (importo < 100m) return 0m;
        var c = Math.Round(importo * 0.003m, 2);
        return c < 0.20m ? 0.20m : c;
    }
}

// =========================
// OBSERVER (logging eventi)
// =========================
public enum EventType
{
    ClienteCreato,
    ContoCreato,
    Deposito,
    Prelievo,
    Trasferimento,
    StrategyChanged,
    InteressiApplicati,
    Errore
}

public record EventoSistema(EventType Tipo, string Messaggio);

public interface IObserver
{
    void OnNotify(EventoSistema ev);
}

public class ConsoleLogger : IObserver
{
    public void OnNotify(EventoSistema ev)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {ev.Tipo}: {ev.Messaggio}");
    }
}

// =========================
// OPERAZIONI & SERVICE
// =========================
public class Operazione
{
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public string Tipo { get; init; } = "";
    public decimal Importo { get; init; }
    public string Descrizione { get; init; } = "";
}

public static class BankService
{
    private static BankContext Ctx => BankContext.Instance;

    // --- CLIENTI ---
    public static Cliente CreaCliente(string nome, string email)
    {
        int id = Ctx.NuovoClienteId();
        var c = new Cliente(id, nome, email);
        Ctx.Clienti.Add(id, c);
        Ctx.NotifyAll(new EventoSistema(EventType.ClienteCreato, $"Cliente {nome} (Id {id})"));
        return c;
    }

    // --- CONTI ---
    public static IConto CreaConto(int clienteId, string tipo)
    {
        if (!Ctx.Clienti.ContainsKey(clienteId))
            throw new ArgumentException("Cliente inesistente.");

        var conto = ContoFactory.Crea(tipo, clienteId);
        Ctx.Conti.Add(conto.Id, conto);
        Ctx.OperazioniPerConto[conto.Id] = new List<Operazione>();
        Ctx.NotifyAll(new EventoSistema(EventType.ContoCreato, $"Conto {conto.Tipo} Id {conto.Id} per cliente {clienteId}"));
        return conto;
    }

    private static void AggiungiOperazione(int contoId, string tipo, decimal importo, string descr)
    {
        Ctx.OperazioniPerConto[contoId].Add(new Operazione
        {
            Tipo = tipo,
            Importo = importo,
            Descrizione = descr
        });
    }

    // --- MOVIMENTI ---
    public static void Deposita(int contoId, decimal importo)
    {
        if (!Ctx.Conti.TryGetValue(contoId, out var conto))
            throw new ArgumentException("Conto inesistente.");

        conto.Deposita(importo);
        AggiungiOperazione(contoId, "DEPOSITO", importo, "Versamento");
        Ctx.NotifyAll(new EventoSistema(EventType.Deposito, $"Conto {contoId}: +{importo:0.00} {Ctx.Valuta} (saldo {conto.Saldo:0.00})"));
    }

    public static bool Preleva(int contoId, decimal importo)
    {
        if (!Ctx.Conti.TryGetValue(contoId, out var conto))
            throw new ArgumentException("Conto inesistente.");

        bool ok = conto.Preleva(importo);
        if (ok)
        {
            AggiungiOperazione(contoId, "PRELIEVO", importo, "Prelievo");
            Ctx.NotifyAll(new EventoSistema(EventType.Prelievo, $"Conto {contoId}: -{importo:0.00} {Ctx.Valuta} (saldo {conto.Saldo:0.00})"));
        }
        else
        {
            Ctx.NotifyAll(new EventoSistema(EventType.Errore, $"Prelievo rifiutato su conto {contoId} (saldo insufficiente o importo non valido)"));
        }
        return ok;
    }

    public static bool Trasferisci(int fromId, int toId, decimal importo)
    {
        if (!Ctx.Conti.TryGetValue(fromId, out var from) || !Ctx.Conti.TryGetValue(toId, out var to))
            throw new ArgumentException("Conto sorgente/destinazione inesistente.");

        decimal commissione = Ctx.Strategy.CalcolaCommissioneTrasferimento(from, importo);
        decimal totale = importo + commissione;

        if (!from.Preleva(totale))
        {
            Ctx.NotifyAll(new EventoSistema(EventType.Errore, $"Trasferimento fallito: saldo insufficiente su {fromId}"));
            return false;
        }

        to.Deposita(importo);
        AggiungiOperazione(fromId, "TRASF_OUT", totale, $"A {toId} (incl. comm {commissione:0.00})");
        AggiungiOperazione(toId, "TRASF_IN", importo, $"Da {fromId}");

        Ctx.NotifyAll(new EventoSistema(EventType.Trasferimento,
            $"Da {fromId} a {toId}: {importo:0.00} (+comm {commissione:0.00})"));
        return true;
    }

    // --- INTERESSI ---
    public static void ApplicaInteressiATutti()
    {
        foreach (var kv in Ctx.Conti)
        {
            var conto = kv.Value;
            var delta = Ctx.Strategy.CalcolaDeltaInteressi(conto);
            if (delta != 0)
            {
                conto.Deposita(delta);
                AggiungiOperazione(conto.Id, "INTERESSI", delta, Ctx.Strategy.GetType().Name);
                Ctx.NotifyAll(new EventoSistema(EventType.InteressiApplicati,
                    $"Conto {conto.Id}: +{delta:0.00} {Ctx.Valuta} (saldo {conto.Saldo:0.00})"));
            }
        }
    }

    // --- CONSULTAZIONI ---
    public static void StampaOperazioniConto(int contoId)
    {
        if (!Ctx.OperazioniPerConto.ContainsKey(contoId))
        {
            Console.WriteLine($"Nessuna operazione per conto {contoId}");
            return;
        }

        Console.WriteLine($"\n== Operazioni conto {contoId} ==");
        foreach (var op in Ctx.OperazioniPerConto[contoId])
            Console.WriteLine($"{op.Timestamp:yyyy-MM-dd HH:mm:ss} | {op.Tipo,-12} | {op.Importo,8:0.00} | {op.Descrizione}");
    }

    public static void ReportBanca()
    {
        Console.WriteLine("\n== Report banca ==");
        decimal totale = 0m;
        foreach (var c in Ctx.Conti.Values)
        {
            totale += c.Saldo;
            Console.WriteLine($"Conto {c.Id} ({c.Tipo}) Cliente {c.ClienteId} -> Saldo: {c.Saldo:0.00} {Ctx.Valuta}");
        }
        Console.WriteLine($"Totale saldi: {totale:0.00} {Ctx.Valuta}");
    }
}

// =========================
// DEMO
// =========================
public class Program
{
    public static void Main()
    {
        var ctx = BankContext.Instance;
        ctx.Subscribe(new ConsoleLogger()); // observer

        // 1) Clienti
        var alice = BankService.CreaCliente("Alice", "alice@example.com");
        var bob   = BankService.CreaCliente("Bob",   "bob@example.com");
        var carol = BankService.CreaCliente("Carol", "carol@example.com");

        // 2) Conti (Factory)
        var c1 = BankService.CreaConto(alice.Id, "BASE");
        var c2 = BankService.CreaConto(bob.Id,   "PREMIUM");
        var c3 = BankService.CreaConto(carol.Id, "STUDENT");

        // 3) Movimenti base
        BankService.Deposita(c1.Id, 500m);
        BankService.Deposita(c2.Id, 1200m);
        BankService.Deposita(c3.Id, 300m);

        BankService.Prelieva(c1.Id, 100m);
        BankService.Trasferisci(c2.Id, c1.Id, 250m);

        // 4) Strategy: cambio e interessi
        ctx.SetStrategy(new PromoStrategy());
        BankService.ApplicaInteressiATutti();

        // 5) Consultazioni
        BankService.StampaOperazioniConto(c1.Id);
        BankService.StampaOperazioniConto(c2.Id);
        BankService.ReportBanca();

        Console.WriteLine("\nFine demo. Premi Invio per uscire.");
        Console.ReadLine();
    }
}
