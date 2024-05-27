using System;
using System.Collections.Generic;

namespace Space_Shooter
{
    public class CollisionManager
    {
        public static void CheckCollisions(List<Projectile> projectiles, List<Enemy> enemies, Player player)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var projectile = projectiles[i];
                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    var enemy = enemies[j];
                    if (projectile.Owner != enemy && IsColliding(projectile, enemy))
                    {
                        Console.WriteLine($"Collision detected at ({projectile.GetRect().x}, {projectile.GetRect().y})");
                        projectiles.RemoveAt(i);
                        enemies.RemoveAt(j);
                        break; // Break out of the inner loop to avoid issues with index shifting
                    }
                }
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var enemy = enemies[i];
                if (IsColliding(player, enemy))
                {
                    Console.WriteLine($"Collision detected between player and enemy at ({enemy.GetRect().x}, {enemy.GetRect().y})");
                    // Here you can add the logic for what happens when the player collides with an enemy
                    enemies.RemoveAt(i);
                }
            }
        }

        public static bool IsColliding(GameObject object1, GameObject object2)
        {
            var rect1 = object1.GetRect();
            var rect2 = object2.GetRect();

            return rect1.x < rect2.x + rect2.w &&
                   rect1.x + rect1.w > rect2.x &&
                   rect1.y < rect2.y + rect2.h &&
                   rect1.y + rect1.h > rect2.y;
        }
    }
}
