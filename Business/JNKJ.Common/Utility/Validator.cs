using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace JNKJ.Common.Utility
{
    public class Validator
    {
        //        验证数字：^[0-9]*$
        //验证n位的数字：^\d{n}$
        //验证至少n位数字：^\d{n,}$
        //验证m-n位的数字：^\d{m,n}$
        //验证零和非零开头的数字：^(0|[1-9][0-9]*)$
        //验证有两位小数的正实数：^[0-9]+(.[0-9]{2})?$
        //验证有1-3位小数的正实数：^[0-9]+(.[0-9]{1,3})?$
        //验证非零的正整数：^\+?[1-9][0-9]*$
        //验证非零的负整数：^\-[1-9][0-9]*$
        //验证非负整数（正整数 + 0）  ^\d+$
        //验证非正整数（负整数 + 0）  ^((-\d+)|(0+))$
        //验证长度为3的字符：^.{3}$
        //验证由26个英文字母组成的字符串：^[A-Za-z]+$
        //验证由26个大写英文字母组成的字符串：^[A-Z]+$
        //验证由26个小写英文字母组成的字符串：^[a-z]+$
        //验证由数字和26个英文字母组成的字符串：^[A-Za-z0-9]+$
        //验证由数字、26个英文字母或者下划线组成的字符串：^\w+$
        //验证用户密码:^[a-zA-Z]\w{5,17}$ 正确格式为：以字母开头，长度在6-18之间，只能包含字符、数字和下划线。
        //验证是否含有 ^%&',;=?$\" 等字符：[^%&',;=?$\x22]+
        //验证汉字：^[\u4e00-\u9fa5],{0,}$
        //验证Email地址：^\w+[-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$
        //验证InternetURL：^http://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$ ；^[a-zA-z]+://(w+(-w+)*)(.(w+(-w+)*))*(?S*)?$
        //验证电话号码：^(\(\d{3,4}\)|\d{3,4}-)?\d{7,8}$：--正确格式为：XXXX-XXXXXXX，XXXX-XXXXXXXX，XXX-XXXXXXX，XXX-XXXXXXXX，XXXXXXX，XXXXXXXX。
        /// <summary>
        /// 验证用户密码,6到20位
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsPassword(string expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression, @"^[A-Za-z0-9!@#$%^&*.~]{6,25}$");
            return false;
        }
        /// <summary>
        /// 验证用户名，以字母开头，只能包含字母，数字，小数点和@符号
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsUserName(string expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression, @"^[A-Za-z0-9@._]{3,16}$");
            return false;
        }
        /// <summary>
        /// 验证由数字、26个英文字母或者下划线组成的字符串
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsChar(string expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression, @"^\w+$");
            return false;
        }
        /// <summary>
        /// 是否带有特殊字符 ^%&',;=?$\"
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsSpecialChar(string expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression, @"[^%&',;=?$\x22]+");
            return false;
        }
        /// <summary>
        /// 是否是汉字
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsChinese(string expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression, @"^[\u4e00-\u9fa5],{0,}$");
            return false;
        }
        /// <summary>
        /// 是否是有效的电话号码
        /// <example>
        /// 正确格式为： (XXX)XXX-XXXX 格式或 (XXX)XXXX-XXXX
        /// </example>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsPhone(string expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression, @"((\\(\\d{3}\\) ?)|(\\d{3}-))?\\d{3}-\\d{4}|((\\(\\d{3}\\) ?)|(\\d{4}-))?\\d{4}-\\d{4}");
            return false;
        }
        /// <summary>
        /// 判断是否是有效的手机号码
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsMobile(string expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression, @"^0*(13|15)\d{9}$");
            return false;
        }
        /// <summary>
        /// 是否是有效的身份证
        /// 15或者18位
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsIdCard(string expression)
        {
            //身份证正则表达式(15位) 
            string isIDCard1 = @"^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$";
            //身份证正则表达式(18位) 
            string isIDCard2 = @"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{4}$";
            if (expression != null)
                return (Regex.IsMatch(expression, isIDCard1) || Regex.IsMatch(expression, isIDCard2));
            return false;
        }
        /// <summary>
        /// 是否是正浮点数
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsPositiveFloat(string expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression, @"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$");
            return false;
        }
        /// <summary>
        /// 是否是负浮点型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsNegativeFloat(string expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression, @"^(-(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*)))$");
            return false;
        }
        /// <summary>
        /// 是否是浮点型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsFloat(string expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression, @"^(-?\d+)(\.\d+)?$");
            return false;
        }
        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object expression)
        {
            if (expression != null)
                return IsNumeric(expression.ToString());
            return false;
        }
        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(string expression)
        {
            if (expression != null)
            {
                string str = expression;
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 是否为Double类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");
            return false;
        }
        /// <summary>
        /// 判断给定的字符串数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        /// <param name="strNumber">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsNumericArray(string[] strNumber)
        {
            if (strNumber == null)
                return false;
            if (strNumber.Length < 1)
                return false;
            foreach (string id in strNumber)
            {
                if (!IsNumeric(id))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^(?:[\w\!\#\\$\%\\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+\.)*[\w\\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\.)){0,61}[a-zA-Z0-9]?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\[(?:(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\]))$");
        }
        //public static bool IsValidDoEmail(string strEmail)
        //{
        //    return Regex.IsMatch(strEmail, @"^@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        //}
        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsURL(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }
        /// <summary>
        /// 判断是否是有效时间格式
        /// </summary>
        /// <returns></returns>
        public static bool IsTime(string timeval)
        {
            return Regex.IsMatch(timeval, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }
        /// <summary>
        /// 判断字符串是否是yy-mm-dd字符串
        /// </summary>
        /// <param name="str">待判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsDateString(string str)
        {
            return Regex.IsMatch(str, @"(\d{4})-(\d{1,2})-(\d{1,2})");
        }
        /// <summary>
        /// 验证是否为正整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(string str)
        {
            return Regex.IsMatch(str, @"^[0-9]*$");
        }
        /// <summary>
        /// 是否为数值串列表，各数值间用","间隔
        /// </summary>
        /// <param name="numList"></param>
        /// <returns></returns>
        public static bool IsNumericList(string numList)
        {
            if (StrIsNullOrEmpty(numList))
                return false;
            return IsNumericArray(numList.Split(','));
        }
        /// <summary>
        /// 检查颜色值是否为3/6位的合法颜色
        /// </summary>
        /// <param name="color">待检查的颜色</param>
        /// <returns></returns>
        public static bool CheckColorValue(string color)
        {
            if (StrIsNullOrEmpty(color))
                return false;
            color = color.Trim().Trim('#');
            if (color.Length != 3 && color.Length != 6)
                return false;
            //不包含0-9  a-f以外的字符
            if (!Regex.IsMatch(color, "[^0-9a-f]", RegexOptions.IgnoreCase))
                return true;
            return false;
        }
        /// <summary>
        /// 字段串是否为Null或为""(空)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StrIsNullOrEmpty(string str)
        {
            if (str == null || str.Trim() == string.Empty)
                return true;
            return false;
        }
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        public static bool IsIPSect(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
        }
        /// <summary>
        /// 返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool InIPArray(string ip, string[] iparray)
        {
            string[] userip = StrTools.SplitString(ip, @".");
            for (int ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
            {
                string[] tmpip = StrTools.SplitString(iparray[ipIndex], @".");
                int r = 0;
                for (int i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*")
                        return true;
                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i])
                            r++;
                        else
                            break;
                    }
                    else
                        break;
                }
                if (r == 4)
                    return true;
            }
            return false;
        }
        public static string testreg(string reg)
        {
            MatchCollection matches = Regex.Matches(reg, @"\{([^\{^\}]*)\}");
            StringBuilder sb = new StringBuilder();//存放匹配结果
            foreach (Match match in matches)
            {
                string value = match.Value.Trim('{', '}');
                sb.AppendLine(value);
            }
            return sb.ToString();
        }
        public static bool IsImage(string ImgExt)
        {
            return Regex.IsMatch(ImgExt, @"^.(bmp|jpg|gif|png|ioc)$");
        }
    }
}
