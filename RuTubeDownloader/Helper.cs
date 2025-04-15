using System.IO;
using MultiThreadedDownloaderLib;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;

namespace RuTubeDownloader
{
    public static class Helper
    {
        public static bool SaveToFile(this Stream stream, string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrWhiteSpace(filePath))
            {
                try
                {
                    using (Stream fileStream = File.OpenWrite(filePath))
                    {
                        stream.Position = 0L;
                        bool Result = MultiThreadedDownloader.AppendStream(stream, fileStream);
                        return Result;
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return false;
        }

        public static Rectangle ResizeTo(this Rectangle source, Windows.Foundation.Size newSize)
        {
            float aspectSource = (float)(source.Height / (float)source.Width);
            float aspectDest = (float)(newSize.Height / (float)newSize.Width);
            int w = (int)newSize.Width;
            int h = (int)newSize.Height;
            if (aspectSource > aspectDest)
            {
                w = (int)(newSize.Height / aspectSource);
            }
            else if (aspectSource < aspectDest)
            {
                h = (int)(newSize.Width * aspectSource);
            }

            // Fix: Set Width and Height properties instead of using a non-existent constructor
            source.Width = w;
            source.Height = h;
            return source;
        }

        public static Rectangle CenterIn(this Rectangle source, Rectangle dest)
        {
            int x = (int)(dest.Width / 2 - source.Width / 2);
            int y = (int)(dest.Height / 2 - source.Height / 2);

            // Fix: Adjust the position of the source Rectangle
            source.Margin = new Thickness(x, y, 0, 0);
            return source;
        }
    }
}
