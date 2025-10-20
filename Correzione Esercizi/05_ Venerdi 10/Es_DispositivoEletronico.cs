using System;
using System.Collections.Generic;

// Classe astratta (ASTRAZIONE)
abstract class DispositivoElettronico
{
    // Proprietà comune
    public string Modello { get; private set; }

    // Costruttore
    protected DispositivoElettronico(string modello)
    {
        Modello = modello;
    }

    // Metodi astratti: ogni sottoclasse deve implementarli
    public abstract void Accendi();
    public abstract void Spegni();

    // Metodo virtuale: ha un comportamento base, ma può essere ridefinito
    public virtual void MostraInfo()
    {
        Console.WriteLine($"Dispositivo: {GetType().Name} | Modello: {Modello}");
    }
}

// Classe concreta 1: Computer (EREDITARIETÀ + POLIMORFISMO)
class Computer : DispositivoElettronico
{
    public Computer(string modello) : base(modello) { }

    public override void Accendi()
    {
        Console.WriteLine("Il computer si avvia...");
    }

    public override void Spegni()
    {
        Console.WriteLine("Il computer si spegne.");
    }
}

// Classe concreta 2: Stampante (EREDITARIETÀ + POLIMORFISMO)
class Stampante : DispositivoElettronico
{
    public Stampante(string modello) : base(modello) { }

    public override void Accendi()
    {
        Console.WriteLine("La stampante si accende.");
    }

    public override void Spegni()
    {
        Console.WriteLine("La stampante va in standby.");
    }
}

class Program
{
    static void Main()
    {
        // Lista polimorfica di dispositivi
        List<DispositivoElettronico> laboratorio = new List<DispositivoElettronico>
        {
            new Computer("Dell XPS 13"),
            new Stampante("HP LaserJet 200")
        };

        // Per ogni elemento: MostraInfo, Accendi, Spegni
        foreach (var d in laboratorio)
        {
            d.MostraInfo();
            d.Accendi();
            d.Spegni();
            Console.WriteLine(new string('-', 40));
        }

        Console.WriteLine("Fine dimostrazione. Premi un tasto per uscire.");
        Console.ReadKey();
    }
}
