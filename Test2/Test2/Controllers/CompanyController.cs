using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Test2.Models;


namespace Test2.Controllers
{
    public class CompanyController : Controller
    {
        SqlConnection con = new SqlConnection("Server=192.168.0.27;Database=CRM;User Id=sa; Password=123456;");
        SqlDataAdapter ad;
        DataSet ds;
        List<company> com = new List<company>();
        public ActionResult Index()
        {
            try
            {
                con.Open();
                ad = new SqlDataAdapter("SELECT Comp_Name FROM company", con);
                //ad = new SqlDataAdapter("SELECT * FROM CRM;", con);
                ds = new DataSet();
                ad.Fill(ds);

                foreach (DataRow dr in ds.Tables[0].Rows)
				{
                    com.Add(new company() {companyName=dr[0].ToString()});
				}

            }
            catch (Exception )
            {
                throw;
            }
            finally{
                con.Close();
            }
            ViewData["myData"] = com;
            ViewData["DT"] = ds.Tables[0].Rows.Count;
            return View ();
        }
    }
}
