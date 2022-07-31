using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using RuTubeApi;
using static RuTube_downloader.Utils;

namespace RuTube_downloader
{
    public partial class Form1 : Form
    {
        private FrameRuTubeVideo FrameVideo = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            config = new MainConfiguration(Path.GetDirectoryName(Application.ExecutablePath) + "\\RTdown_config.json");
            config.Saving += (s, json) =>
            {
                json["downloadingDirPath"] = config.DownloadingDirPath;
                json["outputFileNameFormat"] = config.OutputFileNameFormat;
                json["useNumberedFileNames"] = config.UseNumberedFileNames;
                json["saveVideoThumbnail"] = config.SaveVideoThumbnail;
                json["saveVideoInfo"] = config.SaveVideoInfo;
            };
            config.Loading += (s, json) =>
            {
                JToken jt = json.Value<JToken>("downloadingDirPath");
                if (jt != null)
                {
                    config.DownloadingDirPath = jt.Value<string>();
                }
                jt = json.Value<JToken>("outputFileNameFormat");
                if (jt != null)
                {
                    config.OutputFileNameFormat = jt.Value<string>();
                }
                jt = json.Value<JToken>("useNumberedFileNames");
                if (jt != null)
                {
                    config.UseNumberedFileNames = jt.Value<bool>();
                }
                jt = json.Value<JToken>("saveVideoThumbnail");
                if (jt != null)
                {
                    config.SaveVideoThumbnail = jt.Value<bool>();
                }
                jt = json.Value<JToken>("saveVideoInfo");
                if (jt != null)
                {
                    config.SaveVideoInfo = jt.Value<bool>();
                }
            };
            config.Loaded += (s) =>
            {
                textBoxDownloadingDirPath.Text = config.DownloadingDirPath;
                textBoxFileNameFormat.Text = config.OutputFileNameFormat;
                checkBoxUseNumberedFileNames.Checked = config.UseNumberedFileNames;
                checkBoxSaveVideoThumbnail.Checked = config.SaveVideoThumbnail;
                checkBoxSaveVideoInfo.Checked = config.SaveVideoInfo;
            };

            config.Load();
            tabControl1.SelectedTab = tabPageSearch;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            config.Save();
        }

        private void panelVideoBkg_Resize(object sender, EventArgs e)
        {
            ResizeFrame();
        }

        private void btnSearchByUrlOrId_Click(object sender, EventArgs e)
        {
            btnSearchByUrlOrId.Enabled = false;
            textBoxUrlOrId.Enabled = false;

            if (string.IsNullOrEmpty(textBoxUrlOrId.Text) || string.IsNullOrWhiteSpace(textBoxUrlOrId.Text))
            {
                MessageBox.Show("Введите ссылку или ID видео!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                btnSearchByUrlOrId.Enabled = true;
                textBoxUrlOrId.Enabled = true;
                return;
            }

            if (FrameVideo != null)
            {
                FrameVideo.Dispose();
                FrameVideo = null;
            }

            string videoId = RuTubeApi.Utils.ExtractVideoIdFromUrl(textBoxUrlOrId.Text);
            if (string.IsNullOrEmpty(videoId))
            {
                videoId = textBoxUrlOrId.Text;
            }

            RuTubeAPI api = new RuTubeAPI();
            RuTubeVideo video = api.GetRuTubeVideo(videoId);
            if (video != null)
            {
                FrameVideo = new FrameRuTubeVideo(panelVideoBkg, video);
                ResizeFrame();
                textBoxUrlOrId.Text = null;
            }
            else
            {
                MessageBox.Show("Видео не найдено!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            textBoxUrlOrId.Enabled = true;
            btnSearchByUrlOrId.Enabled = true;
        }

        private void btnBrowseDownloadingDirPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Выберите папку для скачивания";
            folderBrowserDialog.SelectedPath =
                (!string.IsNullOrEmpty(config.DownloadingDirPath) && !string.IsNullOrWhiteSpace(config.DownloadingDirPath) &&
                Directory.Exists(config.DownloadingDirPath)) ? config.DownloadingDirPath : config.SelfDirPath;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                config.DownloadingDirPath =
                    folderBrowserDialog.SelectedPath.EndsWith("\\")
                    ? folderBrowserDialog.SelectedPath : folderBrowserDialog.SelectedPath + "\\";
                textBoxDownloadingDirPath.Text = config.DownloadingDirPath;
            }
        }

        private void btnSetDefaultFileNameFormat_Click(object sender, EventArgs e)
        {
            config.OutputFileNameFormat = FILENAME_FORMAT_DEFAULT;
            textBoxFileNameFormat.Text = FILENAME_FORMAT_DEFAULT;
        }

        private void textBoxDownloadingDirPath_Leave(object sender, EventArgs e)
        {
            config.DownloadingDirPath = textBoxDownloadingDirPath.Text;
        }

        private void textBoxFileNameFormat_Leave(object sender, EventArgs e)
        {
            config.OutputFileNameFormat = textBoxFileNameFormat.Text;
        }

        private void checkBoxUseNumberedFileNames_CheckedChanged(object sender, EventArgs e)
        {
            config.UseNumberedFileNames = checkBoxUseNumberedFileNames.Checked;
        }

        private void checkBoxSaveVideoThumbnail_CheckedChanged(object sender, EventArgs e)
        {
            config.SaveVideoThumbnail = checkBoxSaveVideoThumbnail.Checked;
        }

        private void checkBoxSaveVideoInfo_CheckedChanged(object sender, EventArgs e)
        {
            config.SaveVideoInfo = checkBoxSaveVideoInfo.Checked;
        }

        private void ResizeFrame()
        {
            if (FrameVideo != null)
            {
                FrameVideo.Left = 0;
                FrameVideo.Top = panelVideoBkg.Height / 2 - FrameVideo.Height / 2;
                FrameVideo.Width = panelVideoBkg.Width;
            }
        }
    }
}
