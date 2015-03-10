using PicHexFileEditorLib.DataType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PicHexFileEditorLib.StaticUtility;

namespace PicHexFileEditorLib
{
    public class PicHexHelper
    {
        #region Variables
        private String hexFilePath;
        private String hexDataString;
        private String string2SearchFor;
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
                this.hexFilePath = hexFilePath;
                Boolean result = false;
                try
                {
                    hexDataString = "";
                    StreamReader streamReader = new StreamReader(hexFilePath);
                    dataHexFileLineList = new List<DataHexFileLine>();
                    int lineIndex = 0;
                    while (streamReader.Peek() >= 0)
                    {
                        /*
                        char[] buffer = new char[1024];
                        await streamReader.ReadAsync(buffer, 0, 1024);*/
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
            this.string2SearchFor = string2SearchFor;
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

        #region replaceHexFileLineWithString
        private void replaceHexFileLineWithString(string replaceString, int stringStartIndex, ref DataHexFileLine dataHexFileLine)
        {
            for (int i = stringStartIndex, j = 0; j < replaceString.Length; i += 2, j += 2)
            {
                dataHexFileLine.lineDatas[i / 2] = (byte)StaticUtilityClass.convertStringToHexValue(replaceString.Substring(j, 2));
            }
            dataHexFileLine.lineDatasString = dataHexFileLine.lineDatasString.Substring(0, stringStartIndex)
                + replaceString
                + dataHexFileLine.lineDatasString.Substring(stringStartIndex + replaceString.Length, dataHexFileLine.lineDatasString.Length - stringStartIndex - replaceString.Length);
            int lineSum = dataHexFileLine.lineDataCount +
                          (dataHexFileLine.lineDataAddress >> 8) + (dataHexFileLine.lineDataAddress & 0x00FF)
                          + dataHexFileLine.lineDataType;
            for (int i = 0; i < dataHexFileLine.lineDatas.Count; i++)
                lineSum += dataHexFileLine.lineDatas[i];
            dataHexFileLine.lineChecksum = (byte)((~lineSum) + 1);
            dataHexFileLine.lineString = String.Format(":{0:X2}{1:X4}{2:X2}{3}{4:X2}",
                dataHexFileLine.lineDataCount, dataHexFileLine.lineDataAddress, dataHexFileLine.lineDataType,
                dataHexFileLine.lineDatasString, dataHexFileLine.lineChecksum);
        } 
        #endregion

        #region replaceWithStringAtFoundIndex
        public Boolean replaceWithStringAtFoundIndex(String replaceString, int index)
        {
            Boolean result = false;
            try
            {
                if (index < dataFoundStringInfoList.Count)  //replace indivial
                {
                    DataFoundStringInfo dataFoundStringInfo = dataFoundStringInfoList.ElementAt(index);
                    DataHexFileLine dataHexFileLine = dataHexFileLineList.ElementAt(dataFoundStringInfo.lineIndex);
                    //Console.WriteLine("Old: " +dataHexFileLine.lineString);
                    if ((dataFoundStringInfo.charIndex + string2SearchFor.Length) <= (dataHexFileLine.lineDatas.Count * 2))  //in same line
                    {
                        replaceHexFileLineWithString(replaceString, dataFoundStringInfo.charIndex, ref dataHexFileLine);
                        //Console.WriteLine("New: " + dataHexFileLine.lineString);
                    }
                    else //in two lines
                    {
                        DataHexFileLine dataHexFileLine2 = dataHexFileLineList.ElementAt(dataFoundStringInfo.lineIndex + 1);
                        String subReplaceString1 = replaceString.Substring(0, dataHexFileLine.lineDatasString.Length - dataFoundStringInfo.charIndex);
                        String subReplaceString2 = replaceString.Substring(subReplaceString1.Length , replaceString.Length - subReplaceString1.Length);
                        //Console.WriteLine("Old1: " + dataHexFileLine.lineString);
                        //Console.WriteLine("Old2: " + dataHexFileLine2.lineString);
                        replaceHexFileLineWithString(subReplaceString1, dataFoundStringInfo.charIndex, ref dataHexFileLine);
                        replaceHexFileLineWithString(subReplaceString2, 0, ref dataHexFileLine2);
                        //Console.WriteLine("New1: " + dataHexFileLine.lineString);
                        //Console.WriteLine("New2: " + dataHexFileLine2.lineString);
                    }
                    result = true;
                }
                else                                        //replace all
                {
                    for (int i = 0; i < dataFoundStringInfoList.Count; i++)
                    {
                        DataFoundStringInfo dataFoundStringInfo = dataFoundStringInfoList.ElementAt(i);
                        DataHexFileLine dataHexFileLine = dataHexFileLineList.ElementAt(dataFoundStringInfo.lineIndex);
                        //Console.WriteLine("Old: " +dataHexFileLine.lineString);
                        if ((dataFoundStringInfo.charIndex + string2SearchFor.Length) <= (dataHexFileLine.lineDatas.Count * 2))  //in same line
                        {
                            replaceHexFileLineWithString(replaceString, dataFoundStringInfo.charIndex, ref dataHexFileLine);
                            //Console.WriteLine("New: " + dataHexFileLine.lineString);
                        }
                        else //in two lines
                        {
                            DataHexFileLine dataHexFileLine2 = dataHexFileLineList.ElementAt(dataFoundStringInfo.lineIndex + 1);
                            String subReplaceString1 = replaceString.Substring(0, dataHexFileLine.lineDatasString.Length - dataFoundStringInfo.charIndex);
                            String subReplaceString2 = replaceString.Substring(subReplaceString1.Length, replaceString.Length - subReplaceString1.Length);
                            replaceHexFileLineWithString(subReplaceString1, dataFoundStringInfo.charIndex, ref dataHexFileLine);
                            replaceHexFileLineWithString(subReplaceString2, 0, ref dataHexFileLine2);
                        }
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine(ex.ToString());
            }
            return result;
        } 
        #endregion

        #region saveModifiedHexToFile
        public Boolean saveModifiedHexToFile(String filePath)
        {
            Boolean result = false;
            try
            {
                String outputString = "";
                foreach(DataHexFileLine dataHexFileLine in dataHexFileLineList)
                {
                    outputString += dataHexFileLine.lineString + "\r\n";
                }
                byte[] outputBytes = Encoding.GetEncoding(28591).GetBytes(outputString);
                File.WriteAllBytes(filePath, outputBytes);
                result = true;
            }
            catch(Exception ex)
            {
                result = false;
                Console.WriteLine(ex.ToString());
            }
            return result;
        } 
        #endregion
    }
}
