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
        Addlist playList = new Addlist();
        Mplay mplay = new Mplay();
        Thread moveBar, longTitle, longArtist, longAlbum;
        StreamReader settings_r, seList_r;
        StreamWriter settings_w, seList_w; // 설정, 선택 목록
        FileStream mkFile, opFile; // 만들기용, 열기용

        string defpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Alplayer";

        //아래 멤버 변수들은 차후에 파일 다룰 때에 저장된 것을 기반으로 설정될 것임.

        public List<string> musics = new List<string>();
        private bool isGoing = false, first = false, ismuted = false, start = false; // 정지여부, 최초로 로드 여부, 음소거여부, 최초 재생여부
        private bool interrupt;

        uint maxDuration, myPosition, movenum = 0;
        int hour, min, sec; // 현 선택된 곡의 시간/분/초
        int hour_cur = 0, min_cur = 0, sec_cur = 0;
        int curVol = 50;
        int loopMode = 0;

        string curMusicPath, curMname, curList = "(default)";

        public AlPlayer()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            setBasic();
        }

        private void MusicList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] copy = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string mupath in copy)
                {
                    string muname = mupath.Substring(mupath.LastIndexOf('\\') + 1); // 파일명 추출
                    string format = muname.Substring(muname.LastIndexOf('.')); // 파일 형식 추출
                    seList_w = new StreamWriter(defpath + "\\Lists\\" + curList + ".alp");
                    if (checkFormat(format) == true)
                    {
                        musics.Add(mupath);
                        MusicList.Items.Add(muname);
                        seList_w.WriteLine(mupath);
                    }
                    if (!first)
                    {
                        mplay.setFirst(mupath);
                        musicDuration.Enabled = true;
                        setPositionBar();
                        first = true;
                        showInfo(mupath);
                        if (mplay.getImg() != null)
                            AlbumImage.Image = mplay.getImg();
                        else
                            AlbumImage.Image = AlbumImage.ErrorImage;
                    }
                }
                seList_w.Close();
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
                                    "(*.mp3,*.wav,*.wma,*.flac)|" +
                                    "*.mp3;*.wav;*.wma;*.flac";
                openMusic.Title = "Open Musics";
                openMusic.FilterIndex = 2;
                openMusic.RestoreDirectory = true;
                openMusic.Multiselect = true;

                if (openMusic.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string[] copy = openMusic.FileNames;
                        seList_w = new StreamWriter(defpath + "\\Lists\\" + curList + ".alp");
                        foreach (string mupath in openMusic.FileNames)
                        {
                            musics.Add(mupath);
                            string muname = mupath.Substring(mupath.LastIndexOf('\\') + 1);
                            MusicList.Items.Add(muname);
                            seList_w.WriteLine(mupath);

                            if (!first)
                            {
                                mplay.setFirst(mupath);
                                musicDuration.Enabled = true;
                                setPositionBar();
                                first = true;
                                showInfo(mupath);
                                if(mplay.getImg()!=null)
                                    AlbumImage.Image = mplay.getImg();
                                else
                                    AlbumImage.Image = AlbumImage.ErrorImage;
                            }

                        }
                        seList_w.Close();
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
            bool swich = false;
            if ((MusicList.SelectedItems != null) || (MusicList.Items.Count > 0))
            {
                for (int x = MusicList.SelectedIndices.Count - 1; x >= 0; x--)
                {
                    if ((MusicList.SelectedItems[x].ToString().Substring(1).Equals(curMname))||(MusicList.SelectedItems[x].ToString().Equals(curMname)))
                    {
                        swich = true;
                        PlayPause.Text = "▶";
                        Stop.Enabled = false;
                        start = false;
                        isGoing = false;
                        first = false;
                        mplay.Stop();
                        if (moveBar != null)
                        {
                            moveBar.Abort();
                            moveBar = null;
                        }
                        if (longTitle != null)
                        {
                            longTitle.Abort();
                            longTitle = null;
                        }
                        if (longArtist != null)
                        {
                            longArtist.Abort();
                            longArtist = null;
                        }
                        if (longAlbum != null)
                        {
                            longAlbum.Abort();
                            longAlbum = null;
                        }
                        musicDuration.Value = 0;
                        myPosition = 0;
                        movenum = 0;
                        hour_cur = 0; min_cur = 0; sec_cur = 0;
                        curPosition_Label.Text = "00:00:00";
                        Duration_Label.Text = "00:00:00";
                    }
                    int index = MusicList.SelectedIndices[x];
                    musics.RemoveAt(index);
                    MusicList.Items.RemoveAt(index);
                }
                if (swich)
                {
                    if (MusicList.Items.Count > 0)
                    {
                        MusicList.SelectedIndex = 0;
                        focusItem(0);
                        mplay.setFirst(musics[0], isGoing);
                        mplay.getLength(out maxDuration);
                        setPositionBar();
                        showInfo(curMusicPath);
                        if (mplay.getImg() != null)
                            AlbumImage.Image = mplay.getImg();
                        else
                            AlbumImage.Image = AlbumImage.ErrorImage;
                        first = true;
                    }
                }
            }
            if (MusicList.Items.Count == 0)
            {
                start = false;
                first = false;
                curMusicPath = null;
                curMname = null;
                MusicTitle.Text = "(none)";
                Album.Text = "(none)";
                Artist.Text = "(none)";
                AlbumImage.Image = AlbumImage.ErrorImage;
            }
        }

        private void PlaylistBar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isGoing)
            {
                isGoing = false;
                mplay.Stop();
                moveBar.Abort();
            }
            curList = PlaylistBar.SelectedItem.ToString();
            musics.Clear();
            MusicList.Items.Clear();
            PlayPause.Text = "▶";
            PlayPause.Enabled = false;
            Stop.Enabled = false;
            musicDuration.Value = 0;
            myPosition = 0;
            movenum = 0;
            hour_cur = 0; min_cur = 0; sec_cur = 0;
            curPosition_Label.Text = "00:00:00";
            Duration_Label.Text = "00:00:00";
            MusicTitle.Text = "(none)";
            Album.Text = "(none)";
            Artist.Text = "(none)";
            AlbumImage.Image = AlbumImage.ErrorImage;
            curMusicPath = null;
            curMname = null;
            start = false;
            if (moveBar != null)
            {
                moveBar.Abort();
                moveBar = null;
            }
            if (longTitle != null)
            {
                longTitle.Abort();
                longTitle = null;
                var loc = MusicTitle.Location;
                loc.X = -3;
                loc.Y = 128;
                MusicTitle.Location = loc;
            }
            if (longArtist != null)
            {
                longArtist.Abort();
                longArtist = null;
                var loc = Artist.Location;
                loc.X = -3;
                loc.Y = 142;
                Artist.Location = loc;
            }
            if (longAlbum != null)
            {
                longAlbum.Abort();
                longAlbum = null;
                var loc = Album.Location;
                loc.X = -3;
                loc.Y = 2;
                Album.Location = loc;
            }
            setMusicLIst(curList);
            if (MusicList.Items.Count != 0)
            {
                focusItem(0);
                MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Insert(0, "*");
                start = true;
                showInfo(curMusicPath);
                if (mplay.getImg() != null)
                    AlbumImage.Image = mplay.getImg();
                else
                    AlbumImage.Image = AlbumImage.ErrorImage;
                PlayPause.Enabled = true;
                //first = true;
                //start = true;
            }
        }

        private void Prev_Click(object sender, EventArgs e)
        {
            if (MusicList.Items.Count > 0)
            {
                mplay.Stop();
                isGoing = false;
                moveBar.Abort();
                if (longTitle != null)
                {
                    longTitle.Abort();
                    longTitle = null;
                }
                if (longArtist != null)
                {
                    longArtist.Abort();
                    longArtist = null;
                }
                if (longAlbum != null)
                {
                    longAlbum.Abort();
                    longAlbum = null;
                }
                curPosition_Label.Text = "00:00:00";
                hour_cur = 0; min_cur = 0; sec_cur = 0;
                myPosition = 0;
                movenum = 0;
                if (musicDuration.Value <= 3000)
                {
                    if (musics.IndexOf(curMusicPath) == 0)
                    {
                        MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Substring(1);
                        focusItem(musics.Count - 1);
                        MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Insert(0, "*");
                    }
                    else
                    {
                        MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Substring(1);
                        focusItem(musics.IndexOf(curMusicPath) - 1);
                        MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Insert(0, "*");
                    }
                }
                musicDuration.Value = 0;
                mplay.PlayPause(PlayPause.Text, isGoing, curMusicPath);
                isGoing = true;
                setPositionBar();
                showInfo(curMusicPath);
                if (mplay.getImg() != null)
                    AlbumImage.Image = mplay.getImg();
                else
                    AlbumImage.Image = AlbumImage.ErrorImage;
            }
        }

        private void PlayPause_Click(object sender, EventArgs e)
        {
            if (MusicList.Items.Count!=0)
            {
                if(!start) // 정지하고 다시 재생했을때 별표가 중복해서 증가하지 않도록 하기!!!!
                {
                    start = true;
                }
                else
                {
                    MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Substring(1);
                }
                focusItem(musics.IndexOf(curMusicPath));
                MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Insert(0, "*");

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
            if (longTitle != null)
            {
                longTitle.Abort();
                longTitle = null;
            }
            if (longArtist != null)
            {
                longArtist.Abort();
                longArtist = null;
            }
            if (longAlbum != null)
            {
                longAlbum.Abort();
                longAlbum = null;
            }
        }

        private void MusicList_DoubleClick(object sender, EventArgs e)
        {
            if ((MusicList.SelectedItem != null) && (MusicList.Items.Count != 0))
            {
                if (!curMusicPath.Equals(MusicList.Items.IndexOf(MusicList.SelectedIndex)))
                {
                    if (start)
                    {
                        MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Substring(1);
                    }
                    else
                    {
                        start = true;
                    }
                    focusItem(MusicList.SelectedIndex);
                    MusicList.Items[MusicList.SelectedIndex] = MusicList.Items[MusicList.SelectedIndex].ToString().Insert(0, "*");
                }
                if (isGoing)
                {
                    whenMusicWasEnd(true);
                }
                else
                {
                    mplay.PlayPause(PlayPause.Text, isGoing, curMusicPath);
                    isGoing = true;
                    Stop.Enabled = true;
                    PlayPause.Text = "||";
                    Duration_Label.Text = "00:00:00";
                    setPositionBar();
                }
                if (longTitle != null)
                {
                    longTitle.Abort();
                    longTitle = null;
                }
                if (longArtist != null)
                {
                    longArtist.Abort();
                    longArtist = null;
                }
                if (longAlbum != null)
                {
                    longAlbum.Abort();
                    longAlbum = null;
                }
                showInfo(curMusicPath);
                if (mplay.getImg() != null)
                    AlbumImage.Image = mplay.getImg();
                else
                    AlbumImage.Image = AlbumImage.ErrorImage;
            }
        }

        private void musicDuration_Scroll(object sender, EventArgs e)
        {
            myPosition = (uint)musicDuration.Value;
            if (myPosition % 1000 != 0)
            {
                movenum = (myPosition % 1000) / 100;
            }
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

        private void Next_Click(object sender, EventArgs e)
        {
            
            if ((curMusicPath != null) && (curMname != null) && (musics.Count > 0))
            {
                start = true;
                MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Substring(1);
                if (curMname.Equals(MusicList.Items[MusicList.Items.Count - 1]))
                {
                    focusItem(0);
                    mplay.setFirst(curMusicPath, isGoing);
                }
                else
                {
                    focusItem(musics.IndexOf(curMusicPath) + 1);
                    mplay.setFirst(curMusicPath, isGoing);
                }
                MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Insert(0, "*");
                mplay.Stop();
                if(moveBar!=null)
                    moveBar.Abort();
                musicDuration.Value = 0;
                myPosition = 0;
                movenum = 0;
                hour_cur = 0; min_cur = 0; sec_cur = 0;
                curPosition_Label.Text = "00:00:00";
                if (PlayPause.Text.Equals("▶"))
                {
                    first = false;
                    setPositionBar();
                    first = true;
                }
                else
                {
                    mplay.getnext(curMusicPath);
                    setPositionBar();
                }
                showInfo(curMusicPath);
                if (mplay.getImg() != null)
                    AlbumImage.Image = mplay.getImg();
                else
                    AlbumImage.Image = AlbumImage.ErrorImage;
            }
        }

        private void Del_Click(object sender, EventArgs e)
        {
            if (isGoing)
            {
                mplay.Stop();
                moveBar.Abort();
            }
            if (PlaylistBar.SelectedIndex == 0)
                MessageBox.Show("기본 목록은 삭제할 수 없습니다.");
            else
            {
                DialogResult confirm = MessageBox.Show("정말로 삭제하시겠습니까?", "삭제 확인", MessageBoxButtons.OKCancel);
                if (confirm == DialogResult.OK)
                {
                    PlaylistBar.Items.RemoveAt(PlaylistBar.SelectedIndex);
                    File.Delete(defpath + "\\Lists\\" + curList + ".alp");
                    PlaylistBar.SelectedIndex = 0;
                    curList = "(default)";
                    musics.Clear();
                    MusicList.Items.Clear();
                    setMusicLIst(curList);
                    MusicTitle.Text = "(none)";
                    Album.Text = "(none)";
                    Artist.Text = "(none)";
                    AlbumImage.Image = AlbumImage.ErrorImage;
                    PlayPause.Text = "▶";
                    PlayPause.Enabled = false;
                    Stop.Enabled = false;
                    start = false;
                    isGoing = false;
                    first = false;
                    if (moveBar != null)
                    {
                        moveBar.Abort();
                        moveBar = null;
                    }
                    if (longTitle != null)
                    {
                        longTitle.Abort();
                        longTitle = null;
                        var loc = MusicTitle.Location;
                        loc.X = -3;
                        loc.Y = 128;
                        MusicTitle.Location = loc;
                    }
                    if (longArtist != null)
                    {
                        longArtist.Abort();
                        longArtist = null;
                        var loc = Artist.Location;
                        loc.X = -3;
                        loc.Y = 142;
                        Artist.Location = loc;
                    }
                    if (longAlbum != null)
                    {
                        longAlbum.Abort();
                        longAlbum = null;
                        var loc = Album.Location;
                        loc.X = -3;
                        loc.Y = 2;
                        Album.Location = loc;
                    }
                    musicDuration.Value = 0;
                    myPosition = 0;
                    movenum = 0;
                    hour_cur = 0; min_cur = 0; sec_cur = 0;
                    curPosition_Label.Text = "00:00:00";
                    Duration_Label.Text = "00:00:00";
                    if (MusicList.Items.Count != 0)
                    {
                        focusItem(0);
                        MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Insert(0, "*");
                        start = true;
                        showInfo(curMusicPath);
                        if (mplay.getImg() != null)
                            AlbumImage.Image = mplay.getImg();
                        else
                            AlbumImage.Image = AlbumImage.ErrorImage;
                        setPositionBar();
                        PlayPause.Enabled = true;
                        //first = true;
                        //start = true;
                    }
                }
            }
        }

        private void Loop_Click(object sender, EventArgs e)
        {
            switch (loopMode)
            {
                case 0:
                    Loop.Image = Image.FromFile(@"..\..\Images\repeat.png");
                    loopMode = 1;
                    break;
                case 1:
                    Loop.Image = Image.FromFile(@"..\..\Images\repeat_one.png");
                    loopMode = 2;
                    break;
                case 2:
                    Loop.Image = Image.FromFile(@"..\..\Images\non-repeat.png");
                    loopMode = 0;
                    break;
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            playList.ShowDialog();

            if ((!playList.toadd.Equals("")) && (playList.toadd != null))
            {
                curList = playList.toadd;
                if (isGoing)
                {
                    mplay.Stop();
                    moveBar.Abort();
                }
                mkFile = File.Open(defpath + "\\Lists\\" + curList + ".alp", FileMode.Create);
                mkFile.Close();
                PlaylistBar.Items.Add(curList);
                PlaylistBar.SelectedIndex = PlaylistBar.Items.IndexOf(curList);
                musics.Clear();
                MusicList.Items.Clear();
                curMusicPath = null;
                curMname = null;
                first = false;
                start = false;
                isGoing = false;
                MusicTitle.Text = "(none)";
                Album.Text = "(none)";
                Artist.Text = "(none)";
                AlbumImage.Image = AlbumImage.ErrorImage;
                if (moveBar != null)
                {
                    moveBar.Abort();
                    moveBar = null;
                }
                if (longTitle != null)
                {
                    longTitle.Abort();
                    longTitle = null;
                    var loc = MusicTitle.Location;
                    loc.X = -3;
                    loc.Y = 128;
                    MusicTitle.Location = loc;
                }
                if (longArtist != null)
                {
                    longArtist.Abort();
                    longArtist = null;
                    var loc = Artist.Location;
                    loc.X = -3;
                    loc.Y = 142;
                    Artist.Location = loc;
                }
                if (longAlbum != null)
                {
                    longAlbum.Abort();
                    longAlbum = null;
                    var loc = Album.Location;
                    loc.X = -3;
                    loc.Y = 2;
                    Album.Location = loc;
                }
                musicDuration.Value = 0;
                myPosition = 0;
                movenum = 0;
                hour_cur = 0; min_cur = 0; sec_cur = 0;
                curPosition_Label.Text = "00:00:00";
                Duration_Label.Text = "00:00:00";
                PlayPause.Text = "▶";
                PlayPause.Enabled = false;
                Stop.Enabled = false;
            }
        }

        private void Mute_Click(object sender, EventArgs e)
        {
            if (ismuted)
            {
                mplay.setVolume(curVol);
                Volume.Value = curVol;
                Mute.Image = Image.FromFile(@"..\..\Images\sound.png");
                ismuted = false;
            }
            
            else
            {
                mplay.setVolume(0);
                Volume.Value = 0;
                Mute.Image = Image.FromFile(@"..\..\Images\mute.png");
                ismuted = true;
            }
        }

        private void Volume_Scroll(object sender, EventArgs e)
        {
            curVol = Volume.Value;
            mplay.setVolume(curVol);
        }

        private bool checkFormat(string format)
        {
            if (format.Equals(".mp3") || format.Equals(".flac") || format.Equals(".wav") || format.Equals(".wma"))
            {
                return true;
            }
            else return false;
        }

        private void focusItem(int index)
        {
            curMusicPath = musics[index];
            curMname = curMusicPath.Substring(curMusicPath.LastIndexOf('\\') + 1);
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
                try
                {
                    myPosition++;
                    musicDuration.Value++;
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
                catch
                {

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
            if (longTitle != null)
            {
                longTitle.Abort();
                longTitle = null;
            }
            if (longArtist != null)
            {
                longArtist.Abort();
                longArtist = null;
            }
            if (longAlbum != null)
            {
                longAlbum.Abort();
                longAlbum = null;
            }

            //여기에 루프 모드별 수행코드 작성
            if (loopMode == 2)
            {
                PlayPause.Text = "||";
                Stop.Enabled = true;
                mplay.PlayPause(PlayPause.Text, isGoing, curMusicPath);
                isGoing = true;
                setPositionBar();
            }
            else if (curMusicPath.Equals(musics[musics.Count-1]))
            {
                switch (loopMode)
                {
                    case 0:
                        moveBar.Abort();
                        break;
                    case 1:
                        MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Substring(1);
                        focusItem(0);
                        MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Insert(0, "*");
                        PlayPause.Text = "||";
                        Stop.Enabled = true;
                        mplay.PlayPause(PlayPause.Text, isGoing, curMusicPath);
                        isGoing = true;
                        Duration_Label.Text = "00:00:00";
                        setPositionBar();
                        showInfo(curMusicPath);
                        if (mplay.getImg() != null)
                            AlbumImage.Image = mplay.getImg();
                        else
                            AlbumImage.Image = AlbumImage.ErrorImage;
                        break;
                }
            }
            else
            {
                MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Substring(1);
                focusItem(musics.IndexOf(curMusicPath) + 1);
                MusicList.Items[musics.IndexOf(curMusicPath)] = MusicList.Items[musics.IndexOf(curMusicPath)].ToString().Insert(0, "*");
                PlayPause.Text = "||";
                Stop.Enabled = true;
                mplay.PlayPause(PlayPause.Text, isGoing, curMusicPath);
                isGoing = true;
                Duration_Label.Text = "00:00:00";
                setPositionBar();
                showInfo(curMusicPath);
                if (mplay.getImg() != null)
                    AlbumImage.Image = mplay.getImg();
                else
                    AlbumImage.Image = AlbumImage.ErrorImage;
            }
        }

        private void whenMusicWasEnd(bool ovride)
        {
            interrupt = false;
            isGoing = false;
            curPosition_Label.Text = "00:00:00";
            Duration_Label.Text = "00:00:00";
            musicDuration.Value = 0;
            myPosition = 0;
            movenum = 0;
            hour_cur = 0; min_cur = 0; sec_cur = 0;
            mplay.Stop();
            moveBar.Abort();
            PlayPause.Text = "||";
            Stop.Enabled = true;
            mplay.PlayPause(PlayPause.Text, isGoing, curMusicPath);
            isGoing = true;
            setPositionBar();
        }

        private void showInfo(string mupath)
        {
            string[] Info = new string[3];
            Info = mplay.getInfo(mupath);

            MusicTitle.Text = Info[0];
            Artist.Text = Info[1];
            Album.Text = Info[2];

            MusicTitle.Location = new System.Drawing.Point(
                                    MusicTitle.Location.X,
                                    MusicTitle.Location.Y);
            int size_t = int.Parse(MusicTitle.Size.Width.ToString());
            if (size_t > 110)
            {
                int temp = size_t - 110 + 11;
                moveTitle.Enabled = true;
                moveTitle.Maximum = temp;
                longTitle = new Thread(new ThreadStart(mvTitle));
                longTitle.Start();
            }
            int size_art = int.Parse(Artist.Size.Width.ToString());
            if (size_art > 110)
            {
                int temp = size_art - 110 + 11;
                moveArtist.Enabled = true;
                moveArtist.Maximum = temp;
                longArtist = new Thread(new ThreadStart(mvArtist));
                longArtist.Start();
            }
            int size_al = int.Parse(Album.Size.Width.ToString());
            if (size_al > 110)
            {
                int temp = size_al - 110 + 11;
                moveAlbum.Enabled = true;
                moveAlbum.Maximum = temp;
                longAlbum = new Thread(new ThreadStart(mvAlbum));
                longAlbum.Start();
            }
        }

        private void mvTitle()
        {
            while (true)
            {
                Thread.Sleep(100);
                if (moveTitle.Value == 0)
                {
                    Thread.Sleep(1900);
                }
                var loc = MusicTitle.Location;
                if (moveTitle.Value == moveTitle.Maximum - 10)
                {
                    Thread.Sleep(2000);
                    moveTitle.Value = 0;
                    loc.X = -3;
                    loc.Y = 128;
                    MusicTitle.Location = loc;
                }
                else
                {
                    moveTitle.Value++;
                    loc.Y = MusicTitle.Location.Y;
                    loc.X = -moveTitle.Value;
                    MusicTitle.Location = loc;
                }
            }
        }

        private void mvArtist()
        {
            while (true)
            {
                Thread.Sleep(100);
                if (moveArtist.Value == 0)
                {
                    Thread.Sleep(1900);
                }
                moveArtist.Value++;
                var loc = Artist.Location;
                loc.Y = Artist.Location.Y;
                loc.X = -moveArtist.Value;
                Artist.Location = loc;
                if (moveArtist.Value == moveArtist.Maximum)
                {
                    Thread.Sleep(2000);
                    moveArtist.Value = 0;
                    loc.X = -3;
                    loc.Y = 142;
                    Artist.Location = loc;
                }
            }
        }

        private void mvAlbum()
        {
            while (true)
            {
                Thread.Sleep(100);
                if (moveAlbum.Value == 0)
                {
                    Thread.Sleep(1900);
                }
                moveAlbum.Value++;
                var loc = Album.Location;
                loc.Y = Album.Location.Y;
                loc.X = -moveAlbum.Value;
                Album.Location = loc;
                if (moveAlbum.Value == moveAlbum.Maximum)
                {
                    Thread.Sleep(2000);
                    moveAlbum.Value = 0;
                    loc.X = -3;
                    loc.Y = 2;
                    Album.Location = loc;
                }
            }
        }

        private void setBasic()
        {
            string list;
            int selected, vol;
            bool isplaying;
            if (!Directory.Exists(defpath)) // 폴더 만들기
            {
                Directory.CreateDirectory(defpath);
            }
            if (File.Exists(defpath + "\\setting.jyh")) // 설정 파일 여부
            {
                opFile = File.Open(defpath + "\\setting.jyh", FileMode.Open, FileAccess.ReadWrite);
                if (opFile.Length == 0) // 누락시
                {
                    opFile.Close();
                    settings_w = new StreamWriter(defpath + "\\setting.jyh");
                    settings_w.WriteLine("List = (default)");
                    settings_w.WriteLine("Selected = -1");
                    settings_w.WriteLine("Volume = 50");
                    settings_w.WriteLine("position = 0");
                    settings_w.WriteLine("isPlaying = false");

                    settings_w.Close();

                    setList("(default)");
                }
                else // 설정된 값으로 바꿔주기
                {
                    int index = 0;
                    opFile.Close();
                    settings_r = new StreamReader(defpath + "\\setting.jyh");
                    while (index <= 4)
                    {
                        string temp;
                        object value;
                        temp = settings_r.ReadLine();
                        switch (index)
                        {
                            case 0:
                                list = temp.Substring(temp.LastIndexOf("=") + 2);
                                curList = list;
                                setList(curList);
                                break;
                            case 1:
                                selected = int.Parse(temp.Substring(temp.IndexOf("=") + 2));
                                if ((selected != -1)&&(musics.Count != 0))
                                {
                                    curMusicPath = musics[selected];
                                    curMname = musics[selected].Substring(musics[selected].LastIndexOf("\\") + 1);
                                    mplay.setFirst(curMusicPath);
                                    showInfo(curMusicPath);
                                    if (mplay.getImg() != null)
                                        AlbumImage.Image = mplay.getImg();
                                    else
                                        AlbumImage.Image = AlbumImage.ErrorImage;
                                    setPositionBar();
                                    first = true;
                                    start = true;
                                }
                                break;
                            case 2:
                                vol = int.Parse(temp.Substring(temp.IndexOf("=") + 2));
                                Volume.Value = vol;
                                if (curMusicPath != null)
                                {
                                    mplay.setVolume(vol);
                                }
                                break;
                            case 3:
                                myPosition = uint.Parse(temp.Substring(temp.IndexOf("=") + 2));
                                if (curMusicPath != null)
                                {
                                    musicDuration.Value = (int)myPosition;
                                    if (myPosition % 1000 != 0)
                                    {
                                        movenum = (myPosition % 1000) / 100;
                                    }
                                    int temp_t = musicDuration.Value;
                                    string hour_s, min_s, sec_s;
                                    hour_cur = temp_t / 360000; temp_t -= hour_cur * 360000; hour_s = hour_cur.ToString();
                                    min_cur = temp_t / 60000; temp_t -= min_cur * 60000; min_s = min_cur.ToString();
                                    sec_cur = temp_t / 1000; sec_s = sec_cur.ToString();

                                    if (hour_cur < 10)
                                        hour_s = hour_s.Insert(0, "0");
                                    if (min_cur < 10)
                                        min_s = min_s.Insert(0, "0");
                                    if (sec_cur < 10)
                                        sec_s = sec_s.Insert(0, "0");

                                    curPosition_Label.Text = hour_s + ":" + min_s + ":" + sec_s;
                                    mplay.PlayPause(PlayPause.Text, isGoing, curMusicPath);
                                    mplay.WhenScrolled(myPosition);
                                    isGoing = true;
                                    mplay.PlayPause("||", isGoing, curMusicPath);
                                    musicDuration.Enabled = true;
                                    PlayPause.Enabled = true;
                                    if (myPosition > 0)
                                        Stop.Enabled = true;
                                    else
                                    {
                                        Stop.Enabled = false;
                                        isGoing = false;
                                    }
                                }
                                break;
                            case 4:
                                isplaying = bool.Parse(temp.Substring(temp.IndexOf("=") + 2));
                                if (isplaying && (myPosition>0))
                                {
                                    mplay.PlayPause(PlayPause.Text, isGoing, curMusicPath);
                                    moveBar = new Thread(new ThreadStart(movePositionBar));
                                    moveBar.Start();
                                    PlayPause.Text = "||";
                                }
                                break;
                            default:
                                break;
                        }
                        index++;
                    }
                    settings_r.Close();
                }
            }
            else // 설정 파일이 없으면
            {
                mkFile = File.Open(defpath + "\\setting.jyh", FileMode.Create, FileAccess.ReadWrite);
                mkFile.Close();

                settings_w = new StreamWriter(defpath + "\\setting.jyh");
                settings_w.WriteLine("List = (default)");
                settings_w.WriteLine("Selected = -1");
                settings_w.WriteLine("Volume = 50");
                settings_w.WriteLine("position = 0");
                settings_w.WriteLine("isPlaying = false");

                settings_w.Close();

                setList("(default)");
            }
        }

        private void setList(string curList)
        {
            string[] lists;
            lists = Directory.GetFiles(defpath + "\\Lists");
            foreach (string t in lists)
                PlaylistBar.Items.Add(t.Substring(t.LastIndexOf("\\")+1, t.Substring(t.LastIndexOf("\\")).Length-5));
            if (!Directory.Exists(defpath + "\\Lists")) // 목록 폴더 여부
            {
                Directory.CreateDirectory(defpath + "\\Lists");
                mkFile = File.Open(defpath + "\\Lists\\(default).alp", FileMode.Create);
                mkFile.Close();
            }
            else
            {
                if (!File.Exists(defpath + "\\Lists\\(default).alp"))
                {
                    File.Create(defpath + "\\Lists\\(default).alp");
                }
            }
            setMusicLIst(curList);
        }

        private void setMusicLIst(string curList)
        {
            if ((!File.Exists(defpath + "\\Lists\\" + curList + ".alp")) || (curList.Equals("(default)"))) // 기본 목록 폴더 지정시
            {
                seList_r = new StreamReader(defpath + "\\Lists\\(default).alp");
                while (true)
                {
                    string t = seList_r.ReadLine();
                    if ((t == null) || (t.Equals("")))
                    {
                        break;
                    }
                    else
                    {
                        musics.Add(t);
                        MusicList.Items.Add(t.Substring(t.LastIndexOf("\\") + 1));
                    }
                }
                seList_r.Close();
                PlaylistBar.SelectedIndex = 0;
            }
            else // 다른 목록
            {
                seList_r = new StreamReader(defpath + "\\Lists\\" + curList + ".alp");
                while (true)
                {
                    string t = seList_r.ReadLine();
                    if ((t == null) || (t.Equals("")))
                    {
                        break;
                    }
                    else
                    {
                        musics.Add(t);
                        MusicList.Items.Add(t.Substring(t.LastIndexOf("\\") + 1));
                    }
                }
                seList_r.Close();
                PlaylistBar.SelectedIndex = PlaylistBar.Items.IndexOf(curList);
            }
        }

        private void AlPlayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (moveBar!=null)
            {
                mplay.Stop();
                moveBar.Abort();
                moveBar.Join();
            }

            settings_w = new StreamWriter(defpath + "\\setting.jyh");
            settings_w.WriteLine("List = " + curList);
            if (((curMusicPath != null) && (!curMusicPath.Equals(""))) || (MusicList.Items.Count == 0))
                settings_w.WriteLine("Selected = " + musics.IndexOf(curMusicPath));
            else
                settings_w.WriteLine("Selected = -1");
            settings_w.WriteLine("Volume = " + Volume.Value);
            settings_w.WriteLine("position = " + myPosition);
            if (PlayPause.Text.Equals("▶"))
                settings_w.WriteLine("isPlaying = false");
            else
                settings_w.WriteLine("isPlaying = true");

            settings_w.Close();
        }
    }
}