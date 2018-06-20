namespace PostgreSqlBox
{
    using System;

    class Program
    {
        static void Main()
        {
            Console.WriteLine(nameof(SelectAllBenchmark));
            SelectAllBenchmark.Run();

            Console.WriteLine(nameof(SelectByIndexBenchmark));
            SelectByIndexBenchmark.Run();
        }
    }
}
