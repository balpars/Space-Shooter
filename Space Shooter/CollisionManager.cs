// File: CollisionManager.cs

using SDL2;
using System;
using System.Collections.Generic;

namespace Space_Shooter
{
    public static class CollisionManager
    {
        public static void CheckEnemyCollisions(List<Projectile> projectiles, List<Enemy> enemies, Player player, Game game)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var projectile = projectiles[i];
                var projectileRect = projectile.GetRect();

                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    var enemy = enemies[j];
                    var enemyRect = enemy.GetRect();

                    // Skip collision check if the projectile owner is an enemy and the target is also an enemy
                    //if (projectile.Owner is Enemy && projectile.Owner != enemy && IsColliding(projectileRect, enemyRect))
                    if (projectile.Owner is Enemy && IsColliding(projectileRect, enemyRect))
                    {
                        continue;
                    }


                    // Player bullet to Enemy
                    if (projectile.Owner != enemy && IsColliding(projectileRect, enemy.GetCollisionRect()))
                    {
                        int effectX = projectileRect.x - projectileRect.w / 2;
                        int effectY = projectileRect.y - projectileRect.h / 2;
                        game.AddCollisionEffect(effectX, effectY);
                        projectiles.RemoveAt(i);
                        if (!enemy.IsHit())
                        {
                            enemy.OnHit();
                            //game.IncreaseScore(enemy.GetPoints());
                        }
                        game.PlayCollisionSound();
                        break;
                    }
                }

                // Enemy bullet to player
                if (projectile.Owner is Enemy && IsColliding(projectileRect, player.GetCollisionRect()))
                {
                    int effectX = projectileRect.x + projectileRect.w / 2 - 5;
                    int effectY = projectileRect.y + projectileRect.h / 2;
                    game.AddCollisionEffect(effectX, effectY);
                    projectiles.RemoveAt(i);
                    game.PlayCollisionSound();
                    if (player.Health > 0)
                    {
                        player.UpdateHealth(-1); // Reduce health by 1
                    }

                    // else
                    // {
                    //    game.GameOver();
                    // }
                }
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var enemy = enemies[i];
                var enemyRect = enemy.GetRect();
                if (IsColliding(player.GetCollisionRect(), enemyRect))
                {
                    int effectX = enemyRect.x + enemyRect.w / 2;
                    int effectY = enemyRect.y + enemyRect.h / 2;
                    game.AddCollisionEffect(effectX, effectY);
                    if (!enemy.IsHit())
                    {
                        enemy.OnHit();
                        //game.IncreaseScore(enemy.GetPoints());
                    }
                    game.PlayCollisionSound();
                    if (player.Health > 0)
                    {
                        player.UpdateHealth(-1); // Reduce health by 1
                    }
                    //else
                    //{
                    //    game.GameOver();
                    //}
                }
            }
        }

        //public static void CheckRockCollisions(List<Rock> rocks, Player player, Game game)
        public static void CheckRockCollisions(List<Rock> rocks, List<Projectile> projectiles,Player player, Game game)
        {
            for (int i = rocks.Count - 1; i >= 0; i--)
            {
                var rock = rocks[i];
                var rockRect = rock.GetRect();

                // Player Rock Collision
                if (IsColliding(rockRect, player.GetCollisionRect()))
                {
                    int effectX = rockRect.x + rockRect.w / 2;
                    int effectY = rockRect.y + rockRect.h / 2;
                    game.AddCollisionEffect(effectX, effectY);
                    rocks.RemoveAt(i);
                    game.PlayCollisionSound();
                    if (player.Health > 0)
                    {
                        player.UpdateHealth(-1); // Reduce health by 1
                    }
                    //else
                    //{
                    //    game.GameOver();
                    //}
                }



                for (int j = projectiles.Count - 1; j >= 0; j--)
                {
                    var projectile = projectiles[j];
                    var projectileRect = projectile.GetRect();
                    
                    if (IsColliding(projectileRect, rockRect))
                    {
                        int effectX = projectileRect.x - projectileRect.w / 2;
                        int effectY = projectileRect.y - projectileRect.h / 2;
                        game.AddCollisionEffect(effectX, effectY);
                        projectiles.RemoveAt(j);
                        game.PlayCollisionSound();
                        break;
                    }


                }
            }
        }

        public static void CheckHealthBoostCollisions(List<HealthBoost> healthBoosts, Player player, Game game)
        {
            for (int i = healthBoosts.Count - 1; i >= 0; i--)
            {
                var healthBoost = healthBoosts[i];
                var healthBoostRect = healthBoost.GetRect();

                if (IsColliding(healthBoostRect, player.GetCollisionRect()))
                {
                    healthBoosts.RemoveAt(i);   
                    game.PlayHealthBoostSound(); // Play health boost sound
                    player.UpdateHealth(1); // Increase health by 1
                }
            }
        }

        public static void CheckBulletBoostCollisions(List<BulletBoost> bulletBoosts, Player player, Game game)
        {
            for (int i = bulletBoosts.Count - 1; i >= 0; i--)
            {
                var bulletBoost = bulletBoosts[i];
                var bulletBoostRect = bulletBoost.GetRect();

                if (IsColliding(bulletBoostRect, player.GetCollisionRect()))
                {
                    bulletBoosts.RemoveAt(i);
                    player.ActivateTripleShot(10000); // Activate triple shot for 10 seconds
                    player.PlayPowerUpSound(); // Play power-up sound
                }
            }
        }

        public static void CheckShieldBoostCollisions(List<ShieldBoost> shieldBoosts, Player player, Game game)
        {
            for (int i = shieldBoosts.Count - 1; i >= 0; i--)
            {
                var shieldBoost = shieldBoosts[i];
                var shieldBoostRect = shieldBoost.GetRect();

                if (IsColliding(shieldBoostRect, player.GetCollisionRect()))
                {
                    shieldBoosts.RemoveAt(i);
                    player.ActivateShield(10000); // Activate shield for 10 seconds
                    player.PlayPowerUpSound();
                }
            }
        }


        public static bool IsColliding(SDL.SDL_Rect rect1, SDL.SDL_Rect rect2)
        {
            return rect1.x < rect2.x + rect2.w &&
                   rect1.x + rect1.w > rect2.x &&
                   rect1.y < rect2.y + rect2.h &&
                   rect1.y + rect1.h > rect2.y;
        }
    }
}
