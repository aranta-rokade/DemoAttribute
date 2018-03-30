using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using AttributesTableColumn;

namespace AplnEmployee_UsingAttribute
{
    public partial class Form1 : Form
    {
        #region referenceVariables
        string path = @"C:\Users\arant\Documents\DemoAttribute\Employee_UsingAtrribute\bin\Debug\Employee_UsingAtrribute.dll";
        string connString = @"Data Source=DESKTOP-Q2BSH83;Initial Catalog=DemoAttributeDB;Integrated Security=True;Connect Timeout=30;";
        SqlConnection conn;
        #endregion
        
        #region assemblyVariables
        Assembly assembly = null;

        Type ClassName;
        object[] ClassAttributes;
        string tableName = "";

        PropertyInfo[] ClassProperties;
        object[] PropertyAttributes;

        string queryCreateTable = "";

        #endregion

        #region formVariables
        Label[] lbl;
        Label[] dtype;
        TextBox[] tbx;
        #endregion


        public Form1()
        {
            InitializeComponent();
            LoadAssembly();
        }

        public void LoadAssembly()
        {
            assembly = Assembly.LoadFrom(path);

            if (assembly != null)
            {
                ClassName = (assembly.GetType("Employee_UsingAtrribute.Employee"));
                ClassAttributes = ClassName.GetCustomAttributes(false);
                if (ClassAttributes[0] is TableAttribute)
                { 
                    tableName = ((TableAttribute)(ClassAttributes[0])).TableName;

                    queryCreateTable = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + tableName + "') BEGIN DROP TABLE " + tableName + " END ";

                    queryCreateTable += "CREATE TABLE " + tableName + " (";

                    ClassProperties = ClassName.GetProperties();
                    PropertyAttributes = new ColumnAttribute[ClassProperties.Count()];
                    lbl = new Label[ClassProperties.Count()];
                    dtype = new Label[ClassProperties.Count()];
                    tbx = new TextBox[ClassProperties.Count()];

                    int i = 0; int x = 15; int y = 30;
                    foreach (PropertyInfo prop in ClassProperties)
                    {
                        PropertyAttributes[i] = prop.GetCustomAttribute(typeof(ColumnAttribute), true);
                        if (PropertyAttributes[i] is ColumnAttribute)
                        {
                           queryCreateTable += " "+((ColumnAttribute)(PropertyAttributes[i])).ColumnName + " ";

                           queryCreateTable += ((ColumnAttribute)(PropertyAttributes[i])).ColumnDataType + " ";

                           if (!(((ColumnAttribute)(PropertyAttributes[i])).ColumnDataType.Equals("int")))
                           { queryCreateTable += "(" + ((ColumnAttribute)(PropertyAttributes[i])).ColumnSize + ") "; }

                           queryCreateTable += ",";
                           lbl[i] = new Label();
                           lbl[i].Text = ((ColumnAttribute)(PropertyAttributes[i])).ColumnName;
                           lbl[i].Location = new Point(x,y);
                           groupBox1.Controls.Add(lbl[i]);
                           dtype[i] = new Label();
                           dtype[i].Text = ((ColumnAttribute)(PropertyAttributes[i])).ColumnDataType;
                           dtype[i].Location = new Point(x + lbl[i].Width + 3, y);
                           groupBox1.Controls.Add(dtype[i]);
                           tbx[i] = new TextBox();
                           tbx[i].Location = new Point(x + lbl[i].Width + dtype[i].Width + 3, y);
                           groupBox1.Controls.Add(tbx[i]);
                           y += 30;
                           
                           i++;
                        }
                    }
                    queryCreateTable = queryCreateTable.Substring(0, queryCreateTable.Length - 1);
                    queryCreateTable += " ); ";
                }
            }
        } 

        private void btnCreate_Click(object sender, EventArgs e)
        {
            DialogResult ans = MessageBox.Show("Clicking yes will lead to deletion of existing table and creation of a new one.(click yes for the first time)", "Create a New Table?", MessageBoxButtons.YesNo);

            if(ans == DialogResult.No)
            {
                MessageBox.Show("Table Creation Cancelled");
            }
            if (ans == DialogResult.Yes)
            {
                using (conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand(queryCreateTable, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    lbl_table.Visible = true;
                    btnSelect.Visible = true;
                    btnInsert.Visible = true;
                    //btnUpdate.Visible = true;
                    //btnDelete.Visible = true;
                    btnExit.Visible = true;
                    dataGridView1.Visible = true;
                    groupBox1.Visible = true;

                    foreach (Label item in lbl)
                    {
                        item.Visible = true;
                    }

                    foreach (Label item in dtype)
                    {
                        item.Visible = true;
                    }

                    foreach (TextBox item in tbx)
                    {
                        item.Visible = true;
                    }

                    MessageBox.Show("Table "+tableName+" created.");
                    BindGridView();
                }
                
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            BindGridView();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            using (conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = null;

                string query = "INSERT INTO " + tableName + " VALUES ( ";
                bool emptyflag = false;

                for (int i = 0; i < PropertyAttributes.Length; i++)
                {
                    if (dtype[i].Text == "int")
                    {
                        if (tbx[i].Text == "")
                        {
                            emptyflag = true;
                            break;
                        }
                        query += tbx[i].Text+",";
                    }
                    else if (tbx[i].Text == "")
                    {
                        query += "'NULL',";

                    }
                    else
                    {
                        query += "'"+tbx[i].Text + "',";
                    }

                }

                if (emptyflag == false)
                {
                    query = query.Substring(0, query.Length - 1);
                    query += ");";

                    try
                    {
                        cmd = new SqlCommand(query, conn);
                        int rowAffected = cmd.ExecuteNonQuery();
                        if (rowAffected != 0)
                        {
                            MessageBox.Show("Row inserted.");
                        }
                        else
                        {
                            MessageBox.Show("Row not inserted.");
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show(sqlEx.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Int values should not be NULL");
                }
                conn.Close();

            }

            BindGridView();
        }

        public void BindGridView()
        {
            using (conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = null;

                string query = "SELECT * FROM " + tableName;
                cmd = new SqlCommand(query, conn);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    dataGridView1.DataSource = dt;
                }
                conn.Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
        //    using (conn = new SqlConnection(connString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = null;

        //        string query = "UPDATE " + tableName + " SET ";
        //        bool emptyflag = false;

        //        for (int i = 0; i < PropertyAttributes.Length; i++)
        //        {
        //            if (dtype[i].Text == "int")
        //            {
        //                if (tbx[i].Text == "")
        //                {
        //                    emptyflag = true;
        //                    break;
        //                }
        //                query += lbl[i].Text + " = " + tbx[i].Text + ",";
        //            }
        //            else if (tbx[i].Text == "")
        //            {
        //                query += lbl[i].Text + " = 'NULL',";

        //            }
        //            else
        //            {
        //                query += lbl[i].Text + " = '" + tbx[i].Text + "',";
        //            }

        //        }

        //        if (emptyflag == false)
        //        {
        //            query = query.Substring(0, query.Length - 1);
        //            query += " WHERE " + lbl[0].Text + " = " + tbx[0].Text + ";";

        //            try
        //            {
        //                cmd = new SqlCommand(query, conn);
        //                int rowAffected = cmd.ExecuteNonQuery();
        //                if (rowAffected != 0)
        //                {
        //                    MessageBox.Show("Row updated.");
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Row not updated.");
        //                }
        //            }
        //            catch (SqlException sqlEx)
        //            {
        //                MessageBox.Show(sqlEx.Message);
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("Int values should not be NULL");
        //        }
        //        conn.Close();

        //    }

        //    BindGridView();
        }

    }
}
