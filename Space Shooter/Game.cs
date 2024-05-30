using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SDL2;

namespace Space_Shooter
{
    public class Game
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
        private List<CollisionEffect> collisionEffects;
        public int windowWidth, windowHeight;
        private GameState gameState;
        private IntPtr backgroundMusic;
        private IntPtr collisionSound;
        private IntPtr gameController;
        private int score;
        private IntPtr font;
        private IntPtr scoreTexture;
        private SDL.SDL_Rect scoreRect;

        public Game()
        {
            isRunning = true;
            window = IntPtr.Zero;
            renderer = new Renderer();
            inputHandler = new InputHandler();
            backgrounds = new List<Background>();
            collisionEffects = new List<CollisionEffect>();
            gameState = GameState.TitleScreen;
            score = 0;
            scoreTexture = IntPtr.Zero;
        }

        public void Init(string title, int width, int height)
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_GAMECONTROLLER) < 0)
            {
                Console.WriteLine($"SDL could not initialize! SDL_Error: {SDL.SDL_GetError()}");
                isRunning = false;
                return;
            }

            if (SDL_mixer.Mix_OpenAudio(22050, SDL.AUDIO_S16SYS, 2, 4096) == -1)
            {
                Console.WriteLine($"SDL_mixer could not initialize! SDL_mixer Error: {SDL.SDL_GetError()}");
                isRunning = false;
                return;
            }

            if (SDL_ttf.TTF_Init() == -1)
            {
                Console.WriteLine($"SDL_ttf could not initialize! SDL_ttf Error: {SDL.SDL_GetError()}");
                isRunning = false;
                return;
            }

            // Create window with SDL_WINDOW_FULLSCREEN_DESKTOP flag to make it fullscreen
            window = SDL.SDL_CreateWindow(title, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
            if (window == IntPtr.Zero)
            {
                Console.WriteLine($"Window could not be created! SDL_Error: {SDL.SDL_GetError()}");
                isRunning = false;
                return;
            }

            renderer.Init(window);

            SDL.SDL_GetWindowSize(window, out windowWidth, out windowHeight);
            enemyList = new List<Enemy>();
            player = new Player(renderer.RendererHandle, windowWidth, windowHeight, enemyList, this);
            enemyManager = new EnemyManager(renderer.RendererHandle, windowWidth, windowHeight, enemyList, this);

            // Initialize parallax backgrounds
            var bg = new Background("Assets/Background/background_1.png", renderer.RendererHandle, 1, windowWidth, windowHeight);
            backgrounds.Add(bg);

            // Initialize title screen
            titleScreen = new TitleScreen("Assets/TitleScreen/title_screen.png", renderer.RendererHandle);
            titleScreen.SetFullScreen(windowWidth, windowHeight);

            // Load sounds
            backgroundMusic = SDL_mixer.Mix_LoadMUS("Assets/Sounds/background_music.mp3");
            if (backgroundMusic == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to load background music! SDL_mixer Error: {SDL.SDL_GetError()}");
            }

            collisionSound = SoundManager.LoadSound("Assets/Sounds/collision.wav");

            // Open game controller
            if (SDL.SDL_NumJoysticks() > 0)
            {
                gameController = SDL.SDL_GameControllerOpen(0);
                if (gameController == IntPtr.Zero)
                {
                    Console.WriteLine($"Could not open game controller! SDL_Error: {SDL.SDL_GetError()}");
                }
            }

            // Play background music
            SDL_mixer.Mix_PlayMusic(backgroundMusic, -1);

            // Load font for score display
            font = SDL_ttf.TTF_OpenFont("Assets/Fonts/arial.ttf", 24);
            if (font == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to load font! SDL_ttf Error: {SDL.SDL_GetError()}");
            }

            UpdateScoreTexture();
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
                else if (e.type == SDL.SDL_EventType.SDL_CONTROLLERBUTTONDOWN)
                {
                    if (e.cbutton.button == (byte)SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START)
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
                inputHandler.HandleInput(player, gameController);
            }
        }

        private void Update()
        {
            if (gameState == GameState.Playing)
            {
                player.Update();
                enemyManager.Update(player);
                CollisionManager.CheckCollisions(player.GetProjectiles(), enemyList, player, this);
                UpdateCollisionEffects();
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
                    bg.Render(renderer.RendererHandle);
                }
                renderer.Draw(player);
                player.RenderProjectiles(renderer);
                enemyManager.Render(renderer);
                RenderCollisionEffects();
                RenderScore();
            }

            renderer.Present();
        }

        private void UpdateScoreTexture()
        {
            SDL.SDL_Color color = new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 };
            IntPtr surface = SDL_ttf.TTF_RenderText_Solid(font, $"Score: {score}", color);

            if (surface == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to create surface for score text! SDL_ttf Error: {SDL.SDL_GetError()}");
                return;
            }

            IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer.RendererHandle, surface);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to create texture for score text! SDL_Error: {SDL.SDL_GetError()}");
                SDL.SDL_FreeSurface(surface);
                return;
            }

            SDL.SDL_Surface sdlSurface = Marshal.PtrToStructure<SDL.SDL_Surface>(surface);

            scoreRect = new SDL.SDL_Rect
            {
                x = 10,
                y = 10,
                w = sdlSurface.w,
                h = sdlSurface.h
            };

            SDL.SDL_FreeSurface(surface);

            if (scoreTexture != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(scoreTexture);
            }
            scoreTexture = texture;
        }

        private void RenderScore()
        {
            SDL.SDL_RenderCopy(renderer.RendererHandle, scoreTexture, IntPtr.Zero, ref scoreRect);
        }

        public void IncreaseScore(int points)
        {
            score += points;
            UpdateScoreTexture();
        }

        private void UpdateCollisionEffects()
        {
            for (int i = collisionEffects.Count - 1; i >= 0; i--)
            {
                if (!collisionEffects[i].IsActive())
                {
                    collisionEffects[i].Cleanup();
                    collisionEffects.RemoveAt(i);
                }
            }
        }

        private void RenderCollisionEffects()
        {
            foreach (var effect in collisionEffects)
            {
                effect.Render(renderer.RendererHandle);
            }
        }

        private void Cleanup()
        {
            foreach (var bg in backgrounds)
            {
                bg.Cleanup();
            }
            titleScreen.Cleanup();
            renderer.Cleanup();

            foreach (var effect in collisionEffects)
            {
                effect.Cleanup();
            }

            // Free sounds
            SDL_mixer.Mix_FreeMusic(backgroundMusic);
            SDL_mixer.Mix_FreeChunk(collisionSound);
            SoundManager.Cleanup();

            if (gameController != IntPtr.Zero)
            {
                SDL.SDL_GameControllerClose(gameController);
            }

            SDL.SDL_DestroyTexture(scoreTexture);
            SDL_ttf.TTF_CloseFont(font);

            if (window != IntPtr.Zero)
            {
                SDL.SDL_DestroyWindow(window);
            }
            SDL.SDL_Quit();
        }

        public void PlayCollisionSound()
        {
            SoundManager.PlaySound(collisionSound);
        }

        public void AddCollisionEffect(int x, int y)
        {
            collisionEffects.Add(new CollisionEffect(x, y, 50, "Assets/Effects/collision.png", 500, renderer.RendererHandle));
        }
    }
}
