using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartMobility
{
    // ==========================
    // INTERFACCE
    // ==========================

    /// <summary>
    /// Contratto per mezzi elettrici: espone stato batteria e ricarica.
    /// </summary>
    public interface IElettrico
    {
        int Batteria { get; }              // 0..100
        void Ricarica(int percento);       // +percento, clamp 0..100
        /// <summary>
        /// Consumo batteria in funzione della durata (minuti) di una corsa.
        /// L'implementazione concreta decide quanto consumare.
        /// </summary>
        void ConsumaBatteria(int minuti);
    }

    /// <summary>
    /// Contratto per mezzi pieghevoli.
    /// </summary>
    public interface IPieghevole
    {
        bool EPiegato { get; }
        void Piega();
        void Apri();
    }

    // ==========================
    // CLASSE ASTRATTA BASE
    // ==========================

    /// <summary>
    /// Classe base astratta per tutti i veicoli.
    /// Dimostra: incapsulamento (campi privati + proprietà), ereditarietà, polimorfismo su CalcolaCosto.
    /// </summary>
    public abstract class Veicolo
    {
        private int _id;
        private string _modello = "";
        private decimal _tariffaBaseAlMinuto;
        private bool _isDisponibile = true;

        public int Id
        {
            get => _id;
            protected set
            {
                if (value <= 0) throw new ArgumentException("Id deve essere positivo.");
                _id = value;
            }
        }

        public string Modello
        {
            get => _modello;
            protected set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Modello obbligatorio.");
                _modello = value.Trim();
            }
        }

        /// <summary>
        /// Tariffa per minuto > 0
        /// </summary>
        public decimal TariffaBaseAlMinuto
        {
            get => _tariffaBaseAlMinuto;
            protected set
            {
                if (value <= 0) throw new ArgumentException("TariffaBaseAlMinuto deve essere > 0.");
                _tariffaBaseAlMinuto = value;
            }
        }

        /// <summary>
        /// True se il veicolo è prenotabile/fermo.
        /// </summary>
        public bool IsDisponibile
        {
            get => _isDisponibile;
            protected set => _isDisponibile = value;
        }

        protected Veicolo(int id, string modello, decimal tariffaBaseAlMinuto)
        {
            Id = id;
            Modello = modello;
            TariffaBaseAlMinuto = tariffaBaseAlMinuto;
            IsDisponibile = true;
        }

        /// <summary>
        /// Polimorfismo: ogni veicolo definisce come si calcola il costo in base ai minuti.
        /// </summary>
        public abstract decimal CalcolaCosto(int minuti, bool condizioniSpeciali = false);

        /// <summary>
        /// Passa lo stato a "in uso".
        /// </summary>
        public virtual void AvviaCorsa()
        {
            if (!IsDisponibile) throw new InvalidOperationException("Veicolo già in uso.");
            IsDisponibile = false;
        }

        /// <summary>
        /// Torna disponibile alla fine della corsa. 
        /// Le sottoclassi/servizi possono agganciare logiche aggiuntive (consumi, fee, ecc.).
        /// </summary>
        public virtual void TerminaCorsa()
        {
            IsDisponibile = true;
        }

        public override string ToString()
        {
            return $"[{Id}] {GetType().Name} - \"{Modello}\" - {TariffaBaseAlMinuto:C}/min - {(IsDisponibile ? "Disponibile" : "In uso")}";
        }
    }

    // ==========================
    // SOTTOCLASSI CONCRETE
    // ==========================

    /// <summary>
    /// Monopattino elettrico: costo base + sovrapprezzo se batteria < 15%.
    /// Consumo: ~1% ogni 4 minuti (arrotondato per eccesso minimo 1% se minuti>0).
    /// </summary>
    public class MonopattinoElettrico : Veicolo, IElettrico
    {
        private int _batteria;

        public int Batteria => _batteria;

        public MonopattinoElettrico(int id, string modello, decimal tariffaBaseAlMinuto, int batteriaIniziale = 100)
            : base(id, modello, tariffaBaseAlMinuto)
        {
            _batteria = ClampBatteria(batteriaIniziale);
        }

        public override decimal CalcolaCosto(int minuti, bool condizioniSpeciali = false)
        {
            if (minuti <= 0) throw new ArgumentException("Minuti deve essere > 0.");
            decimal costo = TariffaBaseAlMinuto * minuti;

            // Sovrapprezzo batteria bassa
            if (Batteria < 15)
                costo += 1.50m; // fee emergenza

            // condizioniSpeciali non utilizzate qui, ma mantenuto per firma coerente
            return decimal.Round(costo, 2);
        }

        public void Ricarica(int percento)
        {
            if (percento <= 0) return;
            _batteria = ClampBatteria(_batteria + percento);
        }

        public void ConsumaBatteria(int minuti)
        {
            if (minuti <= 0) return;
            int consumo = Math.Max(1, (int)Math.Ceiling(minuti / 4.0)); // 1% ogni 4 minuti, minimo 1%
            _batteria = ClampBatteria(_batteria - consumo);
        }

        private static int ClampBatteria(int x) => Math.Max(0, Math.Min(100, x));

        public override string ToString() => base.ToString() + $" - Batteria: {Batteria}%";
    }

    /// <summary>
    /// Bici pieghevole: costo base, con sconto del 20% sui primi 10 minuti se PARTI piegato.
    /// </summary>
    public class BiciPieghevole : Veicolo, IPieghevole
    {
        public bool EPiegato { get; private set; }

        public BiciPieghevole(int id, string modello, decimal tariffaBaseAlMinuto, bool inizialmentePiegato = false)
            : base(id, modello, tariffaBaseAlMinuto)
        {
            EPiegato = inizialmentePiegato;
        }

        public override decimal CalcolaCosto(int minuti, bool partiPiegato)
        {
            if (minuti <= 0) throw new ArgumentException("Minuti deve essere > 0.");
            decimal costo = TariffaBaseAlMinuto * minuti;

            // Se la corsa parte con la bici piegata, sconto 20% sui primi 10 minuti
            if (partiPiegato)
            {
                int minutiScontati = Math.Min(10, minuti);
                decimal sconto = (TariffaBaseAlMinuto * minutiScontati) * 0.20m;
                costo -= sconto;
            }

            return decimal.Round(costo, 2);
        }

        public void Piega() => EPiegato = true;
        public void Apri() => EPiegato = false;

        public override string ToString() => base.ToString() + $" - {(EPiegato ? "Piegata" : "Aperta")}";
    }

    /// <summary>
    /// Scooter elettrico: costo base + fee avvio di 0,80 €; consumo batteria più alto.
    /// Consumo: ~1% ogni 2 minuti.
    /// </summary>
    public class ScooterElettrico : Veicolo, IElettrico
    {
        private int _batteria;
        public int Batteria => _batteria;

        public ScooterElettrico(int id, string modello, decimal tariffaBaseAlMinuto, int batteriaIniziale = 100)
            : base(id, modello, tariffaBaseAlMinuto)
        {
            _batteria = ClampBatteria(batteriaIniziale);
        }

        public override decimal CalcolaCosto(int minuti, bool condizioniSpeciali = false)
        {
            if (minuti <= 0) throw new ArgumentException("Minuti deve essere > 0.");
            decimal costo = TariffaBaseAlMinuto * minuti + 0.80m; // fee avvio

            if (Batteria < 15)
                costo += 2.00m; // fee emergenza maggiore

            return decimal.Round(costo, 2);
        }

        public void Ricarica(int percento)
        {
            if (percento <= 0) return;
            _batteria = ClampBatteria(_batteria + percento);
        }

        public void ConsumaBatteria(int minuti)
        {
            if (minuti <= 0) return;
            int consumo = Math.Max(1, (int)Math.Ceiling(minuti / 2.0)); // 1% ogni 2 minuti
            _batteria = ClampBatteria(_batteria - consumo);
        }

        private static int ClampBatteria(int x) => Math.Max(0, Math.Min(100, x));

        public override string ToString() => base.ToString() + $" - Batteria: {Batteria}%";
    }

    // ==========================
    // SERVICE E MODELLI DI DOMINIO
    // ==========================

    /// <summary>
    /// Rappresenta una corsa attiva o conclusa.
    /// </summary>
    public class Corsa
    {
        public int IdVeicolo { get; }
        public int MinutiPrevisti { get; }       // per semplicità, fissiamo a prenotazione
        public bool PartiPiegato { get; }        // utile per IPieghevole
        public bool Aperta { get; private set; } = true;
        public decimal? CostoFinale { get; private set; }

        public Corsa(int idVeicolo, int minutiPrevisti, bool partiPiegato)
        {
            if (minutiPrevisti <= 0) throw new ArgumentException("Minuti previsti deve essere > 0.");
            IdVeicolo = idVeicolo;
            MinutiPrevisti = minutiPrevisti;
            PartiPiegato = partiPiegato;
        }

        public void Chiudi(decimal costo)
        {
            Aperta = false;
            CostoFinale = decimal.Round(costo, 2);
        }
    }

    /// <summary>
    /// Gestisce il parco mezzi, prenotazioni e chiusure corse.
    /// </summary>
    public class NoleggioService
    {
        private readonly List<Veicolo> _parco = new();
        private readonly Dictionary<int, Corsa> _corseAttive = new(); // key: idVeicolo

        public IReadOnlyList<Veicolo> Parco => _parco;

        public void AggiungiVeicolo(Veicolo v)
        {
            if (v is null) throw new ArgumentNullException(nameof(v));
            if (_parco.Any(x => x.Id == v.Id))
                throw new InvalidOperationException($"Esiste già un veicolo con Id={v.Id}.");
            _parco.Add(v);
        }

        public IEnumerable<Veicolo> Disponibili() => _parco.Where(v => v.IsDisponibile);

        /// <summary>
        /// Prenota il veicolo e crea una corsa. 
        /// partiPiegato serve per applicare lo sconto su mezzi pieghevoli (stato iniziale).
        /// </summary>
        public void Prenota(int idVeicolo, int minuti, bool partiPiegato = false)
        {
            var v = _parco.FirstOrDefault(x => x.Id == idVeicolo)
                ?? throw new KeyNotFoundException($"Veicolo {idVeicolo} non trovato.");

            if (!v.IsDisponibile) throw new InvalidOperationException("Veicolo non disponibile.");

            // Se pieghevole e si parte piegati, assicurati di impostare lo stato all'avvio
            if (v is IPieghevole pieghevole)
            {
                if (partiPiegato) pieghevole.Piega(); else pieghevole.Apri();
            }

            v.AvviaCorsa();
            _corseAttive[idVeicolo] = new Corsa(idVeicolo, minuti, partiPiegato);
        }

        /// <summary>
        /// Termina la corsa, calcola costo polimorficamente, aggiorna batterie e stato.
        /// Ritorna il costo finale.
        /// </summary>
        public decimal Termina(int idVeicolo)
        {
            if (!_corseAttive.TryGetValue(idVeicolo, out var corsa))
                throw new InvalidOperationException("Nessuna corsa attiva per questo veicolo.");

            var v = _parco.First(x => x.Id == idVeicolo);

            // Calcolo costo: polimorfismo su 'Veicolo'
            decimal costo = v switch
            {
                BiciPieghevole bici => bici.CalcolaCosto(corsa.MinutiPrevisti, corsa.PartiPiegato),
                _ => v.CalcolaCosto(corsa.MinutiPrevisti) // altre classi ignorano flag
            };

            // Consumi e stati post-corsa
            if (v is IElettrico elettrico)
            {
                elettrico.ConsumaBatteria(corsa.MinutiPrevisti);
            }

            v.TerminaCorsa();

            // Se pieghevole, riportiamo lo stato ad "aperto" post-corsa (decisione di UX)
            if (v is IPieghevole pieghevole)
            {
                pieghevole.Apri();
            }

            corsa.Chiudi(costo);
            _corseAttive.Remove(idVeicolo);
            return costo;
        }
    }

    // ==========================
    // DEMO CONSOLE
    // ==========================

    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var service = new NoleggioService();

            // Seed parco (almeno 6 mezzi)
            service.AggiungiVeicolo(new MonopattinoElettrico(1, "MonoX Urban", 0.20m, batteriaIniziale: 40));
            service.AggiungiVeicolo(new MonopattinoElettrico(2, "MonoZ City", 0.22m, batteriaIniziale: 12)); // batteria bassa -> fee
            service.AggiungiVeicolo(new BiciPieghevole(3, "Foldy 3000", 0.08m, inizialmentePiegato: true));
            service.AggiungiVeicolo(new BiciPieghevole(4, "Foldy Lite", 0.07m));
            service.AggiungiVeicolo(new ScooterElettrico(5, "Scoot E-Pro", 0.35m, batteriaIniziale: 55));
            service.AggiungiVeicolo(new ScooterElettrico(6, "Scoot Mini", 0.30m, batteriaIniziale: 16));

            StampaParco("STATO INIZIALE", service);

            // Prenotazioni (dimostrare polimorfismo + interfacce)
            Console.WriteLine("\n--- PRENOTAZIONI ---");
            service.Prenota(1, minuti: 12); // MonopattinoElettrico
            Console.WriteLine("Prenotato veicolo #1 per 12 minuti.");

            service.Prenota(3, minuti: 8, partiPiegato: true); // Bici piegata -> sconto
            Console.WriteLine("Prenotato veicolo #3 per 8 minuti (parti piegato = true).");

            service.Prenota(5, minuti: 15); // Scooter elettrico
            Console.WriteLine("Prenotato veicolo #5 per 15 minuti.");

            StampaDisponibili("DOPO PRENOTAZIONI (disponibili)", service);

            // Terminazioni (calcolo costi)
            Console.WriteLine("\n--- TERMINE CORSE ---");
            decimal costo1 = service.Termina(1);
            Console.WriteLine($"Termine #1, costo: {costo1:C}");

            decimal costo3 = service.Termina(3);
            Console.WriteLine($"Termine #3, costo: {costo3:C}");

            decimal costo5 = service.Termina(5);
            Console.WriteLine($"Termine #5, costo: {costo5:C}");

            StampaParco("STATO FINALE", service);

            Console.WriteLine("\nPremi un tasto per uscire...");
            Console.ReadKey();
        }

        private static void StampaParco(string titolo, NoleggioService service)
        {
            Console.WriteLine($"\n=== {titolo} ===");
            foreach (var v in service.Parco)
            {
                Console.WriteLine(v.ToString());
            }
        }

        private static void StampaDisponibili(string titolo, NoleggioService service)
        {
            Console.WriteLine($"\n=== {titolo} ===");
            foreach (var v in service.Disponibili())
            {
                Console.WriteLine(v.ToString());
            }
        }
    }
}
