using System;

// ====== DOMAIN / CONTRATTI ======
public enum TipoPagamento { Carta = 1, PayPal = 2, Bonifico = 3 }

public interface IPagamento
{
    // Esegue il pagamento e ritorna un id transazione
    string Paga(decimal importoNetto);
}

public interface ILogger
{
    void Log(string msg);
}

public interface IDiscountPolicy
{
    decimal Applica(decimal importoLordo);
}

// Delegate per la notifica di pagamento completato
public delegate void PagamentoCompletatoHandler(string transazioneId, decimal totaleNetto);

// ====== IMPLEMENTAZIONI CONCRETE ======
public class ConsoleLogger : ILogger
{
    public void Log(string msg) => Console.WriteLine($"[LOG] {msg}");
}

public class NoDiscountPolicy : IDiscountPolicy
{
    public decimal Applica(decimal importoLordo) => importoLordo;
}

public class PercentDiscountPolicy : IDiscountPolicy
{
    private readonly decimal _percent; // es. 0.10 = 10%
    public PercentDiscountPolicy(decimal percent) => _percent = percent;
    public decimal Applica(decimal importoLordo) => Math.Round(importoLordo * (1 - _percent), 2);
}

// Metodi di pagamento
public class PagamentoCarta : IPagamento
{
    public string Paga(decimal importoNetto)
    {
        // simulazione
        return $"CARD-{Guid.NewGuid():N}".Substring(0, 12);
    }
}
public class PagamentoPayPal : IPagamento
{
    public string Paga(decimal importoNetto)
    {
        return $"PYPL-{Guid.NewGuid():N}".Substring(0, 12);
    }
}
public class PagamentoBonifico : IPagamento
{
    public string Paga(decimal importoNetto)
    {
        return $"BONF-{Guid.NewGuid():N}".Substring(0, 12);
    }
}

// ====== FACTORY PATTERN ======
public static class PagamentoFactory
{
    public static IPagamento Crea(TipoPagamento tipo) => tipo switch
    {
        TipoPagamento.Carta    => new PagamentoCarta(),
        TipoPagamento.PayPal   => new PagamentoPayPal(),
        TipoPagamento.Bonifico => new PagamentoBonifico(),
        _ => throw new ArgumentOutOfRangeException(nameof(tipo))
    };
}

// ====== APPLICATION / SERVICE (DI + Delegate) ======
public class PaymentService
{
    private readonly IPagamento _metodo;
    private readonly ILogger _logger;
    private readonly IDiscountPolicy _sconti;

    // DI: injection nel costruttore
    public PaymentService(IPagamento metodo, ILogger logger, IDiscountPolicy sconti)
    {
        _metodo = metodo;
        _logger = logger;
        _sconti = sconti;
    }

    // Evento basato sul delegate
    public event PagamentoCompletatoHandler? OnPagamentoCompletato;

    public void EseguiPagamento(decimal importo)
    {
        if (importo <= 0) throw new ArgumentException("Importo non valido.");

        _logger.Log($"Importo lordo: {importo:0.00}");
        var netto = _sconti.Applica(importo);
        _logger.Log($"Importo netto dopo sconto: {netto:0.00}");

        var transId = _metodo.Paga(netto);
        _logger.Log($"Pagamento riuscito. Transazione: {transId}");

        // Notifica via evento (Delegate)
        OnPagamentoCompletato?.Invoke(transId, netto);
    }
}

// ====== PRESENTATION / MAIN ======
class Program
{
    static void Main()
    {
        Console.WriteLine("=== Pagamenti (Factory + DI + Delegate) ===");
        Console.WriteLine("1) Carta  2) PayPal  3) Bonifico");
        Console.Write("Scegli metodo: ");
        if (!int.TryParse(Console.ReadLine(), out int scelta) ||
            !Enum.IsDefined(typeof(TipoPagamento), scelta))
        {
            Console.WriteLine("Scelta non valida.");
            return;
        }

        Console.Write("Importo: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal importo) || importo <= 0)
        {
            Console.WriteLine("Importo non valido.");
            return;
        }

        // --- FACTORY: creo il metodo di pagamento concreto
        var tipo = (TipoPagamento)scelta;
        IPagamento metodo = PagamentoFactory.Crea(tipo);

        // --- DI manuale: inietto logger e politica sconti
        ILogger logger = new ConsoleLogger();
        IDiscountPolicy sconti = new PercentDiscountPolicy(0.10m); // 10% di sconto

        var service = new PaymentService(metodo, logger, sconti);

        // --- Delegate / Event: sottoscrizione di 2 handler diversi
        service.OnPagamentoCompletato += (id, totale) =>
            Console.WriteLine($"[EMAIL] Inviata ricevuta per transazione {id}, totale {totale:0.00}.");

        service.OnPagamentoCompletato += NotificaCRM;

        // Esecuzione
        service.EseguiPagamento(importo);

        Console.WriteLine("\nPremi un tasto per uscire.");
        Console.ReadKey();
    }

    // Altro handler (metodo separato): es. integrazione CRM
    static void NotificaCRM(string transazioneId, decimal totaleNetto)
    {
        Console.WriteLine($"[CRM] Registrata transazione {transazioneId} con importo {totaleNetto:0.00}.");
    }
}
