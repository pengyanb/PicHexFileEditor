using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicHexFileEditorLib.StaticUtility
{
    public static class StaticUtilityClass
    {
        public static int getLineDataLength(String inputString)
        {
            return Convert.ToInt32(inputString, 16);
        }
    }
}
