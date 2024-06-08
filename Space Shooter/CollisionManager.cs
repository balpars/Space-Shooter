﻿// File: CollisionManager.cs

using SDL2;
using System;
using System.Collections.Generic;

namespace Space_Shooter
{
    public static class CollisionManager
    {
        public static void CheckCollisions(List<Projectile> projectiles, List<Enemy> enemies, Player player, Game game)
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
                    if (projectile.Owner is Enemy && projectile.Owner != enemy && IsColliding(projectileRect, enemyRect))
                    {
                        continue;
                    }
                    if (projectile.Owner != enemy && IsColliding(projectileRect, enemyRect))
                    {
                        int effectX = projectileRect.x + projectileRect.w / 2;
                        int effectY = projectileRect.y + projectileRect.h / 2;
                        game.AddCollisionEffect(effectX, effectY);
                        projectiles.RemoveAt(i);
                        if (!enemy.IsHit())
                        {
                            enemy.OnHit();
                            game.IncreaseScore(enemy.GetPoints());
                        }
                        game.PlayCollisionSound();
                        break;
                    }
                }

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
                    else
                    {
                        game.GameOver();
                    }
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
                        game.IncreaseScore(enemy.GetPoints());
                    }
                    game.PlayCollisionSound();
                    if (player.Health > 0)
                    {
                        player.UpdateHealth(-1); // Reduce health by 1
                    }
                    else
                    {
                        game.GameOver();
                    }
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
