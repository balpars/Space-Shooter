using SDL2;
using System;

namespace Space_Shooter
{
    public static class SoundManager
    {
        public static IntPtr LoadSound(string path)
        {
            IntPtr sound = SDL_mixer.Mix_LoadWAV(path);
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