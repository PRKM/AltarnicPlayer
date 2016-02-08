using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;


namespace AltanicPlayer
{
    public partial class AlPlayer : Form
    {
        Addlist addlist = new Addlist();
        Mplay mplay = new Mplay();
        Thread moveBar;
        public List<string> musics = new List<string>();
        private bool isGoing = false, first;
        private bool interrupt;

        uint maxDuration, myPosition, movenum = 0;
        int hour, min, sec; // 현 선택된 곡의 시간/분/초
        int hour_cur = 0, min_cur = 0, sec_cur = 0;

        string curMusicPath;

        public AlPlayer()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void MusicList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] copy = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string mupath in copy)
                {
                    musics.Add(mupath);
                    string muname = mupath.Substring(mupath.LastIndexOf('\\') + 1); // 파일명 추출
                    string format = muname.Substring(muname.LastIndexOf('.')); // 파일 형식 추출
                    if (checkFormat(format) == true)
                    {
                        MusicList.Items.Add(muname);
                    }
                    if (!first)
                    {
                        mplay.setFirst(mupath);
                        musicDuration.Enabled = true;
                        setPositionBar();
                        first = true;
                    }
                }
                PlayPause.Enabled = true;
                
                MusicList.SelectedIndex = 0;
                if (curMusicPath == null)
                {
                    focusItem(0);
                }
            }
        }

        private void MusicList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] copy = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string mupath in copy)
                {
                    musics.Add(mupath);
                    string muname = mupath.Substring(mupath.LastIndexOf('\\') + 1); // 파일명 추출
                    string format = muname.Substring(muname.LastIndexOf('.')); // 파일 형식 추출
                    if (checkFormat(format) == true)
                    {
                        e.Effect = DragDropEffects.Copy | DragDropEffects.Scroll;
                    }
                }
            }
        }

        private void LoadMusic_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openMusic = new OpenFileDialog())
            {
                openMusic.Filter = "Music Formats" +
                                    "(*.mp3,*.ram,*.rm,*.wav,*.wma,*.mid,*.flac)|" +
                                    "*.mp3;*.ram;*.rm;*.wav;*.wma;*.mid;*.flac";
                openMusic.Title = "Open Musics";
                openMusic.FilterIndex = 2;
                openMusic.RestoreDirectory = true;
                openMusic.Multiselect = true;

                if (openMusic.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string[] copy = openMusic.FileNames;
                        foreach (string mupath in openMusic.FileNames)
                        {
                            musics.Add(mupath);
                            string muname = mupath.Substring(mupath.LastIndexOf('\\') + 1);
                            MusicList.Items.Add(muname);

                            if (!first)
                            {
                                mplay.setFirst(mupath);
                                musicDuration.Enabled = true;
                                setPositionBar();
                                first = true;
                            }
                        }
                        PlayPause.Enabled = true;

                        MusicList.SelectedIndex = 0;
                        if (curMusicPath == null)
                        {
                            focusItem(0);
                        }
                    }
                    catch (ArgumentNullException ex)
                    {
                        MessageBox.Show("Error. Could not read file from disk.\n more>>>\n" + ex.Message);
                    }
                }
            }
        }

        private void DelMusic_Click(object sender, EventArgs e)
        {
            if (MusicList.SelectedItems != null)
            {
                for (int x = MusicList.SelectedIndices.Count - 1; x >= 0; x--)
                {
                    int index = MusicList.SelectedIndices[x];
                    musics.RemoveAt(index);
                    MusicList.Items.RemoveAt(index);
                }
            }
            if (MusicList.Items == null)
                first = false;
        }

        private void PlaylistBar_SelectedIndexChanged(object sender, EventArgs e)
        {
            string listname = MusicList.SelectedItem.ToString();
        }

        private void Prev_Click(object sender, EventArgs e)
        {
            
        }

        private void PlayPause_Click(object sender, EventArgs e)
        {
            if (musics != null)
            {
                focusItem(MusicList.SelectedIndex);
                mplay.PlayPause(PlayPause.Text, isGoing, curMusicPath);
                if (!isGoing)
                {
                    Stop.Enabled = true;
                    isGoing = true;
                    PlayPause.Text = "||";
                    setPositionBar();
                }
                else if (PlayPause.Text.Equals("▶"))
                {
                    PlayPause.Text = "||";
                    interrupt = false;
                    moveBar = new Thread(new ThreadStart(movePositionBar));
                    moveBar.Start();
                }
                else
                {
                    PlayPause.Text = "▶";
                    interrupt = true;
                    moveBar.Abort();
                }
                Stop.Enabled = true;
                
            }
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            PlayPause.Text = "▶";
            Stop.Enabled = false;
            isGoing = false;
            musicDuration.Value = 0;
            myPosition = 0;
            movenum = 0;
            hour_cur = 0; min_cur = 0; sec_cur = 0;
            curPosition_Label.Text = "00:00:00";
            mplay.Stop();
            moveBar.Abort();
        }

        private void MusicList_DoubleClick(object sender, EventArgs e)
        {
            if (MusicList.SelectedItem != null)
            {
                if (!curMusicPath.Equals(MusicList.Items.IndexOf(MusicList.SelectedIndex)))
                {
                    focusItem(MusicList.SelectedIndex);
                }
                if (isGoing)
                {
                    whenMusicWasEnd();
                    moveBar.Abort();
                }
                mplay.PlayPause(PlayPause.Text, isGoing, curMusicPath);
                isGoing = true;

                Stop.Enabled = true;
                PlayPause.Text = "||";
                myPosition = 0;
                setPositionBar();
            }
        }

        private void musicDuration_Scroll(object sender, EventArgs e)
        {
            myPosition = (uint)musicDuration.Value;
            int temp = musicDuration.Value;
            string hour_s, min_s, sec_s;
            hour_cur = temp / 360000; temp -= hour_cur * 360000; hour_s = hour_cur.ToString();
            min_cur = temp / 60000; temp -= min_cur * 60000; min_s = min_cur.ToString();
            sec_cur = temp / 1000; sec_s = sec_cur.ToString();

            if (hour_cur < 10)
                hour_s = hour_s.Insert(0, "0");
            if (min_cur < 10)
                min_s = min_s.Insert(0, "0");
            if (sec_cur < 10)
                sec_s = sec_s.Insert(0, "0");

            curPosition_Label.Text = hour_s + ":" + min_s + ":" + sec_s;
            mplay.WhenScrolled(myPosition);
        }

        private void Volume_Scroll(object sender, EventArgs e)
        {
            mplay.setVolume(Volume.Value);
        }

        private bool checkFormat(string format)
        {
            if (format.Equals(".mp3") || format.Equals(".flac") || format.Equals(".wav") || format.Equals(".ram") || format.Equals(".rm") || format.Equals(".wma") || format.Equals(".mid"))
            {
                return true;
            }
            else return false;
        }

        private void focusItem(int index)
        {
            curMusicPath = musics[index];
        }

        public void setPositionBar()
        {
            mplay.getLength(out maxDuration);
            int temp = (int)maxDuration;
            string hour_s, min_s, sec_s;
            hour = temp / 360000; temp -= hour * 360000; hour_s = hour.ToString();
            min = temp / 60000; temp -= min * 60000; min_s = min.ToString();
            sec = temp / 1000; sec_s = sec.ToString();

            if (hour < 10)
                hour_s = hour_s.Insert(0, "0");
            if (min < 10)
                min_s = min_s.Insert(0, "0");
            if (sec < 10)
                sec_s = sec_s.Insert(0, "0");

            musicDuration.Maximum = (int)maxDuration;
            Duration_Label.Text = hour_s + ":" + min_s + ":" + sec_s;

            if (!first)
            {
                return;
            }

            moveBar = new Thread(new ThreadStart(movePositionBar));
            moveBar.Start();
        }

        private void movePositionBar()
        {
            string hour_s, min_s, sec_s;
            while (myPosition < maxDuration)
            {
                myPosition++;
                musicDuration.Value += 1;
                if (myPosition % 100 == 0)
                {
                    Thread.Sleep(100);
                    movenum++;
                    if (movenum == 10)
                    {
                        sec_cur++;
                        movenum = 0;
                        if (sec_cur == 60)
                        {
                            min_cur++;
                            sec_cur = 0;
                            if (min_cur == 60)
                            {
                                min_cur = 0;
                                hour_cur++;
                            }
                        }
                        hour_s = hour_cur.ToString();
                        min_s = min_cur.ToString();
                        sec_s = sec_cur.ToString();
                        if (hour_cur < 10)
                            hour_s = hour_s.Insert(0, "0");
                        if (min_cur < 10)
                            min_s = min_s.Insert(0, "0");
                        if (sec_cur < 10)
                            sec_s = sec_s.Insert(0, "0");
                        curPosition_Label.Text = hour_s + ":" + min_s + ":" + sec_s;
                    }
                }
                if (interrupt)
                {
                    break;
                }
            }
            if (myPosition == maxDuration)
            {
                whenMusicWasEnd();
            }
        }

        private void whenMusicWasEnd()
        {
            PlayPause.Text = "▶";
            Stop.Enabled = false;
            isGoing = false;
            curPosition_Label.Text = "00:00:00";
            musicDuration.Value = 0;
            myPosition = 0;
            movenum = 0;
            hour_cur = 0; min_cur = 0; sec_cur = 0;
            mplay.Stop();
        }

        private void AlPlayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (moveBar!=null)
            {
                mplay.Stop();
                moveBar.Abort();
                moveBar.Join();
            }
        }
    }
}