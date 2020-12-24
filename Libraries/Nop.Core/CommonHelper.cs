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
    /// ��ʾ������������
    /// </summary>
    public partial class CommonHelper
    {
        /// <summary>
        /// Ensures the subscriber email or throw.
        /// ȷ���������յ��ʼ������ʼ��Լ��쳣��
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
        /// ��֤�ַ����Ƿ�����Ч�ĵ����ʼ���ʽ
        /// </summary>
        /// <param name="email">Email to verify�����ʼ���֤</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not
        /// ����ַ�������Ч�ĵ����ʼ���ַ����Ϊtrue;������ǣ���Ϊfalse</returns>
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
        /// ��֤���ַ����Ƿ�����Ч��ip��ַ
        /// </summary>
        /// <param name="ipAddress">IPAddress to verifyIP��ַ����֤</param>
        /// <returns>true if the string is a valid IpAddress and false if it's not
        /// ����ַ�����һ����Ч��IpAddress����Ϊtrue;������ǣ���Ϊfalse</returns>
        public static bool IsValidIpAddress(string ipAddress)
        {
            IPAddress ip;
            return IPAddress.TryParse(ipAddress, out ip);
        }

        /// <summary>
        /// Generate random digit code
        /// �������������
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
        /// ����ָ����Χ�ڵ��������
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
        /// ȷ���ַ����������������ĳ���
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="postfix">A string to add to the end if the original string was shorten
        /// ���ԭʼ�ַ��������̣�Ҫ��ӵ�ĩβ���ַ���</param>
        /// <returns>Input string if its lengh is OK; otherwise, truncated input string
        /// ������Ⱥ��ʣ��������ַ���;���򣬽ض������ַ���</returns>
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
        /// ȷ���ַ���ֻ��������ֵ
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Input string with only numeric values, empty string if input is null/empty
        /// �����ַ���ֻ������ֵ���������Ϊnull/�գ���Ϊ���ַ���</returns>
        public static string EnsureNumericOnly(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : new string(str.Where(p => char.IsDigit(p)).ToArray());
        }

        /// <summary>
        /// Ensure that a string is not null
        /// ȷ���ַ�����Ϊ��
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Result</returns>
        public static string EnsureNotNull(string str)
        {
            return str ?? string.Empty;
        }

        /// <summary>
        /// Indicates whether the specified strings are null or empty strings
        /// ָʾָ�����ַ����ǿ��ַ������ǿ��ַ���
        /// </summary>
        /// <param name="stringsToValidate">Array of strings to validate
        /// Ҫ��֤���ַ�������</param>
        /// <returns>Boolean</returns>
        public static bool AreNullOrEmpty(params string[] stringsToValidate)
        {
            return stringsToValidate.Any(p => string.IsNullOrEmpty(p));
        }

        /// <summary>
        /// Compare two arrasy
        /// �Ƚ���������
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
        //AspNet����Ȩ�޼���
        private static AspNetHostingPermissionLevel? _trustLevel;
        /// <summary>
        /// Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// �����������е�Ӧ�ó�������μ���
        /// </summary>
        /// <returns>The current trust level.��ǰ���μ���</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                //set minimum������С
                _trustLevel = AspNetHostingPermissionLevel.None;

                //determine maximumȷ�����
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
                        break; //we've set the highest permission we can�����Ѿ��趨�����Ȩ��
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
        /// �������ϵ���������Ϊֵ��
        /// </summary>
        /// <param name="instance">The object whose property to set.Ҫ���������ԵĶ���</param>
        /// <param name="propertyName">The name of the property to set.Ҫ���õ����Ե����ơ�</param>
        /// <param name="value">The value to set the property to.Ҫ����������Ϊ��ֵ��</param>
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
        /// ��ֵת��ΪĿ�����͡�
        /// </summary>
        /// <param name="value">The value to convert.Ҫת����ֵ��</param>
        /// <param name="destinationType">The type to convert the value to.Ҫ��ֵת��Ϊ�����͡�</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// ��ֵת��ΪĿ�����͡�
        /// </summary>
        /// <param name="value">The value to convert.Ҫת����ֵ��</param>
        /// <param name="destinationType">The type to convert the value to.Ҫ��ֵת��Ϊ�����͡�</param>
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
        /// ��ֵת��ΪĿ�����͡�
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
        /// ת��ǰ��ö��
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
            //ȷ��û�пո�(���һ����ĸ�Ǵ�д)
            result = result.TrimStart();
            return result;
        }

        /// <summary>
        /// Set Telerik (Kendo UI) culture
        /// ����Telerik (Kendo UI)�Ļ�
        /// </summary>
        public static void SetTelerikCulture()
        {
            //little hack here
            //always set culture to 'en-US' (Kendo UI has a bug related to editing decimal values in other cultures). Like currently it's done for admin area in Global.asax.cs
            //С�ڿ�
            //��������cultureΪ'en-US' (Kendo UI��һ��bug�����������Ļ��б༭ʮ����ֵ�й�)��
            //����Ŀǰ��Global.asax.cs�Ĺ���������������
            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Get difference in years
        /// ��ݲ���
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            //source: http://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
            //this assumes you are looking for the western idea of age and not using East Asian reckoning.
            //��ٶ�������Ѱ��������������������ʹ�ö��ǵļ��㷽����
            int age = endDate.Year - startDate.Year;
            if (startDate > endDate.AddYears(-age))
                age--;
            return age;
        }

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// ������·��ӳ�䵽�������·����
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"--ӳ���·�����硣��~ / bin��</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"--����·�����硣��c: \ inetpub \ wwwroot \ bin��</returns>
        public static string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }

            //not hosted. For example, run in unit tests
            //û�оٰ졣���磬�ڵ�Ԫ����������
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }        
    }
}
