using SDL2;
using System;

namespace Space_Shooter
{
    public class GameOver
    {
        // add logic
        public void QuitGame()
        {
            // Quit SDL subsystems
            SDL.SDL_Quit();
            // Exit the application
            Environment.Exit(0);
        }
    }
}
