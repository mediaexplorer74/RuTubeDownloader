using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using RuTubeApi;
using Multi_threaded_downloader;
using static RuTube_downloader.Utils;
using System.Collections.Generic;

namespace RuTube_downloader
{
    public partial class FrameRuTubeVideo : UserControl
    {
        public RuTubeVideo VideoInfo { get; private set; }

        public FrameRuTubeVideo(Control parent, RuTubeVideo videoInfo)
        {
            InitializeComponent();

            Parent = parent;
            SetVideoInfo(videoInfo);
        }

        private void FrameRuTubeVideo_Load(object sender, EventArgs e)
        {
            lblProgress.Text = null;
        }

        private void SetVideoInfo(RuTubeVideo video)
        {
            VideoInfo = video;
            if (video != null)
            {
                lblVideoTitle.Text = video.Title;
                lblChannelName.Text = "Канал: " +
                    (video.ChannelOwned == null ? "Не доступно" : video.ChannelOwned.Name);
                lblDateUploaded.Text = "Дата загрузки: " +
                    (video.DateUploaded == DateTime.MaxValue ? "Не доступно" :
                    video.DateUploaded.ToString("yyyy.MM.dd HH:mm:ss"));
                lblDatePublished.Text = "Дата публикации: " +
                    (video.DatePublished == DateTime.MaxValue ? "Не доступно" :
                    video.DatePublished.ToString("yyyy.MM.dd HH:mm:ss"));
            }
            else
            {
                lblVideoTitle.Text = "Ошибка! Видео не доступно! Pointer is null!";
                lblChannelName.Text = "Канал: Не доступно";
                lblDateUploaded.Text = "Дата загрузки: Не доступно";
                lblDatePublished.Text = "Дата публикации: Не доступно";
            }
        }

        private async Task<int> DownloadFormat(RuTubeVideoFormat ruTubeVideoFormat, string outputFileName)
        {
            UrlList chunks = ruTubeVideoFormat.ChunkUrls;
            int chunksCount = chunks.Count;
            progressBarDownload.Value = 0;
            progressBarDownload.Maximum = chunksCount;
            lblProgress.Text = $"Скачивание чанков видео: 0 / {chunksCount} (0.0%), {ruTubeVideoFormat.GetShortInfo()}";
            Progress<int> progressDash = new Progress<int>();
            progressDash.ProgressChanged += (s, n) =>
            {
                int val = n + 1;
                progressBarDownload.Value = val;
                double percent = 100.0 / chunksCount * val;
                string percentString = string.Format("{0:F1}", percent);
                lblProgress.Text = $"Скачивание чанков видео: {val} / {chunksCount} ({percentString}%)" +
                    $", {ruTubeVideoFormat.GetShortInfo()}";
            };

            return await Task.Run(() =>
            {
                if (File.Exists(outputFileName))
                {
                    File.Delete(outputFileName);
                }

                IProgress<int> reporter = progressDash;
                int errorCode = 400;
                try
                {
                    using (Stream fileStream = File.OpenWrite(outputFileName))
                    {
                        FileDownloader d = new FileDownloader();
                        for (int i = 0; i < chunksCount; ++i)
                        {
                            d.Url = chunks.Urls[i];
                            using (Stream memChunk = new MemoryStream())
                            {
                                errorCode = d.Download(memChunk);
                                if (errorCode != 200)
                                {
                                    break;
                                }
                                memChunk.Position = 0;
                                if (!MultiThreadedDownloader.AppendStream(memChunk, fileStream))
                                {
                                    errorCode = 400;
                                    System.Diagnostics.Debug.WriteLine("Appending failed!");
                                    break;
                                }
                                reporter.Report(i);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return 400;
                }
                return 200;
            });
        }

        private async void OnMenuItemClick(object sender, EventArgs e)
        {
            btnDownload.Enabled = false;
            if (string.IsNullOrEmpty(config.DownloadingDirPath) || string.IsNullOrWhiteSpace(config.DownloadingDirPath))
            {
                MessageBox.Show("Не указана папка для скачивания!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnDownload.Enabled = true;
                return;
            }
            if (!Directory.Exists(config.DownloadingDirPath))
            {
                MessageBox.Show("Папка для скачивания не найдена!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnDownload.Enabled = true;
                return;
            }
            if (string.IsNullOrEmpty(config.OutputFileNameFormat) || string.IsNullOrWhiteSpace(config.OutputFileNameFormat))
            {
                MessageBox.Show("Не указан формат имени файла!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnDownload.Enabled = true;
                return;
            }

            string path = config.DownloadingDirPath.EndsWith("\\") ? config.DownloadingDirPath : config.DownloadingDirPath + "\\";
            string fixedFileName = FixFileName(FormatFileName(config.OutputFileNameFormat, VideoInfo));
            string fn = $"{path}{fixedFileName}.ts";
            RuTubeVideoFormat videoFormat = (sender as ToolStripMenuItem).Tag as RuTubeVideoFormat;
            int errorCode = await DownloadFormat(videoFormat, fn);
            if (errorCode == 200)
            {
                FileInfo fileInfo = new FileInfo(fn);
                string fileSizeString = fileInfo != null ? $"{fileInfo.Length} байт" : "Не доступно";
                lblProgress.Text = $"Состояние: Скачано. Размер файла: {fileSizeString}. {videoFormat.GetShortInfo()}.";
                MessageBox.Show($"{VideoInfo.Title}\nСкачано!", "Успех, батенька!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblProgress.Text = $"Состояние: Ошибка {errorCode}.";
                MessageBox.Show($"Код ошибки: {errorCode}\nГугол в помощь, как говоритса!", "Неудача! :'(",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnDownload.Enabled = true;
        }

        private ContextMenuStrip BuildMenuDownloadList(IEnumerable<RuTubeVideoFormat> formats)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            foreach (RuTubeVideoFormat format in formats)
            {
                string fmt = $"{format.GetShortInfo()}, {format.Codecs}, {format.ChunkUrls.Count} chunks";
                ToolStripMenuItem menuItem = new ToolStripMenuItem(fmt);
                menuItem.Tag = format;
                menuItem.Click += OnMenuItemClick;
                menu.Items.Add(menuItem);
            }
            return menu;
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            btnDownload.Enabled = false;

            progressBarDownload.Value = 0;
            lblProgress.Text = null;

            RuTubeVideo video = await Task.Run(() =>
            {
                RuTubeAPI api = new RuTubeAPI();
                return api.GetRuTubeVideo(VideoInfo.Id);
            });
            if (video != null)
            {
                ContextMenuStrip menu = BuildMenuDownloadList(video.Formats);

                Point pt = PointToScreen(new Point(btnDownload.Left + btnDownload.Width, btnDownload.Top));
                menu.Show(pt.X, pt.Y);
            }
            btnDownload.Enabled = true;
        }
    }
}
