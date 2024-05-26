using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Extensions
{
    public static class StringExtensions
    {
        public static bool isNullOrEmpty(this string s)
        {
            if(s == null || s == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool isNotNullOrEmpty(this string s)
        {
            if(s == null || s == "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
