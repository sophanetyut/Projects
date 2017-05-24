using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.Data.SqlClient;
using XAppointmentV2.Models;
using sapi;

namespace XAppointmentV2.Controllers
{
    public class HomeController : Controller
    {
        db data = new db();
        public ActionResult Index()
        {
            data.connect();
            Response.Write(data.readData("item_Name", "SELECT item_Name FROM TblItem WHERE item_ItemID=@id", new List<SqlParameter>(){
                new SqlParameter("@id",15)
            }));

            data.close();
            return View();
        }

        public ActionResult Item(){
            
			data.connect();
            List<comboboxITem> l = new List<comboboxITem>();
            DataTable dt;
            dt= data.readData("SELECT itmg_ItemGroupID, itmg_Name FROM tblItemGroup", new List<SqlParameter>() { });
            foreach (DataRow item in dt.Rows)
            {
                l.Add(new comboboxITem(){
                    ID=int.Parse(item["itmg_ItemGroupID"].ToString()),
                    text=item["itmg_Name"].ToString()
                });
            }
            ViewData["IG"] = l;

            List<comboboxITem> ls = new List<comboboxITem>();
            dt = data.readData("SELECT size_sizeID, size_Name FROM tblSize", new List<SqlParameter>());
            foreach (DataRow item in dt.Rows)
            {
                ls.Add(new comboboxITem()
                {
                    ID = int.Parse(item["size_sizeID"].ToString()),
                    text=item["size_Name"].ToString()
                });
            }
            ViewData["ID"] = ls;


            string ss = data.readData("boom", "SELECT IDENT_CURRENT('tblItem') as boom", new List<SqlParameter>() { });
            ViewData["IDD"] = ss;

            data.close();
            return View();
        }

        public ActionResult SaveDB(){
            data.connect();
            ViewData["status"]=data.execData(@"INSERT INTO tblItem(item_Name, item_ItemGroupID, item_Price, item_Qty, item_Currency,item_Tag, item_isFeature, item_useSerial, item_isStock, item_ItemSize) VALUES( @Name, @GID, @Price, @Qty, @cur , @tag, @feature, @Serial, @stock, @iSize)", "", new List<SqlParameter>(){
                new SqlParameter("@Name", Request.Form["inName"])
                ,new SqlParameter("@GID", Request.Form["cbGroup"])
                ,new SqlParameter("@Price", Request.Form["inPrice"])
                ,new SqlParameter("@Qty", Request.Form["inQty"])
                ,new SqlParameter("@cur", Request.Form["inCur"])
                ,new SqlParameter("@tag", Request.Form["inTag"])
                ,new SqlParameter("@feature", Request.Form["inFea"])
                ,new SqlParameter("@Serial", Request.Form["inSer"])
                ,new SqlParameter("@stock", Request.Form["inStock"])
                ,new SqlParameter("@iSize", Request.Form["cbSize"])
            });
            data.close();
            return View();
        }



        public ActionResult ShowItem (){




            return View();
        }
    }
}