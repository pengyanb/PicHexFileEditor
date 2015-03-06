using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicHexFileEditorLib.DataType
{
    public class ScrollingEventArgs : EventArgs
    {
        public ScrollingEventArgs(System.Windows.Forms.Message m)
        {
            this.m = m;
        }
        public System.Windows.Forms.Message m;
    }
    public class SynchronizedRichTextBox : RichTextBox
    {
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int WM_MOUSEWHEEL = 0x20A;

        public SynchronizedRichTextBox()
            : base()
        {
        }

        public event EventHandler Scrolling;
       
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_VSCROLL || m.Msg == WM_MOUSEWHEEL)
            {
                if (this.Scrolling != null)
                {
                    this.Scrolling(this, new ScrollingEventArgs(m));
                }
            }
        }

        public void PublicWndProc(ref System.Windows.Forms.Message msg)
        {
            base.WndProc(ref msg);
        }
    }
}

