using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bamboo
{
    public interface IPropertyProvider
    {
        Dictionary<string, string> Properties { get; }
    }

    public delegate bool ValueConverter<T>(string source, out T value);

    public static class IPropertyProviderExtensions
    {
        public static bool Contains(this IPropertyProvider source, string name)
        {
            if (name == null)
                return false;

            return source.Properties.ContainsKey(name);
        }

        public static void Set(this IPropertyProvider source, string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(name);

            source.Properties[name] = value;
        }

        public static void Set(this IPropertyProvider source, string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(name);

            if (value == null)
                TryDelete(source, name);
            else
                source.Properties[name] = value.ToString();
        }

        public static string SetJson(this IPropertyProvider source, string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(name);

            if (value == null)
            {
                TryDelete(source, name);
                return null;
            }
            else
            {
                var text = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                source.Properties[name] = text;
                return text;
            }
        }

        public static T GetJson<T>(this IPropertyProvider source, string name, T defaultValue = default(T))
        {
            if (!source.Properties.ContainsKey(name))
                return defaultValue;

            var text = source.Properties[name];
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text);
        }

        public static string Get(this IPropertyProvider source, string name, string defaultValue = null)
        {
            if (!source.Properties.ContainsKey(name))
                return defaultValue;
            else
                return source.Properties[name];
        }

        public static int GetInt(this IPropertyProvider source, string name, int defaultValue)
        {
            if (!source.Properties.ContainsKey(name))
                return defaultValue;

            var textValue = source.Properties[name];
            return int.TryParse(textValue, out int value) ? value : defaultValue;
        }

        public static int? GetInt(this IPropertyProvider source, string name)
        {
            if (!source.Properties.ContainsKey(name))
                return null;

            var textValue = source.Properties[name];
            return int.TryParse(textValue, out int value) ? value : (int?)null;
        }

        public static int GetIntDefault(this IPropertyProvider source, string name)
        {
            if (!source.Properties.ContainsKey(name))
                return 0;

            var textValue = source.Properties[name];
            return int.TryParse(textValue, out int value) ? value : 0;
        }

        public static bool TryGetInt(this IPropertyProvider source, string name, out int value)
        {
            value = default(int);
            if (!source.Properties.ContainsKey(name))
                return false;

            var textValue = source.Properties[name];
            return int.TryParse(textValue, out value);
        }

        public static long GetLong(this IPropertyProvider source, string name, long defaultValue)
        {
            if (!source.Properties.ContainsKey(name))
                return defaultValue;

            var textValue = source.Properties[name];
            return long.TryParse(textValue, out long value) ? value : defaultValue;
        }

        public static long? GetLong(this IPropertyProvider source, string name)
        {
            if (!source.Properties.ContainsKey(name))
                return null;

            var textValue = source.Properties[name];
            return long.TryParse(textValue, out long value) ? value : (long?)null;
        }

        public static long GetLongDefault(this IPropertyProvider source, string name)
        {
            if (!source.Properties.ContainsKey(name))
                return 0;

            var textValue = source.Properties[name];
            return long.TryParse(textValue, out long value) ? value : 0;
        }

        public static bool TryGetLong(this IPropertyProvider source, string name, out long value)
        {
            value = default(long);
            if (!source.Properties.ContainsKey(name))
                return false;

            var textValue = source.Properties[name];
            return long.TryParse(textValue, out value);
        }

        public static bool GetBool(this IPropertyProvider source, string name, bool defaultValue)
        {
            if (!source.Properties.ContainsKey(name))
                return defaultValue;

            var textValue = source.Properties[name];
            return bool.TryParse(textValue, out bool value) ? value : defaultValue;
        }

        public static bool? GetBool(this IPropertyProvider source, string name)
        {
            if (!source.Properties.ContainsKey(name))
                return null;

            var textValue = source.Properties[name];
            return bool.TryParse(textValue, out bool value) ? value : (bool?)null;
        }

        public static bool GetBoolDefault(this IPropertyProvider source, string name)
        {
            if (!source.Properties.ContainsKey(name))
                return false;

            var textValue = source.Properties[name];
            return bool.TryParse(textValue, out bool value) ? value : false;
        }

        public static bool TryGetBool(this IPropertyProvider source, string name, out bool value)
        {
            value = false;
            if (!source.Properties.ContainsKey(name))
                return false;

            var textValue = source.Properties[name];
            return bool.TryParse(textValue, out value);
        }

        public static double GetDouble(this IPropertyProvider source, string name, double defaultValue)
        {
            if (!source.Properties.ContainsKey(name))
                return defaultValue;

            var textValue = source.Properties[name];
            return double.TryParse(textValue, out double value) ? value : defaultValue;
        }

        public static double? GetDouble(this IPropertyProvider source, string name)
        {
            if (!source.Properties.ContainsKey(name))
                return null;

            var textValue = source.Properties[name];
            return double.TryParse(textValue, out double value) ? value : (double?)null;
        }

        public static double GetDoubleDefault(this IPropertyProvider source, string name)
        {
            if (!source.Properties.ContainsKey(name))
                return 0d;

            var textValue = source.Properties[name];
            return double.TryParse(textValue, out double value) ? value : 0d;
        }

        public static bool TryGetDouble(this IPropertyProvider source, string name, out double value)
        {
            value = double.NaN;
            if (!source.Properties.ContainsKey(name))
                return false;

            var textValue = source.Properties[name];
            return double.TryParse(textValue, out value);
        }

        public static DateTime GetDateTime(this IPropertyProvider source, string name, DateTime defaultValue)
        {
            if (!source.Properties.ContainsKey(name))
                return defaultValue;

            var textValue = source.Properties[name];
            return DateTime.TryParse(textValue, out DateTime value) ? value : defaultValue;
        }

        public static DateTime? GetDateTime(this IPropertyProvider source, string name)
        {
            if (!source.Properties.ContainsKey(name))
                return null;

            var textValue = source.Properties[name];
            if (DateTime.TryParse(textValue, out DateTime value))
                return value;
            else
                return null;
        }

        public static bool TryGetDateTime(this IPropertyProvider source, string name, out DateTime value)
        {
            value = default(DateTime);
            if (!source.Properties.ContainsKey(name))
                return false;

            var textValue = source.Properties[name];
            return DateTime.TryParse(textValue, out value);
        }

        public static T GetEnum<T>(this IPropertyProvider source, string name, T defaultValue)
            where T : struct
        {
            if (!source.Properties.ContainsKey(name))
                return defaultValue;

            var value = source.Properties[name];
            return Enum.TryParse(value, out T tv) ? tv : defaultValue;
        }

        public static T? GetEnum<T>(this IPropertyProvider source, string name)
            where T : struct
        {
            if (!source.Properties.ContainsKey(name))
                return null;

            var value = source.Properties[name];
            return Enum.TryParse(value, out T tv) ? tv : (T?)null;
        }

        public static bool TryGetEnum<T>(this IPropertyProvider source, string name, out T value)
            where T : struct
        {
            value = default(T);
            if (!source.Properties.ContainsKey(name))
                return false;

            var textValue = source.Properties[name];
            return Enum.TryParse(textValue, out value);
        }

        public static bool TryDelete(this IPropertyProvider source, string name)
        {
            if (source.Properties.ContainsKey(name))
            {
                source.Properties.Remove(name);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SetArray(this IPropertyProvider source, string name, IEnumerable value, char separator = ',')
        {
            if (value == null)
            {
                source.TryDelete(name);
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var item in value)
                {
                    if (sb.Length > 0)
                        sb.Append(separator);
                    if (sb != null)
                        sb.Append(item.ToString());
                }
                source.Set(name, sb.ToString());
            }
        }

        public static int[] GetIntArray(this IPropertyProvider source, string name, char separator = ',')
        {
            return GetArray<int>(source, name, int.TryParse);
        }

        public static double[] GetDoubleArray(this IPropertyProvider source, string name, char separator = ',')
        {
            return GetArray<double>(source, name, double.TryParse);
        }

        public static long[] GetLongArray(this IPropertyProvider source, string name, char separator = ',')
        {
            return GetArray<long>(source, name, long.TryParse);
        }

        public static T[] GetArray<T>(this IPropertyProvider source, string name, ValueConverter<T> valueConverter, char separator = ',')
        {
            if (source.Contains(name))
            {
                T result = default(T);
                var str = source.Get(name);
                return (from s in str.Split(separator)
                        where valueConverter(s, out result)
                        select result).ToArray();
            }
            else
            {
                return null;
            }
        }

        public static string[] GetArray(this IPropertyProvider source, string name, char separator = ',')
        {
            if (source.Contains(name))
            {
                var str = source.Get(name);
                return str.Split(separator).ToArray();
            }
            else
            {
                return null;
            }
        }
    }
}
