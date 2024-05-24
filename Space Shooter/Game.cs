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
        private List<Enemy> enemies;
        private Random random;
        private int spawnInterval;
        private int spawnTimer;

        public Game()
        {
            isRunning = true;
            window = IntPtr.Zero;
            renderer = new Renderer();
            inputHandler = new InputHandler();
            player = new Player(100, 100, 50, 50);
            enemies = new List<Enemy>();
            random = new Random();
            spawnInterval = 1000; // Spawn every 1000ms
            spawnTimer = 0;
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
            player.Update();
            foreach (var enemy in enemies)
            {
                enemy.Update();
            }
            SpawnEnemies();
            RemoveOffScreenEnemies();
        }

        private void Render()
        {
            renderer.Clear();
            renderer.Draw(player);
            foreach (var enemy in enemies)
            {
                renderer.Draw(enemy);
            }
            renderer.Present();
        }

        private void Cleanup()
        {
            renderer.Cleanup();
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }

        private void SpawnEnemies()
        {
            spawnTimer += 16; // Increment timer by ~16ms for each frame
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0;
                int x = random.Next(0, 800 - 50); // Assuming enemy width is 50 and screen width is 800
                enemies.Add(new Enemy(x, -50, 50, 50)); // Spawn above the screen
            }
        }

        private void RemoveOffScreenEnemies()
        {
            enemies.RemoveAll(enemy => enemy.GetRect().y > 600); // Assuming screen height is 600
        }
    }
}
