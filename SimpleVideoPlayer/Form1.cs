using System;
using System.Drawing;
using LibVLCSharp.Shared;
using System.Windows.Forms;
namespace SimpleVideoPlayer
{
    public partial class Form1 : Form
    {

        //VLC stuff
        public LibVLC _libVLC;
        public MediaPlayer _mp;
        public Media media;


        public bool isFullscreen = false;
        public bool isPlaying = false;
        public Size oldVideoSize;
        public Size oldFormSize;
        public Point oldVideoLocation;

        System.Timers.Timer Timer1 = new System.Timers.Timer(1000);
        System.Timers.Timer Timer2 = new System.Timers.Timer(50);


        public Form1()
        {
            InitializeComponent();
            Core.Initialize(Environment.CurrentDirectory + @"\libvlc\x64\"); //select libvlc folder to all dlls
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(ShortcutEvent);
            oldVideoSize = videoView1.Size;
            oldFormSize = this.Size;
            oldVideoLocation = videoView1.Location;

            //VLC stuff
            _libVLC = new LibVLC();
            _mp = new MediaPlayer(_libVLC);
            videoView1.MediaPlayer = _mp;
        }

        private void ShortcutEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && isFullscreen)       // from fullscreen to window

            {
                this.FormBorderStyle = FormBorderStyle.Sizable; // change form style
                this.WindowState = FormWindowState.Normal;      // back to normal size
                this.Size = oldFormSize;
                menuStrip2.Visible = true;                      // the return of the menu strip 
                videoView1.Size = oldVideoSize;                 // make video the same size as the form
                videoView1.Location = oldVideoLocation;         // remove the offset
                isFullscreen = false;
            }

            if (isPlaying) 
            {
                if (e.KeyCode == Keys.Space) 
                {
                    if (_mp.State == VLCState.Playing) 
                    {
                        _mp.Pause(); 
                    }
                    else
                    {
                        _mp.Play();
                    }
                }

                if (e.KeyCode == Keys.J)
                {
                    _mp.Position -= 0.01f;
                }
                if (e.KeyCode == Keys.L) // skip 1% forwards
                {
                    _mp.Position += 0.01f;
                }
            }
        }

        

        public void PlayFile(string file)
        {            
            _mp.Play(new Media(_libVLC, file));
            isPlaying = true;
            
        }





        public void GetAudioTrack()
        {
            foreach (var audioTracks in videoView1.MediaPlayer.AudioTrackDescription)
            {
                ToolStripMenuItem tMenu = new ToolStripMenuItem();
                tMenu = audioTracksToolStripMenuItem;
                tMenu.DropDownItems.Add(audioTracks.Name);
                tMenu.DropDownItemClicked += new ToolStripItemClickedEventHandler(AudioTrackHandleDynamicMenuClick);
            }
        }
        private void AudioTrackHandleDynamicMenuClick(object sender, ToolStripItemClickedEventArgs e)
        {
            var parent = (ToolStripMenuItem)sender;
            int index = parent.DropDownItems.IndexOf(e.ClickedItem);
            foreach (ToolStripMenuItem item in audioTracksToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
                ((ToolStripMenuItem)audioTracksToolStripMenuItem.DropDownItems[index]).Checked = true;
            }
        }

        #region Get_Set_VideoTrack 
        public void GetVideoTrack()
        {
            foreach (var videoTracks in _mp.VideoTrackDescription)
            {
                ToolStripMenuItem tMenu = new ToolStripMenuItem();
                tMenu = videoTracksToolStripMenuItem;
                tMenu.DropDownItems.Add(videoTracks.Name);
                tMenu.DropDownItemClicked += new ToolStripItemClickedEventHandler(VideoTrackHandleDynamicMenuClick);
            }
        }
        private void VideoTrackHandleDynamicMenuClick(object sender, ToolStripItemClickedEventArgs e)
        {
            var parent = (ToolStripMenuItem)sender;
            int index = parent.DropDownItems.IndexOf(e.ClickedItem);
            videoView1.MediaPlayer.SetVideoTrack(index);
            foreach (ToolStripMenuItem item in videoTracksToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
                ((ToolStripMenuItem)videoTracksToolStripMenuItem.DropDownItems[index]).Checked = true;
            }
        }
        #endregion
        #region Get_Set_Subhtitles 
        public void GetSubtitle()
        {
            foreach (var subtitleTracks in videoView1.MediaPlayer.SpuDescription)
            {
                ToolStripMenuItem tMenu = new ToolStripMenuItem();
                tMenu = subTrackToolStripMenuItem;
                tMenu.DropDownItems.Add(subtitleTracks.Name);
                tMenu.DropDownItemClicked += new ToolStripItemClickedEventHandler(SubtitleHandleDynamicMenuClick);
            }
        }
        private void SubtitleHandleDynamicMenuClick(object sender, ToolStripItemClickedEventArgs e)
        {
            var parent = (ToolStripMenuItem)sender;
            int index = parent.DropDownItems.IndexOf(e.ClickedItem);
            videoView1.MediaPlayer.SetSpu(index);
            foreach (ToolStripMenuItem item in subTrackToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
                ((ToolStripMenuItem)subTrackToolStripMenuItem.DropDownItems[index]).Checked = true;
            }
        }
        #endregion



        public void GetChapters()
        {
            foreach (var allchapters in videoView1.MediaPlayer.FullChapterDescriptions())
            {
                ToolStripMenuItem tMenu = new ToolStripMenuItem();
                tMenu = chapterToolStripMenuItem;
                tMenu.DropDownItems.Add(allchapters.Name);
                tMenu.DropDownItemClicked += new ToolStripItemClickedEventHandler(ChaptersHandleDynamicMenuClick);
            }
        }

        private void ChaptersHandleDynamicMenuClick(object sender, ToolStripItemClickedEventArgs e)
        {
            var parent = (ToolStripMenuItem)sender;
            int index = parent.DropDownItems.IndexOf(e.ClickedItem);
            //videoView1.MediaPlayer.;
            foreach (ToolStripMenuItem item in chapterToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
                ((ToolStripMenuItem)chapterToolStripMenuItem.DropDownItems[index]).Checked = true;
            }

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                var t = TimeSpan.FromMilliseconds(videoView1.MediaPlayer.Time);
                CrntTimeLabel.Text = string.Format("{0:00}:{1:00}:{2:00}", t.TotalHours, t.Minutes, t.Seconds);
                TimeSpan d = TimeSpan.FromMilliseconds(videoView1.MediaPlayer.Media.Duration);
                TotalTimeLabel.Text = string.Format("{0:00}:{1:00}:{2:00}", d.TotalHours, d.Minutes, d.Seconds);
            
                if (videoView1.MediaPlayer.Media != null & !((int)videoView1.MediaPlayer.Time > VideoSeekBar.Maximum) & !((int)videoView1.MediaPlayer.Time <= 0))
                {                
                    VideoSeekBar.Value = Convert.ToInt32(videoView1.MediaPlayer.Time);
                }
                VideoSeekBar.Maximum = Convert.ToInt32(videoView1.MediaPlayer.Media.Duration);


                if (EqualizerCheckBox.Checked == true)
                {
                    Eq_10Band();
                }
                else
                {
                    videoView1.MediaPlayer.UnsetEqualizer();
                }

            }
            catch (Exception)
            {

             
            }

        }


        //private void OnPlaying(object sender, EventArgs e)
        //{
        //    if (videoView1.MediaPlayer.Media != null) videoView1.MediaPlayer.Media.Dispose(); // If video found then dispose it
            
        //    timer3.Start();
        //    videoView1.MediaPlayer.EnableMouseInput = false;
        //    videoView1.MediaPlayer.EnableKeyInput = false;
        //    videoView1.MediaPlayer.AspectRatio = videoView1.Width + ":" + videoView1.Height;
            
        //}

        private void VolBar_Scroll(object sender, EventArgs e)
        {
            videoView1.MediaPlayer.Volume = VolBar.Value;
            Label3.Text = VolBar.Value + "%";
        }
        private void VolBtn_Click(object sender, EventArgs e)
        {
            if (videoView1.MediaPlayer.Media != null)
            {
                if (videoView1.MediaPlayer.Mute)
                {
                    VolBtn.Image = Properties.Resources.volume;
                }
                else
                {
                    VolBtn.Image = Properties.Resources.mute;
                }

                videoView1.MediaPlayer.ToggleMute();
            }
        }
        private void FulScrBtn_Click(object sender, EventArgs e)
        {
            menuStrip2.Visible = false;
            videoView1.Size = this.Size;
            videoView1.Location = new Point(0, 0);
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            isFullscreen = true;
        }
        private void PlayBtn_Click(object sender, EventArgs e)
        {
            if (videoView1.MediaPlayer.Media != null)
            {
                if (videoView1.MediaPlayer.IsPlaying)
                {
                    videoView1.MediaPlayer.Pause();
                    PlayBtn.Image = Properties.Resources.play;
                    timer3.Stop();
                }
                else
                {
                    videoView1.MediaPlayer.Play();
                    PlayBtn.Image = Properties.Resources.pause;
                    timer3.Start();
                }
            }
        }
        private void StopBtn_Click(object sender, EventArgs e)
        {
            videoView1.MediaPlayer.Stop();
        }
        private void VideoSeekBar_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeProgress(VideoSeekBar, e);
        }

        private void ChangeProgress(ProgressBar bar, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var mousepos = Math.Min(Math.Max(e.X, 0), bar.ClientSize.Width);
                var value = System.Convert.ToInt32(bar.Minimum + (bar.Maximum - bar.Minimum) * mousepos / (double)bar.ClientSize.Width);
                if (value > bar.Value & value < bar.Maximum)
                {
                    bar.Value = value + 1;
                    bar.Value = value;
                }
                else bar.Value = value;
                if (videoView1.MediaPlayer.Media != null) videoView1.MediaPlayer.Time = VideoSeekBar.Value;
            }
        }

 
        private void ImageAdjustcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ImageAdjustcheckBox.Checked == true)
            {
                groupBox1.Enabled = true;       
                _mp.SetAdjustFloat(VideoAdjustOption.Enable, 1);
            }
            else
            {
                groupBox1.Enabled = false;                
                _mp.SetAdjustFloat(VideoAdjustOption.Enable, 0);
            }
        }

        

        private void hizYazdir()
        {
            label5.Text = "Mevcut Hız:" + Math.Round(videoView1.MediaPlayer.Rate, 2).ToString() + "x";
        }



        private void button7_Click(object sender, EventArgs e)
        {
            videoView1.MediaPlayer.SetRate(-1.40f);
            hizYazdir();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            videoView1.MediaPlayer.SetRate(1.0f);
            hizYazdir();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            videoView1.MediaPlayer.SetRate(1.40f);
            hizYazdir();
        }
        private void DirectPlaybutton_Click(object sender, EventArgs e)
        {
            PlayFile(@"D:\Aaina The Body.mp4");
            videoView1.MediaPlayer.SetVideoTitleDisplay(Position.Bottom, 5000);
            timer3.Start();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            videoView1.MediaPlayer.TakeSnapshot(0, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\" + System.IO.Path.GetFileNameWithoutExtension("fileName") + ".jpg", 0, 0);
        }
        private void ChapterNextbutton_Click(object sender, EventArgs e)
        {
            videoView1.MediaPlayer.NextChapter();
        }
        private void ChapterPreviousbutton_Click(object sender, EventArgs e)
        {
            videoView1.MediaPlayer.PreviousChapter();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            videoView1.MediaPlayer.AspectRatio = comboBox1.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            videoView1.MediaPlayer.CropGeometry = comboBox2.Text;
        }
        
        
        private void HuetrackBar_Scroll(object sender, EventArgs e)
        {
            float value = (float)HuetrackBar.Value;
            Huelabel.Text = value.ToString();

            videoView1.MediaPlayer.SetAdjustFloat(VideoAdjustOption.Hue, value);
        }
        private void BrightnesstrackBar_Scroll(object sender, EventArgs e)
        {
            float value = (float)BrightnesstrackBar.Value / 10.0f;
            Brightnesslabel.Text = value.ToString();

            videoView1.MediaPlayer.SetAdjustFloat(VideoAdjustOption.Brightness, value);
        }
        

        private void ContrasttrackBar_Scroll(object sender, EventArgs e)
        {
            float value = (float)ContrasttrackBar.Value / 10.0f;
            Contrastlabel.Text = value.ToString();

            videoView1.MediaPlayer.SetAdjustFloat(VideoAdjustOption.Contrast, value);
        }

        private void SaturationtrackBar_Scroll(object sender, EventArgs e)
        {
            float value = (float)SaturationtrackBar.Value / 10.0f;
            Saturationlabel.Text = value.ToString();

            videoView1.MediaPlayer.SetAdjustFloat(VideoAdjustOption.Saturation, value);
        }

        private void GammatrackBar_Scroll(object sender, EventArgs e)
        {
            float value = (float)GammatrackBar.Value;
            Gammalabel.Text = value.ToString();
            videoView1.MediaPlayer.SetAdjustFloat(VideoAdjustOption.Gamma, value);
        }

        private void Eq_10Band()
        {
            var equilizer = new Equalizer();

            equilizer.SetPreamp((float)PreAmpTrackBar.Value);

            equilizer.SetAmp((float)Band1.Value, 0);
            equilizer.SetAmp((float)Band2.Value, 1);
            equilizer.SetAmp((float)Band3.Value, 2);
            equilizer.SetAmp((float)Band4.Value, 3);
            equilizer.SetAmp((float)Band5.Value, 4);
            equilizer.SetAmp((float)Band6.Value, 5);
            equilizer.SetAmp((float)Band7.Value, 6);
            equilizer.SetAmp((float)Band8.Value, 7);
            equilizer.SetAmp((float)Band9.Value, 8);
            equilizer.SetAmp((float)Band10.Value, 9);            

            videoView1.MediaPlayer.SetEqualizer(equilizer);
        }
        


        private void Equalizer_ValueText(object sender, EventArgs e)
        {
            float valuePreamp = PreAmpTrackBar.Value / 100f;
            PreAmpLabel.Text = valuePreamp.ToString("0.0");

            float valueBand1 = Band1.Value / 100f;
            Label60Hz.Text = valueBand1.ToString("0.0" + " db");

            float valueBand2 = Band2.Value / 100f;
            Label170Hz.Text = valueBand2.ToString("0.0" + " db");

            float valueBand3 = Band3.Value / 100f;
            Label310Hz.Text = valueBand3.ToString("0.0" + " db");

            float valueBand4 = Band4.Value / 100f;
            Label600Hz.Text = valueBand3.ToString("0.0" + " db");

            float valueBand5 = Band5.Value / 100f;
            Label1Khz.Text = valueBand5.ToString("0.0" + " db");

            float valueBand6 = Band6.Value / 100f;
            Label3Khz.Text = valueBand6.ToString("0.0" + " db");

            float valueBand7 = Band7.Value / 100f;
            Label6Khz.Text = valueBand7.ToString("0.0" + " db");

            float valueBand8 = Band8.Value / 100f;
            Label12Khz.Text = valueBand8.ToString("0.0" + " db");

            float valueBand9 = Band9.Value / 100f;
            Label14Khz.Text = valueBand9.ToString("0.0" + " db");

            float valueBand10 = Band10.Value / 100f;
            Label16Khz.Text = valueBand10.ToString("0.0" + " db");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PlayFile(ofd.FileName);
                videoView1.MediaPlayer.SetVideoTitleDisplay(Position.Bottom, 5000);

                timer3.Start();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.GetAudioTrack();
            this.GetVideoTrack();
            this.GetSubtitle();
            this.GetChapters();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var equilizer = new Equalizer();
            for (int i = 0; i <= 17; i++)
            {
                comboBox3.Items.Add(equilizer.PresetName((uint)i).ToString());
            }
        }

        private void TextSettings()
        {
            videoView1.MediaPlayer.SetMarqueeString(VideoMarqueeOption.Text, myVLCtextBox.Text);
            switch (TextPositioncomboBox.SelectedItem.ToString().Trim())
            {
                case "Center":
                    videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Position, 0);
                    break;

                case "Left":
                    videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Position, 1);
                    break;

                case "Right":
                    videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Position, 2);
                    break;

                case "Top":
                    videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Position, 4);
                    break;

                case "Bottom":
                    videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Position, 8);
                    break;

                case "Top-Left":
                    videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Position, 5);
                    break;

                case "Top-Right":
                    videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Position, 6);
                    break;

                case "Bottom-Left":
                    videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Position, 9);
                    break;

                case "Bottom-Right":
                    videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Position, 10);
                    break;
            }            
        }

        private void AddTextcheckBox_CheckedChanged(object sender, EventArgs e)
        {            
            if (AddTextcheckBox.Checked == true)
            {
                videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Enable, 1);
                groupBox3.Enabled = true;
                TextSettings();
            }
            else
            {
                videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Enable, 0);
                groupBox2.Enabled = false;

            }
        }

        private void AddLogocheckBox_CheckedChanged(object sender, EventArgs e)
        {            
            videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Enable, 1);
            if (AddLogocheckBox.Checked == true)
            {
                groupBox2.Enabled = true;
                LogoSettings();
            }
            else
            {
                groupBox2.Enabled = false;
                videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Enable, 0);
            }
        }

        private void LogoSettings()
        {

            videoView1.MediaPlayer.SetLogoString(VideoLogoOption.File, LogotextBox.Text);
            switch (LogoPositioncomboBox.SelectedItem.ToString().Trim())
            {
                case "Center":
                    videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Position, 0);
                    break;

                case "Left":
                    videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Position, 1);
                    break;

                case "Right":
                    videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Position, 2);
                    break;

                case "Top":
                    videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Position, 4);
                    break;

                case "Bottom":
                    videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Position, 8);
                    break;

                case "Top-Left":
                    videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Position, 5);
                    break;

                case "Top-Right":
                    videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Position, 6);
                    break;

                case "Bottom-Left":
                    videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Position, 9);
                    break;

                case "Bottom-Right":
                    videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Position, 10);
                    break;
            }
                        
            videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.X, Convert.ToInt16(LogoLeftUpDown.Value));
            videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Y, Convert.ToInt16(LogoTopUpDown.Value));
            videoView1.MediaPlayer.SetLogoInt(VideoLogoOption.Opacity, LogotrackBar.Value);
        }


    }
}
