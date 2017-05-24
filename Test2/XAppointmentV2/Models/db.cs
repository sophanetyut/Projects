using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using Newtonsoft.Json;

namespace sapi
{
    public class db
    {
        //public SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["server"]);

        public SqlConnection con = new SqlConnection(ConfigurationSettings.AppSettings["server"]);
        SqlTransaction trn;
        string dateForm = "dd/mm/yyyy";

        public string sqlStr(string str)
        {
            if (str != "" && str != null)
            {
                return "N'" + str.Replace("'", "''") + "'";
            }
            else
            {
                return "NULL";
            }
        }

        public string sqlStrN(string str)
        {
            if (str != "" && str != null)
            {
                return "'" + str.Replace("'", "''") + "'";
            }
            else
            {
                return "NULL";
            }
        }

        public string str2Date(string dateStr, string formatType = "dd/mm/yyyy", char sep = '/')
        {
            string[] str = dateStr.Split(sep);
            if (str.Length > 2)
            {
                if (formatType == "dd/mm/yyyy")
                {
                    return str[2] + "-" + str[1] + "-" + str[0];
                }
            }
            return "";
        }

        public string sqlStrLike(string str)
        {
            return "N'" + str.Replace("'", "''") + "%'";
        }

        public string sqlStrLikeAll(string str)
        {
            return "N'%" + str.Replace("'", "''") + "%'";
        }

        public bool connect()
        {
            if (con.State == ConnectionState.Open)
            {
                return true;
            }
            try
            {
                con.Open();

                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public void beginTran()
        {
            trn = con.BeginTransaction();
        }

        public void commit()
        {
            trn.Commit();
        }

        public void rollback()
        {
            trn.Rollback();
        }

        public void close()
        {
            con.Close();
        }

        /// <summary>
        /// Reads the data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="SQL">Sql query string.</param>
        /// <param name="param">Sql Parameter to support the Query string from the first parameter.</param>
        public DataTable readData(string SQL , List<SqlParameter> param = null)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(SQL);
                cmd.Connection = con;

                if (param != null)
                {
                    foreach (SqlParameter pram in param)
                    {
                        cmd.Parameters.Add(pram);
                    }
                }

                if (trn != null)
                    cmd.Transaction = trn;
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable tbl = new DataTable();
                da.Fill(tbl);
                if (param != null) param.Clear();
                return tbl;
            }
            catch (Exception e)
            {
                writeLog(SQL, e);
                return null;
            }
        }

        public DataTable readData(string SQL, SqlConnection ocon, List<SqlParameter> param = null)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(SQL);
                cmd.Connection = ocon;

                if (param != null)
                {
                    foreach (SqlParameter pram in param)
                    {
                        cmd.Parameters.Add(pram);
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable tbl = new DataTable();
                da.Fill(tbl);
                if (param != null) param.Clear();
                return tbl;
            }
            catch (Exception e)
            {
                if (param != null) param.Clear();
                writeLog(SQL, e);
                return null;
            }
        }

        public string readData(string col, string SQL, List<SqlParameter> param = null)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(SQL);
                cmd.Connection = con;

                if (param != null)
                {
                    foreach (SqlParameter pram in param)
                    {
                        cmd.Parameters.Add(pram);
                    }
                }

                if (trn != null)
                    cmd.Transaction = trn;
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable tbl = new DataTable();
                da.Fill(tbl);
                if (param != null) param.Clear();
                if (tbl.Rows.Count > 0)
                    return tbl.Rows[0][col].ToString();
                else
                    return "";
            }
            catch (Exception e)
            {
                if (param != null) param.Clear();
                writeLog(SQL, e);
                return null;
            }
        }

        public string execData(string SQL, string tblName = "", List<SqlParameter> param = null)
        {
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Connection = con;
            if (param != null)
            {
                foreach (SqlParameter pram in param)
                {
                    cmd.Parameters.Add(pram);
                }
            }

            if (trn != null)
                cmd.Transaction = trn;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
                if (tblName != "")
                {
                    cmd.CommandText = "Select @@Identity From " + tblName;
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        string ID = dr[0].ToString();
                        dr.Close();
                        return "ok" + ID;
                    }
                }
                if (param != null) param.Clear();
                return "ok";
            }
            catch (SqlException e)
            {
                if (param != null) param.Clear();
                writeLog(SQL, e);
                return e.Message;
            }
        }

        public double cNum(string data)
        {
            if (data == null)
                data = "";
            data = data.Replace(",", "");
            double re = 0;
            if (double.TryParse(data, out re))
            {
                re = double.Parse(data);
            }
            return re;
        }

        public bool isNum(string data)
        {
            Double Temp;
            if (Double.TryParse(data, out Temp) == true)
            {
                return true;
            }
            else
                return false;

        }

        public bool isDate(string data)
        {
            DateTime Temp;
            if (DateTime.TryParse(data, out Temp) == true)
                return true;
            else
                return false;
        }





        public string tblToJson(DataTable dt)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(dt,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        }





        public void writeLog(string SQL, dynamic e)
        {
            string fname = DateTime.Now.ToString("yyyyMMdd");
            System.IO.StreamWriter wr = new System.IO.StreamWriter(HttpContext.Current.Server.MapPath("~/Logs/" + fname + ".log"), true);
            wr.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            if (e != null)
                wr.WriteLine(e.Message);
            wr.WriteLine(SQL);
            wr.WriteLine("--------------------------//--------------------------");
            wr.Close();
        }




        public string getDate(string dval, int type = 0) // 0 = db date ; 1 = out date ; 2 = date out format
        {
            string re = "";
            var tmp = dval.Split(' ');
            var val = tmp[0];
            var val2 = "";
            if (tmp.Length > 1)
            {
                val2 = tmp[1];
            }
            char spliter = '/';
            if (dateForm.IndexOf("/") > 0)
            {
                spliter = '/';
            }
            else if (dateForm.IndexOf(".") > 0)
            {
                spliter = '.';
            }
            else
            {
                spliter = '-';
            }
            string[] dateForms = dateForm.Split(spliter);

            if (type == 1)
            {
                string[] vals = val.Split('-');

                if (vals.Length > 2)
                {
                    string[] dateFormOuts = new string[] { "", "", "" };

                    dateFormOuts[Array.FindIndex(dateForms, x => x.Contains("d"))] = vals[2];
                    dateFormOuts[Array.FindIndex(dateForms, x => x.Contains("m"))] = vals[1];
                    dateFormOuts[Array.FindIndex(dateForms, x => x.Contains("y"))] = vals[0];
                    foreach (string str in dateFormOuts)
                    {
                        re = re + (str + spliter);
                    }
                }
                re = re.Substring(0, re.Length - 1) + " " + val2;
            }
            else if (type == 2)
            {
                string[] dateFormOuts = new string[] { "", "", "" };
                dateFormOuts[Array.FindIndex(dateForms, x => x.Contains("d"))] = "dd";
                dateFormOuts[Array.FindIndex(dateForms, x => x.Contains("m"))] = "MM";
                dateFormOuts[Array.FindIndex(dateForms, x => x.Contains("y"))] = "yyyy";
                foreach (string str in dateFormOuts)
                {
                    re = re + (str + spliter);
                }
                re = re.Substring(0, re.Length - 1);
            }
            else
            {
                string[] vals = val.Split(spliter);
                if (vals.Length > 2)
                {
                    re = (vals[Array.FindIndex(dateForms, x => x.Contains("y"))] +
                        "-" + vals[Array.FindIndex(dateForms, x => x.Contains("m"))] +
                        "-" + vals[Array.FindIndex(dateForms, x => x.Contains("d"))] +
                        " " + val2);
                }
            }
            return re;
        }
    }
}