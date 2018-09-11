using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bamboo
{
    public static class StringHelper
    {
        public static bool TryGetInt(object value, out int intValue)
        {
            intValue = 0;
            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            switch (value)
            {
                case null:
                case DBNull db:
                    return false;
                case int i:
                    intValue = i;
                    return true;
                case string s:
                    return int.TryParse(s, out intValue);
                case decimal d:
                    intValue = (int)d;
                    return true;
                case double d:
                    intValue = (int)d;
                    return true;
                default:
                    return false;
            }
        }

        public static bool TryParseHex(string text, out int value)
        {
            value = 0;
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            if (text.StartsWith("0x") || text.StartsWith("0X"))
            {
                return int.TryParse(text.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
            }
            else
            {
                return int.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
            }
        }

        public static bool IsAlphabet(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        public static string AddAccelerator(string str, char accelerator)
        {
            if (accelerator > 0)
            {
                int index = str.IndexOf(new string(accelerator, 1), StringComparison.OrdinalIgnoreCase);
                if (index > -1)
                {
                    str.Insert(index, "&");
                }
                else
                {
                    str += string.Format("(&{0})", accelerator);
                }
            }

            return str;
        }

        public static string GetString(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            return value.ToString();
        }

        public static int? GetInt(object obj)
        {
            switch (obj)
            {
                case int i:
                    return i;
                case decimal d:
                    return (int)d;
                case float f:
                    return (int)f;
                case double d:
                    return (int)d;
                case byte b:
                    return b;
                case short s:
                    return s;
                case long l:
                    return (int)l;
                case string s when int.TryParse(s, out int si):
                    return si;
                default:
                    return null;
            }
        }

        public static int GetInt(object obj, int defaultValue)
        {
            return GetInt(obj) ?? defaultValue;
        }

        public static int GetIntDefault(object obj)
        {
            return GetInt(obj, 0);
        }

        public static bool GetBoolDefault(object value)
        {
            return GetBool(value, false);
        }

        public static bool? GetBool(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case bool b:
                    return b;
                case int i:
                    return i > 0;
                case decimal dec:
                    return dec > 0;
                case long l:
                    return l > 0;
                case short s:
                    return s > 0;
                case byte by:
                    return by > 0;
                default:
                    var str = value.ToString().ToLower();
                    switch (str)
                    {
                        case "1":
                        case "y":
                        case "yes":
                        case "true":
                            return true;
                        case "0":
                        case "n":
                        case "no":
                        case "false":
                            return false;
                        default:
                            return null;
                    }
            }
        }

        public static bool GetBool(object value, bool defaultValue)
        {
            return GetBool(value) ?? defaultValue;
        }

        public static string EncodeBase64(string code)
        {
            if (code == null)
            {
                return null;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(code);
            return Convert.ToBase64String(bytes);
        }

        public static string DecodeBase64(string code)
        {
            if (code == null)
            {
                return null;
            }

            byte[] bytes = Convert.FromBase64String(code);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string JoinArray(IEnumerable array)
        {
            return JoinArray(array, ",");
        }

        public static string JoinArray(IEnumerable array, string separator)
        {
            var sb = new StringBuilder();
            foreach (object item in array)
            {
                if (item == null)
                {
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }

                sb.Append(item.ToString());
            }

            return sb.ToString();
        }

        public static string JoinArray(IEnumerable array, Func<object, string> formater, string separator = ",")
        {
            if (formater == null)
            {
                throw new ArgumentNullException();
            }

            var sb = new StringBuilder();
            foreach (object item in array)
            {
                if (item == null)
                {
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }

                sb.Append(formater(item));
            }

            return sb.ToString();
        }

        public static string JoinArray<TSource>(IEnumerable<TSource> array, Func<TSource, string> formater, string separator = ",")
        {
            if (formater == null)
            {
                throw new ArgumentNullException();
            }

            var sb = new StringBuilder();
            foreach (TSource item in array)
            {
                if (item == null)
                {
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }

                sb.Append(formater(item));
            }

            return sb.ToString();
        }

        public static string JoinArray(IList array, int startIndex, int count)
        {
            return JoinArray(array, startIndex, count, ",");
        }

        public static string JoinArray(IList array, int startIndex, int count, string separator)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = startIndex; i < startIndex + count; i++)
            {
                if (array[i] == null)
                {
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }

                sb.Append(array[i].ToString());
            }

            return sb.ToString();
        }

        public static string JoinArray(IEnumerable array, string prefix, string suffix, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object item in array)
            {
                if (item == null)
                {
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }

                sb.AppendFormat("{0}{1}{2}", prefix, item, suffix);
            }

            return sb.ToString();
        }

        public static string[] GetArray(string text, char separator = ',', bool trim = true)
        {
            string[] result = text.Split(separator);
            if (trim)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = result[i].Trim();
                }
            }

            return result;
        }

        public static string[] GetArray(string text, string separator, bool trim = true)
        {
            string[] result = text.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            if (trim)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = result[i].Trim();
                }
            }

            return result;
        }

        public static IEnumerable<string> SplitWithEscaple(string header, char separator, char escaple = '\\')
        {
            if (header == null)
            {
                yield break;
            }

            char[] buffer = new char[256];
            int bufferIndex = 0;

            for (int i = 0; i < header.Length; i++)
            {
                var c = header[i];

                // 判读是否是转义符
                if (c == escaple && i < header.Length - 1 && header[i + 1] == separator)
                {
                    i++;
                    c = header[i];
                }
                else if (c == separator || bufferIndex >= buffer.Length) // 判断是否是分隔符或者达到缓存上限
                {
                    if (bufferIndex > 0)
                    {
                        yield return new string(buffer, 0, bufferIndex);
                        bufferIndex = 0;
                    }

                    // 如果是分隔符， 则不计入缓存
                    if (c == separator)
                    {
                        continue;
                    }
                }

                buffer[bufferIndex++] = c;
            }

            if (bufferIndex > 0)
            {
                yield return new string(buffer, 0, bufferIndex);
            }
        }

        public static int[] GetIntArray(string text)
        {
            return GetIntArray(text, ',');
        }

        public static int[] GetIntArray(string text, params char[] separators)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new int[0];
            }

            string[] strArray = text.Split(separators);

            int value = 0;
            var list = from s in strArray
                       where s != null
                       let str = s.Trim()
                       where !string.IsNullOrEmpty(str) && int.TryParse(str, out value)
                       select value;

            return list.ToArray();
        }

        public static string FileSizeToString(long bytes)
        {
            return FileSizeFormatProvider.Format(bytes);
        }

        public static T GetEnumValue<T>(object value, T defaultValue)
            where T : struct
        {
            if (value == null)
            {
                return defaultValue;
            }

            return GetEnumValue<T>(value.ToString(), defaultValue);
        }

        public static T GetEnumValue<T>(string text, T defaultValue)
            where T : struct
        {
            try
            {
                object obj = Enum.Parse(typeof(T), text, true);
                if (obj is T)
                {
                    return (T)obj;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T? GetEnumValue<T>(object value)
            where T : struct
        {
            if (value == null)
            {
                return null;
            }

            if (Enum.TryParse(value.ToString(), true, out T result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static DateTime GetDateTime(object p, DateTime nullValue)
        {
            if (p == null || p is DBNull)
            {
                return nullValue;
            }

            if (p is DateTime)
            {
                return (DateTime)p;
            }
            else if (p is string)
            {
                DateTime dt;
                if (DateTime.TryParse((string)p, out dt))
                {
                    return dt;
                }
                else
                {
                    return nullValue;
                }
            }
            else
            {
                return nullValue;
            }
        }

        public static DateTime GetDateTimeExtend(string text, DateTime defaultValue)
        {
            var d = ParseDateTimeExtend(text);
            if (!d.HasValue)
            {
                return defaultValue;
            }
            else
            {
                return d.Value;
            }
        }

        public static DateTime? ParseDateTimeExtend(string text)
        {
            return ParseDateTimeExtendTemplate(text, DateTime.Today);
        }

        public static DateTime? ParseDateTimeExtendTemplate(string text, DateTime template)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            var reg = new Regex(@"(?:(?:(\d+)[/\-])?(\d+)[/\-](\d+) )?(\d+)[:.](\d+)(?:[:.](\d+))?");
            if (!reg.IsMatch(text))
            {
                return null;
            }

            var match = reg.Match(text);
            int[] values = new int[] { template.Year, template.Month, template.Day, template.Hour, template.Minute, template.Second };
            for (int i = 0; i < values.Length; i++)
            {
                if (i + 1 >= match.Groups.Count)
                {
                    break;
                }

                if (match.Groups[i + 1].Success)
                {
                    values[i] = StringHelper.GetInt(match.Groups[i + 1].Value, values[i]);
                }
            }

            if (values[0] < 1 || values[0] > 9999
                || values[1] < 1 || values[1] > 12
                || values[2] < 1 || values[2] > DateTime.DaysInMonth(values[0], values[1])
                || values[3] < 0 || values[3] > 23
                || values[4] < 0 || values[4] > 59
                || values[5] < 0 || values[5] > 59)
            {
                return null;
            }

            return new DateTime(values[0], values[1], values[2], values[3], values[4], values[5]);
        }

        public static DateTime? GetDateTime(object p)
        {
            if (p == null || p is DBNull)
            {
                return null;
            }

            if (p is DateTime)
            {
                return (DateTime)p;
            }
            else if (p is string)
            {
                if (DateTime.TryParse((string)p, out DateTime dt))
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static bool TryGetDateTime(string text, out DateTime dt)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (text.Contains("."))
                {
                    text = text.Replace(".", ":");
                }

                if (DateTime.TryParse(text, out dt))
                {
                    return true;
                }
            }

            dt = DateTime.MinValue;
            return false;
        }

        public static Dictionary<string, string> GetQueryParameters(Uri uri, IEqualityComparer<string> comparer = null)
        {
            if (uri == null)
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            if (uri.IsAbsoluteUri)
            {
                return GetQueryParameters(uri.Query, comparer);
            }
            else if (uri.OriginalString != null)
            {
                return GetQueryParameters(uri.OriginalString, comparer);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static Dictionary<string, string> GetQueryParameters(string url, IEqualityComparer<string> comparer = null)
        {
            if (url == null)
            {
                throw new ArgumentNullException();
            }

            var dic = new Dictionary<string, string>(comparer ?? StringComparer.OrdinalIgnoreCase);
            var matches = Regex.Matches(url, @"[\?&](([^&=]+)(=([^&=#]*))?)", RegexOptions.Compiled);
            foreach (var m in matches.OfType<Match>())
            {
                dic[Uri.UnescapeDataString(m.Groups[2].Value)] = Uri.UnescapeDataString(m.Groups[4].Value);
            }

            return dic;
        }

        public static string[] SplitKeywords(string search, int maxKeywords, char separator = ' ')
        {
            if (string.IsNullOrEmpty(search))
            {
                return new string[0];
            }

            var list = search.Split(separator)
                .Select(w => w.Trim())
                .Where(w => !string.IsNullOrEmpty(w))
                .Distinct();

            if (maxKeywords > 0)
            {
                return list.Take(maxKeywords).ToArray();
            }
            else
            {
                return list.ToArray();
            }
        }

        public static string LimitStringLength(string str, int maxLength, bool singleLine = false)
        {
            if (str == null)
            {
                return str;
            }

            if (str.Length > maxLength)
            {
                if (maxLength > 0)
                {
                    str = str.Substring(0, maxLength - 1) + "…";
                }
                else
                {
                    str = string.Empty;
                }
            }

            if (singleLine)
            {
                str = str.Replace("\r", "");
                str = str.Replace("\n", "");
            }

            return str;
        }

        /// <summary>
        /// 分解单词
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ParseWords(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            Regex regex = new Regex(@"\W*((?:\d+)|(?:[A-Z]{2})|(?:[A-Z][a-z]*))", RegexOptions.None);
            MatchCollection matchs = regex.Matches(text);
            if (matchs.Count > 0)
            {
                List<string> list = new List<string>();
                foreach (Match match in matchs)
                {
                    if (match.Length == 0 || match.Groups.Count == 0)
                    {
                        continue;
                    }

                    var word = match.Groups[1].Value;
                    word = PreprocessWord(word);
                    list.Add(word);
                }

                if (list.Count > 0)
                {
                    return JoinArray(list.ToArray(), " ");
                }
            }

            return text;
        }

        public static string ParseWords(string text, params char[] separators)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string[] strArray = text.Split(separators);

            var list = (from s in strArray
                        where s != null
                        let str = s.Trim()
                        where !string.IsNullOrEmpty(str)
                        select WordCase(str)).ToArray();

            if (list.Length > 0)
            {
                return JoinArray(list.ToArray(), " ");
            }
            else
            {
                return string.Empty;
            }
        }

        public static string WordCase(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return word;
            }

            if (word.Length == 1)
            {
                return word.ToUpper();
            }
            else
            {
                return word.Substring(0, 1).ToUpper() + word.Substring(1, word.Length - 1).ToLower();
            }
        }

        public static string PascalToUnderline(string text)
        {
            return Regex.Replace(text, "((?<=[a-z])(?=[A-Z]))|((?<=[A-Z])(?=[A-Z][a-z]))", "_");
        }

        public static string PascalToWords(string text)
        {
            return Regex.Replace(text, "((?<=[a-z])(?=[A-Z]))|((?<=[A-Z])(?=[A-Z][a-z]))", " ");
        }

        /// <summary>
        /// 预处理分解出来的单词
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static string PreprocessWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return word;
            }

            //if (word == "NO")
            //    return "NO.";
            //else
            return word;
        }

        public static string EscapeFileName(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return filename;
            }

            var illegal = Path.GetInvalidFileNameChars();
            var charts = from c in filename
                         where !illegal.Contains(c)
                         select c;
            return new string(charts.ToArray());
        }

        public static string EscapePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            var illegal = Path.GetInvalidPathChars();
            var charts = from c in path
                         where !illegal.Contains(c)
                         select c;
            return new string(charts.ToArray());
        }

        public static string ToString(TimeSpan value)
        {
            if (value.Days > 0)
            {
                return string.Format("{0}d {1}:{2}:{3}", value.Days, value.Hours, value.Minutes, value.Seconds);
            }
            else
            {
                return string.Format("{0}:{1}:{2}", value.Hours, value.Minutes, value.Seconds);
            }
        }

        public static string toCamelCase(string text)
        {
            if (text != null && text.Length > 0)
            {
                return text.Substring(0, 1).ToLower() + text.Substring(1, text.Length - 1);
            }
            else
            {
                return text;
            }
        }
    }
}
