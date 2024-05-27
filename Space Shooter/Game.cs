using System;
using System.Collections.Generic;
using SDL2;

namespace Space_Shooter
{
    class Game
    {
        private bool isRunning;
        private IntPtr window;
        private Renderer renderer;
        private InputHandler inputHandler;
        private Player player;
        private SpawnEnemy spawnEnemy;
        private List<Enemy> enemyList;
        private List<Background> backgrounds;
        public int windowWidth, windowHeight;
        


        public Game()
        {
            isRunning = true;
            window = IntPtr.Zero;
            renderer = new Renderer();
            inputHandler = new InputHandler();
            backgrounds = new List<Background>();
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

            SDL.SDL_GetWindowSize(window, out windowWidth, out windowHeight);
            enemyList = new List<Enemy>();
            player = new Player(renderer.RendererHandle, windowWidth, windowHeight, enemyList);
            spawnEnemy = new SpawnEnemy(renderer.RendererHandle, windowWidth, windowHeight, enemyList);

            // Initialize parallax backgrounds
            backgrounds.Add(new Background("Assets/Background/background_1.png", renderer.RendererHandle, 1));
            //backgrounds.Add(new Background("Assets/Background/background_2.png", renderer.RendererHandle, 2));
            //backgrounds.Add(new Background("Assets/Background/background_3.png", renderer.RendererHandle, 3));
            //backgrounds.Add(new Background("Assets/Background/background_4.png", renderer.RendererHandle, 4));
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
            player.Update();
            spawnEnemy.Update(player);
            foreach (var bg in backgrounds)
            {
                bg.Update();
            }
        }

        private void Render()
        {
            renderer.Clear();
            foreach (var bg in backgrounds)
            {
                renderer.DrawBackground(bg);
            }
            renderer.Draw(player);
            player.RenderProjectiles(renderer);
            spawnEnemy.Render(renderer);
            renderer.Present();
        }

        private void Cleanup()
        {
            foreach (var bg in backgrounds)
            {
                bg.Cleanup();
            }
            renderer.Cleanup();
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }
    }
}
