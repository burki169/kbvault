using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace KBVault.Web.Helpers
{
    public static class KbVaultHtmlHelperExtensions
    {
        // Jquery helpers functions
        // taken from:
        // http://www.codeproject.com/Articles/62031/JQueryUI-Datepicker-in-ASP-NET-MVC
        public static string ConvertDateFormat(this HtmlHelper html)
        {
            return ConvertDateFormat(Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern);
        }

        public static string ConvertDateFormat(string format)
        {
            /*
                *  Date used in this comment : 5th - Nov - 2009 (Thursday)
                *
                *  .NET    JQueryUI        Output      Comment
                *  --------------------------------------------------------------
                *  d       d               5           day of month(No leading zero)
                *  dd      dd              05          day of month(two digit)
                *  ddd     D               Thu         day short name
                *  dddd    DD              Thursday    day long name
                *  M       m               11          month of year(No leading zero)
                *  MM      mm              11          month of year(two digit)
                *  MMM     M               Nov         month name short
                *  MMMM    MM              November    month name long.
                *  yy      y               09          Year(two digit)
                *  yyyy    yy              2009        Year(four digit)             *
                */

            var currentFormat = format;

            // Convert the date
            currentFormat = currentFormat.Replace("dddd", "DD");
            currentFormat = currentFormat.Replace("ddd", "D");

            // Convert month
            if (currentFormat.Contains("MMMM"))
            {
                currentFormat = currentFormat.Replace("MMMM", "MM");
            }
            else if (currentFormat.Contains("MMM"))
            {
                currentFormat = currentFormat.Replace("MMM", "M");
            }
            else if (currentFormat.Contains("MM"))
            {
                currentFormat = currentFormat.Replace("MM", "mm");
            }
            else
            {
                currentFormat = currentFormat.Replace("M", "m");
            }

            // Convert year
            currentFormat = currentFormat.Contains("yyyy") ?
            currentFormat.Replace("yyyy", "yy") : currentFormat.Replace("yy", "y");

            return currentFormat;
        }
    }
}