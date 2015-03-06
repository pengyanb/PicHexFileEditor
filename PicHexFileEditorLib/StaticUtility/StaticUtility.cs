using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicHexFileEditorLib.StaticUtility
{
    public static class StaticUtilityClass
    {
        #region getLineDataLength
        public static int getLineDataLength(String inputString)
        {
            return Convert.ToInt32(inputString, 16);
        } 
        #endregion

        public static String convertStringToHexString(String inputString)
        {
            String outputString = "";
            char[] inputStringLetters = inputString.ToCharArray();
            foreach(char letter in inputStringLetters)
            {
                outputString += String.Format("{0:x2}", Convert.ToInt32(letter));
            }   
            return outputString;
        }

        public static List<int> searchForString(String string2SearchFrom, String string2SearchFor)
        {
            List<int> indexList = new List<int>();
            int index = 0;
            while((index = string2SearchFrom.IndexOf(string2SearchFor, index)) != -1)    // StringComparison.CurrentCultureIgnoreCase)
            {
                int foundIndex = index;
                indexList.Add(foundIndex);
                index += string2SearchFor.Length + 1 ;
                Console.WriteLine(String.Format("Static Untility - found {0} at Index {1}",
                    string2SearchFrom.Substring(foundIndex, string2SearchFor.Length), foundIndex));
            }
            return indexList;
        }
    }
}
