using OfficeOpenXml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Core;

namespace HelpDesk
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Base_TitanEntities db;

        public MainWindow()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            db = new Base_TitanEntities();
            BidDataGrid.ItemsSource = db.Bids.ToList();
            LoadBidDataGrid();
            LoadComboBoxData();
        }

        private void LoadBidDataGrid(string bidIDFilter = null, string bidFIOClientFilter = null, string statusFilter = null)
        {
            var bidQuery = from b in db.Bids
                           join c in db.Clients on b.Client_ID equals c.Client_ID
                           join s in db.Staffs on b.Staff_ID equals s.Staff_ID
                           join st in db.Statuses on b.Status_ID equals st.Status_ID
                           select new
                           {
                               b.Bid_ID,
                               b.Purchase_ID,
                               ClientName = c.Surname_Client + " " + c.Name_Client + " " + c.Patronymic_Client,
                               StaffName = s.Surname_Staff + " " + s.Name_Staff + " " + s.Patronymic_Staff,
                               b.Address,
                               Status = st.Title_Status
                           };

            // Фильтрация по ФИО клиента
            if (!string.IsNullOrEmpty(bidFIOClientFilter))
            {
                bidQuery = bidQuery.Where(bid => (bid.ClientName.Contains(bidFIOClientFilter)));
            }

            // Фильтрация по статусу заявки
            if (!string.IsNullOrEmpty(statusFilter))
            {
                bidQuery = bidQuery.Where(bid => bid.Status.Contains(statusFilter));
            }

            BidDataGrid.ItemsSource = bidQuery.ToList();
        }

        // Событие для фильтрации по текстовому полю (ФИО клиента)
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string fioFilter = tbFiltration.Text;
            string statusFilter = (cbFiltration.SelectedItem as ComboBoxItem)?.Content.ToString();
            LoadBidDataGrid(null, fioFilter, statusFilter);
        }

        // Событие для фильтрации по статусу заявки (ComboBox)
        private void cbFiltration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string statusFilter = (cbFiltration.SelectedItem as ComboBoxItem)?.Content.ToString();
            string fioFilter = tbFiltration.Text;
            LoadBidDataGrid(null, fioFilter, statusFilter);
        }

        // Кнопка для очистки фильтров
        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            tbFiltration.Clear();  // Очистка TextBox
            cbFiltration.SelectedIndex = -1;  // Сброс ComboBox в начальное состояние
            LoadBidDataGrid();  // Перезагрузка данных без фильтров
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

        private void ExportToBidPdf(string filePath, string fioFilter = null, string statusFilter = null)
        {
            // Создаем запрос с фильтрами
            var query = from b in db.Bids
                        join c in db.Clients on b.Client_ID equals c.Client_ID
                        join s in db.Staffs on b.Staff_ID equals s.Staff_ID
                        join st in db.Statuses on b.Status_ID equals st.Status_ID
                        join p in db.Purchases on b.Purchase_ID equals p.Purchase_ID
                        select new
                        {
                            ClientName = c.Surname_Client + " " + c.Name_Client + " " + c.Patronymic_Client,
                            ClientPhone = c.Contact_Details,
                            b.Address,
                            Status = st.Title_Status,
                            StaffName = s.Surname_Staff + " " + s.Name_Staff + " " + s.Patronymic_Staff,
                            p.Amount
                        };

            // Применяем фильтрацию по ФИО клиента
            if (!string.IsNullOrEmpty(fioFilter))
            {
                query = query.Where(bid => (bid.ClientName.Contains(fioFilter)));
            }

            // Применяем фильтрацию по статусу заявки
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(bid => bid.Status.Contains(statusFilter));
            }

            var bids = query.ToList();

            // Создаем PDF документ
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Отчет по заявкам";

            // Создаем страницу с альбомной ориентацией
            PdfPage page = document.AddPage();
            page.Orientation = PageOrientation.Landscape;  // Альбомная ориентация
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Шрифт для текста (уменьшенный размер шрифта)
            XFont font = new XFont("Verdana", 10); // уменьшен размер шрифта
            XFont headerFont = new XFont("Verdana", 12); // Заголовки с жирным шрифтом

            // Заголовок
            gfx.DrawString("Отчет по заявкам", headerFont, XBrushes.Black, new XPoint(300, 40));

            // Ширина столбцов и начальная позиция
            int column1X = 40;
            int column2X = 190;
            int column3X = 300;
            int column4X = 500;
            int column5X = 600;

            // Заголовки столбцов (ширина столбцов подгоняется по альбомной ориентации)
            gfx.DrawString("ФИО клиента", font, XBrushes.Black, new XPoint(column1X, 80));
            gfx.DrawString("Телефон", font, XBrushes.Black, new XPoint(column2X, 80));
            gfx.DrawString("Адрес заявки", font, XBrushes.Black, new XPoint(column3X, 80));
            gfx.DrawString("Статус заявки", font, XBrushes.Black, new XPoint(column4X, 80));
            gfx.DrawString("ФИО сотрудника", font, XBrushes.Black, new XPoint(column5X, 80));

            // Заполнение таблицы данными
            int yPosition = 100;
            foreach (var bid in bids)
            {
                gfx.DrawString(bid.ClientName, font, XBrushes.Black, new XPoint(column1X, yPosition));
                gfx.DrawString(bid.ClientPhone, font, XBrushes.Black, new XPoint(column2X, yPosition));
                gfx.DrawString(bid.Address, font, XBrushes.Black, new XPoint(column3X, yPosition));
                gfx.DrawString(bid.Status, font, XBrushes.Black, new XPoint(column4X, yPosition));
                gfx.DrawString(bid.StaffName, font, XBrushes.Black, new XPoint(column5X, yPosition));

                yPosition += 15; // уменьшен отступ между строками
            }

            // Сохраняем PDF файл
            document.Save(filePath);
        }

        // Обработчик для кнопки "Отчет по заявкам PDF"
        private void Button_BidPDF_Click(object sender, RoutedEventArgs e)
        {
            // Открываем диалоговое окно для сохранения PDF файла
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Отчет по заявкам.pdf",
                Filter = "PDF files (*.pdf)|*.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // Получаем текущие фильтры
                string fioFilter = tbFiltration.Text; // текст в TextBox
                string statusFilter = (cbFiltration.SelectedItem as ComboBoxItem)?.Content.ToString(); // выбранный элемент в ComboBox

                // Экспортируем данные в выбранный файл PDF
                ExportToBidPdf(saveFileDialog.FileName, fioFilter, statusFilter);
                MessageBox.Show("Отчет успешно экспортирован в PDF");
            }
        }

        private void ExportToClientPdf(string filePath, string fioFilter = null, string statusFilter = null)
        {
            // Создаем запрос с фильтрами
            var query = from c in db.Clients
                        select new
                        {
                            ClientName = c.Surname_Client + " " + c.Name_Client + " " + c.Patronymic_Client,
                            ContactDetails = c.Contact_Details,
                        };

            // Применяем фильтрацию по ФИО клиента
            if (!string.IsNullOrEmpty(fioFilter))
            {
                query = query.Where(bid => (bid.ClientName.Contains(fioFilter)));
            }

            var clients = query.ToList();

            // Создаем PDF документ
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Отчет по клиентам";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Шрифт для текста (уменьшенный размер шрифта)
            XFont font = new XFont("Verdana", 10); // уменьшен размер шрифта
            XFont headerFont = new XFont("Verdana", 12); // Заголовки с жирным шрифтом

            // Заголовок
            gfx.DrawString("Отчет по клиентам", headerFont, XBrushes.Black, new XPoint(250, 40));

            // Ширина столбцов и начальная позиция
            int column1X = 40;
            int column2X = 200;

            // Заголовки столбцов (ширина столбцов подгоняется по альбомной ориентации)
            gfx.DrawString("ФИО клиента", font, XBrushes.Black, new XPoint(column1X, 80));
            gfx.DrawString("Контактные данные", font, XBrushes.Black, new XPoint(column2X, 80));

            // Заполнение таблицы данными
            int yPosition = 100;
            foreach (var client in clients)
            {
                gfx.DrawString(client.ClientName, font, XBrushes.Black, new XPoint(column1X, yPosition));
                gfx.DrawString(client.ContactDetails, font, XBrushes.Black, new XPoint(column2X, yPosition));

                yPosition += 15; // уменьшен отступ между строками
            }

            // Сохраняем PDF файл
            document.Save(filePath);
        }
        
        // Обработчик для кнопки "Отчет по клиентам PDF"
        private void Button_ClientPDF_Click(object sender, RoutedEventArgs e)
        {
            // Открываем диалоговое окно для сохранения PDF файла
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Отчет по клиентам.pdf",
                Filter = "PDF files (*.pdf)|*.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // Получаем текущие фильтры
                string fioFilter = tbFiltration.Text; // текст в TextBox
                string statusFilter = (cbFiltration.SelectedItem as ComboBoxItem)?.Content.ToString(); // выбранный элемент в ComboBox

                // Экспортируем данные в выбранный файл PDF
                ExportToClientPdf(saveFileDialog.FileName, fioFilter, statusFilter);
                MessageBox.Show("Отчет успешно экспортирован в PDF");
            }
        }

        private void RefreshDatagrid()
        {
            // Запрос для получения данных с ФИО клиентов и сотрудников
            var bidQuery = from b in db.Bids
                           join c in db.Clients on b.Client_ID equals c.Client_ID
                           join s in db.Staffs on b.Staff_ID equals s.Staff_ID
                           join st in db.Statuses on b.Status_ID equals st.Status_ID
                           select new
                           {
                               b.Bid_ID,
                               b.Purchase_ID,
                               ClientName = c.Surname_Client + " " + c.Name_Client + " " + c.Patronymic_Client,
                               StaffName = s.Surname_Staff + " " + s.Name_Staff + " " + s.Patronymic_Staff,
                               b.Address,
                               Status = st.Title_Status,
                           };

            BidDataGrid.ItemsSource = bidQuery.ToList();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            // Получаем значения ID клиента, сотрудника и закупки из текстовых полей
            int clientId = Convert.ToInt32(tbClientId.Text);
            int staffId = Convert.ToInt32(tbStaffId.Text);
            int purchaseId = Convert.ToInt32(tbPurchaseId.Text); // Новый ID для закупки
            int statusId = Convert.ToInt32(tbStatusId.Text); // Новый ID для статуса

            // Получаем данные клиента по Client_ID
            var client = db.Clients.FirstOrDefault(c => c.Client_ID == clientId);

            // Получаем данные сотрудника по Staff_ID
            var staff = db.Staffs.FirstOrDefault(s => s.Staff_ID == staffId);

            // Получаем данные закупки по Purchase_ID
            var purchase = db.Purchases.FirstOrDefault(p => p.Purchase_ID == purchaseId);

            // Создаем новую заявку
            Bids bid = new Bids
            {
                Client_ID = clientId,
                Staff_ID = staffId,
                Purchase_ID = purchaseId,
                Status_ID = statusId,
                Address = tbAddress.Text // Получаем адрес из текстового поля
            };

            // Добавляем заявку в базу данных
            db.Bids.Add(bid);
            db.SaveChanges();

            // Обновляем DataGrid
            RefreshDatagrid();
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, является ли введенное значение числом
            if (int.TryParse(tbBidId.Text, out int selectId))
            {
                // Поиск заявки по ID
                var selectDeleteBid = db.Bids.Where(b => b.Bid_ID == selectId).FirstOrDefault();

                // Если заявка найдена, удалить её
                if (selectDeleteBid != null)
                {
                    db.Bids.Remove(selectDeleteBid);
                    db.SaveChanges();
                    RefreshDatagrid();
                }
                else
                {
                    MessageBox.Show("Заявка с таким ID не найдена");
                }
            }
            else
            {
                MessageBox.Show("Введен некорректный ID!");
            }
        }

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            int selectId = Convert.ToInt32(tbBidId.Text);
            var selectUpdateOrder = db.Bids.Where(b => b.Bid_ID == selectId).FirstOrDefault();

            selectUpdateOrder.Bid_ID = Convert.ToInt32(tbBidId.Text);
            selectUpdateOrder.Client_ID = Convert.ToInt32(tbClientId.Text);
            selectUpdateOrder.Staff_ID = Convert.ToInt32(tbStaffId.Text);
            selectUpdateOrder.Status_ID = Convert.ToInt32(tbStaffId.Text);
            selectUpdateOrder.Address = tbAddress.Text; 

            db.SaveChanges();
            RefreshDatagrid();
        }
    }
}