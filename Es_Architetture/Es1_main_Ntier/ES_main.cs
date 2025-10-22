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

public enum OrderStatus { New, Paid, Shipped, Cancelled }
