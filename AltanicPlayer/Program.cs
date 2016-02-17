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
        TagLib.File finfo;
        float curVol = 0.5f;

        public void setFirst(string mupath)
        {
            curMusic = player.Play2D(mupath);
            Stop();
        }

        public void setFirst(string mupath, bool isGoing)
        {
            if (isGoing)
                Stop();
            curMusic = player.Play2D(mupath);
            Stop();
        }

        public void PlayPause(string Mode, bool isGoing, string curMusicPath)
        {
            if (!isGoing)
            {
                curMusic = player.Play2D(curMusicPath);
                curMusic.Volume = curVol;
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

        public void getnext(string mupath)
        {
            curMusic = player.Play2D(mupath);
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

        public string[] getInfo(string mupath)
        { // 목록에 아무것도 없는 상태에서 음악 로드시
          // 플레이하던 곡이 끝날 때
          // 곡 선택시
            string[] Info = new string[3];
            string artists = "";
            finfo = TagLib.File.Create(mupath);

            if (finfo.Tag.Title != null)
                Info[0] = finfo.Tag.Title;
            else
            {
                string mName = mupath.Substring(mupath.LastIndexOf("\\") + 1);
                Info[0] = mName;
            }

            if (finfo.Tag.Artists.Length != 0)
            {
                foreach (string artist in finfo.Tag.Artists)
                {
                    artists = artists + artist + ", ";
                }
                artists = artists.Substring(0, artists.LastIndexOf(","));
                Info[1] = artists;
            }
            else
                Info[1] = "(none)";

            if (finfo.Tag.Album != null)
                Info[2] = finfo.Tag.Album;
            else
                Info[2] = "(none)";

            return Info;
        }

        public System.Drawing.Image getImg()
        {
            if (finfo.Tag.Pictures.Length != 0)
            {
                System.IO.MemoryStream getImg = new System.IO.MemoryStream(finfo.Tag.Pictures[0].Data.Data);
                System.Drawing.Image albimg = System.Drawing.Image.FromStream(getImg);

                return albimg;
            }
            return null;
        }
    }
}
