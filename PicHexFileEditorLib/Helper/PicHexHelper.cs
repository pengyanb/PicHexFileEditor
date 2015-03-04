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
        private String hexFilePath;
        #endregion

        #region Constructor
		public PicHexHelper()
        {
        } 
	    #endregion

        public async Task<Boolean> init(String hexFilePath)
        {
            return await Task.Run(async () => {
                Boolean result = false;
                try
                {
                    StreamReader streamReader = new StreamReader(hexFilePath);
                    List<DataHexFileLine> dataHexFileLineList = new List<DataHexFileLine>();
                    while(streamReader.Peek()>=0)
                    {
                        String hexFileLine = await streamReader.ReadLineAsync();
                        DataHexFileLine dataHexFileLine = new DataHexFileLine(hexFileLine);
                        dataHexFileLineList.Add(dataHexFileLine);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    result = false;
                }
                return result;
            });
            
        }
    }
}
