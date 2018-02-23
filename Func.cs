using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;
using System.Runtime.InteropServices;

namespace abxch.WebsiteUITest
{
    public static class Func
    {
        public static string DisplayName(this Enum value)
        {
            string displayName = "";
            if (value != null)
            {
                displayName = value.ToString();

                FieldInfo fi = value.GetType().GetField(displayName);
                if (fi != null)
                {
                    DescriptionAttribute[] descriptionAttributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (descriptionAttributes != null)
                    {
                        DescriptionAttribute attribute = descriptionAttributes.FirstOrDefault();

                        if (attribute != null)
                        {
                            if (!string.IsNullOrEmpty(attribute.Description))
                            {
                                displayName = attribute.Description;
                            }
                        }
                    }
                }
            }

            return displayName;
        }

        public static string DisplayName(this Type type)
        {
            string displayName = "";
            if (type != null)
            {
                displayName = type.ToString();

                DescriptionAttribute[] descriptionAttributes = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (descriptionAttributes != null)
                {
                    DescriptionAttribute attribute = descriptionAttributes.FirstOrDefault();

                    if (attribute != null)
                    {
                        if (!string.IsNullOrEmpty(attribute.Description))
                        {
                            displayName = attribute.Description;
                        }
                    }
                }

            }

            return displayName;
        }

        public static IEnumerable<T> GetValues<T>(this Type type)
        {
            if (!type.IsEnum)
            {
                throw new Exception("type parameter must be an enum");
            }

            var valuesArray = Enum.GetValues(type);

            IEnumerable<T> valuesList = new List<T>((T[])valuesArray);

            return valuesList;
        }


        /// <summary>
        /// Get the value of the element
        /// </summary>
        /// <param name="element"></param>
        /// <returns>The value of the element</returns>
        public static string Value(this IWebElement element)
        {
            return element.GetAttribute("value");
        }

        /// <summary>
        /// Return date formatted as "MM/dd/yyyy";
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultDate"></param>
        /// <returns></returns>
        public static string ToShortDateStringWithLeadingZeroes(this DateTime? value)
        {
            const string standard4DigitYearDateFormat = "MM/dd/yyyy";

            string returnValue = null;

            if (value != null)
            {
                returnValue = value.Value.ToString(standard4DigitYearDateFormat);
            }


            return returnValue;
        }

        /// <summary>
        /// Return date formatted as "MM/dd/yyyy";
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultDate"></param>
        /// <returns></returns>
        public static string ToShortDateStringWithLeadingZeroes(this DateTime value)
        {
            return ToShortDateStringWithLeadingZeroes((DateTime?)value);
        }

        public static string GetDownloadFolderPath()
        {

            const string downloadFolderGuid = "{374DE290-123F-4565-9164-39C4925E467B}";

            const uint dontVerify = 0x00004000;

            string path = "";

            IntPtr outPath;

            int result = NativeMethods.SHGetKnownFolderPath(new Guid(downloadFolderGuid), dontVerify, new IntPtr(0), out outPath);

            if (result >= 0)
            {
                path = Marshal.PtrToStringUni(outPath);
            }
            else
            {
                throw new Exception($"Unable to retrieve the known folder path. It may not be available on this system. {result}");
            }

            return path;


        }
    }


    static class NativeMethods
    {
        [DllImport("Shell32.dll")]
        internal static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);

    }
}
