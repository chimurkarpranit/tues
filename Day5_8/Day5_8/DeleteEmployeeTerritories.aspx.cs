using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

namespace Day5_8
{
    public partial class DeleteEmployeeTerritories : System.Web.UI.Page
    {
        AllMessage m = new AllMessage();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
                //Adding an Attribute to Server Control(i.e. ButtonDeleteRecord)  
                ButtonDeleteRecord.Attributes.Add("onclick", "javascript:return DeleteConfirm()");
            }
        }
        protected void BindGrid()
        {
            try
            {
                string cstring = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                MySqlConnection con = new MySqlConnection(cstring);
                MySqlCommand cmd = new MySqlCommand(m.strEmTer, con);
                MySqlDataAdapter sda = new MySqlDataAdapter();
                sda.SelectCommand = cmd;
                DataSet ds = new DataSet();
                sda.Fill(ds);
                GridView1.DataSource = ds;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                m = null;
            }
        }
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            BindGrid();
        }
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            BindGrid();
        }
        protected void ButtonDeleteRecord_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow grow in GridView1.Rows)
                {
                    //Searching CheckBox("CheckBoxDelete") in an individual row of Grid
                    CheckBox checkdel = (CheckBox)grow.FindControl("CheckBoxDelete");
                    //If CheckBox is checked than delete the record with particular empid and terid
                    if (checkdel.Checked)
                    {
                        int empID = Convert.ToInt32(GridView1.DataKeys[grow.RowIndex].Values["EmployeeID"]);
                        int terID = Convert.ToInt32(GridView1.DataKeys[grow.RowIndex].Values["TerritoryID"]);
                        string cstring = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                        MySqlConnection con = new MySqlConnection(cstring);
                        MySqlCommand cmd1 = new MySqlCommand(m.strEmTerDel + empID + "' and TerritoryID = '" + terID + "'", con);
                        con.Open();
                        cmd1.ExecuteNonQuery();
                        con.Close();
                        BindGrid();
                        ClientScript.RegisterStartupScript(GetType(), "msgbox", "alert('Record is deleted successfully');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                m = null;
            }
        }
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            object EmpUpID, TerrUpID; //Objects needed to fetch the current values of gridview rows
            MySqlCommand verifyBoth;
            int bothIDValue;
            EmpUpID = GridView1.DataKeys[e.RowIndex].Values["EmployeeID"];
            TerrUpID = GridView1.DataKeys[e.RowIndex].Values["TerritoryID"];
            GridViewRow row = GridView1.Rows[e.RowIndex];
            string em = ((TextBox)row.FindControl("txtEmployeeID")).Text;
            string ter = ((TextBox)row.FindControl("txtTerritoryID")).Text;
            try
            {
                string cstring = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                MySqlConnection con = new MySqlConnection(cstring);
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                //Checking if both ids exists or not in employeeterritories Table
                StringBuilder strBoth = new StringBuilder(m.strEmTerCheck + em + "' and TerritoryID='" + ter + "'");
                verifyBoth = new MySqlCommand(strBoth.ToString(), con);
                bothIDValue = Convert.ToInt32(verifyBoth.ExecuteScalar());
                if (bothIDValue > 0)
                {
                    ClientScript.RegisterStartupScript(GetType(), "msgbox", "alert('EmployeeID and TerritoryID already exists in EmployeeTerritories Table');", true);
                }
                else
                {
                    GridView1.EditIndex = -1;
                    MySqlCommand cmd = new MySqlCommand(m.strEmTerUpdate + em + "',TerritoryID='" + ter + "' where EmployeeID = '" + EmpUpID + "' and TerritoryID = '" + TerrUpID + "'", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    BindGrid();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                EmpUpID = null;
                TerrUpID = null;
                verifyBoth = null;
                m = null;
            }
        }
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGrid();
        }
        protected void Insert(object sender, EventArgs e)
        {
            MySqlCommand verifyBothAdd;
            int bothExistsAdd;
            string EmployeeIDAdd = textEmployeeID.Text;
            string TerritoryIDAdd = textTerritoryID.Text;
            textEmployeeID.Text = "";
            textTerritoryID.Text = "";
            try
            {
                string cstring = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                MySqlConnection con = new MySqlConnection(cstring);
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                //Checking if both ids exists or not in employeeterritories Table
                StringBuilder strBothAdd = new StringBuilder(m.strEmTerCheck + EmployeeIDAdd + "' and TerritoryID='" + TerritoryIDAdd + "'");
                verifyBothAdd = new MySqlCommand(strBothAdd.ToString(), con);
                bothExistsAdd = Convert.ToInt32(verifyBothAdd.ExecuteScalar());
                if (bothExistsAdd > 0)
                {
                    ClientScript.RegisterStartupScript(GetType(), "msgbox", "alert('EmployeeID and TerritoryID already exists in EmployeeTerritories Table');", true);
                }
                else
                {
                    MySqlCommand cmd1 = new MySqlCommand(m.strEmTerInsert + "'" + EmployeeIDAdd + "'" + "," + "'" + TerritoryIDAdd + "')", con);
                    cmd1.ExecuteNonQuery();
                    con.Close();
                    BindGrid();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                verifyBothAdd = null;
                m = null;
            }
        }
    }
}