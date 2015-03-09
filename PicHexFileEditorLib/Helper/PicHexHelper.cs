using PicHexFileEditorLib.DataType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicHexFileEditorLib
{
    public class PicHexHelper
    {
        #region Variables
        //private String hexFilePath;
        private String hexDataString;
        public List<DataHexFileLine> dataHexFileLineList { get; private set; }
        public List<DataFoundStringInfo> dataFoundStringInfoList { get; private set; }
        #endregion

        #region Constructor
		public PicHexHelper()
        {
            hexDataString = "";
        } 
	    #endregion

        #region init
        public async Task<Boolean> init(String hexFilePath)
        {
            return await Task.Run(async () =>
            {
                Boolean result = false;
                try
                {
                    hexDataString = "";
                    StreamReader streamReader = new StreamReader(hexFilePath);
                    dataHexFileLineList = new List<DataHexFileLine>();
                    int lineIndex = 0;
                    while (streamReader.Peek() >= 0)
                    {
                        String hexFileLine = await streamReader.ReadLineAsync();
                        DataHexFileLine dataHexFileLine = new DataHexFileLine(lineIndex, hexFileLine);
                        dataHexFileLineList.Add(dataHexFileLine);
                        hexDataString += dataHexFileLine.lineDatasString;
                        lineIndex++;
                    }
                    //Console.WriteLine(hexDataString);
                    result = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    result = false;
                }
                return result;
            });

        } 
        #endregion

        #region searchForString
        public void searchForString(string string2SearchFor)
        {
            List<int> indexList  = StaticUtility.StaticUtilityClass.searchForString(hexDataString, string2SearchFor);
            dataFoundStringInfoList = new List<DataFoundStringInfo>();
            foreach(int index in indexList)
            {
                int indexInFile = 0;
                //Console.WriteLine("Helper Found: " + hexDataString.Substring(index, string2SearchFor.Length) + " at FileIndex: " + index);
                int lineIndex = 0;
                while(indexInFile < index)  //each byte is 0x00 in string, take two characters
                {
                    if(indexInFile + (dataHexFileLineList.ElementAt(lineIndex).lineDataCount * 2)< index)
                    {
                        indexInFile += (dataHexFileLineList.ElementAt(lineIndex).lineDataCount * 2);
                        lineIndex++;
                    }
                    else
                    {
                        DataFoundStringInfo dataFoundStringInfo = new DataFoundStringInfo(lineIndex, index - indexInFile);
                        dataFoundStringInfoList.Add(dataFoundStringInfo);
                        break;
                    }
                }
            }
        } 
        #endregion

        public Boolean replaceWithString(String replaceString, int index)
        {
            return false;
        }
    }
}
