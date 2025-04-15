using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RuTubeDownloader
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            string videoId = VideoIdTextBox.Text.Trim();
            string folderPath = FolderPathTextBox.Text.Trim();

            if (string.IsNullOrEmpty(videoId))
            {
                await new MessageDialog("Please enter a valid video ID.").ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(folderPath))
            {
                await new MessageDialog("Please select a download folder.").ShowAsync();
                return;
            }

            try
            {
                await DownloadVideo(videoId, folderPath);
                await new MessageDialog("Video downloaded successfully!").ShowAsync();
            }
            catch (Exception ex)
            {
                await new MessageDialog($"Error: {ex.Message}").ShowAsync();
            }
        }

        private async Task DownloadVideo(string videoId, string folderPath)
        {
            string videoUrl = $"https://rutube.ru/api/video/{videoId}";
            string fileName = $"{videoId}.mp4";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(videoUrl);
                response.EnsureSuccessStatusCode();

                byte[] videoData = await response.Content.ReadAsByteArrayAsync();
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteBytesAsync(file, videoData);
            }
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop
            };
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                FolderPathTextBox.Text = folder.Path;
            }
        }


        // Experimental part *********************
        private FrameRuTubeVideo FrameVideo = null;

       

        private void Form1_Load(object sender, EventArgs e)
        {
            var Config = new MainConfiguration(Path.GetDirectoryName(/*Application.ExecutablePath*/default) 
                + "\\RTdown_config.json");
            Config.Saving += (s, json) =>
            {
                json["downloadingDirPath"] = Config.DownloadingDirPath;
                json["outputFileNameFormat"] = Config.OutputFileNameFormat;
                json["userAgent"] = Config.UserAgent;
                json["useNumberedFileNames"] = Config.UseNumberedFileNames;
                json["saveVideoThumbnail"] = Config.SaveVideoThumbnail;
                json["saveVideoInfo"] = Config.SaveVideoInfo;
            };
            Config.Loading += (s, json) =>
            {
                JToken jt = json.Value<JToken>("downloadingDirPath");
                if (jt != null)
                {
                    Config.DownloadingDirPath = jt.Value<string>();
                }
                jt = json.Value<JToken>("outputFileNameFormat");
                if (jt != null)
                {
                    Config.OutputFileNameFormat = jt.Value<string>();
                }
                jt = json.Value<JToken>("userAgent");
                if (jt != null)
                {
                    Config.UserAgent = jt.Value<string>();
                }
                jt = json.Value<JToken>("useNumberedFileNames");
                if (jt != null)
                {
                    Config.UseNumberedFileNames = jt.Value<bool>();
                }
                jt = json.Value<JToken>("saveVideoThumbnail");
                if (jt != null)
                {
                    Config.SaveVideoThumbnail = jt.Value<bool>();
                }
                jt = json.Value<JToken>("saveVideoInfo");
                if (jt != null)
                {
                    Config.SaveVideoInfo = jt.Value<bool>();
                }
            };
            Config.Loaded += (s) =>
            {
                //textBoxDownloadingDirPath.Text = Config.DownloadingDirPath;
                //textBoxFileNameFormat.Text = Config.OutputFileNameFormat;
                //textBoxUserAgent.Text = Config.UserAgent;
                //checkBoxUseNumberedFileNames.Checked = Config.UseNumberedFileNames;
                //checkBoxSaveVideoThumbnail.Checked = Config.SaveVideoThumbnail;
                //checkBoxSaveVideoInfo.Checked = Config.SaveVideoInfo;
            };

            Config.Load();
            //tabControl1.SelectedTab = tabPageSearch;
        }

        //private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    config.Save();
        //}

        private void panelVideoBkg_Resize(object sender, EventArgs e)
        {
            ResizeFrame();
        }

        private void btnSearchByUrlOrId_Click(object sender, EventArgs e)
        {
            /*btnSearchByUrlOrId.Enabled = false;
            textBoxUrlOrId.Enabled = false;
            textBoxUserAgent.Enabled = false;

            if (string.IsNullOrEmpty(textBoxUrlOrId.Text) || string.IsNullOrWhiteSpace(textBoxUrlOrId.Text))
            {
                //MessageBox.Show("Введите ссылку или ID видео!", "Ошибка!",
                //    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                btnSearchByUrlOrId.Enabled = true;
                textBoxUrlOrId.Enabled = true;
                textBoxUserAgent.Enabled = true;
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

            RuTubeAPI.UserAgent = config.UserAgent;
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
            textBoxUserAgent.Enabled = true;*/
        }

        private void btnBrowseDownloadingDirPath_Click(object sender, EventArgs e)
        {
            /*FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
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
            }*/
        }

        private void btnSetDefaultFileNameFormat_Click(object sender, EventArgs e)
        {
            //config.OutputFileNameFormat = FILENAME_FORMAT_DEFAULT;
            //textBoxFileNameFormat.Text = FILENAME_FORMAT_DEFAULT;
        }

        private void btnSetDefaultUserAgent_Click(object sender, EventArgs e)
        {
            //config.UserAgent = USER_AGENT_DEFAULT;
            //textBoxUserAgent.Text = USER_AGENT_DEFAULT;
        }

        private void textBoxDownloadingDirPath_Leave(object sender, EventArgs e)
        {
            //config.DownloadingDirPath = textBoxDownloadingDirPath.Text;
        }

        private void textBoxFileNameFormat_Leave(object sender, EventArgs e)
        {
            //config.OutputFileNameFormat = textBoxFileNameFormat.Text;
        }

        private void textBoxUserAgent_Leave(object sender, EventArgs e)
        {
            //config.UserAgent = textBoxUserAgent.Text;
        }

        private void checkBoxUseNumberedFileNames_CheckedChanged(object sender, EventArgs e)
        {
            //config.UseNumberedFileNames = checkBoxUseNumberedFileNames.Checked;
        }

        private void checkBoxSaveVideoThumbnail_CheckedChanged(object sender, EventArgs e)
        {
            //config.SaveVideoThumbnail = checkBoxSaveVideoThumbnail.Checked;
        }

        private void checkBoxSaveVideoInfo_CheckedChanged(object sender, EventArgs e)
        {
            //config.SaveVideoInfo = checkBoxSaveVideoInfo.Checked;
        }

        private void ResizeFrame()
        {
            if (FrameVideo != null)
            {
                //FrameVideo.Left = 0;
                //FrameVideo.Top = panelVideoBkg.Height / 2 - FrameVideo.Height / 2;
                //FrameVideo.Width = panelVideoBkg.Width;
            }
        }




        // ****************************************
    }
}
