using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using XAppointment.Models;

namespace XAppointment.Controllers
{
    public class WarhouseController : Controller
    {
		SqlConnection con = new SqlConnection("Server=192.168.0.27;Database=XAppoinment;User Id=sa; Password=123456;");
		SqlCommand com;
        public ActionResult Index()
        {
            try
            {
				con.Open();
                com = new SqlCommand("SELECT WorkFlowID ,Name FROM sys_workflow", con);
				SqlDataReader reader = com.ExecuteReader();
                List<ItemSizeModel> list = new List<ItemSizeModel>();
				while (reader.Read())
				{
                    list.Add(new ItemSizeModel()
                    {
                        size = int.Parse(reader["WorkFlowID"].ToString()),
                        sName = reader["Name"].ToString()
                    });
				}
                ViewData["WorkFlow"] = list;
				reader.Close();

				com = new SqlCommand("SELECT WorkFlowStateID ,State FROM sys_workflowstate", con);
				reader = com.ExecuteReader();
                List<ItemSizeModel> lists = new List<ItemSizeModel>();
				while (reader.Read())
				{
                    lists.Add(new ItemSizeModel(){
                        size=int.Parse(reader["WorkFlowStateID"].ToString()),
                        sName=reader["State"].ToString()
                    });
				}
                ViewData["State"] = lists;
				reader.Close();
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

        public ActionResult SaveDB(){
            com = new SqlCommand("INSERT INTO tblWarehouse( ware_WorkflowID, ware_StateID, ware_CreatedDate, ware_Name, ware_Address)" +
                               "VALUES(@wfid, @wsid, GETDATE(), @wname, @wAddr)", con);
            com.Parameters.AddWithValue("@wfid", Request.Form["idWorkFlow"]);
            com.Parameters.AddWithValue("@wsid", Request.Form["idState"]);
            com.Parameters.AddWithValue("@wname", Request.Form["inName"]);
            com.Parameters.AddWithValue("@wAddr", Request.Form["inAddre"]);

            try
            {
                con.Open();
                com.ExecuteNonQuery();
                ViewData["status"] = "Success";
            }
            catch (Exception ex)
            {
                ViewData["status"] = ex.Message;
            }
            finally{
                con.Close();
            }
            return View();
        }

        public ActionResult WarehouseList(){
            if (Request.Form["dID"]!=null)
            {
                com = new SqlCommand("DELETE FROM tblWarehouse WHERE ware_WarehouseID = @ID", con);
                com.Parameters.AddWithValue("@ID", Request.Form["dID"]);
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
			com = new SqlCommand("SELECT ware_WarehouseID, (SELECT Name from sys_workflow where WorkFlowID=ware_WorkFlowID)as 'WorkFlow', ware_CreatedDate , ware_UpdatedDate,(SELECT [State] from sys_workflowstate where ware_StateID=WorkFlowStateID)as 'State', ware_Name, ware_Address FROM tblWarehouse", con);

			try
            {
                con.Open();
                SqlDataReader reader = com.ExecuteReader();
                List<warehouseModel> list = new List<warehouseModel>();

                int i = 0;
                while (reader.Read())
                {
                    list.Add(new warehouseModel()
                    {
                        iID = i,
                        ID = int.Parse(reader["ware_WarehouseID"].ToString()),
                        WorkFlow = reader["WorkFlow"].ToString(),
                        Date=reader["ware_CreatedDate"].ToString(),
                        Update=reader["ware_UpdatedDate"].ToString(),
                        State = reader["State"].ToString(),
                        Name = reader["ware_Name"].ToString(),
                        Address = reader["ware_Address"].ToString()

                    });
                    i++;
                }

				string param = "0";
                float lcount =list.Count/5;
                if (Request.Form["inn"] != null && int.Parse(Request.Form["inn"]) < (int)lcount)
				{
					param = (int.Parse(Request.Form["inn"].ToString()) + 1).ToString();
                }else if (Request.Form["idPre"] != null && int.Parse(Request.Form["idPre"]) >= 1)
				{
					param = (int.Parse(Request.Form["idPre"].ToString()) - 1).ToString();
				}

				ViewData["next"] = param;
				ViewData["prev"] = param;
                //ViewData["wld"] = list.GetRange(int.Parse(param) * 5, 5);
                ViewData["wld"] = list.Where(l => l.iID >= int.Parse(param) * 5).Take(5);

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
			try
			{
				con.Open();
				com = new SqlCommand("SELECT WorkFlowID ,Name FROM sys_workflow", con);
				SqlDataReader reader = com.ExecuteReader();
				List<ItemSizeModel> list = new List<ItemSizeModel>();
				while (reader.Read())
				{
					list.Add(new ItemSizeModel()
					{
						size = int.Parse(reader["WorkFlowID"].ToString()),
						sName = reader["Name"].ToString()
					});
				}
				ViewData["WorkFlow"] = list;
				reader.Close();

				com = new SqlCommand("SELECT WorkFlowStateID ,State FROM sys_workflowstate", con);
				reader = com.ExecuteReader();
				List<ItemSizeModel> lists = new List<ItemSizeModel>();
				while (reader.Read())
				{
					lists.Add(new ItemSizeModel()
					{
						size = int.Parse(reader["WorkFlowStateID"].ToString()),
						sName = reader["State"].ToString()
					});
				}
				ViewData["State"] = lists;
				reader.Close();


                com = new SqlCommand("SELECT ware_Name, ware_Address FROM tblwarehouse WHERE ware_WarehouseID=@id", con);
                com.Parameters.AddWithValue("@id", Request.Form["ID"]);
                reader = com.ExecuteReader();
                List<warehouseUpdateModel> lum = new List<warehouseUpdateModel>();
                while (reader.Read())
                {
                    lum.Add(new warehouseUpdateModel()
                    {
                        ID = int.Parse(Request.Form["ID"]),
                        Name=reader["ware_Name"].ToString(),
                        Address=reader["ware_Address"].ToString()
                    });
                }
				ViewData["lum"] = lum;

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

        public ActionResult UpdatedDB(){
            com = new SqlCommand("UPDATE tblWarehouse SET ware_WorkFlowID =@wID, ware_StateID = @sID, ware_Name = @wName, ware_Address = @wAddress, ware_UpdatedDate = GETDATE() WHERE ware_WarehouseID = @ID", con);
            com.Parameters.AddWithValue("@wID", Request.Form["idWorkFlow"]);
            com.Parameters.AddWithValue("@sID", Request.Form["idState"]);
            com.Parameters.AddWithValue("@wName", Request.Form["inName"]);
            com.Parameters.AddWithValue("@wAddress", Request.Form["inAddre"]);
            com.Parameters.AddWithValue("@ID", Request.Form["wID"]);
            try
            {
                con.Open();
                com.ExecuteNonQuery();
                ViewData["status"] = "success";
            }
            catch (Exception ex)
            {
                ViewData["status"] = ex.Message;
            }
            finally{
                con.Close();
            }
            return View();
        }
    }
}