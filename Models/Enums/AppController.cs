using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models.Enums
{
    public abstract class AppController
    {
        public enum ContentPageHeader
        {
            Index = 0,
            Login = 1,
            Registration = 2
        }

        

        public static T GetEnumValue<T>(string str) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }
           // T val;
            return (T)Enum.ToObject(enumType, str);
            //return Enum.TryParse<T>(str, true, out val) ? val : default(T);
        }

        public static T GetEnumValue<T>(int intValue) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }

            return (T)Enum.ToObject(enumType, intValue);
        }

    }
}
