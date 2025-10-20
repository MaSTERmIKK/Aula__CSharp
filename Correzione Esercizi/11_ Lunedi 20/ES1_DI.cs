using System;

// 1) Contratto: il servizio che "sa salutare"
public interface IGreeter
{
    void Greet(string name);
}

// 2) Implementazione concreta: stampa su console
public class ConsoleGreeter : IGreeter
{
    public void Greet(string name)
    {
        Console.WriteLine($"Ciao {name}! Benvenuto nell'app DI 👋");
    }
}

// 3) Servizio che dipende da IGreeter e lo riceve via costruttore (DI)
public class GreetingService
{
    private readonly IGreeter _greeter;

    // Constructor Injection: niente new ConsoleGreeter() qui dentro.
    public GreetingService(IGreeter greeter)
    {
        _greeter = greeter ?? throw new ArgumentNullException(nameof(greeter));
    }

    public void Run(string name) => _greeter.Greet(name);
}

public class Program
{
    public static void Main()
    {
        // "Composizione" a livello di avvio (manuale): scegli l'implementazione
        IGreeter greeter = new ConsoleGreeter();

        // Inietto la dipendenza nel costruttore
        var app = new GreetingService(greeter);

        // Uso del servizio
        app.Run("Alice");
    }
}
