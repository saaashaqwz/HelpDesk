using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HelpDesk
{
    /// <summary>
    /// Логика взаимодействия для OutputOfTablesWindow.xaml
    /// </summary>
    public partial class OutputOfTablesWindow : Window
    {
        Base_TitanEntities db;

        public OutputOfTablesWindow()
        {
            db = new Base_TitanEntities();
            InitializeComponent();
            LoadComboBoxData();
        }

        // Загрузка данных в ComboBox
        private void LoadComboBoxData()
        {
            // Добавляем названия таблиц в ComboBox
            comboBoxTables.Items.Add("Clients");
            comboBoxTables.Items.Add("Posts");
            comboBoxTables.Items.Add("Staffs");
            comboBoxTables.Items.Add("Products");
            comboBoxTables.Items.Add("Services");
            comboBoxTables.Items.Add("Purchases");
            comboBoxTables.Items.Add("Bids");
            comboBoxTables.Items.Add("Roles");
            comboBoxTables.Items.Add("Users");
            comboBoxTables.Items.Add("Statuses");

            comboBoxTables.SelectedIndex = 0; // Устанавливаем первый элемент выбранным
        }

        // Обработчик изменения выбора в ComboBox
        private void comboBoxTables_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxTables.SelectedItem != null)
            {
                string selectedTable = comboBoxTables.SelectedItem.ToString();
                LoadData(selectedTable);
            }
        }

        // Загрузка данных в DataGrid в зависимости от выбранной таблицы
        private void LoadData(string selectedTable)
        {
            db.Configuration.ProxyCreationEnabled = false;

            switch (selectedTable)
            {
                case "Clients":
                    DataGridView.ItemsSource = db.Clients
                        .Select(c => new
                        {
                            c.Client_ID,
                            c.Surname_Client,
                            c.Name_Client,
                            c.Patronymic_Client,
                            c.Contact_Details
                        }).ToList();
                    break;

                case "Posts":
                    DataGridView.ItemsSource = db.Posts
                        .Select(p => new
                        {
                            p.Post_ID,
                            p.Title,
                            p.Salary
                        }).ToList();
                    break;

                case "Staffs":
                    DataGridView.ItemsSource = db.Staffs
                        .Select(s => new
                        {
                            s.Staff_ID,
                            s.Post_ID,
                            s.Surname_Staff,
                            s.Name_Staff,
                            s.Patronymic_Staff
                        }).ToList();
                    break;

                case "Products":
                    DataGridView.ItemsSource = db.Products
                        .Select(p => new
                        {
                            p.Product_ID,
                            p.Title_Product,
                            p.Price_Product
                        }).ToList();
                    break;

                case "Services":
                    DataGridView.ItemsSource = db.Services
                        .Select(s => new
                        {
                            s.Service_ID,
                            s.Title_Service,
                            s.Tarif,
                            s.Price_Service
                        }).ToList();
                    break;

                case "Purchases":
                    DataGridView.ItemsSource = db.Purchases
                        .Select(p => new
                        {
                            p.Purchase_ID,
                            p.Service_ID,
                            p.Product_ID,
                            p.Amount
                        }).ToList();
                    break;

                case "Bids":
                    DataGridView.ItemsSource = db.Bids
                        .Select(b => new
                        {
                            b.Bid_ID,
                            b.Client_ID,
                            b.Staff_ID,
                            b.Purchase_ID,
                            b.Status_ID,
                            b.Address
                        }).ToList();
                    break;

                case "Roles":
                    DataGridView.ItemsSource = db.Roles
                        .Select(r => new
                        {
                            r.Role_ID,
                            r.Title_Role
                        }).ToList();
                    break;

                case "Users":
                    DataGridView.ItemsSource = db.Users
                        .Select(u => new
                        {
                            u.User_ID,
                            u.Role_ID,
                            u.Login,
                            u.Password,
                            u.Salt
                        }).ToList();
                    break;

                case "Statuses":
                    DataGridView.ItemsSource = db.Statuses
                        .Select(s => new
                        {
                            s.Status_ID,
                            s.Title_Status
                        }).ToList();
                    break;

                default:
                    MessageBox.Show("Выберите таблицу");
                    break;
            }
        }
    }
}
