using System;
using System.Collections.Generic;

#region SINGLETON: ConfigurazioneSistema
/// <summary>
/// Raccoglie le impostazioni dell'app in un'unica istanza condivisa.
/// </summary>
public sealed class ConfigurazioneSistema
{
    // Lazy + thread-safe
    private static readonly Lazy<ConfigurazioneSistema> _lazy =
        new Lazy<ConfigurazioneSistema>(() => new ConfigurazioneSistema());

    public static ConfigurazioneSistema Instance => _lazy.Value;

    // Dizionario chiave->valore per le configurazioni
    private readonly Dictionary<string, string> _config = new Dictionary<string, string>();

    // Costruttore privato: impedisce new dall'esterno
    private ConfigurazioneSistema() { }

    public void Imposta(string chiave, string valore) => _config[chiave] = valore;

    public string Leggi(string chiave) =>
        _config.TryGetValue(chiave, out var v) ? v : "(non impostato)";

    public void StampaTutte()
    {
        Console.WriteLine("\n== Configurazioni correnti ==");
        if (_config.Count == 0)
        {
            Console.WriteLine("(nessuna)");
            return;
        }
        foreach (var kv in _config)
            Console.WriteLine($"{kv.Key} = {kv.Value}");
    }
}
#endregion

#region FACTORY: dispositivi
public interface IDispositivo
{
    void Avvia();
    void MostraTipo();
}

public class Computer : IDispositivo
{
    public void Avvia()      => Console.WriteLine("Il computer si avvia.");
    public void MostraTipo() => Console.WriteLine("Tipo: Computer");
}

public class Stampante : IDispositivo
{
    public void Avvia()      => Console.WriteLine("La stampante si accende.");
    public void MostraTipo() => Console.WriteLine("Tipo: Stampante");
}

/// <summary>
/// Factory Method semplice: crea IDispositivo a partire da una stringa.
/// </summary>
public static class DispositivoFactory
{
    public static IDispositivo? CreaDispositivo(string tipo)
    {
        switch ((tipo ?? "").Trim().ToLower())
        {
            case "computer":   return new Computer();
            case "stampante":  return new Stampante();
            default:           return null; // tipo non valido
        }
    }
}
#endregion

#region Moduli che USANO Singleton + Factory
public class ModuloA
{
    public void Esegui()
    {
        var cfg = ConfigurazioneSistema.Instance;             // usa il Singleton
        cfg.Imposta("tema", "scuro");
        Console.WriteLine($"[ModuloA] tema = {cfg.Leggi("tema")}");
    }
}

public class ModuloB
{
    public void Esegui()
    {
        var cfg = ConfigurazioneSistema.Instance;             // stessa istanza
        cfg.Imposta("lingua", "it-IT");
        Console.WriteLine($"[ModuloB] lingua = {cfg.Leggi("lingua")}");
    }
}
#endregion

class Program
{
    static void Main()
    {
        // 1) Simula due moduli che lavorano sulla stessa configurazione
        var a = new ModuloA();
        var b = new ModuloB();
        a.Esegui();
        b.Esegui();

        // 2) Verifica che sia la stessa istanza
        Console.WriteLine("\nStessa istanza? " +
            Object.ReferenceEquals(ConfigurazioneSistema.Instance, ConfigurazioneSistema.Instance));

        // 3) Stampa le configurazioni finali
        ConfigurazioneSistema.Instance.StampaTutte();

        // 4) Usa la Factory per creare un dispositivo scelto dall'utente
        Console.Write("\nCrea dispositivo (computer/stampante): ");
        var tipo = Console.ReadLine();
        var disp = DispositivoFactory.CreaDispositivo(tipo);

        if (disp is null)
        {
            Console.WriteLine("Tipo non riconosciuto. Uscita.");
        }
        else
        {
            disp.Avvia();
            disp.MostraTipo();
        }

        Console.WriteLine("\nFine. Premi un tasto per uscire.");
        Console.ReadKey();
    }
}

