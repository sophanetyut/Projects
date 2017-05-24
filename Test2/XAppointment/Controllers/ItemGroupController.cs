using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using XAppointment.Models;

namespace XAppointment.Controllers
{
    public class ItemGroupController : Controller
    {
		SqlConnection con = new SqlConnection("Server=192.168.0.27;Database=XAppoinment;User Id=sa; Password=123456;");
		SqlCommand com;
        public ActionResult Index()
        {
            com = new SqlCommand("SELECT cate_ItemCategoryID, cate_Name FROM tblItemCategory", con);
            List<ItemSizeModel> list = new List<ItemSizeModel>();
            SqlDataReader reader;

            try
            {
                con.Open();
                reader = com.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new ItemSizeModel()
                    {
                        size = int.Parse(reader["cate_ItemCategoryID"].ToString()),
                        sName = reader["cate_Name"].ToString()
                    });
                }
                ViewData["Cate"] = list;
                reader.Close();

                com = new SqlCommand("SELECT itmg_ItemGroupID, itmg_Name FROM tblItemGroup", con);
                reader = com.ExecuteReader();
                List<ItemSizeModel> lis = new List<ItemSizeModel>();
                while (reader.Read())
                {
                    lis.Add(new ItemSizeModel(){
                        size=int.Parse(reader["itmg_ItemGroupID"].ToString()),
                        sName=reader["itmg_Name"].ToString()
                    });
                }
                ViewData["itG"] = lis;
                reader.Close();

				com = new SqlCommand("SELECT WorkFlowID ,Name FROM sys_workflow", con);
                List<ItemSizeModel> lwf = new List<ItemSizeModel>();
                reader = com.ExecuteReader();
                while (reader.Read())
                {
                    lwf.Add(new ItemSizeModel()
                    {
                        size = int.Parse(reader["WorkFlowID"].ToString()),
                        sName=reader["Name"].ToString()
                    });
                }
                ViewData["lwf"] = lwf;
                reader.Close();

				com = new SqlCommand("SELECT WorkFlowStateID ,State FROM sys_workflowstate", con);
                List<ItemSizeModel> ls = new List<ItemSizeModel>();
                reader = com.ExecuteReader();
                while (reader.Read())
                {
                    ls.Add(new ItemSizeModel(){
                        size=int.Parse(reader["WorkFlowStateID"].ToString()),
                        sName=reader["State"].ToString()
                    });
                }
                ViewData["ls"] = ls;
                reader.Close();


            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally{
                con.Close();
            }

            return View ();
        }

        public ActionResult SaveDB(){

            com = new SqlCommand("INSERT INTO tblItemGroup(itmg_WorkFlowID, itmg_StateID, itmg_CreatedDate, itmg_Name" +
                                 ", itmg_ParentGroup, itmg_ItemCategoryID) VALUES(@iwf, @sid, GETDATE(), @iN, @ipg, @iici)", con);
            com.Parameters.AddWithValue("@iwf", Request.Form["idWorkFlow"]);
            com.Parameters.AddWithValue("@sid", Request.Form["idState"]);
            com.Parameters.AddWithValue("@iN", Request.Form["inName"]);
            com.Parameters.AddWithValue("@ipg", Request.Form["pg"]);
            com.Parameters.AddWithValue("@iici", Request.Form["c"]);

            try
            {
                con.Open();
                com.ExecuteNonQuery();

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

        public ActionResult ItemGroupList(){

            if (Request.Form["del"]!=null)
            {
                com = new SqlCommand("DELETE FROM tblItemGroup where itmg_ItemGroupID=@id", con);
                com.Parameters.AddWithValue("@id", Request.Form["del"]);
                try
                {
                    con.Open();
                    com.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }
                finally{
                    con.Close();
                }
            }
            com = new SqlCommand("SELECT  itmg_ItemGroupID, (SELECT Name FROM sys_workflow WHERE itmg_WorkFlowID=WorkFlowID)AS 'WorkFlow',(select [state] from sys_workflowstate where WorkFlowStateID=itmg_StateID )as 'State', " +
                                 "itmg_CreatedDate, itmg_Name, (SELECT cate_Name FROM tblItemCategory WHERE cate_ItemCategoryID=itmg_ItemCategoryID)AS 'Category' FROM tblItemGroup", con);
            List<ItemGroupListModel> il = new List<ItemGroupListModel>();
            try
            {
                con.Open();
                SqlDataReader reader = com.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    il.Add(new ItemGroupListModel()
                    {
                        ID = i,
                        iID=int.Parse(reader["itmg_ItemGroupID"].ToString()),
                        WorkFlow=reader.IsDBNull(1)?"":reader["WorkFlow"].ToString(),
                        State=reader.IsDBNull(2)?"": reader["State"].ToString(),
                        CreatedDate= reader.IsDBNull(3)?"": reader["itmg_CreatedDate"].ToString(),
                        Name= reader.IsDBNull(4)?"" : reader["itmg_Name"].ToString(),
                        Category= reader.IsDBNull(5)?"" : reader["Category"].ToString()
                    });
                    i++;
                }

				string param = "0";
                float lcount = il.Count / 5;
				if (Request.Form["inn"] != null && int.Parse(Request.Form["inn"]) < (int)lcount)
				{
					param = (int.Parse(Request.Form["inn"].ToString()) + 1).ToString();
				}
				else if (Request.Form["idPre"] != null && int.Parse(Request.Form["idPre"]) >= 1)
				{
					param = (int.Parse(Request.Form["idPre"].ToString()) - 1).ToString();
				}

				ViewData["next"] = param;
				ViewData["prev"] = param;
                ViewData["il"] = il.Where(l => l.ID >= int.Parse(param) * 5).Take(5); 

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            return View();
        }
         


        public ActionResult UpdateDB(){
			SqlDataReader reader;


			try
			{
                con.Open();

                com = new SqlCommand("SELECT itmg_ItemGroupID, itmg_WorkFlowID, itmg_StateID, itmg_Name, itmg_ItemCategoryID FROM tblItemGroup WHERE itmg_ItemGroupID=@id", con);
                com.Parameters.AddWithValue("@id", Request.Form["edits"]);
                List<ItemGroupListModel> iglm = new List<ItemGroupListModel>();
                reader = com.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    iglm.Add(new ItemGroupListModel()
                    {
                        ID = i,
                        iID=int.Parse(reader["itmg_ItemGroupID"].ToString()),
                        WorkFlow=reader["itmg_WorkFlowID"].ToString(),
                        State=reader["itmg_StateID"].ToString(),
                        Name=reader["itmg_Name"].ToString(),
                        Category=reader["itmg_ItemCategoryID"].ToString()

                    });
                    i++;
                }
                ViewData["boom"] = iglm;
                reader.Close(); 

                com = new SqlCommand("SELECT cate_ItemCategoryID, cate_Name FROM tblItemCategory", con);
				List<ItemSizeModel> list = new List<ItemSizeModel>();
				reader = com.ExecuteReader();
				while (reader.Read())
				{
					list.Add(new ItemSizeModel()
					{
						size = int.Parse(reader["cate_ItemCategoryID"].ToString()),
						sName = reader["cate_Name"].ToString()
					});
				}
				ViewData["Cate"] = list;
				reader.Close();

				com = new SqlCommand("SELECT itmg_ItemGroupID, itmg_Name FROM tblItemGroup", con);
				reader = com.ExecuteReader();
				List<ItemSizeModel> lis = new List<ItemSizeModel>();
				while (reader.Read())
				{
					lis.Add(new ItemSizeModel()
					{
						size = int.Parse(reader["itmg_ItemGroupID"].ToString()),
						sName = reader["itmg_Name"].ToString()
					});
				}
				ViewData["itG"] = lis;
				reader.Close();

				com = new SqlCommand("SELECT WorkFlowID ,Name FROM sys_workflow", con);
				List<ItemSizeModel> lwf = new List<ItemSizeModel>();
				reader = com.ExecuteReader();
				while (reader.Read())
				{
					lwf.Add(new ItemSizeModel()
					{
						size = int.Parse(reader["WorkFlowID"].ToString()),
						sName = reader["Name"].ToString()
					});
				}
				ViewData["lwf"] = lwf;
				reader.Close();

				com = new SqlCommand("SELECT WorkFlowStateID ,State FROM sys_workflowstate", con);
				List<ItemSizeModel> ls = new List<ItemSizeModel>();
				reader = com.ExecuteReader();
				while (reader.Read())
				{
					ls.Add(new ItemSizeModel()
					{
						size = int.Parse(reader["WorkFlowStateID"].ToString()),
						sName = reader["State"].ToString()
					});
				}
				ViewData["ls"] = ls;
				reader.Close();


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