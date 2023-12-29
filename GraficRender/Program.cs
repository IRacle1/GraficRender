using System;
using System.Threading.Tasks;

namespace GraficRender;

internal class Program {

    private static MainGame MainGame;
    private static void Main(string[] args) {
        Task.Run(ConsoleHandle);
        MainGame = new MainGame();
        MainGame.Run();
    }

    private static void ConsoleHandle()
    {
        while (true)
        {
            Console.WriteLine("amogus");
            switch (Console.ReadLine())
            {
                case "reset":
                    MainGame.Functions = LoaderHelper.LoadAll(true);
                    break;
                default:
                    break;
            }
        }
    }
}