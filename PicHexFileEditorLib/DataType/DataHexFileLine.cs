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
        public DataHexFileLine(String lineString)
        {
            this.lineString = lineString;
            lineDataCount = (byte) StaticUtilityClass.getLineDataLength(lineString.Substring(1, 2));
            lineDataAddress = (UInt16)StaticUtilityClass.getLineDataLength(lineString.Substring(3, 4));
            lineDataType = (byte)StaticUtilityClass.getLineDataLength(lineString.Substring(7, 2));
            lineChecksum = (byte)StaticUtilityClass.getLineDataLength(lineString.Substring(lineString.Length - 2, 2));
            lineDatasString = lineString.Substring(9, lineDataCount*2);
            lineDatas = new List<byte>();
            for(int i=0; i<lineDataCount; i++)
            {
                byte data = (byte) StaticUtilityClass.getLineDataLength(lineDatasString.Substring(i*2, 2));
                lineDatas.Add(data);
            }
        }
        public String       lineString { set; get; }
        public String       lineDatasString { set; get; }

        public byte         lineDataCount { get; set; }
        public UInt16       lineDataAddress { get; set; }
        public byte         lineDataType { get; set; }
        public List<byte>   lineDatas { get; set; }
        public byte         lineChecksum { get; set; }

    }
}
