using SDL2;


namespace Space_Shooter
{
    public class AdvancedEnemy : Enemy
    {
        private Direction shootDirection;
        private uint stayDuration = 20000; // 20 seconds in milliseconds
        private uint stayStartTime;
        private bool isStaying;
        private int moveDirection;
        private int moveSpeed = 2; // Speed of lateral movement
        private int hitCount;

        public AdvancedEnemy(int x, int y, int size, IntPtr renderer, int speedX, int speedY, Game game, Direction shootDirection)
            : base(x, y, size, renderer, 250, speedX, speedY, game)
        {
            assetPath = "Assets/Enemy/advanced_enemy.png";
            texture = SDL_image.IMG_LoadTexture(renderer, assetPath);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load texture {assetPath}! SDL_Error: {SDL.SDL_GetError()}");
            }
            this.shootDirection = shootDirection;
            this.stayStartTime = SDL.SDL_GetTicks();
            this.isStaying = true;
            this.moveDirection = speedX > 0 ? 1 : -1;
            this.hitCount = 0; 
        }

        public override void Update()
        {
            uint currentTime = SDL.SDL_GetTicks();

            if (isStaying)
            {
                if (currentTime > stayStartTime + stayDuration)
                {
                    isStaying = false;
                    SpeedY = 2; // Resume downward movement after staying
                }
                else
                {
                    // Move left and right while staying
                    if (rect.x <= 0 || rect.x + rect.w >= game.windowWidth)
                    {
                        moveDirection *= -1; // Change direction if hitting screen edges
                        shootDirection = moveDirection == 1 ? Direction.RightDiagonal : Direction.LeftDiagonal; // Change shoot direction
                    }
                    Move(moveSpeed * moveDirection, 0);
                }
            }
            else
            {
                base.Update();
            }
        }

        public void Shoot(List<Projectile> projectiles)
        {
            int projectileX = rect.x + rect.w / 2;
            int projectileY = rect.y + rect.h;
            var projectile = new AdvancedProjectile(projectileX, projectileY, 20, false, this);
            switch (shootDirection)
            {
                case Direction.LeftDiagonal:
                    projectile.SpeedX = -5;
                    projectile.Speed = 5;
                    break;
                case Direction.RightDiagonal:
                    projectile.SpeedX = 5;
                    projectile.Speed = 5;
                    break;
                case Direction.Straight:
                    projectile.SpeedX = 0;
                    projectile.Speed = 5;
                    break;
            }
            projectiles.Add(projectile);
        }

        public override void OnHit()
        {
            
            hitCount++;
            if (hitCount >= 3)
            {
                game.IncreaseScore(250);
                base.OnHit();
            }
        }
    }

    public enum Direction
    {
        LeftDiagonal,
        RightDiagonal,
        Straight
    }
}
