using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.UI;

namespace Day5_8
{
    public partial class Assignment_5 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Assign();
            }
        }
        protected void Assign()
        {
            try
            {
                string cstring = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                MySqlConnection con = new MySqlConnection(cstring);
                MySqlCommand cmd = new MySqlCommand("select EmployeeID from employees", con);
                MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                DlEmpID.DataSource = dt;
                DlEmpID.DataTextField = "EmployeeID";
                DlEmpID.DataValueField = "EmployeeID";
                DlEmpID.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
        protected void AddButtonClick(object sender, EventArgs e)
        {
            AllMessage m = new AllMessage();
            MySqlCommand verifyTerID;
            MySqlCommand verifyEmpTerID;
            int terIDvalue;
            int empIDvalue;
            try
            {
                string cstring = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
                MySqlConnection con = new MySqlConnection(cstring);
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                //Check if TerritoryID Present in in Territory Table
                StringBuilder strTerr = new StringBuilder(m.strTerrCheck + textTerritoryID.Text + "'");
                verifyTerID = new MySqlCommand(strTerr.ToString(), con);
                terIDvalue = Convert.ToInt32(verifyTerID.ExecuteScalar());

                //Check if TerritoryID for corresponding EmployeeID are present in Employeeterritories Table
                StringBuilder strEmpTerr = new StringBuilder(m.strEmpTerCheck + DlEmpID.SelectedValue + "' AND TerritoryID='" + textTerritoryID.Text + "'");
                verifyEmpTerID = new MySqlCommand(strEmpTerr.ToString(), con);
                empIDvalue = Convert.ToInt32(verifyEmpTerID.ExecuteScalar());
                if (terIDvalue > 0)
                {
                    ClientScript.RegisterStartupScript(GetType(), "msgbox", "alert('TerritoryID already exists in Territories Table');", true);
                }
                else if (empIDvalue > 0)
                {
                    ClientScript.RegisterStartupScript(GetType(), "msgbox", "alert('EmployeeID and TerritoryID already exists in Employees table');", true);
                }
                else
                {
                    MySqlCommand cmd2 = new MySqlCommand(m.strInsertEmpTer + DlEmpID.SelectedItem.Value.ToString() + "','" + textTerritoryID.Text + "');", con);
                    cmd2.ExecuteNonQuery();
                    con.Close();
                    ClientScript.RegisterStartupScript(GetType(), "msgbox", "alert('Records successfully Added');", true);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
}