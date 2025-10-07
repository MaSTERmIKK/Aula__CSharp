using System;
using System.Collections.Generic;

// Classe base (superclasse)
class Veicolo
{
    public string Marca { get; set; }
    public string Modello { get; set; }

    // Costruttore base
    public Veicolo(string marca, string modello)
    {
        Marca = marca;
        Modello = modello;
    }

    // Metodo virtuale che può essere sovrascritto dalle classi derivate
    public virtual void StampaInfo()
    {
        Console.WriteLine($"Veicolo: {Marca} {Modello}");
    }
}

// Classe derivata Auto
class Auto : Veicolo
{
    public int NumeroPorte { get; set; }

    // Costruttore che richiama il costruttore della classe base
    public Auto(string marca, string modello, int numeroPorte)
        : base(marca, modello)
    {
        NumeroPorte = numeroPorte;
    }

    // Override del metodo StampaInfo() per includere le informazioni specifiche
    public override void StampaInfo()
    {
        Console.WriteLine($"Auto: {Marca} {Modello}, Porte: {NumeroPorte}");
    }
}

// Classe derivata Moto
class Moto : Veicolo
{
    public string TipoManubrio { get; set; }

    // Costruttore che richiama il costruttore della classe base
    public Moto(string marca, string modello, string tipoManubrio)
        : base(marca, modello)
    {
        TipoManubrio = tipoManubrio;
    }

    // Override del metodo StampaInfo() per includere le informazioni specifiche
    public override void StampaInfo()
    {
        Console.WriteLine($"Moto: {Marca} {Modello}, Manubrio: {TipoManubrio}");
    }
}

// Classe principale del programma
class Program
{
    static void Main()
    {
        List<Veicolo> garage = new List<Veicolo>();
        bool continua = true;

        while (continua)
        {
            Console.WriteLine("\n--- MENU GARAGE ---");
            Console.WriteLine("1. Inserisci un nuovo veicolo");
            Console.WriteLine("2. Visualizza tutti i veicoli");
            Console.WriteLine("3. Esci");
            Console.Write("Scelta: ");

            string scelta = Console.ReadLine();

            switch (scelta)
            {
                case "1":
                    Console.Write("Inserisci tipo (Auto/Moto): ");
                    string tipo = Console.ReadLine().ToLower();

                    Console.Write("Marca: ");
                    string marca = Console.ReadLine();

                    Console.Write("Modello: ");
                    string modello = Console.ReadLine();

                    if (tipo == "auto")
                    {
                        Console.Write("Numero porte: ");
                        int porte = int.Parse(Console.ReadLine());
                        garage.Add(new Auto(marca, modello, porte));
                    }
                    else if (tipo == "moto")
                    {
                        Console.Write("Tipo manubrio: ");
                        string manubrio = Console.ReadLine();
                        garage.Add(new Moto(marca, modello, manubrio));
                    }
                    else
                    {
                        Console.WriteLine("Tipo non valido!");
                    }
                    break;

                case "2":
                    Console.WriteLine("\n--- Veicoli nel garage ---");
                    foreach (var v in garage)
                    {
                        v.StampaInfo(); // Polimorfismo: chiama la versione corretta
                    }
                    break;

                case "3":
                    continua = false;
                    Console.WriteLine("Uscita dal programma...");
                    break;

                default:
                    Console.WriteLine("Scelta non valida!");
                    break;
            }
        }
    }
}
