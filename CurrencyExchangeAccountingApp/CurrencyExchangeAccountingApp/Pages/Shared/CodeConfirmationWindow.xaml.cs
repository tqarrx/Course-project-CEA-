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

namespace CurrencyExchangeAccountingApp
{
    /// <summary>
    /// Логика взаимодействия для CodeConfirmationWindow.xaml
    /// </summary>
    public partial class CodeConfirmationWindow : Window
    {
        public CodeConfirmationWindow(string code)
        {
            InitializeComponent();
            // Установка полученного кода в текстовый блок
            CodeTextBlock.Text = code;
        }
    }
}
