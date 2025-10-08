using System;

public class VoloAereo
{
    // campo privato: posti occupati
    private int postiOccupati;

    // costante: numero massimo di posti
    public const int MaxPosti = 150;

    // proprietà pubblica: codice del volo con get e set
    public string CodiceVolo { get; set; }

    // proprietà a sola lettura: quanti posti sono occupati
    public int PostiOccupati => postiOccupati;

    // proprietà a sola lettura: quanti posti sono liberi
    public int PostiLiberi => MaxPosti - postiOccupati;

    // costruttore (opzionale, comodo per impostare il codice)
    public VoloAereo(string codiceVolo)
    {
        CodiceVolo = codiceVolo;
        postiOccupati = 0;
    }

    // EffettuaPrenotazione: aggiunge posti se ci sono abbastanza posti liberi
    public bool EffettuaPrenotazione(int numeroPosti)
    {
        if (numeroPosti <= 0)
        {
            Console.WriteLine("Numero posti non valido.");
            return false;
        }

        if (numeroPosti <= PostiLiberi)
        {
            postiOccupati += numeroPosti;
            Console.WriteLine($"Prenotati {numeroPosti} posti.");
            return true;
        }

        Console.WriteLine("Prenotazione rifiutata: posti insufficienti.");
        return false;
    }

    // AnnullaPrenotazione: riduce i posti occupati se il numero è valido
    public bool AnnullaPrenotazione(int numeroPosti)
    {
        if (numeroPosti <= 0)
        {
            Console.WriteLine("Numero posti non valido.");
            return false;
        }

        if (numeroPosti <= postiOccupati)
        {
            postiOccupati -= numeroPosti;
            Console.WriteLine($"Annullati {numeroPosti} posti.");
            return true;
        }

        Console.WriteLine("Annullamento rifiutato: non ci sono così tanti posti occupati.");
        return false;
    }

    // VisualizzaStato: mostra codice volo, posti occupati e liberi
    public void VisualizzaStato()
    {
        Console.WriteLine($"Volo: {CodiceVolo} | Occupati: {PostiOccupati} | Liberi: {PostiLiberi}");
    }
}

public class Program
{
    public static void Main()
    {
        // Crea un oggetto VoloAereo
        VoloAereo volo = new VoloAereo("AZ123");

        // Stato iniziale
        volo.VisualizzaStato();

        // Esegui varie prenotazioni
        volo.EffettuaPrenotazione(20);
        volo.VisualizzaStato();

        volo.EffettuaPrenotazione(50);
        volo.VisualizzaStato();

        // Tentativo oltre la capacità
        volo.EffettuaPrenotazione(100);
        volo.VisualizzaStato();

        // Annullamenti
        volo.AnnullaPrenotazione(10);
        volo.VisualizzaStato();

        // Annullamento non valido (più di quelli occupati)
        volo.AnnullaPrenotazione(1000);
        volo.VisualizzaStato();

        Console.WriteLine("Fine demo. Premere un tasto per uscire...");
        Console.ReadKey();
    }
}
