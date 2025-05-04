using NCalc;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FastChartingCore
{
    public static class Gen
    {
        /// <summary>
        /// Gets a spin.
        /// String Params: Determine where the note should be applied through the numbers 1 to 8 (e.g.123467), slide and hold are merged into HSlide for configuration.
        /// </summary>
        /// <param name="circles">how many circles</param>
        /// <param name="b">is BreakNote</param>
        /// <param name="x">is EXNote</param>
        /// <param name="h">is hold</param>
        /// <param name="slide">slide suffix(e.g. pp2), if it is empty</param>
        /// <param name="HSlide">the application scope of slide or hold</param>
        /// <param name="time">The time of slide or hold (e.g. 8:1  114#1:2  0##0.1)</param>
        /// <param name="breakslide">is BreakSlide</param>
        /// <returns>a spin in Simai Syntax</returns>
        public static string GetSpin(int circles, string b, string x, bool h, string slide, string HSlide, string time, bool breakslide)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i <= 8; i++)
            {
                sb.Append(i);
                if (InString(b, i.ToString()[0])) sb.Append('b'); //暴力解决莫名其妙的问题
                if (InString(x, i.ToString()[0])) sb.Append('x');
                if (InString(HSlide, i.ToString()[0]) && h) sb.Append('h');
                if (InString(HSlide, i.ToString()[0]) && slide != string.Empty) sb.Append(slide[0]).Append(ParsePlaceholder(slide.Substring(1), i));
                if (InString(HSlide, i.ToString()[0]) && time != string.Empty) sb.Append('[').Append(time).Append(']');
                if (InString(HSlide, i.ToString()[0]) && breakslide) sb.Append('b');
                sb.Append(',');
            }
            StringBuilder sb2 = new StringBuilder();
            for (int i = 1; i <= circles; i++)
            {
                sb2.Append(sb.ToString());
            }
            return sb2.ToString();
        }

        private static bool InString(string str, char c)
        {
            return str.IndexOf(c) != -1;
        }

        /// <summary>
        /// Parses the placeholder.
        /// </summary>
        /// <param name="ph">The ph.(e.g. %i+2% )</param>
        /// <param name="i">The i.</param>
        /// <returns>parse result</returns>
        private static string ParsePlaceholder(string ph, int i)
        {
            if (Regex.IsMatch(ph, @"%[^%]+%"))
            {
                ph = Regex.Replace(ph, @"%[^%]+%", m =>
                {
                    string str = m.Value;
                    int res = (int)new Expression(str.Substring(1, str.Length - 2).Replace("i", i.ToString())).Evaluate();
                    if (res > 8) res -= 8;         //顺时针旋转
                    else if (res < 1) res += 8;    //逆时针旋转
                    return res.ToString();
                });
                return ph;
            }
            else return ph;
        }

        /// <summary>
        /// Unifies the slide begin time.
        /// </summary>
        /// <param name="bpm">The BPM.</param>
        /// <param name="slideBeginTime">The time distance between the first slide note and the slide begining.</param>
        /// <param name="slideNumber">The slide number.</param>
        /// <param name="slideInterval">The each slide interval.</param>
        /// <param name="slideTime">The slide time.</param>
        /// <returns>a list of slide time (e.g. [0.191981##0.114514] )</returns>
        public static List<string> UnifyBeginTime(double bpm, string slideBeginTime, int slideNumber, string slideInterval, string slideTime)
        {
            double begin = ParseTime(slideBeginTime, bpm);
            double interval = ParseTime(slideInterval, bpm);
            double time = ParseTime(slideTime, bpm);

            List<string> result = new List<string>();
            for (int i = 0; i < slideNumber; i++)
            {
                result.Add(new StringBuilder().Append("[").Append((begin - i * interval).ToString("0.000000")).Append("##").Append(time.ToString("0.000000")).Append(']').ToString());
            }

            return result;
        }


        /// <summary>
        /// Unifies the slide end time.
        /// </summary>
        /// <param name="bpm">The BPM.</param>
        /// <param name="slideBeginTime">The slide begin time.</param>
        /// <param name="slideNumber">The slide number.</param>
        /// <param name="slideInterval">The each slide interval.</param>
        /// <param name="slideTime">The time distance between the first slide note and the slide ending.</param>
        /// <returns>a list of slide time (e.g. [0.191981##0.114514] )</returns>
        public static List<string> UnifyEndTime(double bpm, string slideBeginTime, int slideNumber, string slideInterval, string slideTime)
        {
            double begin = ParseTime(slideBeginTime, bpm);
            double interval = ParseTime(slideInterval, bpm);
            double time = ParseTime(slideTime, bpm);

            List<string> result = new List<string>();
            for (int i = 0; i < slideNumber; i++)
            {
                result.Add(new StringBuilder().Append("[").Append(begin.ToString("0.000000")).Append("##").Append((time - i * interval).ToString("0.000000")).Append(']').ToString());
            }

            return result;
        }

        /// <summary>
        /// Parses the time in Simai.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="bpm">The BPM.</param>
        /// <returns>time in seconds</returns>
        private static double ParseTime(string time, double bpm)
        {
            if (time.Contains(":"))
            {
                string[] timeSplit = time.Split(':');
                if (timeSplit.Length == 2)
                {
                    int beat = int.Parse(timeSplit[0]);
                    int tick = int.Parse(timeSplit[1]);
                    return 60 / bpm * 4 / beat * tick;
                }
                else return -1;
            }
            else
            {
                return double.Parse(time);
            }
        }
    }
}
