using SDL2;
using System;

namespace Space_Shooter
{
    public class InputHandler
    {
        public void HandleInput(Player player, IntPtr gameController)
        {
            int numKeys;
            IntPtr keysPtr = SDL.SDL_GetKeyboardState(out numKeys);
            byte[] keys = new byte[numKeys];
            System.Runtime.InteropServices.Marshal.Copy(keysPtr, keys, 0, numKeys);

            player.HandleInput(keys, gameController);
        }
    }
}
