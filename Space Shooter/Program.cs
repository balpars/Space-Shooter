using System;

namespace Space_Shooter
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Init("Space Shooter", 800, 600);
            game.Run();
        }
    }
}
