using System;

// Classe base (superclasse)
class Animale
{
    public string Nome;

    public Animale(string nome)
    {
        Nome = nome;
    }

    // Metodo comune a tutti gli animali, non può essere ridefinito (sealed)
    public sealed void Dormi()
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
    public Cane(string nome) : base(nome)
    { }

    // Override del metodo Verso, ma sealed impedisce ulteriori override
    public override sealed void Verso()
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
    int etaGatto = 0;

    public Gatto(string nome, int eta) : base(nome)
    {
        etaGatto = eta;
    }

    // Override del metodo Verso
    public override sealed void Verso()
    {
        Console.WriteLine(Nome + " miagola: Miao! da ormai " + etaGatto + " anni.");
    }

    // Overload del metodo FaLeFusa
    public virtual void FaLeFusa()
    {
        Console.WriteLine(Nome + " fa le fusa...");
    }

    public virtual void FaLeFusa(int eta)
    {
        Console.WriteLine(Nome + " fa le fusa da " + eta + " anni.");
    }
}

class Program
{
    // Metodo che accetta qualsiasi oggetto di tipo Animale (polimorfismo)
    static void GestisciAnimali(Animale x)
    {
        x.Dormi();
        x.Verso();
    }

    static void Main()
    {
        Gatto g = new Gatto("Micia", 3);
        Cane c = new Cane("Fido");

        // Chiamata del metodo polimorfico
        GestisciAnimali(c);
        GestisciAnimali(g);

        // Chiamate specifiche delle sottoclassi
        g.FaLeFusa();
        g.FaLeFusa(3);
        c.Scodinzola();
    }
}