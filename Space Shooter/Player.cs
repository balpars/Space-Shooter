using SDL2;
using System.Collections.Generic;

namespace Space_Shooter
{
    public class Player : GameObject
    {
        protected string assetPath;
        public int PositionX { get; private set; }
        public int PositionY { get; private set; }
        private int screenWidth;
        private int screenHeight;
        private List<Projectile> projectiles;
        private List<Enemy> enemies;
        private uint lastShootTime;
        private int shootInterval = 500; // milliseconds
        private Game game;

        public Player(IntPtr renderer, int w, int h, List<Enemy> enemies, Game game) : base((w - 100) / 2, (h - 100) / 2, 100, 100)
        {
            assetPath = "Assets/Player/player.png";
            this.PositionX = (w - 100) / 2;
            this.PositionY = (h - 100) / 2;
            this.screenWidth = w;
            this.screenHeight = h;
            this.enemies = enemies;
            this.game = game;
            projectiles = new List<Projectile>();
            lastShootTime = SDL.SDL_GetTicks();
        }

        public override void Update()
        {
            UpdateProjectiles();
            CollisionManager.CheckCollisions(projectiles, enemies, this, game);
        }

        public void HandleInput(byte[] keys)
        {
            int newX = PositionX;
            int newY = PositionY;

            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] == 1)
            {
                newY -= 5;
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN] == 1)
            {
                newY += 5;
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1)
            {
                newX -= 5;
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1)
            {
                newX += 5;
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE] == 1)
            {
                Shoot();
            }

            // Ekran sınırları içinde kalmayı sağla
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

        public void RenderProjectiles(Renderer renderer)
        {
            foreach (var projectile in projectiles)
            {
                projectile.Render(renderer);
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
    }
}
