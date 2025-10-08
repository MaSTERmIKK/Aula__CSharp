using System;
using System.Collections.Generic;

// Classe base
class Veicolo
{
    public string Targa { get; }

    public Veicolo(string targa)
    {
        Targa = targa;
    }

    // Metodo virtuale: comportamento generico
    public virtual string Ripara()
    {
        return "Il veicolo viene controllato.";
    }
}

// Derivata: Auto
class Auto : Veicolo
{
    public Auto(string targa) : base(targa) { }

    public override string Ripara()
    {
        return "Controllo olio, freni e motore dell'auto.";
    }
}

// Derivata: Moto
class Moto : Veicolo
{
    public Moto(string targa) : base(targa) { }

    public override string Ripara()
    {
        return "Controllo catena, freni e gomme della moto.";
    }
}

// Derivata: Camion
class Camion : Veicolo
{
    public Camion(string targa) : base(targa) { }

    public override string Ripara()
    {
        return "Controllo sospensioni, freni rinforzati e carico del camion.";
    }
}

class Program
{
    static void Main()
    {
        // Crea la lista di Veicolo con istanze di Auto, Moto e Camion
        List<Veicolo> officina = new List<Veicolo>
        {
            new Auto("AB123CD"),
            new Moto("ZX987YZ"),
            new Camion("TR555AA")
        };

        // Scorri e mostra targa + tipo di riparazione (polimorfismo con override)
        foreach (var v in officina)
        {
            Console.WriteLine($"{v.Targa} -> {v.Ripara()}");
        }
    }
}
