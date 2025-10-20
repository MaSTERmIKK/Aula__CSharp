using System;
using System.Collections.Generic;

#region INTERFACCE
// --- INTERFACCIA OBSERVER ---
public interface IObserver
{
    void NotificaCreazione(string nomeUtente);
}

// --- INTERFACCIA SOGGETTO ---
public interface ISoggetto
{
    void Registra(IObserver o);
    void Rimuovi(IObserver o);
    void Notifica(string nomeUtente);
}
#endregion

#region SINGLETON + SOGGETTO
// --- SINGLETON + SOGGETTO CONCRETO ---
public sealed class GestoreCreazioneUtente : ISoggetto
{
    private static readonly GestoreCreazioneUtente _istanza = new GestoreCreazioneUtente();
    public static GestoreCreazioneUtente Instance => _istanza;

    private readonly List<IObserver> _osservatori = new List<IObserver>();

    // Costruttore privato per garantire il SINGLETON
    private GestoreCreazioneUtente() { }

    public void Registra(IObserver o)
    {
        if (!_osservatori.Contains(o))
            _osservatori.Add(o);
    }

    public void Rimuovi(IObserver o)
    {
        _osservatori.Remove(o);
    }

    public void Notifica(string nomeUtente)
    {
        foreach (var obs in _osservatori)
            obs.NotificaCreazione(nomeUtente);
    }

    // Simula la creazione di un utente e notifica gli observer
    public void CreaUtente(string nome)
    {
        var nuovoUtente = UserFactory.Crea(nome);
        Console.WriteLine($"\n[Gestore] Creato nuovo utente: {nuovoUtente}");
        Notifica(nuovoUtente.Nome);
    }
}
#endregion

#region FACTORY
// --- FACTORY STATIC ---
public static class UserFactory
{
    public static Utente Crea(string nome)
    {
        return new Utente(nome);
    }
}
#endregion

#region MODELLO DI DOMINIO
// --- CLASSE UTENTE ---
public class Utente
{
    public string Nome { get; private set; }

    public Utente(string nome)
    {
        Nome = nome;
    }

    public override string ToString()
    {
        return $"Utente: {Nome}";
    }
}
#endregion

#region OBSERVER CONCRETI
// --- MODULO DI LOG ---
public class ModuloLog : IObserver
{
    public void NotificaCreazione(string nomeUtente)
    {
        Console.WriteLine($"[ModuloLog] Nuovo utente registrato: {nomeUtente} (log salvato).");
    }
}

// --- MODULO DI MARKETING ---
public class ModuloMarketing : IObserver
{
    public void NotificaCreazione(string nomeUtente)
    {
        Console.WriteLine($"[ModuloMarketing] Inviata email di benvenuto a {nomeUtente}!");
    }
}
#endregion

#region MAIN
class Program
{
    static void Main()
    {
        Console.WriteLine("=== Sistema di Creazione Utenti con Singleton + Factory + Observer ===");

        // Ottieni istanza del Singleton
        var gestore = GestoreCreazioneUtente.Instance;

        // Registra i moduli (Observer)
        gestore.Registra(new ModuloLog());
        gestore.Registra(new ModuloMarketing());

        Console.WriteLine("\nInserisci nomi utente da creare (scrivi 'exit' per terminare):");
        while (true)
        {
            Console.Write("→ Nome utente: ");
            string input = Console.ReadLine()?.Trim() ?? "";

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Nome non valido, riprova.");
                continue;
            }

            // Usa la Factory attraverso il Singleton
            gestore.CreaUtente(input);
        }

        Console.WriteLine("\n=== Fine programma ===");
        Console.ReadKey();
    }
}
#endregion
