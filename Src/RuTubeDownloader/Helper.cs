﻿using System.Drawing;
using System.IO;
using MultiThreadedDownloaderLib;

namespace RuTube_downloader
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
                        bool Result =  MultiThreadedDownloader.AppendStream(stream, fileStream);
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

        public static Rectangle ResizeTo(this Rectangle source, Size newSize)
        {
            float aspectSource = source.Height / (float)source.Width;
            float aspectDest = newSize.Height / (float)newSize.Width;
            int w = newSize.Width;
            int h = newSize.Height;
            if (aspectSource > aspectDest)
            {
                w = (int)(newSize.Height / aspectSource);
            }
            else if (aspectSource < aspectDest)
            {
                h = (int)(newSize.Width * aspectSource);
            }
            return new Rectangle(0, 0, w, h);
        }

        public static Rectangle CenterIn(this Rectangle source, Rectangle dest)
        {
            int x = dest.Width / 2 - source.Width / 2;
            int y = dest.Height / 2 - source.Height / 2;
            return new Rectangle(x, y, source.Width, source.Height);
        }
    }
}
