// File: Player.cs

using SDL2;
using System.Collections.Generic;

namespace Space_Shooter
{
    public class Player : GameObject
    {
        protected string assetPath;
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

        public Player(IntPtr renderer, int w, int h, List<Enemy> enemies, Game game, int health) : base((w - 100) / 2, (h - 100) / 2, 100, 100)
        {
            assetPath = "Assets/Player/player.png";
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
                int projectileX = PositionX + rect.w / 2 - 5; // Adjust the x position to center the projectile
                int projectileY = PositionY; // Projectile starts at the top of the player
                projectiles.Add(new BasicProjectile(projectileX, projectileY, 20, true, this)); // Add projectile to the list
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
            return assetPath;
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
    }
}
