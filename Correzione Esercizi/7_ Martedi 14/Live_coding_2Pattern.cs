using System;

// Classe Singleton che funge da Factory
public class BevandaFactory
{
    //  Campo statico privato: tiene l'unica istanza della classe
    private static BevandaFactory istanza;

    //  Costruttore privato: impedisce la creazione diretta dall'esterno
    private BevandaFactory()
    {
        Console.WriteLine("Factory inizializzata una sola volta.");
    }

    //  Metodo pubblico statico per accedere all’unica istanza (pattern Singleton)
    public static BevandaFactory OttieniIstanza()
    {
        // Se non esiste ancora, la crea
        if (istanza == null)
            istanza = new BevandaFactory();

        // Altrimenti restituisce quella già esistente
        return istanza;
    }

    //  Metodo Factory: crea oggetti di tipo Bevanda in base al parametro
    public Bevanda CreaBevanda(string tipo)
    {
        if (tipo.ToLower() == "caffe")
            return new Caffe();
        else if (tipo.ToLower() == "tè" || tipo.ToLower() == "te")
            return new Te();
        else
            return new Acqua();
    }
}

// Classe base (superclasse astratta o generica)
public abstract class Bevanda
{
    public abstract void Servi();
}

// Classi derivate (prodotti concreti)
public class Caffe : Bevanda
{
    public override void Servi()
    {
        Console.WriteLine(" È pronto un caffè caldo!");
    }
}

public class Te : Bevanda
{
    public override void Servi()
    {
        Console.WriteLine(" È pronto un tè profumato!");
    }
}

public class Acqua : Bevanda
{
    public override void Servi()
    {
        Console.WriteLine(" Ecco un bicchiere d’acqua fresca!");
    }
}

// Classe principale per testare tutto
class Program
{
    static void Main()
    {
        // Ottenere l’unica istanza della factory (Singleton)
        BevandaFactory factory = BevandaFactory.OttieniIstanza();

        // Creare bevande tramite il metodo factory
        Bevanda b1 = factory.CreaBevanda("caffe");
        Bevanda b2 = factory.CreaBevanda("te");
        Bevanda b3 = factory.CreaBevanda("acqua");

        // Servire le bevande
        b1.Servi();
        b2.Servi();
        b3.Servi();

        // Test: la factory è davvero unica?
        BevandaFactory factory2 = BevandaFactory.OttieniIstanza();

        // Mostra che le due variabili puntano alla stessa istanza
        Console.WriteLine(factory == factory2
            ? "La factory è la stessa istanza (Singleton funzionante)."
            : "Sono state create più istanze!");
    }
}
