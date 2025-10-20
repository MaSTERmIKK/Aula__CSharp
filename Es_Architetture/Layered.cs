// UI -> Application -> Domain -> Infrastructure (solo verso il basso).
using System;
using System.Collections.Generic;

// Domain
namespace Domain
{
    public class Prodotto
    {
        public string Nome { get; }
        public Prodotto(string nome) => Nome = nome ?? throw new ArgumentNullException();
    }
    public interface IProdottoStore { IEnumerable<Prodotto> All(); void Add(Prodotto p); }
}

// Infrastructure
namespace Infrastructure
{
    using Domain;
    public class InMemoryStore : IProdottoStore
    {
        private readonly List<Prodotto> _db = new() { new("Pane") };
        public IEnumerable<Prodotto> All() => _db;
        public void Add(Prodotto p) => _db.Add(p);
    }
}

// Application
namespace Application
{
    using Domain;
    public class ProdottoApp
    {
        private readonly IProdottoStore _store;
        public ProdottoApp(IProdottoStore store) => _store = store;
        public IEnumerable<Prodotto> Elenca() => _store.All();
        public void Crea(string nome) => _store.Add(new Prodotto(nome));
    }
}

// UI
class Program
{
    static void Main()
    {
        var app = new Application.ProdottoApp(new Infrastructure.InMemoryStore());
        foreach (var p in app.Elenca()) Console.WriteLine(p.Nome);
        app.Crea("Latte");
    }
}
