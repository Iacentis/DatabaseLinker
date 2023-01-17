namespace ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DatabaseLinker.DatabaseLinkerService databaseLinkerService = new();
            databaseLinkerService.Start(args);
        }
    }
}