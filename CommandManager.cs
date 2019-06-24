using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Servidor.Datos
{
    public class CommandManager
    {
        private OracleCommand _insert, _delete, _select, _update;
        private String insertar, borrar, buscar, actualizar;
        private OracleConnection con;
        public CommandManager(OracleConnection conexion)
        {
            this.con = conexion;
        }
        #region Comandos
        public bool Insert<T>(T objeto, bool autoId = true) where T : class, new()
        {
            var miembros = typeof(T).GetProperties();
            String tabla = typeof(T).Name;
            FormatearComando();
            String val = "";
            SetFieldsForInsert<T>(out val, objeto);
            if (autoId)
            {
                val = "null," + val;
            }
            else
            {
                if (miembros[0].GetValue(objeto) is String)
                {
                    val = "'"+ miembros[0].GetValue(objeto) + "'," + val;
                }
                else
                {
                    val = miembros[0].GetValue(objeto).ToString() + "," + val;
                }
            }
            insertar = insertar.Replace("VALORES", val);
            insertar = insertar.Replace("TABLA", tabla);
            Console.WriteLine("Val = " + insertar);
            try
            {
                _insert = new OracleCommand(insertar, con);
                _insert.CommandType = CommandType.Text;
                if (_insert.ExecuteNonQuery() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public T Get<T>(dynamic id) where T : class, new()
        {
            FormatearComando();
            String tabla = typeof(T).Name;
            String condicion ="";
            String val ="";
            SetFieldsForSelect<T>(out condicion, out val, id,typeof(T).GetProperties()[0],id is String);
            buscar = buscar.Replace("VALORES", val);
            buscar = buscar.Replace("TABLA", tabla);
            buscar = buscar.Replace("CONDICION", condicion);
            Console.WriteLine(buscar);
            try
            {
                _select = new OracleCommand(buscar, con);
                _select.CommandType = CommandType.Text;
                OracleDataReader dReader = _select.ExecuteReader();
                Object[] obj = new Object[typeof(T).GetProperties().Length];
                while (dReader.Read())
                {
                    dReader.GetValues(obj);
                }
                dReader.Close();
                if (obj[0] == null)
                {
                    return default(T);
                }
                else
                {
                    T t = new T();
                    var m = typeof(T).GetProperties();
                    int l = 0;
                    foreach (var item in m)
                    {
                        item.SetValue(t, obj[l]);
                        l++;
                    }
                    return t;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return default(T);
            }
        }
        public List<T> GetAll<T>() where T : class, new()
        {
            String tabla = typeof(T).Name;
            FormatearComando();
            String val = "";
            SetFieldsForAll(out val);
            buscar = buscar.Replace("VALORES", val);
            buscar = buscar.Replace("TABLA", tabla);
            buscar = buscar.Replace("WHERE CONDICION", "");
            Console.WriteLine(buscar);
            try
            {
                _select = new OracleCommand(buscar, con);
                _select.CommandType = CommandType.Text;
                OracleDataReader dReader = _select.ExecuteReader();
                OracleDataReader dr = _select.ExecuteReader();
                int cuentaObjetos = 0;
                while (dr.Read())
                {
                    cuentaObjetos++;
                }
                dr.Close();
                List<T> lista = new List<T>();
                Object[][] obj = new Object[cuentaObjetos][];
                int l = 0;
                while (dReader.Read())
                {
                    obj[l] = new Object[typeof(T).GetProperties().Length];
                    for(int j = 0;j< typeof(T).GetProperties().Length; j++)
                    {
                        obj[l][j] = dReader.GetValue(j);
                    }
                    l++;
                }
                dReader.Close();
                T t;
                var m = typeof(T).GetProperties();
                foreach (var ob in obj)
                {
                    l = 0;
                    t = new T();
                    foreach (var item in m)
                    {
                        try
                        {
                            item.SetValue(t, ob[l]);
                        }
                        catch(ArgumentException e)
                        {
                            try
                            {
                                Char c = Char.Parse((String)ob[l]);
                                item.SetValue(t, c);
                            }
                            catch (ArgumentException ex)
                            {
                                bool b = ((String)ob[l]).Equals("1");
                                item.SetValue(t, b);
                            }
                        }
                        l++;
                    }
                    lista.Add(t);
                }
                return lista;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public bool Update<T>(T objeto) where T : class, new()
        {
            String tabla = typeof(T).Name;
            var miembros = typeof(T).GetProperties();
            bool b = miembros[0].GetValue(objeto) is String;
            FormatearComando();
            String val = "";
            String condicion = "";
            SetFieldsForUpdate<T>(out condicion,out val,objeto:objeto,idIsString:b);
            actualizar = actualizar.Replace("VALORES", val);
            actualizar = actualizar.Replace("TABLA", tabla);
            actualizar = actualizar.Replace("CONDICION", condicion);
            Console.WriteLine(actualizar);
            try
            {
                _update = new OracleCommand(actualizar, con);
                _update.CommandType = CommandType.Text;
                if (_update.ExecuteNonQuery() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return false;
            }

        }
        public bool Delete<T>(T objeto) where T : class, new()
        {
            String tabla = typeof(T).Name;
            var miembros = typeof(T).GetProperties();
            FormatearComando();
            String condicion = "";
            if(miembros[0].GetValue(objeto) is String)
            {
                condicion = FormatId(miembros[0].Name, miembros[0].GetValue(objeto), true);
            }
            else
            {
                condicion = FormatId(miembros[0].Name, miembros[0].GetValue(objeto));
            }
            borrar = borrar.Replace("TABLA", tabla);
            borrar = borrar.Replace("CONDICION", condicion);
            Console.WriteLine(borrar);
            try
            {
                _delete = new OracleCommand(borrar, con);
                _delete.CommandType = CommandType.Text;
                if (_delete.ExecuteNonQuery() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return false;
            }
        }
        #endregion
        private void FormatearComando()
        {
            insertar = "INSERT INTO TABLA VALUES(VALORES)";
            buscar = "SELECT VALORES FROM TABLA WHERE CONDICION";
            actualizar = "UPDATE TABLA SET VALORES WHERE CONDICION";
            borrar = "DELETE FROM TABLA WHERE CONDICION";

        }
        private void SetFieldsForSelect<T>(out String id, out String valores, dynamic idValue,PropertyInfo pi, bool idIsString = false) where T : class
        {//SELECT
            SetFieldsForAll(out valores);
            id = FormatId(pi.Name, idValue, idIsString);
        }
        private void SetFieldsForAll(out String valores)
        {//SELECT ALL
            valores = "*";
        }
        private void SetFieldsForUpdate<T>(out String id,out String valores, T objeto, bool idIsString = false) where T : class
        {//UPDATE
            var miembros = typeof(T).GetProperties();
            SetFieldsForInsert<T>(out valores,objeto,true);
            var idValue = miembros[0].GetValue(objeto);
            id = FormatId(miembros[0].Name, idValue, idIsString);
        }
        private void SetFieldsForInsert<T>(out String valores, T objeto, bool update = false) where T : class
        {//INSERT
            var miembros = typeof(T).GetProperties();
            valores = "";
            int i = 0;
            String[] vals = new String[miembros.Length];
            foreach(var f in miembros)
            {
                if (f.GetValue(objeto) == null)
                {
                    vals[i] = null;
                }
                else
                {
                    if (f.GetValue(objeto) is String)
                    {
                        vals[i] = "'" + f.GetValue(objeto).ToString() + "'";
                    }
                    if (f.GetValue(objeto) is int || f.GetValue(objeto) is short 
                        || f.GetValue(objeto) is Int32 || f.GetValue(objeto) is Int64
                        || f.GetValue(objeto) is float || f.GetValue(objeto) is double)
                    {
                        vals[i] = f.GetValue(objeto).ToString();
                    }
                    if (f.GetValue(objeto) is bool)
                    {
                        vals[i] = ((bool)f.GetValue(objeto)) ? "'1'" : "'0'";
                    }
                    if (f.GetValue(objeto) is DateTime)
                    {
                        vals[i] = "TO_DATE('" + Tools.DateToString((DateTime)f.GetValue(objeto),DateFormat.YearMonthDay,'-') + "','YYYY-MM-DD')";
                    }
                }
                i++;
            }
            i = 0;
            //ANULAR LA ID PORQUE LA GENERA LA BDD O ES CONDICION
            foreach (var inf in miembros)
            {
                if (i > 0)
                {
                    if (update)
                    {
                        if (vals[i] != null)
                        {
                            valores += inf.Name + "=" + vals[i] + ",";
                        }
                    }
                    else
                    {
                        if (vals[i] != null)
                        {
                            valores += vals[i] + ",";
                        }
                        else
                        {
                            valores += "null,";
                        }
                    }
                }
                i++;
            }
            valores = valores.Substring(0, valores.Length - 1);
        }
        private String FormatId(String field, dynamic idValue,bool idIsString = false)
        {
            if (idIsString)
            {
                return field + "='" + idValue + "'";
            }
            else
            {
                return field + "=" + idValue.ToString();
            }
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
