// Ogni cambio stato è un evento persistito; letture e scritture sono separate (CQRS).
using System;
using System.Collections.Generic;
using System.Linq;

interface IEvent { DateTime When { get; } }
record ProdottoCreato(string Nome, DateTime When) : IEvent;

class EventStore
{
    private readonly List<IEvent> _events = new();
    public void Append(IEvent e) => _events.Add(e);
    public IEnumerable<IEvent> All() => _events;
}

// WRITE (Command)
class CreaProdottoCommand { public string Nome { get; init; } = ""; }

class CommandHandler
{
    private readonly EventStore _store;
    public CommandHandler(EventStore store) => _store = store;
    public void Handle(CreaProdottoCommand cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd.Nome)) throw new ArgumentException("Nome vuoto");
        _store.Append(new ProdottoCreato(cmd.Nome, DateTime.UtcNow));
    }
}

// READ (Query) – ricostruisce uno stato "derivato"
class CatalogoReadModel
{
    public IEnumerable<string> Nomi { get; private set; } = Enumerable.Empty<string>();
    public void Rebuild(IEnumerable<IEvent> events) =>
        Nomi = events.OfType<ProdottoCreato>().Select(e => e.Nome).ToList();
}

class Program
{
    static void Main()
    {
        var store = new EventStore();
        var handler = new CommandHandler(store);

        handler.Handle(new CreaProdottoCommand { Nome = "Pane" });
        handler.Handle(new CreaProdottoCommand { Nome = "Latte" });

        var read = new CatalogoReadModel();
        read.Rebuild(store.All()); // proietta gli eventi in un modello di lettura
        Console.WriteLine(string.Join(", ", read.Nomi));
    }
}
