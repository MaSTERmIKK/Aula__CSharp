using System;
using System.Collections.Generic;

namespace TwoRulesESMedio
{
    // ===== Classe base =====
    public class Soldato
    {
        // Campi privati (incapsulamento)
        private string nome;
        private string grado;
        private int anniServizio;

        // Proprietà pubbliche con controlli minimi
        public string Nome
        {
            get => nome;
            set => nome = string.IsNullOrWhiteSpace(value) ? "Sconosciuto" : value.Trim();
        }

        public string Grado
        {
            get => grado;
            set => grado = string.IsNullOrWhiteSpace(value) ? "Recluta" : value.Trim();
        }

        public int AnniServizio
        {
            get => anniServizio;
            set => anniServizio = (value >= 0) ? value : 0; // solo valori >= 0
        }

        public Soldato(string nome, string grado, int anniServizio)
        {
            Nome = nome;
            Grado = grado;
            AnniServizio = anniServizio;
        }

        // Metodo virtuale
        public virtual string Descrizione()
        {
            return $"Soldato: {Nome} | Grado: {Grado} | Anni di servizio: {AnniServizio}";
        }
    }

    // ===== Derivata 1: Fante =====
    public class Fante : Soldato
    {
        private string arma;

        public string Arma
        {
            get => arma;
            set => arma = string.IsNullOrWhiteSpace(value) ? "N/D" : value.Trim();
        }

        public Fante(string nome, string grado, int anniServizio, string arma)
            : base(nome, grado, anniServizio)
        {
            Arma = arma;
        }

        public override string Descrizione()
        {
            return base.Descrizione() + $" | Arma: {Arma}";
        }
    }

    // ===== Derivata 2: Artigliere =====
    public class Artigliere : Soldato
    {
        private int calibro;

        public int Calibro
        {
            get => calibro;
            set => calibro = (value > 0) ? value : 0; // solo valori positivi
        }

        public Artigliere(string nome, string grado, int anniServizio, int calibro)
            : base(nome, grado, anniServizio)
        {
            Calibro = calibro;
        }

        public override string Descrizione()
        {
            return base.Descrizione() + $" | Calibro gestito: {Calibro} mm";
        }
    }

    // ===== Programma con menu =====
    public class Program
    {
        static void Main()
        {
            List<Soldato> esercito = new List<Soldato>();
            bool esci = false;

            while (!esci)
            {
                Console.WriteLine("\n--- MENU ESERCITO ---");
                Console.WriteLine("1) Aggiungi Fante");
                Console.WriteLine("2) Aggiungi Artigliere");
                Console.WriteLine("3) Visualizza tutti");
                Console.WriteLine("4) Esci");
                Console.Write("Scelta: ");

                string scelta = Console.ReadLine();
                switch (scelta)
                {
                    case "1":
                        AggiungiFante(esercito);
                        break;
                    case "2":
                        AggiungiArtigliere(esercito);
                        break;
                    case "3":
                        Visualizza(esercito);
                        break;
                    case "4":
                        esci = true;
                        break;
                    default:
                        Console.WriteLine("Scelta non valida.");
                        break;
                }
            }

            Console.WriteLine("Chiusura programma. Premi un tasto per uscire.");
            Console.ReadKey();
        }

        // ==== Helper per input ====
        static string LeggiStringa(string prompt, string predef = "")
        {
            Console.Write(prompt);
            string s = Console.ReadLine();
            return string.IsNullOrWhiteSpace(s) ? predef : s.Trim();
        }

        static int LeggiIntero(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int n) && n >= min && n <= max)
                    return n;

                Console.WriteLine("Valore non valido, riprova.");
            }
        }

        // ==== Azioni di menu ====
        static void AggiungiFante(List<Soldato> esercito)
        {
            string nome = LeggiStringa("Nome: ");
            string grado = LeggiStringa("Grado: ", "Recluta");
            int anni = LeggiIntero("Anni di servizio (>=0): ", 0);
            string arma = LeggiStringa("Arma: ", "N/D");

            esercito.Add(new Fante(nome, grado, anni, arma));
            Console.WriteLine("Fante aggiunto.");
        }

        static void AggiungiArtigliere(List<Soldato> esercito)
        {
            string nome = LeggiStringa("Nome: ");
            string grado = LeggiStringa("Grado: ", "Recluta");
            int anni = LeggiIntero("Anni di servizio (>=0): ", 0);
            int calibro = LeggiIntero("Calibro (mm, >0): ", 1);

            esercito.Add(new Artigliere(nome, grado, anni, calibro));
            Console.WriteLine("Artigliere aggiunto.");
        }

        static void Visualizza(List<Soldato> esercito)
        {
            if (esercito.Count == 0)
            {
                Console.WriteLine("Nessun soldato presente.");
                return;
            }

            Console.WriteLine("\n--- ELENCO SOLDATI ---");
            foreach (var s in esercito)
                Console.WriteLine(s.Descrizione());
        }
    }
}
