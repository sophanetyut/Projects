using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using XAppointment.Models;

namespace XAppointment.Controllers
{
    public class ItemController : Controller
    {
		SqlConnection con = new SqlConnection("Server=192.168.0.27;Database=XAppoinment;User Id=sa; Password=123456;");
		SqlCommand com;

        public ActionResult Index()
        {
			com = new SqlCommand("SELECT itmg_ItemGroupID, itmg_Name FROM tblItemGroup", con);

			SqlDataReader reader;
			try
			{
				con.Open();
				reader = com.ExecuteReader();
				List<ItemGroupModel> list = new List<ItemGroupModel>();
				if (reader != null)
				{
					while (reader.Read())
					{
						list.Add(new ItemGroupModel() { 
                            item = reader.GetInt32(0).ToString(), 
                            iName =reader.IsDBNull(1)?"":reader.GetString(1)
                            //iName=reader.IsDBNull()
                        });
					}
					ViewData["IG"] = list;
				}
				reader.Close();

				com = new SqlCommand("SELECT size_SizeID, size_Name FROM tblSize", con);
				List<ItemSizeModel> listSize = new List<ItemSizeModel>();
				reader = com.ExecuteReader();
				if (reader != null)
				{
					while (reader.Read())
					{
						listSize.Add(new ItemSizeModel() { 
                            size =reader.GetInt32(0), 
                            sName = reader.GetString(1) 
                        });
					}

					ViewData["IS"] = listSize;
				}

				reader.Close();


				com = new SqlCommand("SELECT IDENT_CURRENT('tblItem')", con);
				reader = com.ExecuteReader();
				string ss = "";

				while (reader.Read())
				{
					ss = reader.GetDecimal(0).ToString();
				}
				ViewData["IDD"] = ss;

			}
			catch (Exception ex)
			{
				Response.Write(ex.Message);
			}
			finally
			{
				con.Close();
			}
            return View ();
        }
    }
}
