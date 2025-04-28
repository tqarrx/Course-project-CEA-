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
using System.Windows.Navigation; 
using System.Windows.Shapes; 
using System.Windows.Threading; 
using CurrencyExchangeAccountingApp.Data.Models; 
using CurrencyExchangeAccountingApp.Pages.Operator;
using CurrencyExchangeAccountingApp.Pages.Senior;
using System.Data.Entity;

namespace CurrencyExchangeAccountingApp
{
    public partial class MainWindow : Window
    {
        private string _generatedCode; // Хранит сгенерированный код подтверждения
        private DateTime _codeExpirationTime; // Время истечения срока действия кода
        private DispatcherTimer _codeTimer; // Таймер для отслеживания времени действия кода
        private Users _currentUser; // Текущий пользователь, который пытается войти

        public MainWindow()
        {
            InitializeComponent(); // Инициализация компонентов окна

            // Настройка таймера для отслеживания времени действия кода
            _codeTimer = new DispatcherTimer();
            _codeTimer.Interval = TimeSpan.FromSeconds(1); // Интервал таймера - 1 секунда
            _codeTimer.Tick += CodeTimer_Tick; // Подписка на событие тика таймера

            // Блокировка элементов интерфейса при запуске программы
            PasswordBox.IsEnabled = false; // Поле пароля недоступно
            LoginButton.IsEnabled = false; // Кнопка "Вход" недоступна
            CodeTextBox.IsEnabled = false; // Поле ввода кода недоступно
            ResendButton.Visibility = Visibility.Collapsed; // Кнопка "Отправить повторно" скрыта

            // Подписка на события нажатия клавиш для полей ввода
            LoginTextBox.KeyDown += LoginTextBox_KeyDown; // Обработка нажатия клавиш в поле логина
            PasswordBox.KeyDown += PasswordBox_KeyDown; // Обработка нажатия клавиш в поле пароля
            CodeTextBox.KeyDown += CodeTextBox_KeyDown; // Обработка нажатия клавиш в поле кода
            LoginButton.Click += LoginButton_Click; // Обработка клика по кнопке "Вход"
        }

        // Обработчик нажатия клавиш в поле логина
        private void LoginTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Проверка, была ли нажата клавиша Enter
            {
                string login = LoginTextBox.Text; // Получение текста из поля логина
                if (string.IsNullOrWhiteSpace(login)) // Проверка на пустое значение
                {
                    MessageBox.Show("Введите логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; // Если логин пуст, выводим сообщение об ошибке
                }
                // Проверка существования пользователя в базе данных
                _currentUser = CheckUserLogin(login);
                if (_currentUser != null) // Если пользователь найден
                {
                    PasswordBox.IsEnabled = true; // Разблокируем поле пароля
                    LoginButton.IsEnabled = true; // Разблокируем кнопку "Вход"
                    PasswordBox.Focus(); // Устанавливаем фокус на поле пароля
                }
                else
                {
                    MessageBox.Show("Пользователь с таким логином не найден", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // Обработчик клика по кнопке "Вход"
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Если видимо окно ввода кода, проверяем код
            if (CodeGrid.Visibility == Visibility.Visible)
            {
                CodeTextBox_KeyDown(null, new KeyEventArgs(Keyboard.PrimaryDevice,
                                  Keyboard.PrimaryDevice.ActiveSource, 0, Key.Enter));
            }
            // Если активно поле пароля, проверяем пароль
            else if (PasswordBox.IsEnabled)
            {
                PasswordBox_KeyDown(null, new KeyEventArgs(Keyboard.PrimaryDevice,
                                  Keyboard.PrimaryDevice.ActiveSource, 0, Key.Enter));
            }
        }

        // Обработчик нажатия клавиш в поле пароля
        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Проверка на нажатие Enter
            {
                string password = PasswordBox.Password; // Получение введенного пароля
                if (string.IsNullOrWhiteSpace(password)) // Проверка на пустое значение
                {
                    MessageBox.Show("Введите пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; // Если пароль пуст, выводим сообщение об ошибке
                }
                if (CheckPassword(_currentUser, password)) // Проверка правильности пароля
                {
                    // Генерация кода подтверждения
                    _generatedCode = GenerateConfirmationCode();
                    _codeExpirationTime = DateTime.Now.AddSeconds(10); // Установка времени действия кода
                    // Отображение окна с кодом подтверждения
                    var codeWindow = new CodeConfirmationWindow(_generatedCode);
                    codeWindow.Closed += (s, args) =>
                    {
                        CodeGrid.Visibility = Visibility.Visible; // Показываем окно ввода кода
                        CodeTextBox.IsEnabled = true; // Разблокируем поле ввода кода
                        CodeTextBox.Focus(); // Устанавливаем фокус на поле кода
                        ResendButton.Visibility = Visibility.Visible; // Показываем кнопку "Отправить повторно"
                        LoginButton.IsEnabled = true; // Разблокируем кнопку "Вход"
                    };
                    codeWindow.ShowDialog(); // Открываем модальное окно с кодом
                    _codeTimer.Start(); // Запускаем таймер
                }
                else
                {
                    MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // Обработчик нажатия клавиш в поле ввода кода
        private void CodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return) // Проверка на нажатие Enter
            {
                // Проверка совпадения кода и его срока действия
                if (CodeTextBox.Text == _generatedCode && DateTime.Now < _codeExpirationTime)
                {
                    _codeTimer.Stop(); // Останавливаем таймер
                    string roleName = GetUserRoleName(_currentUser.Role_ID); // Получаем роль пользователя
                    MessageBox.Show($"Добро пожаловать, {_currentUser.User_Full_Name}!\nВаша роль: {roleName}",
                                  "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Открываем главное окно в зависимости от роли пользователя
                    Window mainWindow;
                    if (_currentUser.Roles.Role_Name == "Старший кассир")
                    {
                        mainWindow = new GeneralWindowSenior(_currentUser); // Окно для старшего кассира
                    }
                    else
                    {
                        mainWindow = new GeneralWindowOperator(_currentUser); // Окно для оператора
                    }
                    mainWindow.Show(); // Отображаем главное окно
                    this.Close(); // Закрываем текущее окно
                }
                else
                {
                    MessageBox.Show("Неверный код подтверждения или время истекло", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // Обработчик тика таймера
        private void CodeTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now >= _codeExpirationTime) // Проверка истечения времени действия кода
            {
                _codeTimer.Stop(); // Останавливаем таймер
                CodeTextBox.IsEnabled = false; // Блокируем поле ввода кода
                MessageBox.Show("Время действия кода истекло", "Информация",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Обработчик повторной отправки кода
        private void ResendButton_Click(object sender, RoutedEventArgs e)
        {
            _generatedCode = GenerateConfirmationCode(); // Генерация нового кода
            _codeExpirationTime = DateTime.Now.AddSeconds(10); // Установка нового времени действия кода
            CodeTextBox.Text = string.Empty; // Очищаем поле ввода кода
            var codeWindow = new CodeConfirmationWindow(_generatedCode); // Открываем окно с новым кодом
            codeWindow.Closed += (s, args) =>
            {
                CodeTextBox.IsEnabled = true; // Разблокируем поле ввода кода
                CodeTextBox.Focus(); // Устанавливаем фокус на поле кода
            };
            codeWindow.ShowDialog(); // Открываем модальное окно с кодом
            _codeTimer.Start(); // Запускаем таймер заново
        }

        // Проверка логина в базе данных
        private Users CheckUserLogin(string login)
        {
            using (var context = new CurrencyExchangeAccountingEntities()) // Создание контекста базы данных
            {
                return context.Users
                    .Include(u => u.Roles) // Подгрузка связанных данных о ролях
                    .FirstOrDefault(u => u.Login == login); // Поиск пользователя по логину
            }
        }

        // Проверка пароля
        private bool CheckPassword(Users user, string password)
        {
            return user.Password == password; // Проверка совпадения паролей
        }

        // Генерация случайного кода подтверждения
        private string GenerateConfirmationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 4) // Генерация строки из 4 символов
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Получение названия роли пользователя
        private string GetUserRoleName(int roleId)
        {
            using (var context = new CurrencyExchangeAccountingEntities()) // Создание контекста базы данных
            {
                var role = context.Roles.FirstOrDefault(r => r.Role_ID == roleId); // Поиск роли по ID
                return role?.Role_Name ?? "Неизвестная роль"; // Возвращаем название роли или "Неизвестная роль"
            }
        }

        // Обработчик кнопки "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Очистка всех полей ввода
            LoginTextBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            CodeTextBox.Text = string.Empty;

            // Скрытие элементов, связанных с кодом подтверждения
            CodeGrid.Visibility = Visibility.Collapsed;
            ResendButton.Visibility = Visibility.Collapsed;

            // Возврат кнопке "Вход" исходного состояния
            LoginButton.Content = "Вход";

            // Блокировка полей пароля и кода
            PasswordBox.IsEnabled = false;
            CodeTextBox.IsEnabled = false;

            // Деактивация кнопки "Вход"
            LoginButton.IsEnabled = false;

            // Остановка таймера, если он был запущен
            if (_codeTimer.IsEnabled)
            {
                _codeTimer.Stop();
            }

            // Установка фокуса на поле логина
            LoginTextBox.Focus();
        }
    }
}