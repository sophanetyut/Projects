using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
using System.Data;
using System.Data.SqlClient;
using XAppointment.Models;


namespace XAppointment.Controllers
{
    
    public class HomeController : Controller
    {
        
        readonly SqlConnection con = new SqlConnection("Server=192.168.0.27;Database=XAppoinment;User Id=sa; Password=123456;");

        SqlCommand com;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SaveDB(){
            
            com = new SqlCommand("INSERT INTO tblItem(item_Name, item_ItemGroupID, item_Price, item_Qty, item_Currency," +
                                " item_Tag, item_isFeature, item_useSerial, item_isStock, item_ItemSize) VALUES( @Name, @GID, @Price, @Qty, @cur" +
                                 ", @tag, @feature, @Serial, @stock, @iSize)",con);
            com.Parameters.AddWithValue("@Name", Request.Form["inName"]);
            com.Parameters.AddWithValue("@GID", Request.Form["cbGroup"]);
            com.Parameters.AddWithValue("@Price", Request.Form["inPrice"]);
            com.Parameters.AddWithValue("@Qty", Request.Form["inQty"]);
            com.Parameters.AddWithValue("@cur", Request.Form["inCur"]);
            com.Parameters.AddWithValue("@tag", Request.Form["inTag"]);
            com.Parameters.AddWithValue("@feature", Request.Form["inFea"]);
            com.Parameters.AddWithValue("@Serial", Request.Form["inSer"]);
            com.Parameters.AddWithValue("@stock", Request.Form["inStock"]);
            com.Parameters.AddWithValue("@iSize", Request.Form["cbSize"]);

            try
            {
                con.Open();
                com.ExecuteNonQuery();
                @ViewData["result"] = "Success";
            }
            catch(SqlException ex){
                if (ex.Number==8114)
                {
					@ViewData["result"] = "Make sure you input the Number correctly.";
				}
            }
            catch (Exception ex)
            {
                @ViewData["result"] = ex.Message;
			}
            finally{
                con.Close();
            }

            return View();
        }
        //===========================================================================================




        public ActionResult ShowDB(){
            
            if (Request.Form["btDe"]!=null)
            {
				com = new SqlCommand("delete from tblItem where item_ItemID=@id", con);
				com.Parameters.AddWithValue("@id", Request.Form["btDe"]);
				try
				{
					con.Open();
					com.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					Response.Write(ex.Message);
				}
				finally
				{
					con.Close();
				}
			}

            com = new SqlCommand("SELECT item_Name, (select itmg_Name from tblItemGroup where itmg_ItemGroupID = item_ItemGroupID) as item_ItemGroupID, item_Price, item_Qty, item_Currency, " +
                               "item_Tag, item_isFeature, item_useSerial, item_isStock, item_ItemSize, item_ItemID FROM tblItem", con);
            try
            {
                con.Open();
                SqlDataReader reader = com.ExecuteReader();
                List<selectItemsModel> list = new List<selectItemsModel>();
                int i = 0;
                while (reader.Read())
                {
                    list.Add(new selectItemsModel()
                    {
                        indx = i,
                        Name = reader.GetValue(0).ToString(),
                        Group=reader["item_ItemGroupID"].ToString(),
                        Price =reader.GetValue(2).ToString(),
                        Qty=reader.GetValue(3).ToString(),
                        Currency = reader.GetValue(4).ToString(),
                        Tag = reader.GetValue(5).ToString(),
                        Feature = reader.GetValue(6).ToString(),
                        Serial = reader.GetValue(7).ToString(),
                        Stock = reader.GetValue(8).ToString(),
                        Size = reader.GetValue(9).ToString(),
                        itemID=reader.GetInt32(10)
                });
                    i++;
                }

				string param = "0";
				if (Request.Form["next"] != null && int.Parse(Request.Form["next"]) <= list.Count / 5)
				{
					param = (int.Parse(Request.Form["next"]) + 1).ToString();
				}

				if (Request.Form["prev"] != null && int.Parse(Request.Form["prev"]) >= 1)
				{
					param = (int.Parse(Request.Form["prev"]) - 1).ToString();
				}

                ViewData["itemCount"] = list.Count;
                ViewData["next"] = param;
                ViewData["prev"] = param;
                ViewData["dt"] = list.Where(l=>l.indx>=int.Parse(param)*5).Take(5);
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally{
                con.Close();
            }
            return View();
        }


        public ActionResult UpdateDB(){
            
            com = new SqlCommand("SELECT item_Name, (select item_Name from tblItemGroup where itmg_ItemGroupID = item_ItemGroupID) as item_ItemGroupID, item_Price, item_Qty, item_Currency, item_Tag, item_isFeature, item_useSerial, item_isStock, item_ItemSize, item_ItemID FROM tblItem WHERE item_ItemID = @ID", con);
            com.Parameters.AddWithValue("@ID", Request.Form["btEdit"]);
            SqlDataReader reader;
            List<selectItemsModel> list = new List<selectItemsModel>();

            try
            {
                con.Open();
                reader = com.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    list.Add(new selectItemsModel()
                    {
                        indx = i,
                        Name = getReader(reader["item_Name"].ToString()),
                        Group = getReader(reader["item_ItemGroupID"].ToString()),
                        Price = getReader(reader["item_Price"].ToString()),
                        Qty = getReader(reader["item_Qty"].ToString()),
                        Currency = reader.IsDBNull(4)?"hel":reader["item_Currency"].ToString(),
                        Tag = reader.IsDBNull(5)?"tag":reader.GetValue(5).ToString(),
                        Feature = reader.IsDBNull(6)?"":reader.GetValue(6).ToString(),
                        Serial = reader.IsDBNull(7)?"":reader.GetValue(7).ToString(),
                        Stock = reader.IsDBNull(8)?"":reader.GetValue(8).ToString(),
                        Size =reader.IsDBNull(9)?"1":reader.GetValue(9).ToString(),
                        itemID=int.Parse((reader["item_ItemID"].ToString()))

					});
                    i++;
                }
                reader.Close();
                ViewData["e"] = list;

				com = new SqlCommand("SELECT itmg_ItemGroupID, itmg_Name FROM tblItemGroup ", con);
                List<ItemGroupModel> glist = new List<ItemGroupModel>();
                reader= com.ExecuteReader();
                while (reader.Read())
                {
                    glist.Add(new ItemGroupModel(){
                        item = reader.GetInt32(0).ToString(),
						iName = reader.GetString(1)
                    });
                }
                ViewData["glist"] = glist;
                reader.Close();

                com = new SqlCommand("SELECT size_SizeID, size_Name FROM tblSize", con);
                List<ItemSizeModel> slist = new List<ItemSizeModel>();
                reader = com.ExecuteReader();
                while (reader.Read())
                {
                    slist.Add(new ItemSizeModel(){
                        size = reader.GetInt32(0),
						sName = reader.GetString(1)
                    });
                }
                ViewData["slist"] = slist;

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally{
                con.Close();
            }

            return View();
        }

        private string getReader(string obj){
            string s= string.IsNullOrEmpty(obj) ? "" : obj;
            return s;
        }

        public ActionResult ResultUpdate(){
			com = new SqlCommand("UPDATE tblItem SET item_Name=@Name, item_ItemGroupID=@GID, item_Price=@Price, item_Qty=@Qty, item_Currency=@cur, item_Tag=@tag, item_isFeature=@feature, item_useSerial=@Serial, item_isStock=@stock, item_ItemSize=@iSize WHERE item_ItemID=@id", con);
			com.Parameters.AddWithValue("@Name", Request.Form["inName"]);
			com.Parameters.AddWithValue("@GID", Request.Form["cbGroup"]);
			com.Parameters.AddWithValue("@Price", Request.Form["inPrice"]);
			com.Parameters.AddWithValue("@Qty", Request.Form["inQty"]);
			com.Parameters.AddWithValue("@cur", Request.Form["inCur"]);
			com.Parameters.AddWithValue("@tag", Request.Form["inTag"]);
			com.Parameters.AddWithValue("@feature", Request.Form["inFea"]);
			com.Parameters.AddWithValue("@Serial", Request.Form["inSer"]);
			com.Parameters.AddWithValue("@stock", Request.Form["inStock"]);
			com.Parameters.AddWithValue("@iSize", Request.Form["cbSize"]);
            com.Parameters.AddWithValue("@id", Request.Form["hID"]);

			try
			{
				con.Open();
				com.ExecuteNonQuery();
				@ViewData["result"] = "Success";
			}
			catch (Exception ex)
			{
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