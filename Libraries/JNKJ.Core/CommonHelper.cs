using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using JNKJ.Core.ComponentModel;
namespace JNKJ.Core
{
    /// <summary>
    /// 公共帮助类
    /// </summary>
    public partial class CommonHelper
    {
        /// <summary>
        /// 确定是邮箱地址，或者抛出异常.
        /// </summary>
        /// <param name="email">要校验的邮箱地址.</param>
        /// <returns></returns>
        public static string EnsureSubscriberEmailOrThrow(string email)
        {
            string output = EnsureNotNull(email);
            output = output.Trim();
            output = EnsureMaximumLength(output, 255);
            if (!IsValidEmail(output))
            {
                throw new ExceptionExt("Email is not valid.");
            }
            return output;
        }
        /// <summary>
        /// 验证字符串是否是有效的电子邮件格式。
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <returns>true表示是邮箱地址</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            email = email.Trim();
            var result = Regex.IsMatch(email, "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$", RegexOptions.IgnoreCase);
            return result;
        }
        /// <summary>
        /// 生成随机数字代码
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>返回生成的随机码</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = string.Concat(str, random.Next(10).ToString());
            return str;
        }
        /// <summary>
        /// 返回一个指定区间的随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }
        /// <summary>
        /// 返回在指定长度内字符串
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <param name="postfix">如果小于最大长度，用于填充的字符</param>
        /// <returns>处理后的字符串</returns>
        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (str.Length > maxLength)
            {
                var result = str.Substring(0, maxLength);
                if (!string.IsNullOrEmpty(postfix))
                {
                    result += postfix;
                }
                return result;
            }
            else
            {
                return str;
            }
        }
        /// <summary>
        /// 确保字符串只包含数字值
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <returns>处理后的字符串，如果为null,返回null</returns>
        public static string EnsureNumericOnly(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            var result = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsDigit(c))
                    result.Append(c);
            }
            return result.ToString();
        }
        /// <summary>
        /// null字符串处理
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <returns>为null是，返回empty</returns>
        public static string EnsureNotNull(string str)
        {
            if (str == null)
                return string.Empty;
            return str;
        }
        /// <summary>
        /// 判断字符串数组是否有null或者empty
        /// </summary>
        /// <param name="stringsToValidate">要验证的数组</param>
        /// <returns>有空值返回true</returns>
        public static bool AreNullOrEmpty(params string[] stringsToValidate)
        {
            bool result = false;
            Array.ForEach(stringsToValidate, str =>
            {
                if (string.IsNullOrEmpty(str)) result = true;
            });
            return result;
        }
        /// <summary>
        /// 比较两个数组
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="a1">数组一</param>
        /// <param name="a2">数组二</param>
        /// <returns>不相同返回false</returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            //also see Enumerable.SequenceEqual(a1, a2);
            if (ReferenceEquals(a1, a2))
                return true;
            if (a1 == null || a2 == null)
                return false;
            if (a1.Length != a2.Length)
                return false;
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }
        private static AspNetHostingPermissionLevel? _trustLevel = null;
        /// <summary>
        ///查找正在运行的应用程序的信任级别。 
        /// </summary>
        /// <returns>当前信任级别 .</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                //设置最小值
                _trustLevel = AspNetHostingPermissionLevel.None;
                //确认最大值
                foreach (AspNetHostingPermissionLevel trustLevel in
                        new AspNetHostingPermissionLevel[] {
                                AspNetHostingPermissionLevel.Unrestricted,
                                AspNetHostingPermissionLevel.High,
                                AspNetHostingPermissionLevel.Medium,
                                AspNetHostingPermissionLevel.Low,
                                AspNetHostingPermissionLevel.Minimal
                            })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        _trustLevel = trustLevel;
                        break; //设置最高权限
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }
                }
            }
            return _trustLevel.Value;
        }
        /// <summary>
        /// 给对象的属性设置值
        /// </summary>
        /// <param name="instance">要设置属性的对象</param>
        /// <param name="propertyName">要设置的属性名.</param>
        /// <param name="value">要设置的值</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            Type instanceType = instance.GetType();
            PropertyInfo pi = instanceType.GetProperty(propertyName);
            if (pi == null)
                throw new ExceptionExt("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);
            if (!pi.CanWrite)
                throw new ExceptionExt("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType);
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = To(value, pi.PropertyType);
            pi.SetValue(instance, value, new object[0]);
        }
        public static TypeConverter GetJNKJCustomTypeConverter(Type type)
        {
            if (type == typeof(List<int>))
                return new GenericListTypeConverter<int>();
            if (type == typeof(List<decimal>))
                return new GenericListTypeConverter<decimal>();
            if (type == typeof(List<string>))
                return new GenericListTypeConverter<string>();
            return TypeDescriptor.GetConverter(type);
        }
        /// <summary>
        /// 将值转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="destinationType">要转换的类型.</param>
        /// <returns>转换后的值</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 将值转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="destinationType">要转换的类型.</param>
        /// <param name="culture">Culture</param>
        /// <returns>返回值.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();
                TypeConverter destinationConverter = GetJNKJCustomTypeConverter(destinationType);
                TypeConverter sourceConverter = GetJNKJCustomTypeConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);
                if (!destinationType.IsAssignableFrom(value.GetType()))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }
        /// <summary>
        /// 将值转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="destinationType">要转换的类型.</param>
        /// <returns>转换后的值.</returns>
        public static T To<T>(object value)
        {
            return (T)To(value, typeof(T));
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <returns>转换后的值</returns>
        public static string ConvertEnum(string str)
        {
            string result = string.Empty;
            char[] letters = str.ToCharArray();
            foreach (char c in letters)
                if (c.ToString() != c.ToString().ToLower())
                    result += " " + c.ToString();
                else
                    result += c.ToString();
            return result;
        }
        /// <summary>
        /// 设置 Telerik (Kendo UI) 
        /// </summary>
        public static void SetTelerikCulture()
        {
            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
