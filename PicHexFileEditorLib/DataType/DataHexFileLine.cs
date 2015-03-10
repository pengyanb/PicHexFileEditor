using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PicHexFileEditorLib.StaticUtility;

namespace PicHexFileEditorLib.DataType
{
    public class DataHexFileLine
    {
        public DataHexFileLine(int lineIndex, String lineString)
        {
            this.lineIndex = lineIndex;
            this.lineString = lineString;
            lineDataCount = (byte) StaticUtilityClass.convertStringToHexValue(lineString.Substring(1, 2));
            lineDataAddress = (UInt16)StaticUtilityClass.convertStringToHexValue(lineString.Substring(3, 4));
            lineDataType = (byte)StaticUtilityClass.convertStringToHexValue(lineString.Substring(7, 2));
            lineChecksum = (byte)StaticUtilityClass.convertStringToHexValue(lineString.Substring(lineString.Length - 2, 2));
            lineDatasString = lineString.Substring(9, lineDataCount*2);
            lineDatas = new List<byte>();
            for(int i=0; i<lineDataCount; i++)
            {
                byte data = (byte) StaticUtilityClass.convertStringToHexValue(lineDatasString.Substring(i*2, 2));
                lineDatas.Add(data);
            }
        }
        public int          lineIndex { get; private set; }
        public String       lineString { set; get; }
        public String       lineDatasString { set; get; }

        public byte         lineDataCount { get; set; }
        public UInt16       lineDataAddress { get; set; }
        public byte         lineDataType { get; set; }
        public List<byte>   lineDatas { get; set; }
        public byte         lineChecksum { get; set; }

    }
}
