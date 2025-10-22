using System;
using Domain;
using Infrastructure;
using Application;

class Program
{
    static void Main()
    {
        // --- SINGLETON: configurazione globale ---
        IConfigurationProvider config = ConfigurationProvider.Instance;

        // --- INFRASTRUCTURE: creazione manuale degli oggetti concreti ---
        IProductRepository productRepo = new InMemoryProductRepository();
        IOrderRepository orderRepo = new InMemoryOrderRepository();
        INotificationService notifier = new ConsoleNotificationService();

        // --- APPLICATION: iniezione manuale delle dipendenze nel costruttore ---
        var productService = new ProductService(productRepo);
        var orderService = new OrderService(orderRepo, productService, notifier, config);

        // --- DEMO OPERATIVA ---
        Console.WriteLine($"Configurazione (Singleton): IVA={config.TaxRate:P0} Currency={config.Currency}\n");

        // Creazione prodotti base
        productService.CreateProduct("PEN", "Penna Blu", 1.50m);
        productService.CreateProduct("NBK", "Taccuino A5", 4.20m);
        productService.CreateProduct("MUG", "Tazza Logo", 8.90m);

        Console.WriteLine("=== Prodotti Disponibili ===");
        foreach (var p in productService.List())
            Console.WriteLine($"- {p.Code} | {p.Name} | {p.Price:0.00} {config.Currency}");

        // Crea ordine
        var ordine = orderService.CreateOrder("mario.rossi@example.com");
        orderService.AddItem(ordine.Id, "PEN", 2);
        orderService.AddItem(ordine.Id, "MUG", 1);

        // Calcola totali
        var (sub, iva, tot) = orderService.ComputeTotals(ordine.Id);
        Console.WriteLine($"\nTotale ordine {ordine.Id}: Sub {sub:0.00} + IVA {iva:0.00} = {tot:0.00} {config.Currency}");

        // Checkout e spedizione
        orderService.Checkout(ordine.Id, ordine.Customer);
        orderService.Ship(ordine.Id, ordine.Customer);

        // Lista ordini
        Console.WriteLine("\n=== Ordini Creati ===");
        foreach (var o in orderService.List())
            Console.WriteLine($"- {o.Id} | Cliente: {o.Customer} | Stato: {o.Status} | Articoli: {o.Items.Count}");

        Console.WriteLine("\nFine. Premi un tasto per uscire.");
        Console.ReadKey();
    }
}
