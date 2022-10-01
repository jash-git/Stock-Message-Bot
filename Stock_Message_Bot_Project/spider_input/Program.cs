namespace spider_input
{
    static class Program
    {
        public static void pause()
        {
            Console.WriteLine("Press any key to terminate...");
            Console.ReadKey();
        }
        static void Main(string[] args)
        {
            LogFile.Write("spider_input Start");
            LogFile.CleanLog();
            pause();
            LogFile.Write("spider_input Close");
        }
    }
}