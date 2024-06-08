// File: Player.cs

using SDL2;
using System.Collections.Generic;

namespace Space_Shooter
{
    public class Player : GameObject
    {
        protected string[] assetPaths;
        public int PositionX { get; private set; }
        public int PositionY { get; private set; }
        public int Health;
        private int screenWidth;
        private int screenHeight;
        private List<Projectile> projectiles;
        private List<Enemy> enemies;
        private uint lastShootTime;
        private int shootInterval = 200; // milliseconds
        private Game game;
        private int speed;
        public List<Heart> hearts;
        private IntPtr shootSound;
        private Dictionary<int, IntPtr> textures; // Store textures based on health
        private IntPtr texture; // Current texture

        public Player(IntPtr renderer, int w, int h, List<Enemy> enemies, Game game, int health) : base((w - 100) / 2, (h - 100) / 2, 100, 100)
        {
            assetPaths = new string[] {
                "Assets/Player/player_1.png",
                "Assets/Player/player_2.png",
                "Assets/Player/player_3.png",
                "Assets/Player/player_4.png"
            };

            this.PositionX = (w - 100) / 2;
            this.PositionY = (h - 100) / 2;
            this.screenWidth = w;
            this.screenHeight = h;
            this.enemies = enemies;
            this.game = game;
            this.speed = 5;
            this.Health = health;
            projectiles = new List<Projectile>();
            lastShootTime = SDL.SDL_GetTicks();
            hearts = new List<Heart>();
            for (int i = 0; i < health; i++)
            {
                hearts.Add(new Heart((screenWidth / 2) - (health * 15) + (i * 30), 20, 70)); // Adjust the position and size as needed
            }

            shootSound = SoundManager.LoadSound("Assets/Sounds/shoot.wav");

            // Load textures
            textures = new Dictionary<int, IntPtr>();
            for (int i = 0; i < assetPaths.Length; i++)
            {
                IntPtr texture = SDL_image.IMG_LoadTexture(renderer, assetPaths[i]);
                textures.Add(i + 1, texture);
            }

            // Set initial texture
            SetTexture(renderer);
        }

        public override void Update()
        {
            UpdateProjectiles();
            UpdateHearts();
            if (Health <= 0)
            {
                game.GameOver(); // Trigger game over if health is zero
            }
            else
            {
                CollisionManager.CheckCollisions(projectiles, enemies, this, game);
            }
        }

        public void HandleInput(byte[] keys, IntPtr gameController)
        {
            int newX = PositionX;
            int newY = PositionY;

            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] == 1)
            {
                newY -= speed;
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN] == 1)
            {
                newY += speed;
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1)
            {
                newX -= speed;
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1)
            {
                newX += speed;
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE] == 1)
            {
                Shoot();
            }

            if (gameController != IntPtr.Zero)
            {
                int leftX = SDL.SDL_GameControllerGetAxis(gameController, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX);
                int leftY = SDL.SDL_GameControllerGetAxis(gameController, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY);

                const int DEAD_ZONE = 8000;

                if (leftX < -DEAD_ZONE)
                {
                    newX -= speed;
                }
                if (leftX > DEAD_ZONE)
                {
                    newX += speed;
                }
                if (leftY < -DEAD_ZONE)
                {
                    newY -= speed;
                }
                if (leftY > DEAD_ZONE)
                {
                    newY += speed;
                }

                if (SDL.SDL_GameControllerGetButton(gameController, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X) == 1)
                {
                    Shoot();
                }
            }

            // Ensure the player stays within the screen bounds
            if (newX < 0) newX = 0;
            if (newX + rect.w > screenWidth) newX = screenWidth - rect.w;
            if (newY < 0) newY = 0;
            if (newY + rect.h > screenHeight) newY = screenHeight - rect.h;

            Move(newX - PositionX, newY - PositionY);

            PositionX = newX;
            PositionY = newY;
        }

        private void Shoot()
        {
            uint currentTime = SDL.SDL_GetTicks();
            if (currentTime > lastShootTime + shootInterval)
            {
                int projectileXCenter = PositionX + rect.w / 2 - 5; // Adjust the x position to center the projectile
                int projectileY = PositionY; // Projectile starts at the top of the player

                // Single projectile
                var centerProjectile = new BasicProjectile(projectileXCenter, projectileY, 20, true, this);
                centerProjectile.SetSpeed(-10); // Set speed to move upwards
                projectiles.Add(centerProjectile);

                // Additional projectiles if score > 1000
                if (game.GetScore() > 1000)
                {
                    int projectileXLeft = PositionX + rect.w / 4 - 5; // Adjust the x position for the left projectile
                    int projectileXRight = PositionX + 3 * rect.w / 4 - 5; // Adjust the x position for the right projectile

                    var leftProjectile = new BasicProjectile(projectileXLeft, projectileY, 20, true, this);
                    var rightProjectile = new BasicProjectile(projectileXRight, projectileY, 20, true, this);

                    leftProjectile.SetSpeed(-10); // Set speed to move upwards
                    rightProjectile.SetSpeed(-10); // Set speed to move upwards

                    projectiles.Add(leftProjectile);
                    projectiles.Add(rightProjectile);
                }

                lastShootTime = currentTime;
                SoundManager.PlaySound(shootSound); // Play the shoot sound
            }
        }

        private void UpdateProjectiles()
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();
                if (projectiles[i].GetRect().y < 0)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        public void UpdateHearts()
        {
            hearts.Clear();
            for (int i = 0; i < Health; i++)
            {
                hearts.Add(new Heart((screenWidth / 2) - (Health * 15) + (i * 30), 20, 70)); // Adjust the position and size as needed
            }
        }

        public void RenderProjectiles(Renderer renderer)
        {
            foreach (var projectile in projectiles)
            {
                projectile.Render(renderer);
            }
        }

        public void RenderHearts(Renderer renderer)
        {
            foreach (var heart in hearts)
            {
                heart.Render(renderer);
            }
        }

        public string? GetAssetPath()
        {
            int assetIndex = Health switch
            {
                5 => 3, // player_4.png
                4 => 2, // player_3.png
                3 => 1, // player_2.png
                _ => 0  // player_1.png for 2 and 1 health
            };
            return assetPaths[assetIndex]; // Return the current asset path based on health
        }

        public List<Projectile> GetProjectiles()
        {
            return projectiles;
        }

        public SDL.SDL_Rect GetCollisionRect()
        {
            // Return a smaller rectangle for collision detection
            int collisionWidth = rect.w / 3;
            int collisionHeight = rect.h / 3;
            return new SDL.SDL_Rect
            {
                x = rect.x + (rect.w - collisionWidth) / 2 - 5,
                y = rect.y + (rect.h - collisionHeight) / 2 + 5,
                w = collisionWidth,
                h = collisionHeight
            };
        }

        public void SetTexture(IntPtr renderer)
        {
            int assetIndex = Health switch
            {
                5 => 4, // player_4.png
                4 => 3, // player_3.png
                3 => 2, // player_2.png
                _ => 1  // player_1.png for 2 and 1 health
            };
            if (textures.ContainsKey(assetIndex))
            {
                SDL.SDL_DestroyTexture(texture); // Destroy the previous texture
                texture = textures[assetIndex];
            }
        }

        public void UpdateHealth(int amount)
        {
            Health += amount;
            if (Health < 0)
            {
                Health = 0;
            }
            SetTexture(game.renderer.RendererHandle); // Update the texture based on the new health
        }
    }
}
