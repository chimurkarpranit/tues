using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Day5_8
{
    public partial class Sample : System.Web.UI.Page
    {
        AllMessage m = new AllMessage();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                BindGrid();
            }
            GridView1.Visible = false;
            ButtonDeleteRecord.Visible = false;
        }
        protected void BindGrid()
        {
            try
            {
                string cstring = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                MySqlConnection con = new MySqlConnection(cstring);
                MySqlCommand cmd = new MySqlCommand("select * from territories", con);
                MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                GridView1.DataSource = ds;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
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
                        MySqlCommand cmd1 = new MySqlCommand(m.strEmTerDel + empID + "' and TerritoryID = '" + terID + "'; " + m.strEmTerDel + terID + "'", con);
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
                    MySqlCommand cmd = new MySqlCommand(m.strUpdate1 + ter + "' where EmployeeID = '" + EmpUpID + "' and TerritoryID = '" + TerrUpID + "'; "+, con);
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
        protected void AddButtonClick(object sender, EventArgs e)
        {
            AllMessage m = new AllMessage();
            MySqlCommand verifyEmpID;
            MySqlCommand verifyTerID;
            MySqlCommand verifyEmpTerID;
            int empIDvalue;
            int terIDvalue;
            int empTerIDvalue;
            try
            {
                string cstring = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                MySqlConnection con = new MySqlConnection(cstring);
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                //Check if EmployeeID Present in in Employees Table
                StringBuilder strEmpCheck = new StringBuilder(m.strEmpCheck + textEmployeeID.Text + "'");
                verifyEmpID = new MySqlCommand(strEmpCheck.ToString(), con);
                empIDvalue = Convert.ToInt32(verifyEmpID.ExecuteScalar());

                //Check if TerritoryID Present in in Territory Table
                StringBuilder strTerr = new StringBuilder(m.strTerrCheck + textTerritoryID.Text + "'");
                verifyTerID = new MySqlCommand(strTerr.ToString(), con);
                terIDvalue = Convert.ToInt32(verifyTerID.ExecuteScalar());

                //Check if TerritoryID for corresponding EmployeeID are present in Employeeterritories Table
                StringBuilder strEmpTerr = new StringBuilder(m.strEmpTerCheck + textEmployeeID.Text + "' AND TerritoryID='" + textTerritoryID.Text + "'");
                verifyEmpTerID = new MySqlCommand(strEmpTerr.ToString(), con);
                empTerIDvalue = Convert.ToInt32(verifyEmpTerID.ExecuteScalar());
                if(empIDvalue > 0)
                {
                    ClientScript.RegisterStartupScript(GetType(), "msgbox", "alert('EmployeeID already exists in Employees Table');", true);
                }
                else if (terIDvalue > 0)
                {
                    ClientScript.RegisterStartupScript(GetType(), "msgbox", "alert('TerritoryID already exists in Territories Table');", true);
                }
                else if (empTerIDvalue > 0)
                {
                    ClientScript.RegisterStartupScript(GetType(), "msgbox", "alert('EmployeeID and TerritoryID already exists in Employees table');", true);
                }
                else
                {
                    MySqlCommand cmd2 = new MySqlCommand(m.strInsertEmpTer + textEmployeeID.Text + "','" + textTerritoryID.Text + "');", con);
                    cmd2.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
}