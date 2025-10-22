using System;
using System.Collections.Generic;
using System.Linq;

// --- ENTITÀ DI BASE ---
public enum OrderStatus { New, Paid, Shipped, Cancelled }

public class Product
{
    public string Code { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Product(string code, string name, decimal price)
    {
        Code = code;
        Name = name;
        Price = price;
    }
}

public class OrderItem
{
    public Product Product { get; }
    public int Quantity { get; }
    public decimal LineTotal => Product.Price * Quantity;

    public OrderItem(Product product, int qty)
    {
        Product = product;
        Quantity = qty;
    }
}

public class Order
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Customer { get; }
    public OrderStatus Status { get; private set; } = OrderStatus.New;
    public List<OrderItem> Items { get; } = new();

    public Order(string customer)
    {
        Customer = customer;
    }

    public void AddItem(Product p, int qty)
    {
        if (Status != OrderStatus.New)
            throw new InvalidOperationException("Puoi aggiungere articoli solo quando l'ordine è NEW.");
        Items.Add(new OrderItem(p, qty));
    }

    public void Pay()
    {
        if (Status != OrderStatus.New)
            throw new InvalidOperationException("Solo ordini NEW possono essere pagati.");
        Status = OrderStatus.Paid;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException("Solo ordini PAID possono essere spediti.");
        Status = OrderStatus.Shipped;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Non puoi annullare un ordine già spedito.");
        Status = OrderStatus.Cancelled;
    }

    public decimal Subtotal() => Items.Sum(i => i.LineTotal);
}

// --- SINGLETON DI CONFIGURAZIONE ---
public sealed class Configurazione
{
    private static readonly Lazy<Configurazione> _lazy =
        new Lazy<Configurazione>(() => new Configurazione());

    public static Configurazione Instance => _lazy.Value;

    public decimal TaxRate { get; } = 0.22m;
    public string Currency { get; } = "EUR";

    private Configurazione() { }
}

// --- PROGRAMMA PRINCIPALE ---
class Program
{
    static void Main()
    {
        // Singleton
        var config = Configurazione.Instance;

        // Repository in-memory (solo strutture base)
        var prodotti = new List<Product>();
        var ordini = new List<Order>();

        // --- CREA ALCUNI PRODOTTI ---
        prodotti.Add(new Product("PEN", "Penna Blu", 1.50m));
        prodotti.Add(new Product("NBK", "Taccuino", 4.20m));
        prodotti.Add(new Product("MUG", "Tazza Logo", 8.90m));

        Console.WriteLine("=== PRODOTTI DISPONIBILI ===");
        foreach (var p in prodotti)
            Console.WriteLine($"{p.Code} - {p.Name} - {p.Price:0.00} {config.Currency}");

        // --- CREA UN ORDINE ---
        Console.Write("\nInserisci nome cliente: ");
        string cliente = Console.ReadLine();

        var ordine = new Order(cliente);
        ordini.Add(ordine);

        Console.WriteLine($"\nOrdine creato (ID: {ordine.Id}) per {ordine.Customer}");

        // --- AGGIUNGI PRODOTTI ---
        while (true)
        {
            Console.Write("\nInserisci codice prodotto (vuoto per terminare): ");
            string codice = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(codice)) break;

            var prodotto = prodotti.FirstOrDefault(p => p.Code.Equals(codice, StringComparison.OrdinalIgnoreCase));
            if (prodotto == null)
            {
                Console.WriteLine("Prodotto non trovato!");
                continue;
            }

            Console.Write("Quantità: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
            {
                Console.WriteLine("Quantità non valida.");
                continue;
            }

            ordine.AddItem(prodotto, qty);
            Console.WriteLine($"Aggiunto {qty}x {prodotto.Name}");
        }

        // --- CALCOLA TOTALE ---
        var sub = ordine.Subtotal();
        var iva = Math.Round(sub * config.TaxRate, 2);
        var totale = sub + iva;

        Console.WriteLine($"\nTotale ordine: Sub {sub:0.00} + IVA {iva:0.00} = {totale:0.00} {config.Currency}");

        // --- CAMBIA STATO ---
        Console.Write("\nVuoi confermare il pagamento (s/n)? ");
        if (Console.ReadLine()?.Trim().ToLower() == "s")
        {
            ordine.Pay();
            Console.WriteLine($"Ordine pagato. Stato attuale: {ordine.Status}");
        }

        Console.Write("Vuoi spedire l’ordine (s/n)? ");
        if (Console.ReadLine()?.Trim().ToLower() == "s")
        {
            ordine.Ship();
            Console.WriteLine($"Ordine spedito. Stato attuale: {ordine.Status}");
        }

        Console.WriteLine("\n=== RIEPILOGO ORDINI ===");
        foreach (var o in ordini)
        {
            Console.WriteLine($"Ordine {o.Id} | Cliente: {o.Customer} | Stato: {o.Status} | Totale articoli: {o.Items.Count}");
        }

        Console.WriteLine("\nProgramma terminato. Premi un tasto per uscire.");
        Console.ReadKey();
    }
}
