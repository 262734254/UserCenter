using System;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.VisualBasic;
using System.Collections;
namespace JNKJ.Common.Utility
{
    public class StrTools
    {
        private static Regex RegexBr = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);
        #region 返回字符串真实长度, 1个汉字长度为2
        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns>字符长度</returns>
        public static int GetStringLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }
        #endregion
        #region 判断字符串里是否含有制定的字符
        /// <summary>
        /// 判断字符串里是否含有制定的字符
        /// </summary>
        /// <param name="str">要判断的字符串</param>
        /// <param name="stringarray">比较的字符</param>
        /// <param name="strsplit">分隔符</param>
        /// <returns></returns>
        public static bool IsCompriseStr(string str, string stringarray, string strsplit)
        {
            if (StrIsNullOrEmpty(stringarray))
                return false;
            str = str.ToLower();
            string[] stringArray = SplitString(stringarray.ToLower(), strsplit);
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (str.IndexOf(stringArray[i]) > -1)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region 判断指定字符串在指定字符串数组中的位置
        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetInArrayID(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower())
                        return i;
                }
                else if (strSearch == stringArray[i])
                    return i;
            }
            return -1;
        }
        #endregion
        #region 判断指定字符串在指定字符串数组中的位置
        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>		
        public static int GetInArrayID(string strSearch, string[] stringArray)
        {
            return GetInArrayID(strSearch, stringArray, true);
        }
        #endregion
        #region 判断指定字符串是否属于指定字符串数组中的一个元素
        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0;
        }
        #endregion
        #region 判断指定字符串是否属于指定字符串数组中的一个元素
        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">字符串数组</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string[] stringarray)
        {
            return InArray(str, stringarray, false);
        }
        #endregion
        #region 判断指定字符串是否属于指定字符串数组中的一个元素
        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray)
        {
            return InArray(str, SplitString(stringarray, ","), false);
        }
        #endregion
        #region 判断指定字符串是否属于指定字符串数组中的一个元素
        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit)
        {
            return InArray(str, SplitString(stringarray, strsplit), false);
        }
        #endregion
        #region 判断指定字符串是否属于指定字符串数组中的一个元素
        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit, bool caseInsensetive)
        {
            return InArray(str, SplitString(stringarray, strsplit), caseInsensetive);
        }
        #endregion
        #region 删除字符串尾部的回车/换行/空格
        /// <summary>
        /// 删除字符串尾部的回车/换行/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RTrim(string str)
        {
            for (int i = str.Length; i >= 0; i--)
            {
                if (str[i].Equals(" ") || str[i].Equals("\r") || str[i].Equals("\n"))
                {
                    str.Remove(i, 1);
                }
            }
            return str;
        }
        #endregion
        #region 清除给定字符串中的回车及换行符
        /// <summary>
        /// 清除给定字符串中的回车及换行符
        /// </summary>
        /// <param name="str">要清除的字符串</param>
        /// <returns>清除后返回的字符串</returns>
        public static string ClearBR(string str)
        {
            Match m = null;
            for (m = RegexBr.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "");
            }
            return str;
        }
        #endregion
        #region 从字符串的指定位置截取指定长度的子字符串
        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                        startIndex = startIndex - length;
                }
                if (startIndex > str.Length)
                    return "";
            }
            else
            {
                if (length < 0)
                    return "";
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                        return "";
                }
            }
            if (str.Length - startIndex < length)
                length = str.Length - startIndex;
            return str.Substring(startIndex, length);
        }
        #endregion
        #region 去除掉首尾指定的字符
        /// <summary>
        /// 去除掉首尾指定的字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static string TrimString(string str, char fields)
        {
            if (str[0] == fields) str = str.Remove(0, 1);
            int len = str.Length - 1;
            if (str[len] == fields) str = str.Remove(len, 1);
            return str;
        }
        #endregion
        #region 从字符串的指定位置开始截取到字符串结尾的了符串
        /// <summary>
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }
        #endregion
        #region 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }
        #endregion
        #region 含有中文字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// <summary>
        /// 含有中文字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="str">要检查的字符串</param>
        /// <param name="len">指定长度</param>
        /// <param name="p_TailString">用于代替的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetUnicodeSubString(string str, int len, string p_TailString)
        {
            string result = string.Empty;// 最终返回的结果
            int byteLen = System.Text.Encoding.Default.GetByteCount(str);// 单字节字符长度
            int charLen = str.Length;// 把字符平等对待时的字符串长度
            int byteCount = 0;// 记录读取进度
            int pos = 0;// 记录截取位置
            if (byteLen > len)
            {
                for (int i = 0; i < charLen; i++)
                {
                    if (Convert.ToInt32(str.ToCharArray()[i]) > 255)// 按中文字符计算加2
                        byteCount += 2;
                    else// 按英文字符计算加1
                        byteCount += 1;
                    if (byteCount > len)// 超出时只记下上一个有效位置
                    {
                        pos = i;
                        break;
                    }
                    else if (byteCount == len)// 记下当前位置
                    {
                        pos = i + 1;
                        break;
                    }
                }
                if (pos >= 0)
                    result = str.Substring(0, pos) + p_TailString;
            }
            else
                result = str;
            return result;
        }
        #endregion
        #region 取指定长度的字符串
        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            string myResult = p_SrcString;
            byte[] bComments = Encoding.UTF8.GetBytes(p_SrcString);
            foreach (char c in Encoding.UTF8.GetChars(bComments))
            {    //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
                if ((c > '\u0800' && c < '\u4e00') || (c > '\xAC00' && c < '\xD7A3'))
                {
                    //if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") || System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
                    //当截取的起始位置超出字段串长度时
                    if (p_StartIndex >= p_SrcString.Length)
                        return "";
                    else
                        return p_SrcString.Substring(p_StartIndex,
                                                       ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                }
            }
            if (p_Length >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);
                //当字符串长度大于起始位置
                if (bsSrcString.Length > p_StartIndex)
                {
                    int p_EndIndex = bsSrcString.Length;
                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (p_StartIndex + p_Length))
                    {
                        p_EndIndex = p_Length + p_StartIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾
                        p_Length = bsSrcString.Length - p_StartIndex;
                        p_TailString = "";
                    }
                    int nRealLength = p_Length;
                    int[] anResultFlag = new int[p_Length];
                    byte[] bsResult = null;
                    int nFlag = 0;
                    for (int i = p_StartIndex; i < p_EndIndex; i++)
                    {
                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                                nFlag = 1;
                        }
                        else
                            nFlag = 0;
                        anResultFlag[i] = nFlag;
                    }
                    if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                        nRealLength = p_Length + 1;
                    bsResult = new byte[nRealLength];
                    Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);
                    myResult = Encoding.Default.GetString(bsResult);
                    myResult = myResult + p_TailString;
                }
            }
            return myResult;
        }
        #endregion
        #region 删除指定的字符最后出现的位置之后的字符
        /// <summary>
        /// 删除指定的字符最后出现的位置之后的字符
        /// </summary>
        public static string DelLastParam(string str, string param)
        {
            if (str.Length < 1)
            {
                return "";
            }
            return str.Substring(0, str.LastIndexOf(param));
        }
        #endregion
        #region 自定义的替换字符串函数
        /// <summary>
        /// 自定义的替换字符串函数
        /// </summary>
        public static string ReplaceString(string SourceString, string SearchString, string ReplaceString, bool IsCaseInsensetive)
        {
            return Regex.Replace(SourceString, Regex.Escape(SearchString), ReplaceString, IsCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }
        #endregion
        #region 生成指定数量的html空格符号
        /// <summary>
        /// 生成指定数量的html空格符号
        /// </summary>
        public static string GetSpacesString(int spacesCount)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < spacesCount; i++)
            {
                sb.Append(" &nbsp;&nbsp;");
            }
            return sb.ToString();
        }
        #endregion
        #region  转换为简体中文
        /// <summary>
        /// 转换为简体中文
        /// </summary>
        public static string ToSChinese(string str)
        {
            return Strings.StrConv(str, VbStrConv.SimplifiedChinese, 0);
        }
        #endregion
        #region 转换为繁体中文
        /// <summary>
        /// 转换为繁体中文
        /// </summary>
        public static string ToTChinese(string str)
        {
            return Strings.StrConv(str, VbStrConv.TraditionalChinese, 0);
        }
        #endregion
        #region 分割字符串
        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!Validator.StrIsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                    return new string[] { strContent };
                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            string[] result = new string[count];
            string[] splited = SplitString(strContent, strSplit);
            for (int i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }
            return result;
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">被分割的字符串</param>
        /// <param name="strSplit">分割符</param>
        /// <param name="ignoreRepeatItem">忽略重复项</param>
        /// <param name="maxElementLength">单个元素最大长度</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem, int maxElementLength)
        {
            string[] result = SplitString(strContent, strSplit);
            return ignoreRepeatItem ? DistinctStringArray(result, maxElementLength) : result;
        }
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem, int minElementLength, int maxElementLength)
        {
            string[] result = SplitString(strContent, strSplit);
            if (ignoreRepeatItem)
            {
                result = DistinctStringArray(result);
            }
            return PadStringArray(result, minElementLength, maxElementLength);
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">被分割的字符串</param>
        /// <param name="strSplit">分割符</param>
        /// <param name="ignoreRepeatItem">忽略重复项</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem)
        {
            return SplitString(strContent, strSplit, ignoreRepeatItem, 0);
        }
        #endregion
        #region 过滤字符串数组中每个元素为合适的大小
        /// <summary>
        /// 过滤字符串数组中每个元素为合适的大小
        /// 当长度小于minLength时，忽略掉,-1为不限制最小长度
        /// 当长度大于maxLength时，取其前maxLength位
        /// 如果数组中有null元素，会被忽略掉
        /// </summary>
        /// <param name="minLength">单个元素最小长度</param>
        /// <param name="maxLength">单个元素最大长度</param>
        /// <returns></returns>
        public static string[] PadStringArray(string[] strArray, int minLength, int maxLength)
        {
            if (minLength > maxLength)
            {
                int t = maxLength;
                maxLength = minLength;
                minLength = t;
            }
            int iMiniStringCount = 0;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (minLength > -1 && strArray[i].Length < minLength)
                {
                    strArray[i] = null;
                    continue;
                }
                if (strArray[i].Length > maxLength)
                    strArray[i] = strArray[i].Substring(0, maxLength);
                iMiniStringCount++;
            }
            string[] result = new string[iMiniStringCount];
            for (int i = 0, j = 0; i < strArray.Length && j < result.Length; i++)
            {
                if (strArray[i] != null && strArray[i] != string.Empty)
                {
                    result[j] = strArray[i];
                    j++;
                }
            }
            return result;
        }
        #endregion
        #region 清除字符串数组中的重复项
        /// <summary>
        /// 清除字符串数组中的重复项
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <param name="maxElementLength">字符串数组中单个元素的最大长度</param>
        /// <returns></returns>
        public static string[] DistinctStringArray(string[] strArray, int maxElementLength)
        {
            Hashtable h = new Hashtable();
            foreach (string s in strArray)
            {
                string k = s;
                if (maxElementLength > 0 && k.Length > maxElementLength)
                {
                    k = k.Substring(0, maxElementLength);
                }
                h[k.Trim()] = s;
            }
            string[] result = new string[h.Count];
            h.Keys.CopyTo(result, 0);
            return result;
        }
        #endregion
        #region 清除字符串数组中的重复项
        /// <summary>
        /// 清除字符串数组中的重复项
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <returns></returns>
        public static string[] DistinctStringArray(string[] strArray)
        {
            return DistinctStringArray(strArray, 0);
        }
        #endregion
        #region 替换html字符
        /// <summary>
        /// 替换html字符
        /// </summary>
        public static string EncodeHtml(string strHtml)
        {
            if (strHtml != "")
            {
                strHtml = strHtml.Replace(",", "&def");
                strHtml = strHtml.Replace("'", "&dot");
                strHtml = strHtml.Replace(";", "&dec");
                strHtml = strHtml.Replace("<", "&lt;");
                strHtml = strHtml.Replace(">", "&gt;");
                strHtml = strHtml.Replace("{", "&bcl");
                strHtml = strHtml.Replace("}", "&bcr");
                return strHtml;
            }
            return "";
        }
        #endregion
        #region 还原html字符
        /// <summary>
        /// 还原html字符
        /// </summary>
        public static string DecodeHtml(string strCode)
        {
            if (!string.IsNullOrEmpty(strCode))
            {
                strCode = strCode.Replace("&def", ",");
                strCode = strCode.Replace("&dot", "'");
                strCode = strCode.Replace("&dec", ";");
                strCode = strCode.Replace("&lt;", "<");
                strCode = strCode.Replace("&gt;", ">");
                strCode = strCode.Replace("&bcl", "{");
                strCode = strCode.Replace("&bcr", "}");
                return strCode;
            }
            return "";
        }
        #endregion
        #region 进行指定的替换(脏字过滤)
        /// <summary>
        /// 进行指定的替换(脏字过滤)
        /// </summary>
        public static string StrFilter(string str, string bantext)
        {
            string text1 = "", text2 = "";
            string[] textArray1 = SplitString(bantext, "\r\n");
            for (int num1 = 0; num1 < textArray1.Length; num1++)
            {
                text1 = textArray1[num1].Substring(0, textArray1[num1].IndexOf("="));
                text2 = textArray1[num1].Substring(textArray1[num1].IndexOf("=") + 1);
                str = str.Replace(text1, text2);
            }
            return str;
        }
        #endregion
        #region  删除最后一个字符
        /// <summary>
        /// 删除最后一个字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ClearLastChar(string str)
        {
            return (str == "") ? "" : str.Substring(0, str.Length - 1);
        }
        #endregion
        #region 合并字符
        /// <summary>
        /// 合并字符
        /// </summary>
        /// <param name="source">要合并的源字符串</param>
        /// <param name="target">要被合并到的目的字符串</param>
        /// <param name="mergechar">合并符</param>
        /// <returns>合并到的目的字符串</returns>
        public static string MergeString(string source, string target)
        {
            return MergeString(source, target, ",");
        }
        /// <summary>
        /// 合并字符
        /// </summary>
        /// <param name="source">要合并的源字符串</param>
        /// <param name="target">要被合并到的目的字符串</param>
        /// <param name="mergechar">合并符</param>
        /// <returns>并到字符串</returns>
        public static string MergeString(string source, string target, string mergechar)
        {
            if (StrIsNullOrEmpty(target))
                target = source;
            else
                target += mergechar + source;
            return target;
        }
        #endregion
        #region 清除UBB标签
        /// <summary>
        /// 清除UBB标签
        /// </summary>
        /// <param name="sDetail">帖子内容</param>
        /// <returns>帖子内容</returns>
        public static string ClearUBB(string sDetail)
        {
            return Regex.Replace(sDetail, @"\[[^\]]*?\]", string.Empty, RegexOptions.IgnoreCase);
        }
        #endregion
        #region 删除掉指定TEXT中的脚本
        /// <summary>
        /// 删除掉指定TEXT中的脚本
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string CleartHtml(string HTML)
        {
            Regex regEx = new Regex(@"<script[^>]*?>.*?</script>");
            return regEx.Replace(HTML, "");
        }
        #endregion
        #region 从HTML中获取文本,保留br,p,img
        /// <summary>
        /// 从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string HTML)
        {
            Regex regEx = new Regex(@"</?(?!br|/?p|img)[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return regEx.Replace(HTML, "");
        }
        #endregion
        #region 去除HTML标记
        ///   <summary>   
        ///   去除HTML标记   
        ///   </summary>   
        ///   <param   name="NoHTML">包括HTML的源码   </param>   
        ///   <returns>已经去除后的文字</returns>   
        public static string CleartScript(string Htmlstring)
        {
            if (string.IsNullOrEmpty(Htmlstring)) return "";
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"href*=*.*script*.*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            //regex_str="<script type=\\s*[^>]*>[^<]*?</script>";//替换<style>内容</style>为空格
            Htmlstring = Regex.Replace(Htmlstring, @"(?is)<style[^>]*>.*?</style>", "");
            //regex_str = "(&nbsp;)+";//替换&nbsp;为空格
            Htmlstring = Regex.Replace(Htmlstring, @"(?i)&nbsp;", " ");
            //regex_str = "<[^<]*>";//替换Html标签为空
            Htmlstring = Regex.Replace(Htmlstring, @"<[^<>]*>", "");
            //regex_str = "\n*";//替换\n为空
            Htmlstring = Regex.Replace(Htmlstring, @"\n*", "", RegexOptions.IgnoreCase);
            //替换\t为空
            Htmlstring = Regex.Replace(Htmlstring, @"\t*", "", RegexOptions.IgnoreCase);
            //替换'为’
            Htmlstring = Regex.Replace(Htmlstring, @"'", "’", RegexOptions.IgnoreCase);
            //替换若干个空格为一个空格
            Htmlstring = Regex.Replace(Htmlstring, @" +", "  ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<.+?>", "", RegexOptions.IgnoreCase);//替换掉"<"和">"之间的内容
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = System.Web.HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }
        #endregion
        #region 按指定长度生成随即数
        public static string str = @"0123456789abcdefghigklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ";
        /// <summary>
        /// 按指定长度生成随即数
        /// </summary>
        /// <param name="lenght">长度</param>
        /// <param name="type">类型，1，数字，2.字母,3.大写字母 4.大小写字母,0.数字，字母。</param>
        /// <returns></returns>
        public static string RandomString(int lenght, int type)
        {
            Random rnd = new Random();
            string rsstr = "";
            switch (type)
            {
                case 1:
                    for (int i = 0; i < lenght; i++)
                    {
                        // 返回数字  
                        rsstr += rnd.Next(10).ToString();
                    }
                    break;
                case 2:
                    for (int i = 0; i < lenght; i++)
                    {
                        // 返回小写字母  
                        rsstr += str.Substring(10 + rnd.Next(26), 1);
                    }
                    break;
                case 3:
                    for (int i = 0; i < lenght; i++)
                    {
                        // 返回大写字母  
                        rsstr += str.Substring(36 + rnd.Next(26), 1);
                    }
                    break;
                case 4:
                    for (int i = 0; i < lenght; i++)
                    {
                        // 返回大小写字母混合  
                        rsstr += str.Substring(10 + rnd.Next(52), 1);
                    }
                    break;
                case 0:
                    for (int i = 0; i < lenght; i++)
                    {
                        // 返回大小写字母和数字混合  
                        rsstr += str.Substring(0 + rnd.Next(61), 1);
                    }
                    break;
                default:
                    for (int i = 0; i < lenght; i++)
                    {
                        // 返回大小写字母和数字混合  
                        rsstr += str.Substring(0 + rnd.Next(61), 1);
                    }
                    break;
            }
            return rsstr;
        }
        #endregion
        #region 注册脚本
        /// <summary>
        /// 注册脚本
        /// </summary>
        /// <param name="page">页面</param>
        /// <param name="messagestr">脚本</param>
        /// <param name="t"></param>
        public static void RegisterClientScript(System.Web.UI.Page page, string messagestr, int t)
        {
            switch (t)
            {
                case 1:
                    page.ClientScript.RegisterClientScriptBlock(page.GetType(), "regscpt", messagestr);
                    break;
                case 2:
                    page.ClientScript.RegisterClientScriptInclude(page.GetType(), "regscpt", messagestr);
                    break;
                default:
                    page.ClientScript.RegisterStartupScript(page.GetType(), "regscpt", messagestr);
                    break;
            }
        }
        #endregion
        #region 字段串是否为Null或为""(空)
        /// <summary>
        /// 字段串是否为Null或为""(空)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool StrIsNullOrEmpty(string str)
        {
            if (str == null || str.Trim() == string.Empty)
                return true;
            return false;
        }
        #endregion
        #region 获得字符串中开始和结束字符串中间得值
        /// <summary>
        /// 获得字符串中开始和结束字符串中间得值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="s">开始</param>
        /// <param name="e">结束</param>
        /// <returns></returns> 
        public static string GetValue(string str, string s, string e)
        {
            Regex rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(str).Value;
        }
        #endregion
        #region 生成指定长度的字符串
        /// <summary>
        /// 生成指定长度的字符串,即生成strLong个str字符串
        /// </summary>
        /// <param name="strLong">生成的长度</param>
        /// <param name="str">以str生成字符串</param>
        /// <returns></returns>
        public static string StringOfChar(int strLong, string str)
        {
            string ReturnStr = "";
            for (int i = 0; i < strLong; i++)
            {
                ReturnStr += str;
            }
            return ReturnStr;
        }
        public static string FillString(string str, string field, int lenght)
        {
            if (string.IsNullOrEmpty(field)) field = "*";
            if (string.IsNullOrEmpty(str)) return StringOfChar(lenght, field);
            int strlen = str.Length;
            //如果长过预期，那么截取字段
            if (strlen >= lenght) return str.Substring(0, lenght);
            for (int i = 0; i < lenght - strlen; i++)
            {
                str = field + str;
            }
            return str;
        }
        /// <summary>
        /// 带回传函数的添加编辑删除提示
        /// </summary>
        /// <param name="msgtitle">提示文字</param>
        /// <param name="url">返回地址</param>
        /// <param name="msgcss">CSS样式</param>
        /// <param name="callback">JS回调函数</param>
        protected static string JavaSrciptMsg(string msgtitle, string url, string msgcss, string callback)
        {
            string msbox = "parent.jsprint(\"" + msgtitle + "\", \"" + url + "\", \"" + msgcss + "\", " + callback + ")";
            return msbox;
        }
        #endregion
        #region 将字符串转换为Int32类型
        /// <summary>
        /// 将字符串转换为Int32类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string expression, int defValue)
        {
            if (string.IsNullOrEmpty(expression) || expression.Trim().Length >= 11 || !Regex.IsMatch(expression.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
                return defValue;
            int rv;
            if (int.TryParse(expression, out rv))
                return rv;
            return Convert.ToInt32(StrToFloat(expression, defValue));
        }
        #endregion
        #region string型转换为float型
        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(string expression, float defValue)
        {
            if ((expression == null) || (expression.Length > 10))
                return defValue;
            float intValue = defValue;
            if (expression != null)
            {
                bool IsFloat = Regex.IsMatch(expression, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                    float.TryParse(expression, out intValue);
            }
            return intValue;
        }
        #endregion
        #region 删除最后结尾的一个逗号
        /// <summary>
        /// 删除最后结尾的一个逗号
        /// </summary>
        public static string DelLastComma(string str)
        {
            if (str.Length < 1)
            {
                return "";
            }
            return str.Substring(0, str.LastIndexOf(","));
        }
        #endregion
        #region 删除首尾指定字符
        ///<summary>
        /// 删除首尾指定的字符
        ///</summary>
        ///<param name="val">原字符串</param>
        ///<param name="str">要截掉的字符串</param>
        ///<param name="all">是否贪婪</param>
        ///<returns></returns>
        public static string DelFAChar(string val, string str, bool all = false)
        {
            return Regex.Replace(val, @"(^(" + str + ")" + (all ? "*" : "") + "|(" + str + ")" + (all ? "*" : "") + "$)", "");
        }
        #endregion
    }
}
