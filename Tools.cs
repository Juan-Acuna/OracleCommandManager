using System;
using System.Collections.Generic;
using System.Text;

namespace Servidor.Datos
{
    public class ResponseJson
    {
        public String Mensage { get; set; }
        public bool Result { get; set; }
        public ResponseJson(String mensaje)
        {
            this.Mensage = mensaje;
            this.Result = false;
        }
        public ResponseJson(String mensaje, bool res)
        {
            this.Mensage = mensaje;
            this.Result = res;
        }
    }
    public static class Tools
    {
        
        public static DateTime StringToDate(String date,DateFormat format = DateFormat.DayMonthYear)
        {
            String d = "";
            String y = "";
            String m = "";
            date = date.Replace("/","");
            date = date.Replace("-", "");
            switch (format)
            {
                case DateFormat.DayMonthYear:
                    d = date.Substring(0,2);
                    m = date.Substring(2,2);
                    y = date.Substring(4,4);
                    break;
                case DateFormat.MonthDayYear:
                    m = date.Substring(0, 2);
                    d = date.Substring(2, 2);
                    y = date.Substring(4, 4);
                    break;
                case DateFormat.YearMonthDay:
                    y = date.Substring(0, 4);
                    m = date.Substring(4, 2);
                    d = date.Substring(6, 2);
                    break;
            }
            return new DateTime(Int32.Parse(y), Int32.Parse(m), Int32.Parse(d));
        }
        public static String DateToString(DateTime date, DateFormat outputFormat, char divisor = '/')
        {
            String output = "";
            String d;
            String y;
            String m;
            if(date.Day < 10)
            {
                d = "0" + date.Day.ToString();
            }
            else
            {
                d = date.Day.ToString();
            }
            if (date.Month < 10)
            {
                m = "0" + date.Month.ToString();
            }
            else
            {
                m = date.Month.ToString();
            }
            y = date.Year.ToString();
            switch (outputFormat)
            {
                case DateFormat.DayMonthYear:
                    output = d + divisor + m + divisor + y;
                    break;
                case DateFormat.MonthDayYear:
                    output = m + divisor + d + divisor + y;
                    break;
                case DateFormat.YearMonthDay:
                    output = y + divisor + m + divisor + d;
                    break;
            }
            return output;
        }
    }
    public enum DateFormat
    {
        YearMonthDay,
        DayMonthYear,
        MonthDayYear
    }
}
