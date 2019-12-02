using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormDBProjectEX
{
    public partial class Form1 : Form
    {
        MySqlConnection conn;
        MySqlDataAdapter dataAdapter;
        DataSet dataSet;
        int selectedRowIndex;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Table_book.Visible = false;
            Table_customer.Visible = false;
            Table_orders.Visible = false;

            string connStr = "server=localhost;port=3307;database=mydb;uid=root;pwd=nayeon0529";
            conn = new MySqlConnection(connStr);
            dataAdapter = new MySqlDataAdapter("SELECT * FROM book", conn);
            dataSet = new DataSet();
        }

        #region radiobtn 체크 상태에 따라 DB 테이블 불러오기
        private void Book_CheckedChanged(object sender, EventArgs e)
        {
            Table_book.Visible = true;
            Table_customer.Visible = false;
            Table_orders.Visible = false;
            button4.Visible = false;
            dataAdapter = new MySqlDataAdapter("SELECT * FROM book", conn);
            dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "book");
            dataGridView1.DataSource = dataSet.Tables["book"];
        }

        private void Customer_CheckedChanged(object sender, EventArgs e)
        {
            Table_book.Visible = false;
            Table_customer.Visible = true;
            Table_orders.Visible = false;
            button4.Visible = true;
            dataAdapter = new MySqlDataAdapter("SELECT * FROM customer", conn);
            dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "customer");
            dataGridView1.DataSource = dataSet.Tables["customer"];
        }

        private void Orders_CheckedChanged(object sender, EventArgs e)
        {
            Table_book.Visible = false;
            Table_customer.Visible = false;
            Table_orders.Visible = true;
            button4.Visible = true;
            dataAdapter = new MySqlDataAdapter("SELECT * FROM orders", conn);
            dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "orders");
            dataGridView1.DataSource = dataSet.Tables["orders"];

            dataGridView1.Columns["orderid"].DisplayIndex = 0;
            dataGridView1.Columns["custid"].DisplayIndex = 1;
            dataGridView1.Columns["bookid"].DisplayIndex = 2;
        }
        #endregion

        #region 행 클릭 메소드

        private void ALLAHUAKBAR(object sender, DataGridViewCellEventArgs e)
        {
            selectedRowIndex = e.RowIndex;

            if (selectedRowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[selectedRowIndex];

            if (Book.Checked)
            {
                BookTableRow_Click();
            }
            else if (Customer.Checked)
                CusomerTableRow_Click();
            else
                OrdersTableRow_Click();
        }

        private void CusomerTableRow_Click()
        {
            textBox5.Text = dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString();
            textBox6.Text = dataGridView1.Rows[selectedRowIndex].Cells[1].Value.ToString();
            textBox7.Text = dataGridView1.Rows[selectedRowIndex].Cells[2].Value.ToString();
            textBox8.Text = dataGridView1.Rows[selectedRowIndex].Cells[3].Value.ToString();
        }

        private void OrdersTableRow_Click()
        {
            textBox9.Text = dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString();
            textBox10.Text = dataGridView1.Rows[selectedRowIndex].Cells[1].Value.ToString();
            textBox11.Text = dataGridView1.Rows[selectedRowIndex].Cells[2].Value.ToString();
            textBox12.Text = dataGridView1.Rows[selectedRowIndex].Cells[3].Value.ToString();
            textBox13.Text = dataGridView1.Rows[selectedRowIndex].Cells[4].Value.ToString().Substring(0, 10);
        }

        private void BookTableRow_Click()
        {
            textBox1.Text = dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.Rows[selectedRowIndex].Cells[1].Value.ToString();
            textBox3.Text = dataGridView1.Rows[selectedRowIndex].Cells[2].Value.ToString();
            textBox4.Text = dataGridView1.Rows[selectedRowIndex].Cells[3].Value.ToString();
        }

        #endregion

        #region SelectBtn
        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (Book.Checked == true)
            {
                BookTableSelect_Click();
            }
            else if (Customer.Checked == true)
                CusomerTableSelect_Click();
            else
                OrdersTableSelect_Click();

        }

        private void OrdersTableSelect_Click()
        {
            string queryStr;

            #region Select QueryString 만들기
            string[] conditions = new string[5];
            conditions[0] = (textBox9.Text != "") ? "orderid=@id" : null;
            conditions[1] = (textBox10.Text != "") ? "custid=@custid" : null;
            conditions[2] = (textBox11.Text != "") ? "bookid=@bookid" : null;
            conditions[3] = (textBox12.Text != "") ? "saleprice=@saleprice" : null;
            string condition_orderdate;
            if (textBox13.Value.ToString("yyyy-MM-dd") != "" && textBox14.Value.ToString("yyyy-MM-dd") != "")
            {
                condition_orderdate = "orderdate>=@min and orderdate<=@max";
            }
            else if (textBox13.Value.ToString("yyyy-MM-dd") != "" || textBox14.Value.ToString("yyyy-MM-dd") != "")
            {
                if (textBox13.Value.ToString("yyyy-MM-dd") != "")
                    condition_orderdate = "orderdate>=@min";
                else
                    condition_orderdate = "orderdate <= @max";
            }
            else
            {
                condition_orderdate = null;
            }
            conditions[4] = condition_orderdate;

            if (conditions[0] != null || conditions[1] != null || conditions[2] != null || conditions[3] != null || conditions[4] != null)
            {
                queryStr = $"SELECT * FROM orders WHERE ";
                bool firstCondition = true;
                for (int i = 0; i < conditions.Length; i++)
                {
                    if (conditions[i] != null)
                        if (firstCondition)
                        {
                            queryStr += conditions[i];
                            firstCondition = false;
                        }
                        else
                        {
                            queryStr += " and " + conditions[i];
                        }
                }
            }
            else
            {
                queryStr = "SELECT * FROM orders";
            }
            #endregion

            #region SelectCommand 객체 생성 및 Parameters 설정
            dataAdapter.SelectCommand = new MySqlCommand(queryStr, conn);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@id", textBox9.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@custid", textBox10.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@bookid", textBox11.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@saleprice", textBox12.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@min", textBox13.Value.ToString("yyyy-MM-dd"));
            dataAdapter.SelectCommand.Parameters.AddWithValue("@max", textBox14.Value.ToString("yyyy-MM-dd"));

            #endregion

            try
            {
                conn.Open();
                dataSet.Clear();
                if (dataAdapter.Fill(dataSet, "orders") >= 0)
                    dataGridView1.DataSource = dataSet.Tables["orders"];
                else
                    MessageBox.Show("찾는 데이터가 없습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void CusomerTableSelect_Click()
        {
            string queryStr;

            #region Select QueryString 만들기
            string[] conditions = new string[5];
            conditions[0] = (textBox5.Text != "") ? "custid=@id" : null;
            conditions[1] = (textBox6.Text != "") ? "name=@name" : null;
            conditions[2] = (textBox7.Text != "") ? "address=@address" : null;
            conditions[3] = (textBox8.Text != "") ? "phone=@phone" : null;

            if (conditions[0] != null || conditions[1] != null || conditions[2] != null || conditions[3] != null)
            {
                queryStr = $"SELECT * FROM customer WHERE ";
                bool firstCondition = true;
                for (int i = 0; i < conditions.Length; i++)
                {
                    if (conditions[i] != null)
                        if (firstCondition)
                        {
                            queryStr += conditions[i];
                            firstCondition = false;
                        }
                        else
                        {
                            queryStr += " and " + conditions[i];
                        }
                }
            }
            else
            {
                queryStr = "SELECT * FROM customer";
            }
            #endregion

            #region SelectCommand 객체 생성 및 Parameters 설정
            dataAdapter.SelectCommand = new MySqlCommand(queryStr, conn);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@id", textBox5.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@name", textBox6.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@address", textBox7.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@phone", textBox8.Text);
            #endregion

            try
            {
                conn.Open();
                dataSet.Clear();
                if (dataAdapter.Fill(dataSet, "customer") > 0)
                    dataGridView1.DataSource = dataSet.Tables["customer"];
                else
                    MessageBox.Show("찾는 데이터가 없습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void BookTableSelect_Click()
        {
            string queryStr;

            #region Select QueryString 만들기
            string[] conditions = new string[5];
            conditions[0] = (textBox1.Text != "") ? "bookid=@id" : null;
            conditions[1] = (textBox2.Text != "") ? "bookname=@name" : null;
            conditions[2] = (textBox3.Text != "") ? "publisher=@publisher" : null;
            string condition_price;
            if (textBox4.Text != "" && textBox15.Text != "")
            {
                condition_price = "price>=@min and price<=@max";
            }
            else if (textBox4.Text != "" || textBox15.Text != "")
            {
                if (textBox4.Text != "")
                    condition_price = "price>=min";
                else
                    condition_price = "price <= @max";
            }
            else
            {
                condition_price = null;
            }
            conditions[3] = condition_price;

            if (conditions[0] != null || conditions[1] != null || conditions[2] != null || conditions[3] != null)
            {
                queryStr = $"SELECT * FROM book WHERE ";
                bool firstCondition = true;
                for (int i = 0; i < conditions.Length; i++)
                {
                    if (conditions[i] != null)
                        if (firstCondition)
                        {
                            queryStr += conditions[i];
                            firstCondition = false;
                        }
                        else
                        {
                            queryStr += " and " + conditions[i];
                        }
                }
            }
            else
            {
                queryStr = "SELECT * FROM book";
            }
            #endregion

            #region SelectCommand 객체 생성 및 Parameters 설정
            dataAdapter.SelectCommand = new MySqlCommand(queryStr, conn);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@id", textBox1.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@name", textBox2.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@publisher", textBox3.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@min", textBox4.Text);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@max", textBox15.Text);
            #endregion

            try
            {
                conn.Open();
                dataSet.Clear();
                if (dataAdapter.Fill(dataSet, "book") > 0)
                    dataGridView1.DataSource = dataSet.Tables["book"];
                else
                    MessageBox.Show("찾는 데이터가 없습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region InsertBtn

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (Book.Checked == true)
            {
                BookTableInsert_Click();
            }
            else if (Customer.Checked == true)
                CustomerTableInsert_Click();
            else
                OrdersTableInsert_Click();
        }

        private void OrdersTableInsert_Click()
        {
            string[] rowDatas = {
                textBox10.Text,
                textBox11.Text,
                textBox12.Text,
                textBox13.Value.ToString().Substring(0, 10)};

            string queryStr = "INSERT INTO Orders (custid, bookid, saleprice, orderdate) " +
                "VALUES(@custid,@bookid, @saleprice, @orderdate)";
            dataAdapter.InsertCommand = new MySqlCommand(queryStr, conn);
            dataAdapter.InsertCommand.Parameters.Add("@custid", MySqlDbType.Int32);
            dataAdapter.InsertCommand.Parameters.Add("@bookid", MySqlDbType.Int32);
            dataAdapter.InsertCommand.Parameters.Add("@saleprice", MySqlDbType.Int32);
            dataAdapter.InsertCommand.Parameters.Add("@orderdate", MySqlDbType.Date);

            dataAdapter.InsertCommand.Parameters["@custid"].Value = rowDatas[0];
            dataAdapter.InsertCommand.Parameters["@bookid"].Value = rowDatas[1];
            dataAdapter.InsertCommand.Parameters["@saleprice"].Value = rowDatas[2];
            dataAdapter.InsertCommand.Parameters["@orderdate"].Value = rowDatas[3];

            try
            {
                conn.Open();
                dataAdapter.InsertCommand.ExecuteNonQuery();

                dataSet.Clear();                                        // 이전 데이터 지우기
                dataAdapter.Fill(dataSet, "orders");                      // DB -> DataSet
                dataGridView1.DataSource = dataSet.Tables["orders"];      // dataGridView에 테이블 표시                                     // 텍스트 박스 내용 지우기
                MessageBox.Show("Insert 되었습니다");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

                conn.Close();
            }
        }

        private void CustomerTableInsert_Click()
        {
            string[] rowDatas = {
                textBox6.Text,
                textBox7.Text,
                textBox8.Text };

            string queryStr = "INSERT INTO customer (name, address, phone) " +
                "VALUES(@name,@address, @phone)";
            dataAdapter.InsertCommand = new MySqlCommand(queryStr, conn);
            dataAdapter.InsertCommand.Parameters.Add("@name", MySqlDbType.VarChar);
            dataAdapter.InsertCommand.Parameters.Add("@address", MySqlDbType.VarChar);
            dataAdapter.InsertCommand.Parameters.Add("@phone", MySqlDbType.VarChar);

            dataAdapter.InsertCommand.Parameters["@name"].Value = rowDatas[0];
            dataAdapter.InsertCommand.Parameters["@address"].Value = rowDatas[1];
            dataAdapter.InsertCommand.Parameters["@phone"].Value = rowDatas[2];

            try
            {
                conn.Open();
                dataAdapter.InsertCommand.ExecuteNonQuery();

                dataSet.Clear();                                        // 이전 데이터 지우기
                dataAdapter.Fill(dataSet, "customer");                      // DB -> DataSet
                dataGridView1.DataSource = dataSet.Tables["customer"];      // dataGridView에 테이블 표시
                MessageBox.Show("Insert 되었습니다");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void BookTableInsert_Click()
        {
            string[] rowDatas = {
                textBox2.Text,
                textBox3.Text,
                textBox4.Text };

            string queryStr = "INSERT INTO book (bookname, publisher, price) " +
                "VALUES(@name,@publisher, @price)";
            dataAdapter.InsertCommand = new MySqlCommand(queryStr, conn);
            dataAdapter.InsertCommand.Parameters.Add("@name", MySqlDbType.VarChar);
            dataAdapter.InsertCommand.Parameters.Add("@publisher", MySqlDbType.VarChar);
            dataAdapter.InsertCommand.Parameters.Add("@price", MySqlDbType.Int32);

            dataAdapter.InsertCommand.Parameters["@name"].Value = rowDatas[0];
            dataAdapter.InsertCommand.Parameters["@publisher"].Value = rowDatas[1];
            dataAdapter.InsertCommand.Parameters["@price"].Value = rowDatas[2];

            try
            {
                conn.Open();
                dataAdapter.InsertCommand.ExecuteNonQuery();

                dataSet.Clear();                                        // 이전 데이터 지우기
                dataAdapter.Fill(dataSet, "book");                      // DB -> DataSet
                dataGridView1.DataSource = dataSet.Tables["book"];      // dataGridView에 테이블 표시
                MessageBox.Show("Insert 되었습니다");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        #endregion

        #region UpdateBtn

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Book.Checked == true)
            {
                BookTableUpdate_Click();
            }
            else if (Customer.Checked == true)
                CusomerTableUpdate_Click();
            else
                OrdersTableUpdate_Click();
        }

        private void BookTableUpdate_Click()
        {
            string sql = "UPDATE book SET bookname=@name, publisher=@publisher, price=@price WHERE bookid=@id";
            dataAdapter.UpdateCommand = new MySqlCommand(sql, conn);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@id", textBox1.Text);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@name", textBox2.Text);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@publisher", textBox3.Text);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@price", textBox4.Text);

            try
            {
                conn.Open();
                dataAdapter.UpdateCommand.ExecuteNonQuery();

                dataSet.Clear();  // 이전 데이터 지우기
                dataAdapter.Fill(dataSet, "book");
                dataGridView1.DataSource = dataSet.Tables["book"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void CusomerTableUpdate_Click()
        {
            string sql = "UPDATE customer SET name=@name, address=@address, phone=@phone WHERE custid=@id";
            dataAdapter.UpdateCommand = new MySqlCommand(sql, conn);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@id", textBox5.Text);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@name", textBox6.Text);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@address", textBox7.Text);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@phone", textBox8.Text);

            try
            {
                conn.Open();
                dataAdapter.UpdateCommand.ExecuteNonQuery();

                dataSet.Clear();  // 이전 데이터 지우기
                dataAdapter.Fill(dataSet, "customer");
                dataGridView1.DataSource = dataSet.Tables["customer"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void OrdersTableUpdate_Click()
        {
            string sql = "UPDATE orders SET custid=@custid, bookid=@bookid, saleprice=@saleprice, orderdate=@orderdate WHERE orderid=@id";
            dataAdapter.UpdateCommand = new MySqlCommand(sql, conn);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@id", textBox9.Text);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@custid", textBox10.Text);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@bookid", textBox11.Text);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@saleprice", textBox12.Text);
            dataAdapter.UpdateCommand.Parameters.AddWithValue("@orderdate", textBox13.Value.ToString().Substring(0, 10));

            try
            {
                conn.Open();
                dataAdapter.UpdateCommand.ExecuteNonQuery();

                dataSet.Clear();  // 이전 데이터 지우기
                dataAdapter.Fill(dataSet, "orders");
                dataGridView1.DataSource = dataSet.Tables["orders"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region DeleteBtn

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (Book.Checked == true)
            {
                BookTableDelete_Click();
            }
            else if (Customer.Checked == true)
                CusomerTableDelete_Click();
            else
                OrdersTableDelete_Click();

        }

        private void BookTableDelete_Click()
        {
            string sql = "DELETE FROM book WHERE bookid=@id";
            dataAdapter.DeleteCommand = new MySqlCommand(sql, conn);
            dataAdapter.DeleteCommand.Parameters.AddWithValue("@id", dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString());

            try
            {
                conn.Open();
                dataAdapter.DeleteCommand.ExecuteNonQuery();

                dataSet.Clear();
                dataAdapter.Fill(dataSet, "book");
                dataGridView1.DataSource = dataSet.Tables["book"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void CusomerTableDelete_Click()
        {
            string sql = "DELETE FROM customer WHERE custid=@id";
            dataAdapter.DeleteCommand = new MySqlCommand(sql, conn);
            dataAdapter.DeleteCommand.Parameters.AddWithValue("@id", dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString());

            try
            {
                conn.Open();
                dataAdapter.DeleteCommand.ExecuteNonQuery();

                dataSet.Clear();
                dataAdapter.Fill(dataSet, "customer");
                dataGridView1.DataSource = dataSet.Tables["customer"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void OrdersTableDelete_Click()
        {
            string sql = "DELETE FROM orders WHERE orderid=@id";
            dataAdapter.DeleteCommand = new MySqlCommand(sql, conn);
            dataAdapter.DeleteCommand.Parameters.AddWithValue("@id", dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString());

            try
            {
                conn.Open();
                dataAdapter.DeleteCommand.ExecuteNonQuery();

                dataSet.Clear();
                dataAdapter.Fill(dataSet, "orders");
                dataGridView1.DataSource = dataSet.Tables["orders"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }



        #endregion

        #region ClearBtn
        private void btnClear_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
            textBox10.Clear();
            textBox11.Clear();
            textBox12.Clear();
            textBox13.Value = DateTimePicker.MinimumDateTime;
            textBox14.Value = DateTimePicker.MaximumDateTime;
            textBox15.Clear();
        }

        #endregion

        #region DeleteClick
        private void button1_Click(object sender, EventArgs e)
        {
            if (Book.Checked == true)
            {
                textBox1.Clear();
            }
            else if (Customer.Checked == true)
                textBox5.Clear();
            else
                textBox9.Clear();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (Book.Checked == true)
            {
                textBox2.Clear();
            }
            else if (Customer.Checked == true)
                textBox6.Clear();
            else
                textBox10.Clear();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (Book.Checked == true)
            {
                textBox3.Clear();
            }
            else if (Customer.Checked == true)
                textBox7.Clear();
            else
                textBox11.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Book.Checked == true)
            {
                textBox4.Clear();
            }
            else if (Customer.Checked == true)
                textBox8.Clear();
            else
                textBox12.Clear();
        }
        #endregion
    }
}
