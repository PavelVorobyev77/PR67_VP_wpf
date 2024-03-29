﻿using PR67_VP.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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


namespace PR67_VP.Pages
{
    public partial class Workers_Page : Page
    {
        public ObservableCollection<Workers> Workers { get; set; }
        public Workers_Page()
        {
            InitializeComponent();
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            using (var context = new Entities1())
            {
                Workers = new ObservableCollection<Workers>(context.Workers.ToList());
            }
        }

        private void txtSearch_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();

            ICollectionView view = CollectionViewSource.GetDefaultView(LViewProduct.ItemsSource);

            if (view != null)
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    view.Filter = null; //Очистка фильтра, если поле поиска пустое
                }
                else
                {
                    //Устанавливается фильтр для поиска по нескольким полям объекта Workers.
                     
                    view.Filter = item =>
                    {
                        Workers dataItem = item as Workers;

                        if (dataItem != null)
                        {
                            string itemPatronymic = "";
                            string itemName = dataItem.WorkerSurname.ToLower();
                            string itemSurname = dataItem.WorkerName.ToLower();
                            if(dataItem.WorkerPatronymic != null){
                                itemPatronymic = dataItem.WorkerPatronymic.ToLower();
                            }
                            string itemWLogin = dataItem.w_login.ToLower();

                            /*
                             * Поиск по нескольким полям: фамилии, имени, отчеству и логину.
                             */
                            return itemName.Contains(searchText) ||
                                   itemSurname.Contains(searchText) ||
                                   itemPatronymic.Contains(searchText) ||
                                   itemWLogin.Contains(searchText); ;
                        }

                        return false;
                    };
                }
            }
        }

        private void ApplySorting()
        {
            /*
            * Применение сортировки к списку Workers по выбранному параметру и направлению.
            */
            ICollectionView view = CollectionViewSource.GetDefaultView(Workers);

            if (view != null)
            {
                ComboBoxItem selectedSortItem = cmbSorting.SelectedItem as ComboBoxItem;

                if (selectedSortItem != null && selectedSortItem.Tag != null)
                {
                    string[] sortParams = selectedSortItem.Tag.ToString().Split(',');
                    string sortProperty = sortParams[0];
                    ListSortDirection sortDirection = (ListSortDirection)Enum.Parse(typeof(ListSortDirection), sortParams[1]);

                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription(sortProperty, sortDirection));
                }
            }
        }
        private void cmbSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySorting();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddWorker());
        }

        /*
         * Обработчик двойного щелчка по элементу списка Workers.
         * Открывает окно редактирования выбранного работника.
         */
        private void LViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (LViewProduct.SelectedItem != null)
            {
                using (Entities1 db = new Entities1())
                {
                    int id = (LViewProduct.SelectedItem as Workers).ID_Worker;
                    Workers worker = db.Workers.FirstOrDefault(w => w.ID_Worker == id);
                    EditWorkers editWindow = new EditWorkers(worker);
                    editWindow.DataContext = LViewProduct.SelectedItem;
                    bool? result = editWindow.ShowDialog();

                    if (result == true)
                    {
                        LoadData();
                    }
                }
            }
        }
        private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            Workers_Page newWorkerPage = new Workers_Page();
            NavigationService.Navigate(newWorkerPage);
        }

        private void btnPrintList_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                IDocumentPaginatorSource idp = doc;
                pd.PrintDocument(idp.DocumentPaginator, Title);
            } 
        }
    }
}

