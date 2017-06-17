using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App
{
    public partial class Form1 : Form
    {
        private SqlConnection SQLCon;
        private DataSet ds;
        private DataViewManager dsView;

        public Form1()
        {
            InitializeComponent();

            string _MSSQLServer = "x";
            string _MSSQLPort = "x";
            string _MSSQLUser = "x";
            string _MSSQLPass = "x";

            string SQLStringCon =
                @"Data Source =" + _MSSQLServer + "," + _MSSQLPort + ";" +
                "Initial Catalog = northwnd;" +
                "User Id =" + _MSSQLUser + ";" +
                "Password =" + _MSSQLPass + ";";

            SQLCon = new SqlConnection(SQLStringCon);
            ds = new DataSet("CustOrders");

            // Preenche o Dataset com os dados da tabela Customers,
            // mapeia o nome padrão da tabela
            // "Table" para "Customers"
            SqlDataAdapter da1 = new SqlDataAdapter
                ("SELECT * FROM Customers", SQLCon);
            da1.TableMappings.Add("Table", "Customers");
            da1.Fill(ds);

            // Preenche o Dataset com os dados da tabela Orders,
            // mapeia o nome padrão da tabela
            // "Table" to "Orders"
            SqlDataAdapter da2 = new SqlDataAdapter
                ("SELECT * FROM Orders", SQLCon);
            da2.TableMappings.Add("Table", "Orders");
            da2.Fill(ds);

            // Preenche o Dataset com os dados da tabela Order Details,
            // mapeia o nome padrão da tabela
            // "Table" to "OrderDetails"
            SqlDataAdapter da3 = new SqlDataAdapter
                ("SELECT * FROM [Order Details]", SQLCon);
            da3.TableMappings.Add("Table", "OrderDetails");
            da3.Fill(ds);

            // Define o relacionamento "RelCustOrd" 
            // entre Customers ---< Orders
            DataRelation relCustOrd;
            DataColumn colMaster1;
            DataColumn colDetail1;
            colMaster1 = ds.Tables["Customers"].Columns["CustomerID"];
            colDetail1 = ds.Tables["Orders"].Columns["CustomerID"];
            relCustOrd = new System.Data.DataRelation("RelCustOrd", colMaster1, colDetail1);
            ds.Relations.Add(relCustOrd);

            // Define o relacionamento "RelOrdDet" 
            // entre Orders ---< [Order Details]
            DataRelation relOrdDet;
            DataColumn colMaster2;
            DataColumn colDetail2;
            colMaster2 = ds.Tables["Orders"].Columns["OrderID"];
            colDetail2 = ds.Tables["OrderDetails"].Columns["OrderID"];
            relOrdDet = new System.Data.DataRelation("RelOrdDet", colMaster2, colDetail2);
            ds.Relations.Add(relOrdDet);

            dsView = ds.DefaultViewManager;

            // Vinculando com o Grid's
            grdOrders.DataSource = dsView;
            grdOrders.DataMember = "Customers.RelCustOrd";

            grdOrderDetails.DataSource = dsView;
            grdOrderDetails.DataMember = "Customers.RelCustOrd.RelOrdDet";

            //vinculando o combobox
            cbCust.DataSource = dsView;
            cbCust.DisplayMember = "Customers.CompanyName";
            cbCust.ValueMember = "Customers.CustomerID";

            // vinculando com os controles TextBox
            txtContact.DataBindings.Add("Text", dsView, "Customers.ContactName");
            txtPhoneNo.DataBindings.Add("Text", dsView, "Customers.Phone");
            txtFaxNo.DataBindings.Add("Text", dsView, "Customers.Fax");

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            CurrencyManager cm = (CurrencyManager)this.BindingContext[dsView, "Customers"];
            if (cm.Position < cm.Count - 1)
            {
                cm.Position++;
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (this.BindingContext[dsView, "Customers"].Position > 0)
            {
                this.BindingContext[dsView, "Customers"].Position--;
            }
        }
    }
}
