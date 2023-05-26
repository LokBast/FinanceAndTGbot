using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FinanceDesktop
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        public MainForm()
        {
            InitializeComponent();


            //chartControl1.Series["Items"] = dataSet.Items;
            //chartControl1.DataSource = dataSet.Items.ToList();


            //chartControl1.Titles = "asd";
            //chartControl1.Series["s1"].Points.AddPoint("1", 123);
            //chartControl1.Series["s1"].Points.AddPoint("2", 321);
            //chartControl1.Series["s1"].Points.AddPoint("3", 213);


            //chartControl1.Series["Itm"]. = ;  


            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Category", typeof(string));
            dataTable.Columns.Add("Sum", typeof(int));
            // Добавление данных в таблицу
            //dataTable.Rows.Add("Категория 1", 10);
            //dataTable.Rows.Add("Категория 2", 20);
            //dataTable.Rows.Add("Категория 3", 15);


            //chartControl1.DataSource = dataTable;

            //chartControl1.Series["s1"].ArgumentDataMember = "Category"; // Столбец с категориями
            //chartControl1.Series["s1"].ValueDataMembers.AddRange(new string[] { "Value" });

            //// Настройка отображения названий категорий
            //chartControl1.Series["s1"].Label.Visible = true;
            //chartControl1.Series["s1"].Label.TextPattern = "{A}: {V}"; // Формат отображения: {A} - аргумент (название категории), {V} - значение

            //pieChart.Series.Add(series);

            string connectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Finance;Data Source=DESKTOP-3E6ME6N";
            //string procedureNameTGBot = "Pie";




            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Pie", connection))
                {
                    //SqlCommand command = new SqlCommand(procedureNameTGBot, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);

                        chartControl1.DataSource = dataTable;

                        chartControl1.Series["s1"].ArgumentDataMember = "Category"; // Столбец с категориями
                        chartControl1.Series["s1"].ValueDataMembers.AddRange(new string[] { "Sum" });

                        // Настройка отображения процентов в метках
                        chartControl1.Series["s1"].Label.Visible = true;
                        chartControl1.Series["s1"].Label.TextPattern = "{VP:P0}"; // Формат отображения процентов

                        //chartControl1.Series["s1"].Label.TextPattern = "{A}: {V}"; // Формат отображения: {A} - аргумент (название категории), {V} - значение

                        // Настройка отображения чисел в легенде
                        chartControl1.Series["s1"].LegendTextPattern = "{V}"; // Формат отображения чисел в легенде
                    }
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void chartControl1_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
