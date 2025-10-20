// Ogni microservizio ha il proprio modello, storage, e endpoint.
// Qui simuliamo due microservizi in processi logici distinti.

using System;
using System.Collections.Generic;

// Microservizio Catalogo
namespace Catalogo
{
    public record Prodotto(string Sku, string Nome);
    public class Api
    {
        private readonly List<Prodotto> _db = new() { new("SKU1","Pane"), new("SKU2","Latte") };
        public IEnumerable<Prodotto> Get() => _db;         // GET /catalogo
    }
}

// Microservizio Ordini
namespace Ordini
{
    public record RigaOrdine(string Sku, int Qta);
    public class Api
    {
        public void Post(IEnumerable<RigaOrdine> righe)     // POST /ordini
            => Console.WriteLine($"Creato ordine con {System.Linq.Enumerable.Count(righe)} righe");
    }
}

class Program
{
    static void Main()
    {
        var catalogoApi = new Catalogo.Api();
        var ordiniApi = new Ordini.Api();

        // "Chiamata" tra servizi (in realtà via HTTP/queue; qui diretto per brevità)
        foreach (var p in catalogoApi.Get()) Console.WriteLine($"Catalogo: {p.Sku} - {p.Nome}");
        ordiniApi.Post(new[] { new Ordini.RigaOrdine("SKU1", 2) });
    }
}
