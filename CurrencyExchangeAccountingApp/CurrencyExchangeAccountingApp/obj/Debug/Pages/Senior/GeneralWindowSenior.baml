   M S B A M L   `   `   `  ����X  TCurrencyExchangeAccountingApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullg8clr-namespace:CurrencyExchangeAccountingApp.Pages.Senior*CurrencyExchangeAccountingApp.Pages.Senior  5   	   D    >CurrencyExchangeAccountingApp.Pages.Senior.GeneralWindowSenior   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Jlocal8clr-namespace:CurrencyExchangeAccountingApp.Pages.Senior     5   	   -   5          � Title$D  >Учет обмена валют - Старший кассир��5   	    � WindowState$ 	Maximized=�5	   	    � WindowStyle$ SingleBorderWindow=�5
   	    � 
ResizeMode$ 	CanResize=�5   	    � 	Resources 5         MenuButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType	 �� Triggers
 P� Property P� Value��%�  &	        �� 5   
   " ��6(   .�5      ��  � ��
 ���� 6+   �� 5       � ��
 ���� 6,   �� 5       � ��
 �   �6+   �� 5       � ��$
 Left=�6;   �� 5       � ��$ 10,12q�6(   �� 5       � ��$ 0q�6'   �� 5       � ��$ 0,0,0,1q�60   �� 5        � ��
 �   �6,   �� 5"       � ��$ 16(�6)   �� 5$       � �� 5&      �� 5'      " ��6&   .R�5(      �� #��z��6!   #��z��5)   !   #��z��5*   !   ._�5+      �� #��z��6/   #��z��5,   /   $��Center=�5-   /   5/      50      52      	 53      P� 55      
 �@J�IsMouseOver . . �56      ��  � �� �#f9e57c63   58      5:      5;      .��5>      �  � ColumnDefinitions 5@   
   �� 5A      $	��200 �6   �� 5C      $
��Auto �6   �� 5E      $��* �6   .+�5J   
   �� $��1��6   $��1��6!   
���   �6+   �� 5M   
   $��0��6   ���#fbed9e.+�5O      �� -   5         ��Name$ ExchangeRatesBtn��5O      $��Курсы валют��5P      #��[  5Q      5R      �� 5V      -   5        $ CurrenciesBtn��5V      $��Валюты��5W      #��[  5X      5Y      �� 5\      -   5        $ OperationsBtn��5\      $��Операции��5]      #��[  5^      5_      �� 5b      -   5        $ 
ReportsBtn��5b      $��Отчеты��5c      #��[  5d      5e      �� 5h      -   5        $ 	LogoutBtn��5h      $��
Выход��5i      #��[  5j      5k      5l       � 5o   
   -   5          �Name$ 	MainFrame��5o      $��2��5p        � NavigationUIVisibility$ Hidden=�5q      5s      5t      