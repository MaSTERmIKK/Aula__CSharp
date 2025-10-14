using System;

// Interfaccia comune
interface IVeicolo
{
    void Avvia();
    void MostraTipo();
}

// Classi concrete
class Auto : IVeicolo
{
    public void Avvia()
    {
        Console.WriteLine("Avvio dell'auto...");
    }

    public void MostraTipo()
    {
        Console.WriteLine("Tipo: Auto");
    }
}

class Moto : IVeicolo
{
    public void Avvia()
    {
        Console.WriteLine("Avvio della moto...");
    }

    public void MostraTipo()
    {
        Console.WriteLine("Tipo: Moto");
    }
}

class Camion : IVeicolo
{
    public void Avvia()
    {
        Console.WriteLine("Avvio del camion...");
    }

    public void MostraTipo()
    {
        Console.WriteLine("Tipo: Camion");
    }
}

// FACTORY METHOD
class VeicoloFactory
{
    public static IVeicolo CreaVeicolo(string tipo)
    {
        switch (tipo.ToLower())
        {
            case "auto":
                return new Auto();
            case "moto":
                return new Moto();
            case "camion":
                return new Camion();
            default:
                return null; // tipo non valido
        }
    }
}

// MAIN PROGRAM
class Program
{
    static void Main()
    {
        Console.WriteLine("Quale veicolo vuoi creare? (auto/moto/camion)");
        string tipo = Console.ReadLine();

        IVeicolo veicolo = VeicoloFactory.CreaVeicolo(tipo);

        if (veicolo != null)
        {
            veicolo.Avvia();
            veicolo.MostraTipo();
        }
        else
        {
            Console.WriteLine("Errore: tipo di veicolo non riconosciuto!");
        }

        Console.WriteLine("\nFine programma. Premi un tasto per uscire.");
        Console.ReadKey();
    }
}
