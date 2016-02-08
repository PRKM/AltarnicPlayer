using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AltanicPlayer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AlPlayer());
        }
    }

    public class Mplay
    {
        IrrKlang.ISoundEngine player = new IrrKlang.ISoundEngine();
        IrrKlang.ISound curMusic;

        public void setFirst(string mupath)
        {
            curMusic = player.Play2D(mupath);
            Stop();
        }

        public void PlayPause(string Mode, bool isGoing, string curMusicPath)
        {
            if (!isGoing)
            {
                curMusic = player.Play2D(curMusicPath);
            }
            else
            {
                if (Mode.Equals("||"))
                {
                    curMusic.Paused = true;
                }
                else
                {
                    curMusic.Paused = false;
                }
            }
        }

        public void Stop()
        {
            curMusic.Stop();
        }

        public void getLength(out uint maxDuration)
        {
            maxDuration = (uint)curMusic.PlayLength;
        }

        public void WhenScrolled(uint myPosition)
        {
            curMusic.PlayPosition = myPosition;
        }

        public void setVolume(int value)
        {
            if (curMusic != null)
            {
                float vol = value / 100f;
                curMusic.Volume = vol;
            }
        }
    }
}
