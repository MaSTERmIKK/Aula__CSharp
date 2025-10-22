// Program.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

//////////////////////////////
// ======== DOMAIN =========
//////////////////////////////
namespace Domain
{
    public enum OrderStatus { New, Paid, Shipped, Cancelled }

    public record Product(string Code, string Name, decimal Price);

    public record OrderItem(Product Product, int Quantity)
    {
        public decimal LineTotal => Product.Price * Quantity;
    }

    public class Order
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Customer { get; }
        public OrderStatus Status { get; private set; } = OrderStatus.New;
        private readonly List<OrderItem> _items = new();

        public IReadOnlyList<OrderItem> Items => _items;
        public Order(string customer) => Customer = customer;

        public void AddItem(Product p, int qty)
        {
            if (Status != OrderStatus.New) throw new InvalidOperationException("Puoi aggiungere item solo in stato NEW.");
            if (qty <= 0) throw new ArgumentException("Quantità > 0");
            _items.Add(new OrderItem(p, qty));
        }

        public decimal Subtotal() => _items.Sum(i => i.LineTotal);

        public void Pay()    { if (Status != OrderStatus.New) throw new InvalidOperationException(); Status = OrderStatus.Paid; }
        public void Ship()   { if (Status != OrderStatus.Paid) throw new InvalidOperationException(); Status = OrderStatus.Shipped; }
        public void Cancel() { if (Status == OrderStatus.Shipped) throw new InvalidOperationException(); Status = OrderStatus.Cancelled; }
    }

    // Contratti (Domain non dipende da altri layer)
    public interface IOrderRepository
    {
        Order? GetById(Guid id);
        void Add(Order order);
        void Update(Order order);
        IEnumerable<Order> List();
    }

    public interface IProductRepository
    {
        Product? GetByCode(string code);
        void Add(Product product);
        IEnumerable<Product> List();
    }

    public interface INotificationService
    {
        void Send(string subject, string body, string to);
    }

    public interface IConfigurationProvider
    {
        decimal TaxRate { get; }
        string Currency { get; }
    }

    // Singleton (thread-safe) nel Domain
    public sealed class ConfigurationProvider : IConfigurationProvider
    {
        // Esempio: 22% IVA, EUR
        private ConfigurationProvider(decimal taxRate, string currency)
        { TaxRate = taxRate; Currency = currency; }

        private static readonly Lazy<ConfigurationProvider> _lazy =
            new(() => new ConfigurationProvider(0.22m, "EUR"));

        public static ConfigurationProvider Instance => _lazy.Value;

        public decimal TaxRate { get; }
        public string Currency { get; }
    }
}

//////////////////////////////////
// ======== INFRASTRUCTURE =====
//////////////////////////////////
namespace Infrastructure
{
    using Domain;

    public class InMemoryProductRepository : IProductRepository
    {
        private readonly Dictionary<string, Product> _db = new(StringComparer.OrdinalIgnoreCase);
        public void Add(Product product) => _db[product.Code] = product;
        public Product? GetByCode(string code) => _db.TryGetValue(code, out var p) ? p : null;
        public IEnumerable<Product> List() => _db.Values;
    }

    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly Dictionary<Guid, Order> _db = new();
        public void Add(Order order) => _db[order.Id] = order;
        public Order? GetById(Guid id) => _db.TryGetValue(id, out var o) ? o : null;
        public IEnumerable<Order> List() => _db.Values;
        public void Update(Order order) => _db[order.Id] = order;
    }

    public class ConsoleNotificationService : INotificationService
    {
        public void Send(string subject, string body, string to)
        {
            Console.WriteLine($">>> [NOTIFICA] To:{to} | {subject}\n{body}\n");
        }
    }
}

////////////////////////////////
// ======== APPLICATION =======
////////////////////////////////
namespace Application
{
    using Domain;

    public class ProductService
    {
        private readonly IProductRepository _products;

        public ProductService(IProductRepository products)
        { _products = products; }

        public void CreateProduct(string code, string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Codice richiesto.");
            _products.Add(new Product(code, name, price));
        }

        public IEnumerable<Product> List() => _products.List();
        public Product Require(string code) => _products.GetByCode(code)
            ?? throw new InvalidOperationException("Prodotto inesistente.");
    }

    public class OrderService
    {
        private readonly IOrderRepository _orders;
        private readonly ProductService _products;
        private readonly INotificationService _notify;
        private readonly IConfigurationProvider _cfg;

        // DI via costruttore (Application dipende da contratti Domain)
        public OrderService(IOrderRepository orders,
                            ProductService products,
                            INotificationService notify,
                            IConfigurationProvider cfg)
        {
            _orders = orders;
            _products = products;
            _notify  = notify;
            _cfg     = cfg;
        }

        public Order CreateOrder(string customer)
        {
            var o = new Order(customer);
            _orders.Add(o);
            _notify.Send("Nuovo ordine", $"Creato ordine {o.Id} per {customer}", to: customer);
            return o;
        }

        public void AddItem(Guid orderId, string productCode, int qty)
        {
            var o = _orders.GetById(orderId) ?? throw new InvalidOperationException("Ordine non trovato.");
            var p = _products.Require(productCode);
            o.AddItem(p, qty);
            _orders.Update(o);
        }

        public (decimal subtotal, decimal tax, decimal total) ComputeTotals(Guid orderId)
        {
            var o = _orders.GetById(orderId) ?? throw new InvalidOperationException("Ordine non trovato.");
            var sub = o.Subtotal();
            var tax = Math.Round(sub * _cfg.TaxRate, 2);
            var tot = sub + tax;
            return (sub, tax, tot);
        }

        public void Checkout(Guid orderId, string customerEmail)
        {
            var o = _orders.GetById(orderId) ?? throw new InvalidOperationException("Ordine non trovato.");
            o.Pay();
            _orders.Update(o);

            var (sub, tax, tot) = ComputeTotals(orderId);
            _notify.Send("Pagamento ricevuto",
                $"Ordine {o.Id}\nSubtotale: {sub:0.00} {_cfg.Currency}\nIVA: {tax:0.00}\nTotale: {tot:0.00}",
                to: customerEmail);
        }

        public void Ship(Guid orderId, string customerEmail)
        {
            var o = _orders.GetById(orderId) ?? throw new InvalidOperationException("Ordine non trovato.");
            o.Ship();
            _orders.Update(o);
            _notify.Send("Ordine spedito", $"Ordine {o.Id} spedito a {o.Customer}.", to: customerEmail);
        }

        public void Cancel(Guid orderId, string customerEmail)
        {
            var o = _orders.GetById(orderId) ?? throw new InvalidOperationException("Ordine non trovato.");
            o.Cancel();
            _orders.Update(o);
            _notify.Send("Ordine annullato", $"Ordine {o.Id} annullato.", to: customerEmail);
        }

        public IEnumerable<Order> List() => _orders.List();
    }
}

//////////////////////////////////
// ======== PRESENTATION =======
//////////////////////////////////
namespace Presentation
{
    using Domain;
    using Infrastructure;
    using Application;

    class Program
    {
        static void Main()
        {
            // Composition Root: config DI e Singleton
            var services = new ServiceCollection();

            // Domain Singleton registrato come istanza unica
            services.AddSingleton<IConfigurationProvider>(ConfigurationProvider.Instance);

            // Infrastructure
            services.AddSingleton<IProductRepository, InMemoryProductRepository>();
            services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
            services.AddSingleton<INotificationService, ConsoleNotificationService>();

            // Application
            services.AddSingleton<ProductService>(); // riusato da OrderService
            services.AddSingleton<OrderService>();

            var sp = services.BuildServiceProvider();

            // ---- DEMO SEMPLICE (UI testuale super-minima) ----
            var products = sp.GetRequiredService<ProductService>();
            var orders   = sp.GetRequiredService<OrderService>();
            var cfg      = sp.GetRequiredService<IConfigurationProvider>();

            Console.WriteLine($"Configurazione (Singleton): IVA={cfg.TaxRate:P0} Currency={cfg.Currency}\n");

            // Seed prodotti
            products.CreateProduct("PEN",  "Penna Blu", 1.50m);
            products.CreateProduct("NBK",  "Taccuino A5", 4.20m);
            products.CreateProduct("MUG",  "Tazza Logo", 8.90m);

            Console.WriteLine("Prodotti disponibili:");
            foreach (var p in products.List())
                Console.WriteLine($"- {p.Code} | {p.Name} | {p.Price:0.00} {cfg.Currency}");

            // Flusso: crea ordine → aggiungi item → checkout → ship
            var order = orders.CreateOrder("mario.rossi@example.com");
            orders.AddItem(order.Id, "PEN", 2);
            orders.AddItem(order.Id, "MUG", 1);

            var totals = orders.ComputeTotals(order.Id);
            Console.WriteLine($"\nTotali ordine {order.Id}: Sub {totals.subtotal:0.00} + IVA {totals.tax:0.00} = {totals.total:0.00} {cfg.Currency}");

            orders.Checkout(order.Id, order.Customer);
            orders.Ship(order.Id, order.Customer);

            // Stampa elenco ordini
            Console.WriteLine("\nOrdini:");
            foreach (var o in orders.List())
                Console.WriteLine($"- {o.Id} | Cliente:{o.Customer} | Stato:{o.Status} | Righe:{o.Items.Count}");

            Console.WriteLine("\nFine demo. Premi un tasto per uscire.");
            Console.ReadKey();
        }
    }
}
