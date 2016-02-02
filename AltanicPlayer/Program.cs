﻿using System;
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

    public class Mplay : IDisposable
    {
        IrrKlang.ISoundEngine player = new IrrKlang.ISoundEngine();
        IrrKlang.ISound curMusic;

        public void PlayPause(string Mode, ref bool isGoing, string curMusicPath)
        {
            if (!isGoing)
            {
                curMusic = player.Play2D(curMusicPath);
                isGoing = true;
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

        public void Dispose()
        {
            curMusic.Stop();
        }
    }
}