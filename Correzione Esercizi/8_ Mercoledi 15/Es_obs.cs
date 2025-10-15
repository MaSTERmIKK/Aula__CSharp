using System;
using System.Collections.Generic;

// ==== OBSERVER ====
public interface IObserver
{
    void Aggiorna(string messaggio);
}

// ==== SOGGETTO ====
public interface ISoggetto
{
    void Registra(IObserver osservatore);
    void Rimuovi(IObserver osservatore);
    void Notifica(string messaggio);
}

// ==== CONCRETE SUBJECT ====
public class CentroMeteo : ISoggetto
{
    private readonly List<IObserver> _osservatori = new List<IObserver>();

    public void Registra(IObserver osservatore)
    {
        if (osservatore != null && !_osservatori.Contains(osservatore))
            _osservatori.Add(osservatore);
    }

    public void Rimuovi(IObserver osservatore)
    {
        _osservatori.Remove(osservatore);
    }

    public void Notifica(string messaggio)
    {
        foreach (var obs in _osservatori)
            obs.Aggiorna(messaggio);
    }

    // Metodo di "business" che cambia lo stato e notifica
    public void AggiornaMeteo(string dati)
    {
        Console.WriteLine($"[CentroMeteo] Aggiornamento: {dati}");
        Notifica(dati);
    }
}

// ==== CONCRETE OBSERVERS ====
public class DisplayConsole : IObserver
{
    public void Aggiorna(string messaggio)
    {
        Console.WriteLine($"[DisplayConsole] Meteo: {messaggio}");
    }
}

public class DisplayMobile : IObserver
{
    public void Aggiorna(string messaggio)
    {
        Console.WriteLine($"[DisplayMobile] Notifica push → {messaggio}");
    }
}

// ==== MAIN ====
class Program
{
    static void Main()
    {
        var centro = new CentroMeteo();

        // Registra almeno due display
        var consoleDisplay = new DisplayConsole();
        var mobileDisplay = new DisplayMobile();
        centro.Registra(consoleDisplay);
        centro.Registra(mobileDisplay);

        Console.WriteLine("Inserisci aggiornamenti meteo (es. 'Pioggia', 'Sole').");
        Console.WriteLine("Scrivi 'exit' per terminare.\n");

        while (true)
        {
            Console.Write("Nuovo dato meteo: ");
            string input = Console.ReadLine()?.Trim() ?? "";

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("(vuoto) niente da aggiornare.");
                continue;
            }

            centro.AggiornaMeteo(input);
            Console.WriteLine(new string('-', 40));
        }

        // Esempio di unsubscribe opzionale:
        centro.Rimuovi(mobileDisplay);
        Console.WriteLine("\nMobile disiscritto. Ultimo update solo su console:");
        centro.AggiornaMeteo("Vento forte");

        Console.WriteLine("\nFine. Premi un tasto per uscire.");
        Console.ReadKey();
    }
}
