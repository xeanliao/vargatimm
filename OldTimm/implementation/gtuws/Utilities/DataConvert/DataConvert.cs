using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace GTU.Utilities.DataConvert
{
    public static class DataConvert
    {
        /// <summary>
        /// Convert ASCII byte[] Array to String
        /// </summary>
        /// <param name="characters">ASCII byte[] array</param>
        /// <returns>String</returns>
        public static String FromASCIIByteArray(byte[] characters)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// Convert Unicode byte[] Array to String
        /// </summary>
        /// <param name="characters">Unicode byte[] array</param>
        /// <returns>String</returns>
        public static String FromUnicodeByteArray(byte[] characters)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// Clear the end character "\0"
        /// </summary>
        /// <param name="str"></param>
        /// <returns>Return the remanent String without "\0"</returns>
        public static String ClearEndChar(String str)
        {
            if (str == "")
                return "";
            else
                return str.Substring(0, str.Length - 2);
        }

        /// <summary>
        /// Split String to String[] Based on the Separator
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="strSplit"></param>
        /// <returns></returns>
        public static String[] SplitString(String strContent, String strSplit)
        {
            if (strContent.IndexOf(strSplit) < 0)
            {
                String[] tmp = { strContent };
                return tmp;
            }
            return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Split String to String[X] Based on the Separator 
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="strSplit"></param>
        /// <param name="p_3"></param>
        /// <returns></returns>
        public static String[] SplitString(String strContent, String strSplit, int p_3)
        {
            String[] result = new string[p_3];

            String[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < p_3; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }

    }
}
