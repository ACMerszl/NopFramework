using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace Nop.Core
{
    /// <summary>
    /// Represents a common helper
    /// 表示公共帮助程序。
    /// </summary>
    public partial class CommonHelper
    {
        /// <summary>
        /// Ensures the subscriber email or throw.
        /// 确保订阅者收到邮件或发送邮件以及异常。
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static string EnsureSubscriberEmailOrThrow(string email)
        {
            string output = EnsureNotNull(email);
            output = output.Trim();
            output = EnsureMaximumLength(output, 255);

            if (!IsValidEmail(output))
            {
                throw new NopException("Email is not valid.");
            }

            return output;
        }

        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// 验证字符串是否是有效的电子邮件格式
        /// </summary>
        /// <param name="email">Email to verify电子邮件验证</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not
        /// 如果字符串是有效的电子邮件地址，则为true;如果不是，则为false</returns>
        public static bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            email = email.Trim();
            var result = Regex.IsMatch(email, "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$", RegexOptions.IgnoreCase);
            return result;
        }

        /// <summary>
        /// Verifies that string is an valid IP-Address
        /// 验证该字符串是否是有效的ip地址
        /// </summary>
        /// <param name="ipAddress">IPAddress to verifyIP地址来验证</param>
        /// <returns>true if the string is a valid IpAddress and false if it's not
        /// 如果字符串是一个有效的IpAddress，则为true;如果不是，则为false</returns>
        public static bool IsValidIpAddress(string ipAddress)
        {
            IPAddress ip;
            return IPAddress.TryParse(ipAddress, out ip);
        }

        /// <summary>
        /// Generate random digit code
        /// 生成随机数字码
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString());
            return str;
        }

        /// <summary>
        /// Returns an random interger number within a specified rage
        /// 返回指定范围内的随机整数
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length
        /// 确保字符串不超过最大允许的长度
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="postfix">A string to add to the end if the original string was shorten
        /// 如果原始字符串被缩短，要添加到末尾的字符串</param>
        /// <returns>Input string if its lengh is OK; otherwise, truncated input string
        /// 如果长度合适，则输入字符串;否则，截断输入字符串</returns>
        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (str.Length > maxLength)
            {
                var pLen = postfix == null ? 0 : postfix.Length;

                var result = str.Substring(0, maxLength - pLen);
                if (!String.IsNullOrEmpty(postfix))
                {
                    result += postfix;
                }
                return result;
            }

            return str;
        }

        /// <summary>
        /// Ensures that a string only contains numeric values
        /// 确保字符串只包含数字值
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Input string with only numeric values, empty string if input is null/empty
        /// 输入字符串只有数字值，如果输入为null/空，则为空字符串</returns>
        public static string EnsureNumericOnly(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : new string(str.Where(p => char.IsDigit(p)).ToArray());
        }

        /// <summary>
        /// Ensure that a string is not null
        /// 确保字符串不为空
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Result</returns>
        public static string EnsureNotNull(string str)
        {
            return str ?? string.Empty;
        }

        /// <summary>
        /// Indicates whether the specified strings are null or empty strings
        /// 指示指定的字符串是空字符串还是空字符串
        /// </summary>
        /// <param name="stringsToValidate">Array of strings to validate
        /// 要验证的字符串数组</param>
        /// <returns>Boolean</returns>
        public static bool AreNullOrEmpty(params string[] stringsToValidate)
        {
            return stringsToValidate.Any(p => string.IsNullOrEmpty(p));
        }

        /// <summary>
        /// Compare two arrasy
        /// 比较两个集合
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="a1">Array 1</param>
        /// <param name="a2">Array 2</param>
        /// <returns>Result</returns>
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
        //AspNet主机权限级别
        private static AspNetHostingPermissionLevel? _trustLevel;
        /// <summary>
        /// Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// 查找正在运行的应用程序的信任级别
        /// </summary>
        /// <returns>The current trust level.当前信任级别。</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                //set minimum设置最小
                _trustLevel = AspNetHostingPermissionLevel.None;

                //determine maximum确定最大
                foreach (AspNetHostingPermissionLevel trustLevel in new[] {
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
                        break; //we've set the highest permission we can我们已经设定了最高权限
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
        /// Sets a property on an object to a valuae.
        /// 将对象上的属性设置为值。
        /// </summary>
        /// <param name="instance">The object whose property to set.要设置其属性的对象。</param>
        /// <param name="propertyName">The name of the property to set.要设置的属性的名称。</param>
        /// <param name="value">The value to set the property to.要将属性设置为的值。</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            Type instanceType = instance.GetType();
            PropertyInfo pi = instanceType.GetProperty(propertyName);
            if (pi == null)
                throw new NopException("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);
            if (!pi.CanWrite)
                throw new NopException("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType);
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = To(value, pi.PropertyType);
            pi.SetValue(instance, value, new object[0]);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// 将值转换为目标类型。
        /// </summary>
        /// <param name="value">The value to convert.要转换的值。</param>
        /// <param name="destinationType">The type to convert the value to.要将值转换为的类型。</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// 将值转换为目标类型。
        /// </summary>
        /// <param name="value">The value to convert.要转换的值。</param>
        /// <param name="destinationType">The type to convert the value to.要将值转换为的类型。</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();

                var destinationConverter = TypeDescriptor.GetConverter(destinationType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);

                var sourceConverter = TypeDescriptor.GetConverter(sourceType);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);

                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);

                if (!destinationType.IsInstanceOfType(value))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// 将值转换为目标类型。
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }

        /// <summary>
        /// Convert enum for front-end
        /// 转换前端枚举
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Converted string</returns>
        public static string ConvertEnum(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            string result = string.Empty;
            foreach (var c in str)
                if (c.ToString() != c.ToString().ToLower())
                    result += " " + c.ToString();
                else
                    result += c.ToString();

            //ensure no spaces (e.g. when the first letter is upper case)
            //确保没有空格(如第一个字母是大写)
            result = result.TrimStart();
            return result;
        }

        /// <summary>
        /// Set Telerik (Kendo UI) culture
        /// 设置Telerik (Kendo UI)文化
        /// </summary>
        public static void SetTelerikCulture()
        {
            //little hack here
            //always set culture to 'en-US' (Kendo UI has a bug related to editing decimal values in other cultures). Like currently it's done for admin area in Global.asax.cs
            //小黑客
            //总是设置culture为'en-US' (Kendo UI有一个bug，与在其他文化中编辑十进制值有关)。
            //就像目前在Global.asax.cs的管理区域做的那样
            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Get difference in years
        /// 年份差异
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            //source: http://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
            //this assumes you are looking for the western idea of age and not using East Asian reckoning.
            //这假定你是在寻找西方的年龄概念，而不是使用东亚的计算方法。
            int age = endDate.Year - startDate.Year;
            if (startDate > endDate.AddYears(-age))
                age--;
            return age;
        }

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// 将虚拟路径映射到物理磁盘路径。
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"--映射的路径。如。“~ / bin”</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"--物理路径。如。“c: \ inetpub \ wwwroot \ bin”</returns>
        public static string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }

            //not hosted. For example, run in unit tests
            //没有举办。例如，在单元测试中运行
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }        
    }
}
