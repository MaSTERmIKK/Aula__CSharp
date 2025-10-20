// Tutto in un unico "blocco": UI + logica + dati nello stesso eseguibile.
using System;
using System.Collections.Generic;

class Program
{
    // "Database" in memoria
    static List<string> prodotti = new() { "Pane", "Latte" };

    static void Main()
    {
        Console.WriteLine("=== Negozio (Monolite) ===");
        StampaProdotti();                  // UI chiama direttamente logica/dati
        AggiungiProdotto("Uova");          // Logica e dati nello stesso progetto
        StampaProdotti();
    }

    static void StampaProdotti() => prodotti.ForEach(p => Console.WriteLine($"- {p}"));
    static void AggiungiProdotto(string nome) => prodotti.Add(nome);
}
