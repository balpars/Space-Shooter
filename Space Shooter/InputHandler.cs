using SDL2;
using System;
using System.Numerics;

namespace Space_Shooter
{
    class InputHandler
    {
        public bool HandleInput(Player player)
        {
            SDL.SDL_Event e;
            while (SDL.SDL_PollEvent(out e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    return false;
                }
            }

            int numKeys;
            IntPtr keysPtr = SDL.SDL_GetKeyboardState(out numKeys);
            byte[] keys = new byte[numKeys];
            System.Runtime.InteropServices.Marshal.Copy(keysPtr, keys, 0, numKeys);

            player.HandleInput(keys);
            return true;
        }
    }
}
