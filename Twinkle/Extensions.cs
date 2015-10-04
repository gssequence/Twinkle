using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Twinkle
{
    public static class Extensions
    {
        public static string Repeat(this string str, int length)
        {
            var ret = "";
            for (int i = 0; i < length; i++)
                ret += str;
            return ret;
        }

        public static string Unescape(this string str)
        {
            return WebUtility.HtmlDecode(str);
        }

        public static string NextFilePath(this string str)
        {
            var directory = Path.GetDirectoryName(str);
            var name = Path.GetFileNameWithoutExtension(str);
            var ext = Path.GetExtension(str);

            if (!File.Exists(str)) return str;
            for (int i = 1; i < int.MaxValue; i++)
            {
                var path = Path.Combine(directory, name + " (" + i.ToString() + ")" + ext);
                if (!File.Exists(path)) return path;
            }

            return null;
        }
    }
}
