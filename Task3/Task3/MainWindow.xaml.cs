using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Task3
{
    class Core 
    {
        public static ekolesnikovEntities DB = new ekolesnikovEntities();
    }

    public partial class Product
    {
        public Uri ImagePreview
        {
            get
            {
                var imageName = System.IO.Path.Combine(Environment.CurrentDirectory, Image ?? "");
                return System.IO.File.Exists(imageName) ? new Uri(imageName) : new Uri("pack://application:,,,/Images/picture.png");
            }
        }

        public string MaterialsList
        {
            get
            {
                var Result = "";
                foreach (var pm in ProductMaterial)
                {
                    Result += (Result == "" ? "" : ", ") + pm.Material.Title;
                }
                return Result;
            }
        }

        public decimal TotalPrice
        {
            get
            {
                decimal Result = 0;
                foreach (var pm in ProductMaterial)
                    Result += pm.Material.Cost;
                return Result;
            }
        }

        public string BackgroundColor 
        {
            get 
            {
                if(ProductSale.Count==0)
                    return "#faa";
                var LastSale = ProductSale.Max(ps => ps.SaleDate);
                var TimeDelta = DateTime.Now - LastSale;
                if(TimeDelta.TotalDays>30)
                    return "#faa";
                return "#fff";
            }
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private IEnumerable<Product> _ProductList;

        private int SortType = 0;
        public string[] SortList { get; set; } = {
            "Без сортировки",
            "название по убыванию",
            "название по возрастанию",
            "номер цеха по убыванию",
            "номер цеха по возрастанию",
            "цена по убыванию",
            "цена по возрастанию" };

        private int _CurrentPage = 1;       

        public event PropertyChangedEventHandler PropertyChanged;

        public int CurrentPage
        {
            get
            {
                return _CurrentPage;
            }
            set
            {
                if (value > 0)
                {
                    if (value <= _ProductList.Count() / 20)
                    {
                        _CurrentPage = value;
                        Invalidate();
                    }
                }
            }
        }

        private void Invalidate()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProductList"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPage"));
        }

        public IEnumerable<Product> ProductList
        {
            get
            {
                var Result = _ProductList;

                switch (SortType)
                {
                    case 1:
                        Result = Result.OrderBy(p => p.Title);
                        break;
                    case 2:
                        Result = Result.OrderByDescending(p => p.Title);
                        break;
                }

                return Result.Skip((CurrentPage-1)*20).Take(20);
            }
            set
            {
                _ProductList = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ProductList = Core.DB.Product.ToArray();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage--;
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage++;
        }

        private void SortTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortType = SortTypeComboBox.SelectedIndex;
            Invalidate();
        }
    }
}
