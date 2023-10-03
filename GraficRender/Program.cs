namespace GraficRender;

internal class Program {
    private static void Main(string[] args) {
        using var mainGame = new MainGame();
        mainGame.Run();
    }
}