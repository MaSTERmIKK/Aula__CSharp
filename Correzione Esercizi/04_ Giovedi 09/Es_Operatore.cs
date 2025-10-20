using System;
using System.Collections.Generic;

namespace PolimorfismoFacile
{
    // ===== Classe base =====
    public class Operatore
    {
        // Incapsulamento: campi privati
        private string nome;
        private string turno; // "giorno" o "notte"

        // Proprietà con controlli
        public string Nome
        {
            get => nome;
            set => nome = string.IsNullOrWhiteSpace(value) ? "Sconosciuto" : value.Trim();
        }

        public string Turno
        {
            get => turno;
            set
            {
                if (string.Equals(value, "giorno", StringComparison.OrdinalIgnoreCase))
                    turno = "giorno";
                else if (string.Equals(value, "notte", StringComparison.OrdinalIgnoreCase))
                    turno = "notte";
                else
                    turno = "giorno"; // default
            }
        }

        public Operatore(string nome, string turno)
        {
            Nome = nome;
            Turno = turno;
        }

        // Metodo virtuale: verrà ridefinito
        public virtual void EseguiCompito()
        {
            Console.WriteLine("Operatore generico in servizio.");
        }

        // Utile per stampa “tipo, nome, turno”
        public override string ToString() => $"{GetType().Name} | Nome: {Nome} | Turno: {Turno}";
    }

    // ===== Derivata 1 =====
    public class OperatoreEmergenza : Operatore
    {
        private int livelloUrgenza; // 1..5
        public int LivelloUrgenza
        {
            get => livelloUrgenza;
            set => livelloUrgenza = (value < 1) ? 1 : (value > 5 ? 5 : value);
        }

        public OperatoreEmergenza(string nome, string turno, int livello)
            : base(nome, turno) => LivelloUrgenza = livello;

        public override void EseguiCompito()
        {
            Console.WriteLine($"Gestione emergenza di livello {LivelloUrgenza}");
        }
    }

    // ===== Derivata 2 =====
    public class OperatoreSicurezza : Operatore
    {
        private string areaSorvegliata;
        public string AreaSorvegliata
        {
            get => areaSorvegliata;
            set => areaSorvegliata = string.IsNullOrWhiteSpace(value) ? "N/D" : value.Trim();
        }

        public OperatoreSicurezza(string nome, string turno, string area)
            : base(nome, turno) => AreaSorvegliata = area;

        public override void EseguiCompito()
        {
            Console.WriteLine($"Sorveglianza dell'area {AreaSorvegliata}");
        }
    }

    // ===== Derivata 3 =====
    public class OperatoreLogistica : Operatore
    {
        private int numeroConsegne;
        public int NumeroConsegne
        {
            get => numeroConsegne;
            set => numeroConsegne = value < 0 ? 0 : value;
        }

        public OperatoreLogistica(string nome, string turno, int consegne)
            : base(nome, turno) => NumeroConsegne = consegne;

        public override void EseguiCompito()
        {
            Console.WriteLine($"Coordinamento di {NumeroConsegne} consegne");
        }
    }

    // ===== Programma con menù =====
    public class Program
    {
        static void Main()
        {
            var lista = new List<Operatore>();
            bool esci = false;

            while (!esci)
            {
                Console.WriteLine("\n--- MENU ---");
                Console.WriteLine("1) Aggiungi Operatore Emergenza");
                Console.WriteLine("2) Aggiungi Operatore Sicurezza");
                Console.WriteLine("3) Aggiungi Operatore Logistica");
                Console.WriteLine("4) Stampa tutti (tipo, nome, turno)");
                Console.WriteLine("5) Chiama EseguiCompito() su tutti");
                Console.WriteLine("6) Esci");
                Console.Write("Scelta: ");

                switch (Console.ReadLine())
                {
                    case "1": AggiungiEmergenza(lista); break;
                    case "2": AggiungiSicurezza(lista); break;
                    case "3": AggiungiLogistica(lista); break;
                    case "4": Stampa(lista); break;
                    case "5": EseguiTutti(lista); break;
                    case "6": esci = true; break;
                    default: Console.WriteLine("Scelta non valida."); break;
                }
            }

            Console.WriteLine("Fine. Premi un tasto per uscire.");
            Console.ReadKey();
        }

        // --- Helper input rapidi ---
        static string LeggiStringa(string prompt, string def = "")
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            return string.IsNullOrWhiteSpace(s) ? def : s.Trim();
        }

        static int LeggiIntero(string prompt, int min, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int n) && n >= min && n <= max)
                    return n;
                Console.WriteLine("Valore non valido, riprova.");
            }
        }

        // --- Azioni di menù ---
        static void AggiungiEmergenza(List<Operatore> lista)
        {
            string nome = LeggiStringa("Nome: ");
            string turno = LeggiStringa("Turno (giorno/notte): ", "giorno");
            int livello = LeggiIntero("Livello urgenza (1-5): ", 1, 5);
            lista.Add(new OperatoreEmergenza(nome, turno, livello));
            Console.WriteLine("Operatore Emergenza aggiunto.");
        }

        static void AggiungiSicurezza(List<Operatore> lista)
        {
            string nome = LeggiStringa("Nome: ");
            string turno = LeggiStringa("Turno (giorno/notte): ", "giorno");
            string area = LeggiStringa("Area sorvegliata: ", "N/D");
            lista.Add(new OperatoreSicurezza(nome, turno, area));
            Console.WriteLine("Operatore Sicurezza aggiunto.");
        }

        static void AggiungiLogistica(List<Operatore> lista)
        {
            string nome = LeggiStringa("Nome: ");
            string turno = LeggiStringa("Turno (giorno/notte): ", "giorno");
            int consegne = LeggiIntero("Numero consegne (>=0): ", 0);
            lista.Add(new OperatoreLogistica(nome, turno, consegne));
            Console.WriteLine("Operatore Logistica aggiunto.");
        }

        static void Stampa(List<Operatore> lista)
        {
            if (lista.Count == 0) { Console.WriteLine("Nessun operatore."); return; }
            Console.WriteLine("\n--- OPERATORI ---");
            foreach (var op in lista) Console.WriteLine(op.ToString());
        }

        static void EseguiTutti(List<Operatore> lista)
        {
            if (lista.Count == 0) { Console.WriteLine("Nessun operatore."); return; }
            Console.WriteLine("\n--- ESEGUI COMPITO (polimorfismo) ---");
            foreach (var op in lista) op.EseguiCompito();
        }
    }
}
