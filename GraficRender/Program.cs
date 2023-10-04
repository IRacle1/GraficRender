using System;
using System.Threading.Tasks;

namespace GraficRender;

internal class Program {
    private static void Main(string[] args) {
        Task.Run(ConsoleHandle);
        using var mainGame = new MainGame();
        mainGame.Run();
    }

    private static void ConsoleHandle()
    {
        while (true)
        {
            Console.WriteLine("amogus");
            Console.ReadLine();
        }
    }
}