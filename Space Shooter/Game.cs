// File: Game.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SDL2;

namespace Space_Shooter
{
    public class Game
    {
        private bool isRunning;
        private IntPtr window;
        public Renderer renderer;
        private InputHandler inputHandler;
        private Player player;
        private EnemyManager enemyManager;
        private List<Enemy> enemyList;
        private List<Background> backgrounds;
        private TitleScreen titleScreen;
        private List<CollisionEffect> collisionEffects;
        private List<Projectile> projectiles;
        public int windowWidth, windowHeight;
        private GameState gameState;
        private IntPtr backgroundMusic;
        private IntPtr collisionSound;
        private IntPtr healthBoostSound; // Add this line
        private IntPtr gameController;
        private int score;
        private int highScore;
        private string highScoreFile = "highscore.txt"; // High score file
        private IntPtr font;
        private IntPtr scoreTexture;
        private SDL.SDL_Rect scoreRect;
        private SDL.SDL_Rect highScoreRect; // High score rect
        private IntPtr highScoreTexture; // High score texture
        private int playerHealth;
        private GameOver gameOver;
        private IntPtr gameOverSound;
        private bool scoreTransformed;
        private uint scoreTransformStartTime;
        private uint scoreTransformDuration = 500; // Duration in milliseconds
        private bool isFastMode; // To track if fast mode is activated

        public Game()
        {
            isRunning = true;
            window = IntPtr.Zero;
            renderer = new Renderer();
            inputHandler = new InputHandler();
            backgrounds = new List<Background>();
            collisionEffects = new List<CollisionEffect>();
            projectiles = new List<Projectile>();
            gameState = GameState.TitleScreen;
            score = 0;
            scoreTexture = IntPtr.Zero;
            highScoreTexture = IntPtr.Zero; // Initialize high score texture
            playerHealth = 5;
            scoreTransformed = false;
            scoreTransformStartTime = 0;
            isFastMode = false;
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
            player = new Player(renderer.RendererHandle, windowWidth, windowHeight, enemyList, this, playerHealth);
            enemyManager = new EnemyManager(renderer.RendererHandle, windowWidth, windowHeight, enemyList, this);

            var bg = new Background("Assets/Background/background_1.png", renderer.RendererHandle, 1, windowWidth, windowHeight);
            backgrounds.Add(bg);

            titleScreen = new TitleScreen("Assets/TitleScreen/title_screen.png", renderer.RendererHandle);
            titleScreen.SetFullScreen(windowWidth, windowHeight);

            backgroundMusic = SDL_mixer.Mix_LoadMUS("Assets/Sounds/background_music.mp3");
            if (backgroundMusic == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to load background music! SDL_mixer Error: {SDL.SDL_GetError()}");
            }

            collisionSound = SoundManager.LoadSound("Assets/Sounds/collision.wav");
            healthBoostSound = SoundManager.LoadSound("Assets/Sounds/health_boost.wav"); // Add this line
            gameOverSound = SoundManager.LoadSound("Assets/Sounds/game_over.wav");

            if (SDL.SDL_NumJoysticks() > 0)
            {
                gameController = SDL.SDL_GameControllerOpen(0);
                if (gameController == IntPtr.Zero)
                {
                    Console.WriteLine($"Could not open game controller! SDL_Error: {SDL.SDL_GetError()}");
                }
            }

            SDL_mixer.Mix_PlayMusic(backgroundMusic, -1);

            font = SDL_ttf.TTF_OpenFont("Assets/Fonts/arial.ttf", 24);
            if (font == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to load font! SDL_ttf Error: {SDL.SDL_GetError()}");
            }

            LoadHighScore(); // Load high score from file

            UpdateScoreTexture();
            UpdateHighScoreTexture(); // Update high score texture

            gameOver = new GameOver("Assets/GameOver/game_over.png", renderer.RendererHandle, windowWidth, windowHeight);
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

            SaveHighScore(); // Save high score to file
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
                        else if (gameState == GameState.GameOver)
                        {
                            RestartGame();
                        }
                    }
                }
                else if (e.type == SDL.SDL_EventType.SDL_CONTROLLERBUTTONDOWN)
                {
                    if (e.cbutton.button == (byte)SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START ||
                        e.cbutton.button == (byte)SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X)
                    {
                        if (gameState == GameState.TitleScreen)
                        {
                            gameState = GameState.Playing;
                        }
                        else if (gameState == GameState.GameOver)
                        {
                            RestartGame();
                        }
                    }
                }
            }

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

                // Activate fast mode if score > 1000 and not already activated
                if (score > 1000 && !isFastMode)
                {
                    isFastMode = true;
                    enemyManager.SetFastMode(true);
                    foreach (var bg in backgrounds)
                    {
                        bg.SetFastMode(true);
                    }
                }

                // Handle score transformation timing
                if (scoreTransformed && SDL.SDL_GetTicks() - scoreTransformStartTime > scoreTransformDuration)
                {
                    scoreTransformed = false;
                    UpdateScoreTexture();
                }

                // Update high score if needed
                if (score > highScore)
                {
                    highScore = score;
                    UpdateHighScoreTexture();
                }
            }
        }

        public void Render()
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
                player.RenderHearts(renderer);
                enemyManager.Render(renderer);
                RenderCollisionEffects();
                RenderScore();
                RenderHighScore(); // Render high score
            }
            else if (gameState == GameState.GameOver)
            {
                gameOver.Render(renderer.RendererHandle);
            }

            renderer.Present();
        }

        private void UpdateScoreTexture()
        {
            SDL.SDL_Color color = scoreTransformed ? new SDL.SDL_Color { r = 255, g = 0, b = 0, a = 255 } : new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 };
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
                w = scoreTransformed ? sdlSurface.w * 2 : sdlSurface.w,
                h = scoreTransformed ? sdlSurface.h * 2 : sdlSurface.h
            };

            SDL.SDL_FreeSurface(surface);

            if (scoreTexture != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(scoreTexture);
            }
            scoreTexture = texture;
        }

        private void UpdateHighScoreTexture()
        {
            SDL.SDL_Color color = new SDL.SDL_Color { r = 255, g = 255, b = 0, a = 255 };
            IntPtr surface = SDL_ttf.TTF_RenderText_Solid(font, $"High Score: {highScore}", color);

            if (surface == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to create surface for high score text! SDL_ttf Error: {SDL.SDL_GetError()}");
                return;
            }

            IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer.RendererHandle, surface);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to create texture for high score text! SDL_Error: {SDL.SDL_GetError()}");
                SDL.SDL_FreeSurface(surface);
                return;
            }

            SDL.SDL_Surface sdlSurface = Marshal.PtrToStructure<SDL.SDL_Surface>(surface);

            highScoreRect = new SDL.SDL_Rect
            {
                x = windowWidth - sdlSurface.w - 10,
                y = 10,
                w = sdlSurface.w,
                h = sdlSurface.h
            };

            SDL.SDL_FreeSurface(surface);

            if (highScoreTexture != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(highScoreTexture);
            }
            highScoreTexture = texture;
        }

        private void RenderScore()
        {
            SDL.SDL_RenderCopy(renderer.RendererHandle, scoreTexture, IntPtr.Zero, ref scoreRect);
        }

        private void RenderHighScore()
        {
            SDL.SDL_RenderCopy(renderer.RendererHandle, highScoreTexture, IntPtr.Zero, ref highScoreRect);
        }

        public void IncreaseScore(int points)
        {
            score += points;
            scoreTransformed = true;
            scoreTransformStartTime = SDL.SDL_GetTicks();
            UpdateScoreTexture();
        }

        public int GetScore()
        {
            return score;
        }

        private void LoadHighScore()
        {
            if (File.Exists(highScoreFile))
            {
                string fileContent = File.ReadAllText(highScoreFile);
                int.TryParse(fileContent, out highScore);
            }
            else
            {
                highScore = 0;
            }
        }

        private void SaveHighScore()
        {
            File.WriteAllText(highScoreFile, highScore.ToString());
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

            SDL_mixer.Mix_FreeMusic(backgroundMusic);
            SDL_mixer.Mix_FreeChunk(collisionSound);
            SDL_mixer.Mix_FreeChunk(healthBoostSound); // Add this line
            SDL_mixer.Mix_FreeChunk(gameOverSound);
            SoundManager.Cleanup();

            if (gameController != IntPtr.Zero)
            {
                SDL.SDL_GameControllerClose(gameController);
            }

            SDL.SDL_DestroyTexture(scoreTexture);
            SDL.SDL_DestroyTexture(highScoreTexture); // Destroy high score texture
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

        public void PlayHealthBoostSound() // Add this method
        {
            SoundManager.PlaySound(healthBoostSound);
        }

        public void AddCollisionEffect(int x, int y)
        {
            collisionEffects.Add(new CollisionEffect(x, y, 50, "Assets/Effects/collision.png", 500, renderer.RendererHandle));
        }

        public void GameOver()
        {
            gameState = GameState.GameOver;
            SoundManager.PlaySound(gameOverSound);
        }

        private void RestartGame()
        {
            // Reset game state
            score = 0;
            playerHealth = 5;
            enemyList.Clear();
            projectiles.Clear();
            player = new Player(renderer.RendererHandle, windowWidth, windowHeight, enemyList, this, playerHealth);
            enemyManager = new EnemyManager(renderer.RendererHandle, windowWidth, windowHeight, enemyList, this);
            UpdateScoreTexture();
            gameState = GameState.Playing;
        }
    }
}
