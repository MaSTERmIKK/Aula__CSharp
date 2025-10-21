using System;
using System.Collections.Generic;
using System.Linq;

// ======================================================
// DOMAIN: prodotti & ordini
// ======================================================
public interface IProduct
{
    string Code { get; }
    string Name { get; }
    decimal BasePrice { get; }
    bool IsDigital { get; }
}

public class BookPrint : IProduct
{
    public string Code => "BOOK_PRINT";
    public string Name => "Libro cartaceo";
    public decimal BasePrice => 35.00m;
    public bool IsDigital => false;
}

public class BookDigital : IProduct
{
    public string Code => "BOOK_DIGITAL";
    public string Name => "E-book (PDF/EPUB)";
    public decimal BasePrice => 29.00m;
    public bool IsDigital => true;
}

// Factory: chiusa rispetto ai client; mappo codice → tipo concreto
public static class ProductFactory
{
    public static IProduct Create(string productCode) =>
        productCode.ToUpper() switch
        {
            "BOOK_PRINT"   => new BookPrint(),
            "BOOK_DIGITAL" => new BookDigital(),
            _ => throw new ArgumentException($"productCode non supportato: {productCode}")
        };
}

public class OrderItem
{
    public string ProductCode { get; init; } = "";
    public string Name { get; init; } = "";
    public decimal UnitPrice { get; init; }
    public int Qty { get; init; }
    public bool IsDigital { get; init; }
}

public class Order
{
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public List<OrderItem> Items { get; } = new();
    public decimal Total { get; set; }
    public string Status { get; set; } = "Draft";
}

// ======================================================
// PORTS: interfacce per DI
// ======================================================
public interface IInventoryService
{
    bool CheckAvailability(string productCode, int qty);
    bool Reserve(string productCode, int qty); // scala lo stock
}

public interface IPaymentProcessor
{
    PaymentResult Charge(decimal amount, string currency, int customerId);
}

public record PaymentResult(bool Success, string? Error = null);

public interface INotificationSender
{
    void Send(string to, string subject, string body);
}

public interface IPricingStrategy
{
    decimal ComputeTotal(IEnumerable<OrderItem> items);
}

// ======================================================
// ADAPTERS: implementazioni concrete
// ======================================================
public class MemoryInventoryService : IInventoryService
{
    // stock per SKU (solo i fisici hanno stock)
    private readonly Dictionary<string, int> _stock = new(StringComparer.OrdinalIgnoreCase);

    public MemoryInventoryService Seed(string code, int qty)
    {
        _stock[code] = qty;
        return this;
    }

    public bool CheckAvailability(string productCode, int qty)
    {
        if (!_stock.TryGetValue(productCode, out var available))
            return false;
        return available >= qty;
    }

    public bool Reserve(string productCode, int qty)
    {
        if (!CheckAvailability(productCode, qty)) return false;
        _stock[productCode] -= qty;
        return true;
    }
}

public class PaypalProcessor : IPaymentProcessor
{
    public PaymentResult Charge(decimal amount, string currency, int customerId)
    {
        Console.WriteLine($"[PayPal] Addebito {amount:0.00} {currency} a customer {customerId}");
        return new PaymentResult(true);
    }
}

public class EmailSender : INotificationSender
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine($"[EMAIL → {to}] {subject}\n{body}\n");
    }
}

// Strategy “base”: somma secca
public class PlainTotalStrategy : IPricingStrategy
{
    public decimal ComputeTotal(IEnumerable<OrderItem> items) =>
        Math.Round(items.Sum(i => i.UnitPrice * i.Qty), 2);
}

// Strategy promo: -10% oltre 50€
public class Promo10Over50 : IPricingStrategy
{
    public decimal ComputeTotal(IEnumerable<OrderItem> items)
    {
        var sum = items.Sum(i => i.UnitPrice * i.Qty);
        if (sum > 50m) sum *= 0.90m;
        return Math.Round(sum, 2);
    }
}

// ======================================================
// APPLICATION SERVICE: usa DI (constructor + setter)
// ======================================================
public class OrderService
{
    private readonly IInventoryService _inventory;     // obbligatoria → constructor DI
    private readonly IPaymentProcessor _payments;      // obbligatoria → constructor DI

    // opzionali → setter DI (cambiabili a runtime)
    public INotificationSender? NotificationSender { private get; set; }
    public IPricingStrategy? PricingStrategy { private get; set; }

    private readonly Dictionary<int, Order> _orders = new();
    private int _nextId = 1;

    public OrderService(IInventoryService inventory, IPaymentProcessor payments)
    {
        _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        _payments  = payments  ?? throw new ArgumentNullException(nameof(payments));
    }

    public Order CreateOrder(int customerId)
    {
        var o = new Order { Id = _nextId++, CustomerId = customerId };
        _orders[o.Id] = o;
        Console.WriteLine($"[Order] Creato ordine #{o.Id} per customer {customerId}");
        return o;
    }

    public void AddItem(int orderId, string productCode, int qty)
    {
        if (!_orders.TryGetValue(orderId, out var order))
            throw new ArgumentException("Ordine inesistente");

        var product = ProductFactory.Create(productCode);

        // Se fisico, controllo disponibilità
        if (!product.IsDigital && !_inventory.CheckAvailability(productCode, qty))
            throw new InvalidOperationException($"Stock insufficiente per {productCode}");

        order.Items.Add(new OrderItem
        {
            ProductCode = product.Code,
            Name = product.Name,
            UnitPrice = product.BasePrice,
            Qty = qty,
            IsDigital = product.IsDigital
        });

        Console.WriteLine($"[Order {orderId}] + {qty} x {product.Name} ({product.BasePrice:0.00}€ cad.)");
    }

    public bool Checkout(int orderId, string customerEmail, string currency = "EUR")
    {
        if (!_orders.TryGetValue(orderId, out var order))
            throw new ArgumentException("Ordine inesistente");

        if (!order.Items.Any())
            throw new InvalidOperationException("Ordine vuoto");

        // 1) Calcolo totale (Strategy via setter, altrimenti plain)
        var strategy = PricingStrategy ?? new PlainTotalStrategy();
        order.Total = strategy.ComputeTotal(order.Items);

        // 2) Riservo stock dei fisici
        foreach (var it in order.Items.Where(i => !i.IsDigital))
        {
            if (!_inventory.Reserve(it.ProductCode, it.Qty))
            {
                Console.WriteLine($"[Order {orderId}] Prenotazione stock fallita per {it.ProductCode}");
                order.Status = "StockFailed";
                return false;
            }
        }

        // 3) Pagamento
        var pay = _payments.Charge(order.Total, currency, order.CustomerId);
        if (!pay.Success)
        {
            Console.WriteLine($"[Order {orderId}] Pagamento fallito: {pay.Error}");
            order.Status = "PaymentFailed";
            return false;
        }

        order.Status = "Completed";
        Console.WriteLine($"[Order {orderId}] COMPLETATO. Totale: {order.Total:0.00} {currency}");

        // 4) Notifica opzionale (setter DI)
        NotificationSender?.Send(
            to: customerEmail,
            subject: $"Conferma ordine #{order.Id}",
            body: $"Grazie per l'acquisto. Totale: {order.Total:0.00} {currency}");

        return true;
    }
}

// ======================================================
// DEMO
// ======================================================
public class Program
{
    public static void Main()
    {
        // --- Composition Root ---
        // Constructor DI: dipendenze obbligatorie
        var inventory = new MemoryInventoryService()
            .Seed("BOOK_PRINT", 10); // lo stock serve solo ai prodotti fisici
        var payments  = new PaypalProcessor();
        var orders    = new OrderService(inventory, payments);

        Console.WriteLine("\n=== Demo A: senza setter (no notifica, pricing base) ===");
        var a = orders.CreateOrder(customerId: 1);
        orders.AddItem(a.Id, "BOOK_PRINT", 1);
        orders.AddItem(a.Id, "BOOK_DIGITAL", 1);
        orders.Checkout(a.Id, customerEmail: "alice@example.com"); // nessuna notifica; prezzo senza sconti

        Console.WriteLine("\n=== Demo B: abilito i SETTER (notifica + strategia sconto) ===");
        orders.NotificationSender = new EmailSender();            // setter DI
        orders.PricingStrategy    = new Promo10Over50();          // setter DI

        var b = orders.CreateOrder(customerId: 1);
        orders.AddItem(b.Id, "BOOK_PRINT", 1);
        orders.AddItem(b.Id, "BOOK_DIGITAL", 1);                  // totale > 50 → -10%
        orders.Checkout(b.Id, customerEmail: "alice@example.com");

        Console.WriteLine("\n=== Demo C: gestione errore stock ===");
        var c = orders.CreateOrder(customerId: 2);
        orders.AddItem(c.Id, "BOOK_PRINT", 1000);                 // forzo insufficienza stock
        try
        {
            orders.Checkout(c.Id, customerEmail: "bob@example.com");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Errore atteso] {ex.Message}");
        }

        Console.WriteLine("\nFine. Premi Invio per uscire.");
        Console.ReadLine();
    }
}
