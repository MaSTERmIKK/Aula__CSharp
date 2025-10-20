// Core (Domain/Application) indipendente da dettagli esterni; gli adapter si agganciano ai "port".
using System;
using System.Collections.Generic;

// Core: Ports
namespace Core.Ports
{
    public interface ProdottoOutPort { void Save(string nome); IEnumerable<string> All(); }
    public interface LogPort { void Info(string msg); }
}

// Core: Application
namespace Core
{
    using Core.Ports;
    public class ProdottoUseCase
    {
        private readonly ProdottoOutPort _repo;
        private readonly LogPort _log;
        public ProdottoUseCase(ProdottoOutPort repo, LogPort log) { _repo = repo; _log = log; }
        public void Crea(string nome) { _repo.Save(nome); _log.Info($"Creato: {nome}"); }
        public IEnumerable<string> Elenca() => _repo.All();
    }
}

// Adapters: Infrastructure
namespace Adapters.Infra
{
    using Core.Ports;
    using System.Linq;
    public class InMemoryRepo : ProdottoOutPort
    {
        private readonly List<string> _db = new() { "Pane" };
        public void Save(string nome) => _db.Add(nome);
        public IEnumerable<string> All() => _db;
    }
    public class ConsoleLogger : LogPort
    {
        public void Info(string msg) => Console.WriteLine($"[LOG] {msg}");
    }
}

// Adapters: UI
class Program
{
    static void Main()
    {
        var uc = new Core.ProdottoUseCase(new Adapters.Infra.InMemoryRepo(), new Adapters.Infra.ConsoleLogger());
        foreach (var p in uc.Elenca()) Console.WriteLine(p);
        uc.Crea("Latte");
    }
}
