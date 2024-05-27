using SDL2;
using System;

namespace Space_Shooter
{
    public static class SoundManager
    {
        public static void Init()
        {
            if (SDL_mixer.Mix_OpenAudio(22050, SDL.AUDIO_S16SYS, 2, 4096) == -1)
            {
               // Console.WriteLine("SDL_mixer could not initialize! SDL_mixer Error: " + SDL_mixer.Mix_GetError());
            }
        }

        public static IntPtr LoadSound(string path)
        {
            IntPtr sound = SDL_mixer.Mix_LoadWAV(path);
            if (sound == IntPtr.Zero)
            {
                //Console.WriteLine("Failed to load sound effect! SDL_mixer Error: " + SDL_mixer.Mix_GetError());
            }
            return sound;
        }

        public static void PlaySound(IntPtr sound)
        {
            SDL_mixer.Mix_PlayChannel(-1, sound, 0);
        }

        public static void Cleanup()
        {
            SDL_mixer.Mix_CloseAudio();
        }
    }
}
