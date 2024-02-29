using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PR67_VP.Pages
{
    /// <summary>
    /// Логика взаимодействия для ConfirmWindow.xaml
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        private readonly string expectedCode;
        public ConfirmWindow(string expectedCode)
        {
            InitializeComponent();
            this.expectedCode = expectedCode;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredCode = CodeTextBox.Text.Trim();

            if (enteredCode == expectedCode)
            {
                DialogResult = true;
                MessageBox.Show("Код подтверждения верный.");
                Close();
            }
            else
            {
                MessageBox.Show("Введенный код неверный. Пожалуйста, попробуйте еще раз.");
            }
        }
    }
}
