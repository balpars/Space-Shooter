using System;
using SDL2;

namespace Space_Shooter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize SDL
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine($"SDL could not initialize! SDL_Error: {SDL.SDL_GetError()}");
                return;
            }

            // Create window
            IntPtr window = SDL.SDL_CreateWindow("Space Shooter", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, 800, 600, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            if (window == IntPtr.Zero)
            {
                Console.WriteLine($"Window could not be created! SDL_Error: {SDL.SDL_GetError()}");
                SDL.SDL_Quit();
                return;
            }

            // Create renderer
            IntPtr renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine($"Renderer could not be created! SDL_Error: {SDL.SDL_GetError()}");
                SDL.SDL_DestroyWindow(window);
                SDL.SDL_Quit();
                return;
            }

            // Main loop flag
            bool quit = false;

            // Event handler
            SDL.SDL_Event e;

            // Rectangle position and size
            SDL.SDL_Rect fillRect = new SDL.SDL_Rect { x = 100, y = 100, w = 50, h = 50 };

            // Set initial draw color for the rectangle
            SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0x00, 0x00, 0xFF);

            // While application is running
            while (!quit)
            {
                // Handle events on queue
                while (SDL.SDL_PollEvent(out e) != 0)
                {
                    // User requests quit
                    if (e.type == SDL.SDL_EventType.SDL_QUIT)
                    {
                        quit = true;
                    }
                }

                // Get the state of the keyboard
                int numKeys;
                IntPtr keysPtr = SDL.SDL_GetKeyboardState(out numKeys);
                byte[] keys = new byte[numKeys];
                System.Runtime.InteropServices.Marshal.Copy(keysPtr, keys, 0, numKeys);

                // Move the rectangle with arrow keys
                if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] == 1)
                {
                    fillRect.y -= 5;
                }
                if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN] == 1)
                {
                    fillRect.y += 5;
                }
                if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1)
                {
                    fillRect.x -= 5;
                }
                if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1)
                {
                    fillRect.x += 5;
                }

                // Clear screen with white color
                SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);
                SDL.SDL_RenderClear(renderer);

                // Render red filled rectangle
                SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0x00, 0x00, 0xFF);
                SDL.SDL_RenderFillRect(renderer, ref fillRect);

                // Update screen
                SDL.SDL_RenderPresent(renderer);

                // Delay to control frame rate (~60 FPS)
                SDL.SDL_Delay(16);
            }

            // Destroy renderer and window
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);

            // Quit SDL subsystems
            SDL.SDL_Quit();
        }
    }
}
