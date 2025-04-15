using RuTubeApi;
using System;

namespace RuTubeDownloader
{
    public class ToolStripMenuItem
    {
        internal RuTubeVideoFormat Tag;
        internal Action<object, EventArgs> Click;
        private string fmt;

        public ToolStripMenuItem(string fmt)
        {
            this.fmt = fmt;
        }
    }
}