namespace PostgreSqlBox
{
    class Program
    {
        static void Main()
        {
            SelectAllBenchmark.Run();
            SelectByIndexBenchmark.Run();
        }
    }
}
