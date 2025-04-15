using System.IO;
//using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace RuTubeDownloader
{
    internal static class Utils
    {
        internal const string FILENAME_FORMAT_DEFAULT =
            "[<year_published>-<month_published>-<day_published> " +
            "<hour_published>-<minute_published>-<second_published>] " +
            "<video_title> (id_<video_id>)";
        internal const string USER_AGENT_DEFAULT =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:104.0) Gecko/20100101 Firefox/104.0";

        internal static MainConfiguration config;

        private static string LeadZero(int n)
        {
            return n < 10 ? $"0{n}" : n.ToString();
        }

        internal static string FormatFileName(string fmt, RuTubeApi.RuTubeVideo video)
        {
            if (video == null || string.IsNullOrEmpty(fmt) || string.IsNullOrWhiteSpace(fmt))
            {
                return "video";
            }

            string channelName = video.ChannelOwned != null ? video.ChannelOwned.Name : "unknown_channel";
            string t =
                fmt.Replace("<year_published>", LeadZero(video.DatePublished.Year))
                .Replace("<month_published>", LeadZero(video.DatePublished.Month))
                .Replace("<day_published>", LeadZero(video.DatePublished.Day))
                .Replace("<hour_published>", LeadZero(video.DatePublished.Hour))
                .Replace("<minute_published>", LeadZero(video.DatePublished.Minute))
                .Replace("<second_published>", LeadZero(video.DatePublished.Second))
                .Replace("<video_title>", video.Title)
                .Replace("<video_id>", video.Id)
                .Replace("<channel_name>", channelName);
            return t;
        }

        internal static string FixFileName(string fn)
        {
            return fn.Replace("\\", "\u29F9").Replace("|", "\u2758").Replace("/", "\u2044")
                .Replace("?", "\u2753").Replace(":", "\uFE55").Replace("<", "\u227A").Replace(">", "\u227B")
                .Replace("\"", "\u201C").Replace("*", "\uFE61").Replace("^", "\u2303").Replace("\n", string.Empty);
        }
    }

    internal sealed class MainConfiguration
    {
        public string FileName { get; private set; }
        public string SelfDirPath { get; private set; }
        public string DownloadingDirPath { get; set; }
        public string OutputFileNameFormat { get; set; }
        public string UserAgent { get; set; }
        public bool UseNumberedFileNames { get; set; }
        public bool SaveVideoThumbnail { get; set; }
        public bool SaveVideoInfo { get; set; }

        public delegate void SavingDelegate(object sender, JObject root);
        public delegate void LoadingDelegate(object sender, JObject root);
        public delegate void LoadedDelegate(object sender);
        public SavingDelegate Saving;
        public LoadingDelegate Loading;
        public LoadedDelegate Loaded;

        public MainConfiguration(string fileName)
        {
            FileName = fileName;
            //RnD / TODO
            SelfDirPath = Path.GetDirectoryName(/*Application.ExecutablePath*/default) + "\\";

            LoadDefaults();
        }

        public void Save()
        {
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            JObject json = new JObject();
            Saving?.Invoke(this, json);
            File.WriteAllText(FileName, json.ToString());
        }

        public void LoadDefaults()
        {
            OutputFileNameFormat = Utils.FILENAME_FORMAT_DEFAULT;
            UserAgent = Utils.USER_AGENT_DEFAULT;
            UseNumberedFileNames = true;
            SaveVideoThumbnail = true;
            SaveVideoInfo = false;
        }

        public void Load()
        {
            if (File.Exists(FileName))
            {
                JObject json = JObject.Parse(File.ReadAllText(FileName));
                if (json != null)
                {
                    Loading?.Invoke(this, json);
                }
            }
            Loaded?.Invoke(this);
        }
    }
}
