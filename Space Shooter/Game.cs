﻿using System;
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
        private EnemyManager enemyManager;
        private List<Enemy> enemyList;
        private List<Background> backgrounds;
        private TitleScreen titleScreen;
        public int windowWidth, windowHeight;
        private GameState gameState;

        public Game()
        {
            isRunning = true;
            window = IntPtr.Zero;
            renderer = new Renderer();
            inputHandler = new InputHandler();
            backgrounds = new List<Background>();
            gameState = GameState.TitleScreen;
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
            enemyManager = new EnemyManager(renderer.RendererHandle, windowWidth, windowHeight, enemyList);

            // Initialize parallax backgrounds
            backgrounds.Add(new Background("Assets/Background/background_1.png", renderer.RendererHandle, 1));
            // Initialize title screen
            titleScreen = new TitleScreen("Assets/TitleScreen/title_screen.png", renderer.RendererHandle);
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
            SDL.SDL_Event e;
            while (SDL.SDL_PollEvent(out e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    isRunning = false;
                }
                else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                {
                    if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                    {
                        isRunning = false;
                    }
                    else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_SPACE)
                    {
                        if (gameState == GameState.TitleScreen)
                        {
                            gameState = GameState.Playing;
                        }
                    }
                }
            }

            // Oyuncu hareketini yalnızca oyun oynanırken işleme
            if (gameState == GameState.Playing)
            {
                int numKeys;
                IntPtr keysPtr = SDL.SDL_GetKeyboardState(out numKeys);
                byte[] keys = new byte[numKeys];
                System.Runtime.InteropServices.Marshal.Copy(keysPtr, keys, 0, numKeys);

                player.HandleInput(keys);
            }
        }

        private void Update()
        {
            if (gameState == GameState.Playing)
            {
                player.Update();
                enemyManager.Update(player);
                foreach (var bg in backgrounds)
                {
                    bg.Update();
                }
            }
        }

        private void Render()
        {
            renderer.Clear();

            if (gameState == GameState.TitleScreen)
            {
                titleScreen.Render(renderer.RendererHandle);
            }
            else if (gameState == GameState.Playing)
            {
                foreach (var bg in backgrounds)
                {
                    renderer.DrawBackground(bg);
                }
                renderer.Draw(player);
                player.RenderProjectiles(renderer);
                enemyManager.Render(renderer);
            }

            renderer.Present();
        }

        private void Cleanup()
        {
            foreach (var bg in backgrounds)
            {
                bg.Cleanup();
            }
            titleScreen.Cleanup();
            renderer.Cleanup();
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }
    }
}
