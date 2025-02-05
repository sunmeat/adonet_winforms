using Microsoft.Data.SqlClient;
using System.Reflection;

namespace ProductsWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // включение двойной буферизации для формы
            EnableListViewDoubleBuffering(listView1); // включение двойной буферизации для ListView
            LoadProducts();
            ListViewSettings();
        }

        // строка подключения к базе данных
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

                    // запрос для извлечения данных из таблицы продуктов
                    string query = "SELECT id, name, price, quantity, picture_path FROM Product";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // очищаем ListView перед загрузкой новых данных
                            listView1.Items.Clear();
                            listView1.View = View.Details; // задаем отображение данных в виде таблицы

                            // добавляем колонки
                            listView1.Columns.Clear();
                            listView1.Columns.Add("ID", 50, HorizontalAlignment.Left);
                            listView1.Columns.Add("Name", 200, HorizontalAlignment.Left);
                            listView1.Columns.Add("Price", 100, HorizontalAlignment.Right);
                            listView1.Columns.Add("Quantity", 100, HorizontalAlignment.Right);
                            listView1.Columns.Add("Picture", 200, HorizontalAlignment.Left); // столбец для пути к картинке

                            // добавляем строки в ListView
                            while (reader.Read())
                            {
                                // проверяем на null перед получением значений
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
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        // обработчик выбора элемента из ListView
        private void listView1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // проверяем, что выбран элемент в списке
            if (listView1.SelectedItems.Count > 0)
            {
                // получаем данные выбранного элемента
                var selectedItem = listView1.SelectedItems[0];
                var productName = selectedItem.SubItems[1].Text;
                var productPrice = selectedItem.SubItems[2].Text;
                var productQuantity = selectedItem.SubItems[3].Text;
                var picturePath = selectedItem.SubItems[4].Text;

                // обновляем информацию в "карточке" :)
                label1.Text = "Название: " + productName;
                label2.Text = "Цена: " + productPrice;
                label3.Text = "Количество: " + productQuantity;

                // загрузка изображения в PictureBox
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

        // для списка двойная буферизация включается только через рефлексию
        private void EnableListViewDoubleBuffering(ListView listView)
        {
            typeof(ListView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, listView, [true]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // несколько пунктов списка выбрать нельзя, так настроено в примере, поэтому берём первый выбранный [0]
                var selectedItem = listView1.SelectedItems[0];

                // получаем данные из подэлементов
                var productName = selectedItem.SubItems[1].Text;
                var productPrice = selectedItem.SubItems[2].Text;
                var productQuantity = selectedItem.SubItems[3].Text;

                // отображаем сообщение с информацией о выбранном продукте
                MessageBox.Show($"Вы купили: {productName}\nЦена: {productPrice}\nКоличество: {productQuantity}");
            }
            else
            {
                MessageBox.Show("Пожалуйста, сначала выберите продукт из списка.");
            }
        }
    }
}