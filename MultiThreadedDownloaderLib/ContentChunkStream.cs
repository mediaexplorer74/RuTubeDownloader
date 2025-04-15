﻿using System;
using System.IO;

namespace MultiThreadedDownloaderLib
{
	public sealed class ContentChunkStream : IDisposable
	{
		public Stream Stream { get; private set; }
		public string FilePath { get; }

		public ContentChunkStream(Stream stream, string filePath = null)
		{
			Stream = stream;
			FilePath = filePath;
		}

		public void Dispose()
		{
			if (Stream != null)
			{
				Stream.Flush(); // optional, but good practice ?
                Stream.Dispose();
                Stream = null;
			}
		}
	}
}
