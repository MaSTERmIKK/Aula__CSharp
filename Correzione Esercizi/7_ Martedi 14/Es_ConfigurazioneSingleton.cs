using System;
using System.Collections.Generic;

//
// SINGLETON: ConfigurazioneSistema
//
public sealed class ConfigurazioneSistema
{
    // Unica istanza (creazione eager e thread-safe a livello CLR)
    private static readonly ConfigurazioneSistema _instance = new ConfigurazioneSistema();
    public static ConfigurazioneSistema Instance => _instance;

    // Dizionario interno chiave->valore
    private readonly Dictionary<string, string> _config = new Dictionary<string, string>();

    // Costruttore privato
    private ConfigurazioneSistema() { }

    // Aggiunge/aggiorna una configurazione
    public void Imposta(string chiave, string valore)
    {
        _config[chiave] = valore;
    }

    // Ritorna il valore se esiste, altrimenti stringa vuota
    public string Leggi(string chiave)
    {
        return _config.TryGetValue(chiave, out var v) ? v : "";
    }

    // Stampa tutte le configurazioni salvate
    public void StampaTutte()
    {
        Console.WriteLine("== Configurazioni correnti ==");
        foreach (var kv in _config)
            Console.WriteLine($"{kv.Key} = {kv.Value}");
        if (_config.Count == 0) Console.WriteLine("(nessuna)");
    }
}

//
// Simulazione di due moduli che usano lo stesso Singleton
//
public class ModuloA
{
    public void Esegui()
    {
        var cfg = ConfigurazioneSistema.Instance;
        cfg.Imposta("tema", "scuro");
        cfg.Imposta("timeoutSec", "30");
        Console.WriteLine($"[ModuloA] tema = {cfg.Leggi("tema")}");
    }
}

public class ModuloB
{
    public void Esegui()
    {
        var cfg = ConfigurazioneSistema.Instance;
        cfg.Imposta("lingua", "it-IT");
        Console.WriteLine($"[ModuloB] timeoutSec = {cfg.Leggi("timeoutSec")}");
    }
}

class Program
{
    static void Main()
    {
        var a = new ModuloA();
        var b = new ModuloB();

        a.Esegui();
        b.Esegui();

        // Verifica che sia la stessa istanza
        Console.WriteLine("\nStessa istanza? " +
            Object.ReferenceEquals(ConfigurazioneSistema.Instance, ConfigurazioneSistema.Instance));

        // Stampa finale da un unico punto
        Console.WriteLine();
        ConfigurazioneSistema.Instance.StampaTutte();

        Console.WriteLine("\nFine. Premi un tasto per uscire.");
        Console.ReadKey();
    }
}
