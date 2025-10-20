// "Client" chiama un "Server" via interfaccia (simuliamo la rete con una semplice classe).
using System;
using System.Collections.Generic;

interface IServer
{
    IEnumerable<string> GetProdotti();
    void AddProdotto(string nome);
}

class ServerInMemoria : IServer
{
    private readonly List<string> _db = new() { "Pane", "Latte" };
    public IEnumerable<string> GetProdotti() => _db;
    public void AddProdotto(string nome) => _db.Add(nome);
}

class Client
{
    private readonly IServer _server;
    public Client(IServer server) => _server = server;

    public void Run()
    {
        Console.WriteLine("=== Client ===");
        foreach (var p in _server.GetProdotti()) Console.WriteLine($"- {p}");
        _server.AddProdotto("Uova"); // richiesta al "server"
    }
}

class Program
{
    static void Main()
    {
        IServer server = new ServerInMemoria();
        new Client(server).Run();
        Console.WriteLine("=== Stato server ===");
        foreach (var p in server.GetProdotti()) Console.WriteLine($"- {p}");
    }
}
