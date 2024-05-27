using SDL2;
using System;
using System.Collections.Generic;

namespace Space_Shooter
{
    public class TextureManager
    {
        private static Dictionary<string, IntPtr> textureMap = new Dictionary<string, IntPtr>();

        public static IntPtr LoadTexture(string fileName, IntPtr renderer)
        {
            if (!textureMap.ContainsKey(fileName))
            {
                IntPtr texture = SDL_image.IMG_LoadTexture(renderer, fileName);
                if (texture == IntPtr.Zero)
                {
                    Console.WriteLine($"Failed to load texture {fileName}: {SDL.SDL_GetError()}");
                    return IntPtr.Zero;
                }
                textureMap[fileName] = texture;
            }
            return textureMap[fileName];
        }

        public static void Clear()
        {
            foreach (var texture in textureMap.Values)
            {
                SDL.SDL_DestroyTexture(texture);
            }
            textureMap.Clear();
        }
    }
}
