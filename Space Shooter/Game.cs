using SDL2;
using System;
using System.Numerics;

namespace Space_Shooter
{
    class Game
    {
        private bool isRunning;
        private IntPtr window;
        private Renderer renderer;
        private InputHandler inputHandler;
        private Player player;

        public Game()
        {
            isRunning = true;
            window = IntPtr.Zero;
            renderer = new Renderer();
            inputHandler = new InputHandler();
            player = new Player(100, 100, 50, 50);
        }

        public void Init(string title, int width, int height)
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine($"SDL could not initialize! SDL_Error: {SDL.SDL_GetError()}");
                isRunning = false;
                return;
            }

            window = SDL.SDL_CreateWindow(title, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            if (window == IntPtr.Zero)
            {
                Console.WriteLine($"Window could not be created! SDL_Error: {SDL.SDL_GetError()}");
                isRunning = false;
                return;
            }

            renderer.Init(window);
        }

        public void Run()
        {
            while (isRunning)
            {
                HandleEvents();
                Update();
                Render();
                SDL.SDL_Delay(16); // ~60 FPS
            }

            Cleanup();
        }

        private void HandleEvents()
        {
            isRunning = inputHandler.HandleInput(player);
        }

        private void Update()
        {
            // Update game objects here
            player.Update();
        }

        private void Render()
        {
            renderer.Clear();
            renderer.Draw(player);
            renderer.Present();
        }

        private void Cleanup()
        {
            renderer.Cleanup();
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }
    }
}
