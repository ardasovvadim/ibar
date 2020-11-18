using IBAR.SyncerConsole.Initialization;
using System;

namespace IBAR.SyncerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server is running...");

            SyncerApplication.Init();
            SyncerApplication.Run();

            while (true) ;
        }
    }
}
