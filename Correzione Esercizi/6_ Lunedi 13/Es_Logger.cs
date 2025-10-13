using System;

namespace EsempioSingleton
{
    /// <summary>
    /// Logger centralizzato implementato come Singleton.
    /// - sealed: impedisce l'ereditarietà (utile per non rompere il singleton).
    /// - costruttore privato: impedisce new dall'esterno.
    /// - istanza unica statica, creata lazy e thread-safe tramite lock.
    /// </summary>
    public sealed class Logger
    {
        // Campo statico che conterrà l'unica istanza del Logger.
        private static Logger? _istanza;

        // Oggetto di lock per garantire la creazione thread-safe.
        private static readonly object _lock = new();

        // Costruttore privato: nessuno all'esterno può fare "new Logger()".
        private Logger() { }

        /// <summary>
        /// Punto d'accesso globale all'istanza.
        /// Usa inizializzazione lazy + lock per sicurezza in contesti multithread.
        /// </summary>
        public static Logger GetIstanza()
        {
            // Doppio controllo per efficienza:
            if (_istanza is null)
            {
                lock (_lock)
                {
                    if (_istanza is null)
                        _istanza = new Logger();
                }
            }
            return _istanza;
        }

        /// <summary>
        /// Scrive un messaggio con data/ora su console.
        /// </summary>
        public void ScriviMessaggio(string messaggio)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"[{timestamp}] {messaggio}");
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // --- Primo punto del codice: ottengo l'istanza e scrivo un messaggio
            var log1 = Logger.GetIstanza();
            log1.ScriviMessaggio("Avvio applicazione");

            // --- Secondo punto del codice: in un metodo separato
            EseguiOperazioneDiBusiness();

            // --- Terzo punto (facoltativo): dimostrazione che l'istanza è la stessa
            var log2 = Logger.GetIstanza();
            bool stessaIstanza = Object.ReferenceEquals(log1, log2);
            Console.WriteLine();
            Console.WriteLine($"Stessa istanza? {stessaIstanza}");
            Console.WriteLine($"HashCode log1: {log1.GetHashCode()} | HashCode log2: {log2.GetHashCode()}");

            Console.WriteLine("\nPremi un tasto per uscire...");
            Console.ReadKey();
        }

        /// <summary>
        /// Simula un punto diverso dell'applicazione che ha bisogno del logger.
        /// </summary>
        static void EseguiOperazioneDiBusiness()
        {
            var logger = Logger.GetIstanza(); // ottengo (la stessa) istanza
            logger.ScriviMessaggio("Eseguo operazione di business...");
            // ... qui eventuale logica
            logger.ScriviMessaggio("Operazione completata con successo.");
        }
    }
}
