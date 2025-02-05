using Microsoft.Data.SqlClient; // пакет нужно ставить через нугет!
using System.Reflection;

namespace ProductsWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // âêëþ÷åíèå äâîéíîé áóôåðèçàöèè äëÿ ôîðìû
            EnableListViewDoubleBuffering(listView1); // âêëþ÷åíèå äâîéíîé áóôåðèçàöèè äëÿ ListView
            LoadProducts();
            ListViewSettings();
        }

        // ñòðîêà ïîäêëþ÷åíèÿ ê áàçå äàííûõ
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

                    // çàïðîñ äëÿ èçâëå÷åíèÿ äàííûõ èç òàáëèöû ïðîäóêòîâ
                    string query = "SELECT id, name, price, quantity, picture_path FROM Product";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // î÷èùàåì ListView ïåðåä çàãðóçêîé íîâûõ äàííûõ
                            listView1.Items.Clear();
                            listView1.View = View.Details; // çàäàåì îòîáðàæåíèå äàííûõ â âèäå òàáëèöû

                            // äîáàâëÿåì êîëîíêè
                            listView1.Columns.Clear();
                            listView1.Columns.Add("ID", 50, HorizontalAlignment.Left);
                            listView1.Columns.Add("Name", 200, HorizontalAlignment.Left);
                            listView1.Columns.Add("Price", 100, HorizontalAlignment.Right);
                            listView1.Columns.Add("Quantity", 100, HorizontalAlignment.Right);
                            listView1.Columns.Add("Picture", 200, HorizontalAlignment.Left); // ñòîëáåö äëÿ ïóòè ê êàðòèíêå

                            // äîáàâëÿåì ñòðîêè â ListView
                            while (reader.Read())
                            {
                                // ïðîâåðÿåì íà null ïåðåä ïîëó÷åíèåì çíà÷åíèé
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
                MessageBox.Show("Îøèáêà ïðè çàãðóçêå äàííûõ: " + ex.Message);
            }
        }

        // îáðàáîò÷èê âûáîðà ýëåìåíòà èç ListView
        private void listView1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // ïðîâåðÿåì, ÷òî âûáðàí ýëåìåíò â ñïèñêå
            if (listView1.SelectedItems.Count > 0)
            {
                // ïîëó÷àåì äàííûå âûáðàííîãî ýëåìåíòà
                var selectedItem = listView1.SelectedItems[0];
                var productName = selectedItem.SubItems[1].Text;
                var productPrice = selectedItem.SubItems[2].Text;
                var productQuantity = selectedItem.SubItems[3].Text;
                var picturePath = selectedItem.SubItems[4].Text;

                // îáíîâëÿåì èíôîðìàöèþ â "êàðòî÷êå" :)
                label1.Text = "Íàçâàíèå: " + productName;
                label2.Text = "Öåíà: " + productPrice;
                label3.Text = "Êîëè÷åñòâî: " + productQuantity;

                // çàãðóçêà èçîáðàæåíèÿ â PictureBox
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

        // äëÿ ñïèñêà äâîéíàÿ áóôåðèçàöèÿ âêëþ÷àåòñÿ òîëüêî ÷åðåç ðåôëåêñèþ
        private void EnableListViewDoubleBuffering(ListView listView)
        {
            typeof(ListView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, listView, [true]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // íåñêîëüêî ïóíêòîâ ñïèñêà âûáðàòü íåëüçÿ, òàê íàñòðîåíî â ïðèìåðå, ïîýòîìó áåð¸ì ïåðâûé âûáðàííûé [0]
                var selectedItem = listView1.SelectedItems[0];

                // ïîëó÷àåì äàííûå èç ïîäýëåìåíòîâ
                var productName = selectedItem.SubItems[1].Text;
                var productPrice = selectedItem.SubItems[2].Text;
                var productQuantity = selectedItem.SubItems[3].Text;

                // îòîáðàæàåì ñîîáùåíèå ñ èíôîðìàöèåé î âûáðàííîì ïðîäóêòå
                MessageBox.Show($"Âû êóïèëè: {productName}\nÖåíà: {productPrice}\nÊîëè÷åñòâî: {productQuantity}");
            }
            else
            {
                MessageBox.Show("Ïîæàëóéñòà, ñíà÷àëà âûáåðèòå ïðîäóêò èç ñïèñêà.");
            }
        }
    }
}
