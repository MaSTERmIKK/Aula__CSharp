// Componenti reagiscono a "eventi" pubblicati su un bus.
using System;
using System.Collections.Generic;

interface IEvent { }

record ProdottoAggiunto(string Nome) : IEvent;

class EventBus
{
    private readonly Dictionary<Type, List<Action<IEvent>>> _sub = new();

    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        var t = typeof(T);
        if (!_sub.ContainsKey(t)) _sub[t] = new();
        _sub[t].Add(e => handler((T)e));
    }

    public void Publish(IEvent e)
    {
        var t = e.GetType();
        if (_sub.TryGetValue(t, out var handlers))
            foreach (var h in handlers) h(e);
    }
}

class InventarioReadModel
{
    public int Conteggio { get; private set; }
    public void OnProdottoAggiunto(ProdottoAggiunto ev) => Conteggio++;
}

class Program
{
    static void Main()
    {
        var bus = new EventBus();
        var read = new InventarioReadModel();
        bus.Subscribe<ProdottoAggiunto>(read.OnProdottoAggiunto);

        bus.Publish(new ProdottoAggiunto("Pane"));  // nessuna chiamata diretta tra componenti
        bus.Publish(new ProdottoAggiunto("Latte"));

        Console.WriteLine($"Prodotti totali: {read.Conteggio}");
    }
}
