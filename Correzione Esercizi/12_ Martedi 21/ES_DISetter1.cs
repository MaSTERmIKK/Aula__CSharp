using System;

#region Contratti
public interface ILogger
{
    void Log(string message);
}
#endregion

#region Implementazioni
public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[LOG] {message}");
}
#endregion

#region Componente che riceve la dipendenza via SETTER
public class Printer
{
    // Dipendenza impostabile dall'esterno (setter injection)
    public ILogger? Logger { get; set; }

    public void Print(string text)
    {
        // Uso sicuro della dipendenza
        if (Logger == null)
        {
            Console.WriteLine(text);
            return;
        }

        Logger.Log($"Stampa: {text}");
    }
}
#endregion

class Program
{
    static void Main()
    {
        var p = new Printer();

        // Senza logger (dimostra comportamento “di base”)
        p.Print("Ciao senza logger!");

        // Imposto la dipendenza via setter
        p.Logger = new ConsoleLogger();
        p.Print("Ciao con logger!");

        Console.WriteLine("\n[Fine Setter Injection] Premi un tasto per uscire.");
        Console.ReadKey();
    }
}
