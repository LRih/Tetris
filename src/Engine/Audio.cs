using System;
using System.Collections.Generic;
using System.Media;
using System.Windows.Forms;

namespace Tetris
{
    public static class Audio
    {
        //===================================================================== VARIABLES
        private static WMPLib.WindowsMediaPlayer _musicPlayer;
        private static Dictionary<string, string> _audio;

        //===================================================================== INITIALIZE
        static Audio()
        {
            _musicPlayer = new WMPLib.WindowsMediaPlayer();
            _musicPlayer.settings.setMode("loop", true);
            _audio = new Dictionary<string, string>();
        }

        //===================================================================== FUNCTIONS
        public static void Add(string name, string filename)
        {
            _audio.Add(name, Application.StartupPath + @"\sounds\" + filename);
        }

        public static void ToggleMute()
        {
            _musicPlayer.settings.mute = !_musicPlayer.settings.mute;
        }

        public static void PlayMusic(string name)
        {
            StopMusic();
            _musicPlayer.URL = _audio[name];
            ResumeMusic();
        }
        public static void ResumeMusic()
        {
            _musicPlayer.controls.play();
        }
        public static void PauseMusic()
        {
            _musicPlayer.controls.pause();
        }
        public static void StopMusic()
        {
            _musicPlayer.controls.stop();
        }

        public static void PlaySound(string name)
        {
            SoundPlayer player = new SoundPlayer(_audio[name]);
            player.Play();
            player.Dispose();
        }
    }
}
