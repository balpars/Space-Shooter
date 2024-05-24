using SDL2;
using System;
using System.Collections.Generic;

namespace Space_Shooter
{
    static class TextureManager
    {
        private static Dictionary<string, IntPtr> textures = new Dictionary<string, IntPtr>();

        public static IntPtr LoadTexture(string path, IntPtr renderer)
        {
            if (textures.TryGetValue(path, out IntPtr existingTexture))
            {
                return existingTexture;  // Return already loaded texture
            }

            IntPtr loadedSurface = SDL_image.IMG_Load(path);
            if (loadedSurface == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load texture {path}! SDL_Error: {SDL.SDL_GetError()}");
                return IntPtr.Zero;
            }

            IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer, loadedSurface);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to create texture from {path}. SDL Error: {SDL.SDL_GetError()}");
            }

            SDL.SDL_FreeSurface(loadedSurface);

            textures[path] = texture;  // Store texture for future use
            return texture;
        }

        public static void Cleanup()
        {
            foreach (var texture in textures.Values)
            {
                SDL.SDL_DestroyTexture(texture);
            }
            textures.Clear();
        }
    }
}
