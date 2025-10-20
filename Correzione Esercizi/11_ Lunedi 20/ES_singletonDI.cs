using System;

// ==========================
// SINGLETON: configurazione globale
// ==========================
public sealed class AppConfig
{
    private static readonly Lazy<AppConfig> _lazy = new(() => new AppConfig());
    public static AppConfig Instance => _lazy.Value;

    // Impostazioni globali
    public string AppName { get; private set; }
    public string Currency { get; private set; }
    public decimal TaxRate { get; private set; }

    private AppConfig()
    {
        AppName = "MyShop Console";
        Currency = "EUR";
        TaxRate = 0.22m; // 22%
    }

    public void PrintInfo()
    {
        Console.WriteLine($"[CONFIG] {AppName} - Valuta: {Currency}, IVA: {TaxRate * 100}%");
    }
}

// ==========================
// INTERFACCIA LOGGING
// ==========================
public interface ILogger
{
    void Log(string message);
}

// ==========================
// SERVIZIO LOG: riceve AppConfig via DI
// ==========================
public class LoggerService : ILogger
{
    private readonly AppConfig _config;

    // Constructor Injection
    public LoggerService(AppConfig config)
    {
        _config = config;
    }

    public void Log(string message)
    {
        Console.WriteLine($"[{_config.AppName}] {DateTime.Now:HH:mm:ss} - {message}");
    }
}

// ==========================
// SERVIZIO ORDINI: riceve il logger via DI
// ==========================
public class OrderService
{
    private readonly ILogger _logger;
    private int _nextId = 1;

    // Constructor Injection
    public OrderService(ILogger logger)
    {
        _logger = logger;
    }

    public void CreateOrder(string product, decimal price)
    {
        int orderId = _nextId++;
        _logger.Log($"Ordine {orderId} creato per {product} - Prezzo: {price:0.00} EUR");
    }

    public void CompleteOrder(int orderId)
    {
        _logger.Log($"Ordine {orderId} completato con successo ✅");
    }
}

// ==========================
// MAIN PROGRAM
// ==========================
public class Program
{
    public static void Main()
    {
        // --- SINGLETON ---
        var config = AppConfig.Instance;
        config.PrintInfo();

        // --- DEPENDENCY INJECTION MANUALE ---
        ILogger logger = new LoggerService(config); // Inject AppConfig
        var orderService = new OrderService(logger); // Inject Logger

        // --- ESECUZIONE ---
        orderService.CreateOrder("Mouse Logitech", 35.99m);
        orderService.CreateOrder("Tastiera Meccanica", 79.90m);
        orderService.CompleteOrder(1);

        Console.WriteLine("\nPremi un tasto per terminare...");
        Console.ReadKey();
    }
}
