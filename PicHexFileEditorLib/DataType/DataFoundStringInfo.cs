using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicHexFileEditorLib.DataType
{
    public class DataFoundStringInfo
    {
        public int lineIndex { set; get; }
        public int charIndex { set; get; }
        public DataFoundStringInfo(int lineIndex, int charIndex)
        {
            this.lineIndex = lineIndex;
            this.charIndex = charIndex;
        }
    }
}
