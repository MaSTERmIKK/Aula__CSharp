using System;
using System.Collections.Generic;

// Classe base (superclasse)
class Animale
{
    public string Nome;

    // Costruttore

    public Animale(string nome)
    {
        Nome = nome;
    }

    // Metodo comune a tutti gli animali
    public void Dormi()
    {
        Console.WriteLine(Nome + " sta dormendo...");
    }

    // Metodo che può essere ridefinito nelle sottoclassi
    public virtual void Verso()
    {
        Console.WriteLine(Nome + " emette un verso generico.");
    }
}

// Classe derivata (sottoclasse) che eredita da Animale
class Cane : Animale
{
    // Costruttore che usa quello della classe base
    public Cane(string nome) : base(nome)
    {
    }

    // Override: ridefinisce il metodo della classe base
    public override void Verso()
    {
        Console.WriteLine(Nome + " abbaia: Bau Bau!");
    }

    // Metodo specifico del Cane
    public void Scodinzola()
    {
        Console.WriteLine(Nome + " sta scodinzolando felice!");
    }
}

// Altra sottoclasse
class Gatto : Animale
{
    public Gatto(string nome) : base(nome)
    {
    }

    public override void Verso()
    {
        Console.WriteLine(Nome + " miagola: Miao!");
    }

    public void FaLeFusa()
    {
        Console.WriteLine(Nome + " fa le fusa...");
    }
}

class Program
{
    static void Main()
    {
        // Creiamo una lista di animali
        List<Animale> animali = new List<Animale>();

        // Aggiungiamo un cane e un gatto
        animali.Add(new Cane("Fido"));
        animali.Add(new Gatto("Micia"));

        // Tutti gli animali possono dormire e fare un verso
        foreach (Animale a in animali)
        {
            a.Dormi();
            a.Verso();

            if (a is Cane)
            {
                a.Scodinzola();
            }
            else if (a is Gatto)
            {
                a.FaLeFusa();
            }

            Console.WriteLine();
        }

        // Possiamo accedere ai metodi specifici solo se castiamo al tipo corretto
        Cane c = new Cane("Rex");
        c.Scodinzola();

        Gatto g = new Gatto("Luna");
        g.FaLeFusa();
    }
}
