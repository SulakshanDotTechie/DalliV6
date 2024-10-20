using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DALI.Common.Helpers
{
    public enum RemovalAction
    {
        BeginEndOnly = 1,
        All = 2
    }

    public static class HtmlStringHelper
    {
        static readonly string[] breaks = new string[] { "&lt;br&gt;", "&lt;BR&gt;", "<br>", "<br/>", "<BR>", "<BR/>" };

        /// <summary>
        /// Get first paragraph between P tags.
        /// </summary>
        static string GetFirstParagraph(string value)
        {
            Match m = Regex.Match(value, @"<p>\s*(.+?)\s*</p>");
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            else
            {
                return value;
            }
        }

        public static string RemovePTag(string htmlstring, RemovalAction action)
        {
            if (!string.IsNullOrEmpty(htmlstring))
            {
                var result = htmlstring;
                if (action == RemovalAction.BeginEndOnly)
                {
                    result = GetFirstParagraph(htmlstring);
                }
                if (action == RemovalAction.BeginEndOnly)
                {
                    result = result.Replace("<p>", "");
                    result = result.Replace("</p>", "");
                    result = result.Replace("<P>", "");
                    result = result.Replace("</P>", "");
                }
                return result;
            }
            return htmlstring;
        }

        public static string ReplacePTag(string htmlstring, RemovalAction action)
        {
            if (!string.IsNullOrEmpty(htmlstring))
            {
                var result = htmlstring;
                if (action == RemovalAction.BeginEndOnly)
                {
                    result = GetFirstParagraph(htmlstring);
                }
                if (action == RemovalAction.BeginEndOnly)
                {
                    result = result.Replace("<p>", "");
                    result = result.Replace("</p>", "<br/>");
                    result = result.Replace("<P>", "");
                    result = result.Replace("</P>", "br/>");
                }
                return result;
            }
            return htmlstring;
        }

        public static string ReplaceDivTag(string htmlstring, RemovalAction action)
        {
            if (!string.IsNullOrEmpty(htmlstring))
            {
                var result = htmlstring;
                if (action == RemovalAction.BeginEndOnly)
                {
                    result = GetFirstParagraph(htmlstring);
                }
                if (action == RemovalAction.BeginEndOnly)
                {
                    result = result.Replace("<div>", "");
                    result = result.Replace("</div>", "<br/>");
                    result = result.Replace("<P>", "");
                    result = result.Replace("</P>", "<br/>");
                }
                return result;
            }
            return htmlstring;
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static bool StartsWithHyphen(string text)
        {
            return text.StartsWith("-") || text.StartsWith("&#x2D;") || text.StartsWith("&#45;");
        }

        public static string ReplaceHtmlTagHyphen(string html)
        {
            var result = html;
            var text = StripHTML(html);

            if (StartsWithHyphen(text))
            {
                var strPart = text.Substring(0, 1);

                var pos = html.IndexOf(strPart);

                if (pos < 0)
                {
                    return result;
                }
                else
                {
                    result = html.Substring(0, pos) + "&#x27;" + html.Substring(pos);

                    return result;
                }
            }

            return result;
        }

        public static string ReplaceHtmlTagQuotes(string description)
        {
            string result = description.Replace("=&quot;", "=\"");

            result = result.Replace("&quot;&gt;", "\">");

            return result;
        }

        public static string ReplaceHtmlTagBeginEnd(string description)
        {
            string result = description.Replace("&lt;", "<").Replace("&gt;", ">");

            return result;
        }

        public static string ReplaceHtmlTagSingleQuotes(string description)
        {
            string result = description.Replace("&#39;", "'");

            return result;
        }

        public static string ReplaceHtmlTagBullet(string description)
        {
            string result = description.Replace("•", "&#x95");

            return result;
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);

            if (pos < 0)
            {
                return text;
            }

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string ReplaceHtmlTagList(string html)
        {
            string result = string.Empty;

            if (html.Contains("<ul>") || html.Contains("<UL>"))
            {
                result = Regex.Replace(html, "<ul>", "<br>", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, "<li>", "&bull; ", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, "</li>", "<br>", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, "</ul>", string.Empty, RegexOptions.IgnoreCase);
            }
            else
            {
                result = html;
            }

            return result;
        }

        //    public static string ReplaceFirstLineBreaks(this string text)
        //    {
        //        StringBuilder line = new StringBuilder();

        //        foreach (var br in breaks)
        //        {
        //            if (text.StartsWith(br))
        //            {

        //                string[] strArr = text.Split(new string[] { br }, StringSplitOptions.None);
        //                bool skip = true;

        //                foreach (var s in strArr)
        //                {
        //                    if (!string.IsNullOrEmpty(s))
        //                    {
        //                        line.Append(s);
        //                        skip = false;
        //                    }

        //                    if (!skip && string.IsNullOrEmpty(s))
        //                    {
        //                        line.Append(br);
        //                    }
        //                }
        //            }
        //        }

        //        text = line.ToString();

        //        return text;
        //    }
    }
}
