using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace JNKJ.Common.Utility
{
    public class SqlTools
    {
        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }
        /// <summary>
        /// 改正sql语句中的转义字符
        /// </summary>
        public static string mashSQL(string str)
        {
            return (str == null) ? "" : str.Replace("\'", "'");
        }
        /// <summary>
        /// 替换sql语句中的有问题符号
        /// </summary>
        public static string ChkSQL(string str)
        {
            return (str == null) ? "" : str.Replace("'", "''");
        }
        public static string SwithSqlType(object value)
        {
            System.Type type = value.GetType();
            string typen = type.Name.ToLower();
            string rs = "";
            if (typen == "string" || typen == "datetime" || typen == "date" || typen == "")
            {
                rs = "'" + value.ToString() + "'";
            }
            else
            {
                rs = value.ToString();
            }
            return rs;
        }
    }
}
