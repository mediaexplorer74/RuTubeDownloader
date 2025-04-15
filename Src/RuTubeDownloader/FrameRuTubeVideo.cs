using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using RuTubeApi;
using MultiThreadedDownloaderLib;
using static RuTube_downloader.Utils;

namespace RuTube_downloader
{
    public partial class FrameRuTubeVideo : UserControl
    {
        public RuTubeVideo VideoInfo { get; private set; }
        private Image Thumbnail;

        public bool IsDownloading { get; private set; }
        private bool IsCanceling = false;

        public FrameRuTubeVideo(Control parent, RuTubeVideo videoInfo)
        {
            InitializeComponent();
            Disposed += (s, e) =>
            {
                if (Thumbnail != null)
                {
                    Thumbnail.Dispose();
                    Thumbnail = null;
                }
            };

            Parent = parent;
            SetVideoInfo(videoInfo);
        }

        private void FrameRuTubeVideo_Load(object sender, EventArgs e)
        {
            lblProgress.Text = null;
        }

        private void pictureBoxVideoThumbnail_Paint(object sender, PaintEventArgs e)
        {
            if (Thumbnail != null)
            {
                Rectangle thumbnailRect = new Rectangle(0, 0, Thumbnail.Width, Thumbnail.Height);
                Rectangle resizedThumbnailRect = thumbnailRect.ResizeTo(pictureBoxVideoThumbnail.ClientSize)
                    .CenterIn(pictureBoxVideoThumbnail.ClientRectangle);
                e.Graphics.DrawImage(Thumbnail, resizedThumbnailRect);
            }

            if (VideoInfo != null && VideoInfo.Duration.Ticks > 0L)
            {
                try
                {
                    using (Font fnt = new Font("Lucida Console", 10.0f))
                    {
                        TimeSpan hour = new TimeSpan(1, 0, 0);
                        string videoDurationString = VideoInfo.Duration.ToString(VideoInfo.Duration >= hour ? "h':'mm':'ss" : "m':'ss");
                        SizeF sz = e.Graphics.MeasureString(videoDurationString, fnt);
                        float x = pictureBoxVideoThumbnail.Width - sz.Width;
                        float y = pictureBoxVideoThumbnail.Height - sz.Height;
                        e.Graphics.FillRectangle(Brushes.Black, new RectangleF(x, y, sz.Width, sz.Height));
                        e.Graphics.DrawString(videoDurationString, fnt, Brushes.White, new PointF(x, y));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
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

                Thumbnail = video.ImageData != null && video.ImageData.Length > 0 ? Image.FromStream(video.ImageData) : null;
            }
            else
            {
                lblVideoTitle.Text = "Ошибка! Видео не доступно! Pointer is null!";
                lblChannelName.Text = "Канал: Не доступно";
                lblDateUploaded.Text = "Дата загрузки: Не доступно";
                lblDatePublished.Text = "Дата публикации: Не доступно";
                Thumbnail = null;
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
                if (!config.UseNumberedFileNames && File.Exists(outputFileName))
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

                                if (IsCanceling)
                                {
                                    errorCode = FileDownloader.DOWNLOAD_ERROR_CANCELED_BY_USER;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return 400;
                }
                return errorCode;
            });
        }

        private async void OnMenuItemClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(config.DownloadingDirPath) || string.IsNullOrWhiteSpace(config.DownloadingDirPath))
            {
                MessageBox.Show("Не указана папка для скачивания!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Directory.Exists(config.DownloadingDirPath))
            {
                MessageBox.Show("Папка для скачивания не найдена!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(config.OutputFileNameFormat) || string.IsNullOrWhiteSpace(config.OutputFileNameFormat))
            {
                MessageBox.Show("Не указан формат имени файла!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IsDownloading = true;
            btnDownload.Text = "Отмена";
            IsCanceling = false;

            string path = config.DownloadingDirPath.EndsWith("\\") ? config.DownloadingDirPath : config.DownloadingDirPath + "\\";
            string fixedFileName = FixFileName(FormatFileName(config.OutputFileNameFormat, VideoInfo));
            string outputFilePath = $"{path}{fixedFileName}.ts";
            if (config.UseNumberedFileNames)
            {
                outputFilePath = MultiThreadedDownloader.GetNumberedFileName(outputFilePath);
            }
            RuTubeVideoFormat videoFormat = (sender as ToolStripMenuItem).Tag as RuTubeVideoFormat;
            int errorCode = await DownloadFormat(videoFormat, outputFilePath);
            if (errorCode == 200)
            {
                string outputFilePathWithoutExt = outputFilePath.Substring(0, outputFilePath.Length - 3);
                if (config.SaveVideoThumbnail)
                {
                    lblProgress.Text = "Состояние: Сохранение картинки...";
                    lblProgress.Refresh();
                    SaveVideoThumbnailToFile(outputFilePathWithoutExt);
                }

                if (config.SaveVideoInfo)
                {
                    lblProgress.Text = "Состояние: Сохранение информации...";
                    lblProgress.Refresh();
                    SaveVideoInfoToFile(outputFilePathWithoutExt);
                }

                FileInfo fileInfo = new FileInfo(outputFilePath);
                string fileSizeString = fileInfo != null ? $"{fileInfo.Length} байт" : "Не доступно";
                lblProgress.Text = $"Состояние: Скачано. Размер файла: {fileSizeString}. {videoFormat.GetShortInfo()}.";
                MessageBox.Show($"{VideoInfo.Title}\nСкачано!", "Успех, батенька!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (errorCode == FileDownloader.DOWNLOAD_ERROR_CANCELED_BY_USER)
                {
                    lblProgress.Text = "Состояние: Скачивание отменено!";
                    MessageBox.Show("Скачивание отменено!", "Отменятор отменения отмены",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    lblProgress.Text = $"Состояние: Ошибка {errorCode}.";
                    MessageBox.Show($"Код ошибки: {errorCode}\nГугол в помощь, как говоритса!", "Неудача! :'(",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            btnDownload.Text = "Скачать";
            btnDownload.Enabled = true;
            IsDownloading = false;
            IsCanceling = false;
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
            if (IsDownloading)
            {
                IsCanceling = true;
                btnDownload.Enabled = false;
                return;
            }

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

        private bool SaveVideoThumbnailToFile(string filePathWithoutExt)
        {
            if (VideoInfo != null && VideoInfo.ImageData != null && VideoInfo.ImageData.Length > 0)
            {
                string filePath = filePathWithoutExt + "_thumbnail.jpg";
                if (config.UseNumberedFileNames)
                {
                    filePath = MultiThreadedDownloader.GetNumberedFileName(filePath);
                }
                else if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return VideoInfo.ImageData.SaveToFile(filePath);
            }
            return false;
        }

        private bool SaveVideoInfoToFile(string filePathWithoutExt)
        {
            if (VideoInfo != null)
            {
                try
                {
                    string filePath = filePathWithoutExt + "_info.txt";
                    if (config.UseNumberedFileNames)
                    {
                        filePath = MultiThreadedDownloader.GetNumberedFileName(filePath);
                    }
                    else if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    string t = $"{VideoInfo}\n";
                    if (VideoInfo.ChannelOwned != null)
                    {
                        t += $"Channel info:\n{VideoInfo.ChannelOwned}\n";
                    }
                    File.WriteAllText(filePath, t);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return false;
        }
    }
}
