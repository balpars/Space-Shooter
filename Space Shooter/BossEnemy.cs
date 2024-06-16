using SDL2;
using System;
using System.Collections.Generic;

namespace Space_Shooter
{
    public class BossEnemy : Enemy
    {
        private int health;
        private Game game;
        private int movePhase; // Track which phase of movement the boss is in
        private uint phaseStartTime;
        private List<SDL.SDL_Point> positions;
        private SDL.SDL_Point currentPosition;
        private SDL.SDL_Point targetPosition;
        private int shootInterval = 1500; // Shoot every second
        private uint lastShootTime;
        private Direction shootDirection;

        public BossEnemy(int x, int y, int size, IntPtr renderer, Game game)
            : base(x, y, size, renderer, 0, 0, 0, game) // Points set to 0 since boss isn't defeated in one hit
        {
            assetPath = "Assets/Enemy/boss_enemy.png";
            texture = SDL_image.IMG_LoadTexture(renderer, assetPath);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load texture {assetPath}! SDL_Error: {SDL.SDL_GetError()}");
            }
            this.game = game;
            this.health = 10; // Boss health set to 10
            this.movePhase = 0;
            this.phaseStartTime = SDL.SDL_GetTicks();
            this.shootDirection = Direction.Straight; // Initialize shoot direction

            // Define the positions for the boss to move to
            positions = new List<SDL.SDL_Point>
            {
                new SDL.SDL_Point { x = game.windowWidth / 2 - size / 2, y = 0 }, // Top
                new SDL.SDL_Point { x = 0, y = game.windowHeight / 2 - size / 2 }, // Left
                new SDL.SDL_Point { x = game.windowWidth - size, y = game.windowHeight / 2 - size / 2 }, // Right
                new SDL.SDL_Point { x = game.windowWidth / 2 - size / 2, y = game.windowHeight - size } // Bottom
            };

            this.currentPosition = new SDL.SDL_Point { x = x, y = y };
            this.targetPosition = positions[movePhase];
            this.lastShootTime = SDL.SDL_GetTicks();
        }

        public override void Update()
        {
            uint currentTime = SDL.SDL_GetTicks();

            // Move towards the target position
            MoveTowardsTarget();

            // Change phase every 10 seconds
            if (currentTime > phaseStartTime + 10000)
            {
                movePhase++;
                if (movePhase >= positions.Count)
                {
                    // Move out of screen after completing all phases
                    targetPosition = new SDL.SDL_Point { x = currentPosition.x, y = game.windowHeight };
                }
                else
                {
                    phaseStartTime = currentTime;
                    targetPosition = positions[movePhase];
                }
            }

            // Shoot towards the player every second
            if (currentTime > lastShootTime + shootInterval)
            {
                Shoot(game.GetProjectiles());
                lastShootTime = currentTime;
            }

            base.Update();
        }

        private void MoveTowardsTarget()
        {
            int speed = 2;
            if (currentPosition.x < targetPosition.x)
            {
                currentPosition.x += speed;
                if (currentPosition.x > targetPosition.x) currentPosition.x = targetPosition.x;
            }
            else if (currentPosition.x > targetPosition.x)
            {
                currentPosition.x -= speed;
                if (currentPosition.x < targetPosition.x) currentPosition.x = targetPosition.x;
            }

            if (currentPosition.y < targetPosition.y)
            {
                currentPosition.y += speed;
                if (currentPosition.y > targetPosition.y) currentPosition.y = targetPosition.y;
            }
            else if (currentPosition.y > targetPosition.y)
            {
                currentPosition.y -= speed;
                if (currentPosition.y < targetPosition.y) currentPosition.y = targetPosition.y;
            }

            rect.x = currentPosition.x;
            rect.y = currentPosition.y;
        }

        public void Shoot(List<Projectile> projectiles)
        {
            SDL.SDL_Rect playerRect = game.GetPlayer().GetCollisionRect();
            int deltaX = playerRect.x + playerRect.w / 2 - (rect.x + rect.w / 2);
            int deltaY = playerRect.y + playerRect.h / 2 - (rect.y + rect.h / 2);
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            float speed = 10;

            int bulletX = rect.x + rect.w / 2 - 10; // Center of the boss
            int bulletY = rect.y + rect.h;

            var bossBullet = new BossProjectile(bulletX, bulletY, 40, false, this)
            {
                SpeedX = (int)(speed * (deltaX / distance)),
                Speed = (int)(speed * (deltaY / distance))
            };

            projectiles.Add(bossBullet);
        }

        public override void OnHit()
        {
            health--;
            if (health <= 0)
            {
                game.IncreaseScore(1000); // Reward for defeating the boss
                base.OnHit();
            }
        }

      

        public new void Cleanup()
        {
            SDL.SDL_DestroyTexture(texture);
        }
    }
}
