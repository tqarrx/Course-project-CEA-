   M S B A M L   `   `   `  ����X  TCurrencyExchangeAccountingApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullk:clr-namespace:CurrencyExchangeAccountingApp.Pages.Operator,CurrencyExchangeAccountingApp.Pages.Operator  5   	   H    BCurrencyExchangeAccountingApp.Pages.Operator.GeneralWindowOperator   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Llocal:clr-namespace:CurrencyExchangeAccountingApp.Pages.Operator     5   	   -   5          � Title$F  @Учет обмена валют - кассир-оператор��5   	    � WindowState$ 	Maximized=�5	   	    � WindowStyle$ SingleBorderWindow=�5
   	    � 
ResizeMode$ 	CanResize=�5   	    � 	Resources 5         MenuButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType	 �� Triggers
 P� Property P� Value��%�  &	        �� 5   
   " ��6(   .�5      ��  � ��
 ���� 6+   �� 5       � ��
 ���� 6,   �� 5       � ��
 �   �6+   �� 5       � ��$
 Left=�6;   �� 5       � ��$ 10,12q�6(   �� 5       � ��$ 0q�6'   �� 5       � ��$ 0,0,0,1q�60   �� 5       � ��
 �   �6,   �� 5       � ��$ 16(�6)   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   #��z��5   !   #��z��5    !   ._�5!      �� #��z��6/   #��z��5"   /   $��Center=�5#   /   5$      5%      5'      	 5(      P� 5)      
 �@J�IsMouseOver . . �5*      ��  � �� �#f9e57c63   5+      5-      5.      .��51      �  � ColumnDefinitions 53   
   �� 54      $	��200 �6   �� 55      $
��Auto �6   �� 56      $��* �6   .+�5:   
   �� $��1��6   $��1��6!   
���   �6+   �� 5=   
   $��0��6   ���#fbed9e.+�5?      �� -   5         ��Name$ ExchangeRatesBtn��5?      $��Курсы валют��5@      #��[  5A      5B      �� 5D      -   5        $ CurrenciesBtn��5D      $��Валюты��5E      #��[  5F      5G      �� 5I      -   5        $ OperationsBtn��5I      $��Операции��5J      #��[  5K      5L      �� 5N      -   5        $ 	LogoutBtn��5N      $��
Выход��5O      #��[  5P      5Q      5S       � 5V   
   -   5          �Name$ 	MainFrame��5V      $��2��5W        � NavigationUIVisibility$ Hidden=�5X      5Y      5Z      