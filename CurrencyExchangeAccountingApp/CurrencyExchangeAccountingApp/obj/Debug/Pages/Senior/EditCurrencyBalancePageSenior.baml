   M S B A M L   `   `   `  ����!  CurrencyExchangeAccountingAppA    ;CurrencyExchangeAccountingApp.EditCurrencyBalancePageSenior   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        N� Title$4  .Изменение остатка валюты��5       N� FontSize$ 16(�5       � 	Resources 5
         FormButtonStyle �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value  FormTextBlockStyle  FormTextBoxStyle��%M  &	        &	 �    &	 ,    �� 5   
   " ��6(   .�5      ��  � ��$	 120��6&   �� 5       � ��$ 35��6'   �� 5       � ��$
 10,5q�6'   �� 5       � ��$ 16(�6)   �� 5       � �� �#f9e57c6+   �� 5       � ��
 �����6,   �� 5       � ��$ 1q�60    5      P� 5       �@J�IsMouseOver . . �5      ��  � �� �#f7d93363   5      5!      �� 5"   
   " ��6+   .�5#      ��  � ��$
 10,5q�6'   �� 5%       � ��$ 16(�6)   �� 5'       � ��$ Center=�62   5)      �� 5*   
   " ��6)   .�5+      ��  � ��$ 35��6'   �� 5-       � ��$
 10,5q�6'   �� 5/       � ��$ 16(�6)   �� 51       � ��
 �����6,   �� 53       � ��$ 1q�60   �� 55       � ��$ Center=�69   57      58      .��5:      � .+�5<   
   �� $��1��6   $��1��6!   
���   �6+   � 5?   
   $��2��6   
�������	 � RowDefinitions	 5@      �� 5A      $
��Auto �6    �� 5C      $��* �6    �� 5E      $
��Auto �6    .+�5J      �� $��0��6   ���#f0f0f0
 �� Padding$
 10q�67   ._�5K      �� $4��.Изменение остатка валюты��6   $��18(�5L      $
��Bold"�5M      5O      � 5R      $��1��6   $��20q�6    	 5S      �� 5T      $
��Auto �6$   �� 5V      $
��Auto �6$   �� 5X      $
��Auto �6$    � ColumnDefinitions 5[      �� 5\      $
��Auto �6'   �� 5^      $	��300 �6'   .+�5c      �� $��Валюта:��6   $��0��5d      $��0��6)   #��[ 5e      �� 5h      -   5         ��Name$ CurrencyNameTextBlock��5h      $��0��5i      $��1��6)   #��[ 5j      $
��Bold"�5k      �� 5n      $$��Текущий остаток:��6   $��1��5o      $��0��6)   #��[ 5p      �� 5s      -   5        $ CurrentBalanceTextBlock��5s      $��1��5t      $��1��6)   #��[ 5u      $
��Bold"�5v      �� 5y      $ ��Новый остаток:��6   $��2��5z      $��0��6)   #��[ 5{      �� 5~      -   5         ��Name$ NewBalanceTextBox��5~      $��2��5      $��1��6'   #��[ 5�      5�      �� 5�      $��2��6    �� Orientation$ 
Horizontal=�5�      $��Right=�5�      $��Bottom=�5�      $��
0,10,20,20q�5�      .+�5�      �� -   5         ��Name$ 
SaveButton��5�      $��Сохранить��5�      #��[  5�      5�      �� 5�      -   5        $ CancelButton��5�      $��Отмена��5�      #��[  5�      5�      5�      5�      5�      5�      