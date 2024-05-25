using System;
using System.Collections.Generic;

namespace Space_Shooter
{
    public class CollisionManager
    {
        public static void CheckCollisions(List<Projectile> projectiles, List<Enemy> enemies)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var projectile = projectiles[i];
                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    var enemy = enemies[j];
                    if (IsColliding(projectile, enemy))
                    {
                        Console.WriteLine($"Collision detected at ({projectile.GetRect().x}, {projectile.GetRect().y})");
                        projectiles.RemoveAt(i);
                        enemies.RemoveAt(j);
                        break; // Break out of the inner loop to avoid issues with index shifting
                    }
                }
            }
        }

        public static bool IsColliding(Projectile projectile, Enemy enemy)
        {
            int objectSize = projectile.GetRect().w; // Assuming projectile size is used for collision detection
            return projectile.GetRect().x < enemy.GetRect().x + objectSize &&
                   projectile.GetRect().x + objectSize > enemy.GetRect().x &&
                   projectile.GetRect().y < enemy.GetRect().y + objectSize &&
                   projectile.GetRect().y + objectSize > enemy.GetRect().y;
        }
    }
}
