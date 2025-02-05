using Microsoft.Data.SqlClient;
using System.Reflection;

namespace ProductsWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // ��������� ������� ����������� ��� �����
            EnableListViewDoubleBuffering(listView1); // ��������� ������� ����������� ��� ListView
            LoadProducts();
            ListViewSettings();
        }

        // ������ ����������� � ���� ������
        string connectionString = "Server=localhost; Database=Store; Integrated Security=True; TrustServerCertificate=True; MultipleActiveResultSets=True;";

        private void ListViewSettings()
        {
            listView1.FullRowSelect = true;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
        }

        private void LoadProducts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // ������ ��� ���������� ������ �� ������� ���������
                    string query = "SELECT id, name, price, quantity, picture_path FROM Product";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // ������� ListView ����� ��������� ����� ������
                            listView1.Items.Clear();
                            listView1.View = View.Details; // ������ ����������� ������ � ���� �������

                            // ��������� �������
                            listView1.Columns.Clear();
                            listView1.Columns.Add("ID", 50, HorizontalAlignment.Left);
                            listView1.Columns.Add("Name", 200, HorizontalAlignment.Left);
                            listView1.Columns.Add("Price", 100, HorizontalAlignment.Right);
                            listView1.Columns.Add("Quantity", 100, HorizontalAlignment.Right);
                            listView1.Columns.Add("Picture", 200, HorizontalAlignment.Left); // ������� ��� ���� � ��������

                            // ��������� ������ � ListView
                            while (reader.Read())
                            {
                                // ��������� �� null ����� ���������� ��������
                                var id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                var name = reader.IsDBNull(1) ? "Unknown" : reader.GetString(1);
                                var price = reader.IsDBNull(2) ? 0.0 : reader.GetDouble(2);
                                var quantity = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                                var picturePath = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);

                                var listViewItem = new ListViewItem(id.ToString());
                                listViewItem.SubItems.Add(name);
                                listViewItem.SubItems.Add(price.ToString("C")); // currency
                                listViewItem.SubItems.Add(quantity.ToString());
                                listViewItem.SubItems.Add(picturePath);

                                listView1.Items.Add(listViewItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ ��� �������� ������: " + ex.Message);
            }
        }

        // ���������� ������ �������� �� ListView
        private void listView1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // ���������, ��� ������ ������� � ������
            if (listView1.SelectedItems.Count > 0)
            {
                // �������� ������ ���������� ��������
                var selectedItem = listView1.SelectedItems[0];
                var productName = selectedItem.SubItems[1].Text;
                var productPrice = selectedItem.SubItems[2].Text;
                var productQuantity = selectedItem.SubItems[3].Text;
                var picturePath = selectedItem.SubItems[4].Text;

                // ��������� ���������� � "��������" :)
                label1.Text = "��������: " + productName;
                label2.Text = "����: " + productPrice;
                label3.Text = "����������: " + productQuantity;

                // �������� ����������� � PictureBox
                if (!string.IsNullOrEmpty(picturePath) && File.Exists(picturePath))
                {
                    pictureBox1.Image = Image.FromFile(picturePath);
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
        }

        // ��� ������ ������� ����������� ���������� ������ ����� ���������
        private void EnableListViewDoubleBuffering(ListView listView)
        {
            typeof(ListView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, listView, [true]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // ��������� ������� ������ ������� ������, ��� ��������� � �������, ������� ���� ������ ��������� [0]
                var selectedItem = listView1.SelectedItems[0];

                // �������� ������ �� ������������
                var productName = selectedItem.SubItems[1].Text;
                var productPrice = selectedItem.SubItems[2].Text;
                var productQuantity = selectedItem.SubItems[3].Text;

                // ���������� ��������� � ����������� � ��������� ��������
                MessageBox.Show($"�� ������: {productName}\n����: {productPrice}\n����������: {productQuantity}");
            }
            else
            {
                MessageBox.Show("����������, ������� �������� ������� �� ������.");
            }
        }
    }
}