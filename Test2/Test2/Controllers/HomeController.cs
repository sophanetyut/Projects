using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Data;
using System.Threading.Tasks;
using Test2.Models;
using System.Collections.Specialized;
using System.Web.Helpers;


namespace Test2.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection("Server=192.168.0.27;Database=PHANET_DB;User Id=sa; Password=123456;");
        SqlCommand com;
        List<studentModel> list = new List<studentModel>();

        public ActionResult Index()
        {
            return View ();
        }

        public ActionResult SaveDB(){
            com = new SqlCommand("INSERT INTO tb_Student(Stuname,DOB,Age) VALUES(@StuName,@DOB,@Age)", con);
            com.Parameters.AddWithValue("@StuName", Request.Form["inName"]);
            com.Parameters.AddWithValue("@DOB", Request.Form["inDOB"]);
            com.Parameters.AddWithValue("@Age", Request.Form["inAge"]);
			ViewData["SaveResult"] = "";

			try
            {
                con.Open();
                com.ExecuteNonQuery();
                ViewData["SaveResult"] = "Success";
			}
            catch (Exception ex)
            {
                ViewData["SaveResult"] = "Error on "+ ex.Message;
			}
            finally{
                con.Close();
            }
            return View();
        }

        public ActionResult ShowData(int? ind){

           

            SqlDataAdapter adt = new SqlDataAdapter("SELECT * FROM tb_Student", con);
            DataSet ds = new DataSet();

            try
            {
                con.Open();
                adt.Fill(ds);

                ViewData["abc"] = ds.Tables[0].Rows.Count;
                int i = 0;
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    list.Add(new studentModel(){ lID=i, ID=(int)item[0], StuName=item[1].ToString(), DOB=item[2].ToString(), Age=item[3].ToString()});
                    i++;
                }
               
                /*
                switch (Request.Form["btnPrevious"])
                {
                    case "btnPrevious":
                        break;
                    default:
                        break;
                }
                */
                string param = "0";


                if (Request.Form["ind"] != null && int.Parse(Request.Form["ind"])< list.Count/5 ){
                    param = (int.Parse(Request.Form["ind"].ToString())+1 ).ToString();

				}

                if (Request.Form["indBack"]!=null && int.Parse(Request.Form["indBack"])>= 1 )
                {
                    param = (int.Parse(Request.Form["indBack"].ToString() )- 1).ToString();
				}

                ViewData["indBack"] = param;
                ViewData["ind"] = param;
                ViewData["ShowStudent"] = list.Where(l => l.lID >= int.Parse(param) * 5).Take(5);
                //ViewData["ShowStudent"] = list.Take(5).LastOrDefault();
                //list.inde

				//ViewData["ShowStudent"] = list;

			}
            catch (Exception ex)
            {
                //Console.Write(""+ex.Message);
                Response.Write(ex.Message);
            }
            finally{
                con.Close();
            }
            return View();
        }


        public ActionResult DeleteDB(){
            com = new SqlCommand("delete from tb_Student where StuID=@IDd",con);
            com.Parameters.AddWithValue("@IDd", Request.Form["Dele"]);
            ViewData["DeleteResult"] = "";
            try
            {
                con.Open();
                com.ExecuteNonQuery();
                /*
                await con.OpenAsync();
                //com.ExecuteNonQueryAsync();

                if (await com.ExecuteNonQueryAsync()>=1)
                {
                    
                }
                */
                ViewData["DeleteResult"] = "Delete success";

			}
            catch (SqlException esx){
                ViewData["DeleteResult"] = esx.Message;
            }
            catch (Exception ex)
            {
                ViewData["DeleteResult"] = ex.Message;
			}
            finally
            {
                con.Close();
            }
            return View();
        }


        public ActionResult Update(){
            int x = 0;
            int.TryParse(Request.Form["ids"], out x);


			com = new SqlCommand("SELECT * FROM tb_Student where StuID=@SID", con);
            com.Parameters.AddWithValue("@SID", x);

            SqlDataReader rd;
            string[] data = new string[4];
            try
            {
                con.Open();
                rd = com.ExecuteReader();
                while (rd.Read())
                {
                    data[0] = rd.GetInt32(0).ToString();
                    data[1] = rd.GetString(1);
                    data[2] = rd.GetString(2);
                    data[3] = rd.GetString(3);
                }

                if (int.TryParse(data[0], out x))
				{
					ViewData["id"] = x;
				}
				else
					ViewData["id"] = "Invalid ID";

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                con.Close();
            }
            ViewData["ls"] = data;
            return View();
        }

        public ActionResult Updated(){
            com = new SqlCommand("UPDATE tb_Student SET StuName=@Name, DOB=@DOB, Age=@Age WHERE StuID=@SID", con);
            com.Parameters.AddWithValue("@name", Request.Form["inName"]);
            com.Parameters.AddWithValue("@DOB", Request.Form["InDOB"]);
            com.Parameters.AddWithValue("@Age", Request.Form["inAge"]);
            com.Parameters.AddWithValue("@SID", Request.Form["hee"]);
            ViewData["e"] = Request.Form["hee"];
            try
            {
                con.Open();
                com.ExecuteNonQuery();
                ViewData["updated"] = "Updated successfully";

            }
            catch (Exception ex)
            {
                ViewData["updated"] = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return View();
        }


        public ActionResult grid(){
			if (Request.Form["app"] != null)
			{


			}

			SqlDataAdapter adt = new SqlDataAdapter("SELECT * FROM tb_Student", con);
			DataSet ds = new DataSet();

			try
			{
				con.Open();
				adt.Fill(ds);

				ViewData["abc"] = ds.Tables[0].Rows.Count;
				int i = 0;
				foreach (DataRow item in ds.Tables[0].Rows)
				{
					list.Add(new studentModel() { lID = i, ID = (int)item[0], StuName = item[1].ToString(), DOB = item[2].ToString(), Age = item[3].ToString() });
					i++;
				}

				/*
                switch (Request.Form["btnPrevious"])
                {
                    case "btnPrevious":
                        break;
                    default:
                        break;
                }
                */
				string param = "0";


				if (Request.Form["ind"] != null && int.Parse(Request.Form["ind"]) < list.Count / 5)
				{
					param = (int.Parse(Request.Form["ind"].ToString()) + 1).ToString();

				}

				if (Request.Form["indBack"] != null && int.Parse(Request.Form["indBack"]) >= 1)
				{
					param = (int.Parse(Request.Form["indBack"].ToString()) - 1).ToString();
				}

                ViewData["app"] = Request.Form["app"];
				ViewData["indBack"] = param;
				ViewData["ind"] = param;
				ViewData["ShowStudent"] = list.Where(l => l.lID >= int.Parse(param) * 5).Take(5);
				//ViewData["ShowStudent"] = list.Take(5).LastOrDefault();
				//list.inde

				//ViewData["ShowStudent"] = list;

			}
			catch (Exception ex)
			{
				//Console.Write(""+ex.Message);
				Response.Write(ex.Message);
			}
			finally
			{
				con.Close();
			}
			return View();
        }

    }
}
