using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class Int32Extensions
    {
        /// <summary>
        /// 转化为指定的枚举
        /// </summary>
        public static bool TryParseEnum<T>(this int? value, out T e)
        {
            e = default;

            if (value is null || !Enum.IsDefined(typeof(T), value))
                return false;

            e = (T)Enum.ToObject(typeof(T), value);
            return true;
        }

        /// <summary>
        /// 转化为指定的枚举
        /// </summary>
        public static bool TryParseEnum<T>(this int value, out T e)
        {
            e = default;

            if (!Enum.IsDefined(typeof(T), value))
                return false;

            e = (T)Enum.ToObject(typeof(T), value);
            return true;
        }
    }
}
