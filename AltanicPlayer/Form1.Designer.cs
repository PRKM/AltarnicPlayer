﻿namespace AltanicPlayer
{
    partial class AlPlayer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LoadMusic = new System.Windows.Forms.Button();
            this.DelMusic = new System.Windows.Forms.Button();
            this.PlaylistBar = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.Add = new System.Windows.Forms.Button();
            this.musicDuration = new System.Windows.Forms.TrackBar();
            this.PlayPause = new System.Windows.Forms.Button();
            this.Prev = new System.Windows.Forms.Button();
            this.Next = new System.Windows.Forms.Button();
            this.Stop = new System.Windows.Forms.Button();
            this.curPosition_Label = new System.Windows.Forms.Label();
            this.Duration_Label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.MusicList = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.musicDuration)).BeginInit();
            this.SuspendLayout();
            // 
            // LoadMusic
            // 
            this.LoadMusic.Location = new System.Drawing.Point(320, 137);
            this.LoadMusic.Name = "LoadMusic";
            this.LoadMusic.Size = new System.Drawing.Size(20, 20);
            this.LoadMusic.TabIndex = 2;
            this.LoadMusic.Text = "+";
            this.LoadMusic.UseVisualStyleBackColor = true;
            this.LoadMusic.Click += new System.EventHandler(this.LoadMusic_Click);
            // 
            // DelMusic
            // 
            this.DelMusic.Location = new System.Drawing.Point(345, 137);
            this.DelMusic.Name = "DelMusic";
            this.DelMusic.Size = new System.Drawing.Size(20, 20);
            this.DelMusic.TabIndex = 3;
            this.DelMusic.Text = "-";
            this.DelMusic.UseVisualStyleBackColor = true;
            this.DelMusic.Click += new System.EventHandler(this.DelMusic_Click);
            // 
            // PlaylistBar
            // 
            this.PlaylistBar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PlaylistBar.FormattingEnabled = true;
            this.PlaylistBar.Location = new System.Drawing.Point(12, 136);
            this.PlaylistBar.Name = "PlaylistBar";
            this.PlaylistBar.Size = new System.Drawing.Size(302, 20);
            this.PlaylistBar.TabIndex = 1;
            this.PlaylistBar.SelectedIndexChanged += new System.EventHandler(this.PlaylistBar_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(52, 158);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(34, 19);
            this.button1.TabIndex = 5;
            this.button1.Text = "Del";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Add
            // 
            this.Add.Location = new System.Drawing.Point(11, 158);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(35, 19);
            this.Add.TabIndex = 4;
            this.Add.Text = "Add";
            this.Add.UseVisualStyleBackColor = true;
            // 
            // musicDuration
            // 
            this.musicDuration.Location = new System.Drawing.Point(113, 233);
            this.musicDuration.Name = "musicDuration";
            this.musicDuration.Size = new System.Drawing.Size(345, 45);
            this.musicDuration.TabIndex = 10;
            this.musicDuration.TickStyle = System.Windows.Forms.TickStyle.None;
            this.musicDuration.Scroll += new System.EventHandler(this.musicDuration_Scroll);
            // 
            // PlayPause
            // 
            this.PlayPause.Enabled = false;
            this.PlayPause.Location = new System.Drawing.Point(229, 167);
            this.PlayPause.Name = "PlayPause";
            this.PlayPause.Size = new System.Drawing.Size(58, 48);
            this.PlayPause.TabIndex = 6;
            this.PlayPause.Text = "▶";
            this.PlayPause.UseVisualStyleBackColor = true;
            this.PlayPause.Click += new System.EventHandler(this.PlayPause_Click);
            // 
            // Prev
            // 
            this.Prev.Location = new System.Drawing.Point(174, 175);
            this.Prev.Name = "Prev";
            this.Prev.Size = new System.Drawing.Size(40, 33);
            this.Prev.TabIndex = 8;
            this.Prev.Text = "<<";
            this.Prev.UseVisualStyleBackColor = true;
            this.Prev.Click += new System.EventHandler(this.Prev_Click);
            // 
            // Next
            // 
            this.Next.Location = new System.Drawing.Point(359, 175);
            this.Next.Name = "Next";
            this.Next.Size = new System.Drawing.Size(40, 33);
            this.Next.TabIndex = 9;
            this.Next.Text = ">>";
            this.Next.UseVisualStyleBackColor = true;
            // 
            // Stop
            // 
            this.Stop.Enabled = false;
            this.Stop.Location = new System.Drawing.Point(299, 171);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(49, 40);
            this.Stop.TabIndex = 7;
            this.Stop.Text = "■";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // curPosition_Label
            // 
            this.curPosition_Label.AutoSize = true;
            this.curPosition_Label.Location = new System.Drawing.Point(327, 220);
            this.curPosition_Label.Name = "curPosition_Label";
            this.curPosition_Label.Size = new System.Drawing.Size(49, 12);
            this.curPosition_Label.TabIndex = 12;
            this.curPosition_Label.Text = "00:00:00";
            // 
            // Duration_Label
            // 
            this.Duration_Label.AutoSize = true;
            this.Duration_Label.Location = new System.Drawing.Point(402, 220);
            this.Duration_Label.Name = "Duration_Label";
            this.Duration_Label.Size = new System.Drawing.Size(49, 12);
            this.Duration_Label.TabIndex = 12;
            this.Duration_Label.Text = "00:00:00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(382, 220);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "/";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(11, 224);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "lyrics";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // MusicList
            // 
            this.MusicList.AllowDrop = true;
            this.MusicList.FormattingEnabled = true;
            this.MusicList.ItemHeight = 12;
            this.MusicList.Location = new System.Drawing.Point(11, 9);
            this.MusicList.Name = "MusicList";
            this.MusicList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.MusicList.Size = new System.Drawing.Size(354, 124);
            this.MusicList.TabIndex = 13;
            this.MusicList.DragDrop += new System.Windows.Forms.DragEventHandler(this.MusicList_DragDrop);
            this.MusicList.DragEnter += new System.Windows.Forms.DragEventHandler(this.MusicList_DragEnter);
            this.MusicList.DoubleClick += new System.EventHandler(this.MusicList_DoubleClick);
            // 
            // AlPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 259);
            this.Controls.Add(this.MusicList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Duration_Label);
            this.Controls.Add(this.curPosition_Label);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.Next);
            this.Controls.Add(this.Prev);
            this.Controls.Add(this.PlayPause);
            this.Controls.Add(this.musicDuration);
            this.Controls.Add(this.Add);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PlaylistBar);
            this.Controls.Add(this.DelMusic);
            this.Controls.Add(this.LoadMusic);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AlPlayer";
            this.Text = "Altanic Player";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AlPlayer_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.musicDuration)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadMusic;
        private System.Windows.Forms.Button DelMusic;
        private System.Windows.Forms.ComboBox PlaylistBar;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button Add;
        private System.Windows.Forms.TrackBar musicDuration;
        private System.Windows.Forms.Button PlayPause;
        private System.Windows.Forms.Button Prev;
        private System.Windows.Forms.Button Next;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.Label curPosition_Label;
        private System.Windows.Forms.Label Duration_Label;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox MusicList;
    }
}

