using System;
using System.IO;
using System.Text;

#region Contratti
public interface IStorageService
{
    void Save(string fileName, byte[] content);
}
#endregion

#region Implementazioni
public class DiskStorageService : IStorageService
{
    private readonly string _basePath;

    public DiskStorageService(string basePath)
    {
        _basePath = basePath;
        Directory.CreateDirectory(_basePath);
    }

    public void Save(string fileName, byte[] content)
    {
        var fullPath = Path.Combine(_basePath, fileName);
        File.WriteAllBytes(fullPath, content);
        Console.WriteLine($"[Disk] Salvato su {fullPath}");
    }
}

public class MemoryStorageService : IStorageService
{
    // Simulazione semplice: non persistiamo su disco
    public void Save(string fileName, byte[] content)
    {
        Console.WriteLine($"[Memory] Ricevuto {fileName} ({content.Length} bytes) — simulazione OK");
    }
}
#endregion

#region Componente che riceve la dipendenza via COSTRUTTORE
public class FileUploader
{
    private readonly IStorageService _storage;

    // Constructor Injection: dipendenza obbligatoria
    public FileUploader(IStorageService storage)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public void Upload(string fileName, byte[] content)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Nome file non valido.", nameof(fileName));

        _storage.Save(fileName, content);
    }
}
#endregion

class Program
{
    static void Main()
    {
        // Scegli la strategia di storage (Disk o Memory)
        IStorageService storage = new MemoryStorageService();
        // Esempio alternativo su disco:
        // IStorageService storage = new DiskStorageService(Path.Combine(Environment.CurrentDirectory, "uploads"));

        var uploader = new FileUploader(storage);

        // File “simulato”
        var fakeBytes = Encoding.UTF8.GetBytes("Contenuto di esempio");
        uploader.Upload("report.txt", fakeBytes);

        Console.WriteLine("\n[Fine Constructor Injection] Premi un tasto per uscire.");
        Console.ReadKey();
    }
}
