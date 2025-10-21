public interface IDatabaseService
{
    void Connect();
}

public class SqlDatabaseService : IDatabaseService
{
    public void Connect()
    {
        Console.WriteLine("Connessione al database SQL stabilita.");
    }
}

public class ReportGenerator
{
    public IDatabaseService DatabaseService { get; set; }

    public void GenerateReport()
    {
        if (DatabaseService == null)
        {
            Console.WriteLine("DatabaseService non impostato.");
            return;
        }

        DatabaseService.Connect();
        Console.WriteLine("Generazione report in corso...");
    }
}

var generator = new ReportGenerator();
generator.DatabaseService = new SqlDatabaseService();
generator.GenerateReport();
