   M S B A M L   `   `   `  ���� J  FMindForgeClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:MindForgeClient.PagesMindForgeClient.Pages  '    !MindForgeClient.Pages.ProfilePage   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     5local#clr-namespace:MindForgeClient.Pages     -     N� Title$  ProfilePage��.���  � ColumnDefinitions �� �� .+�� $��0�� �� $	��100���� $	��300��$	��400���� $	��100��.+��� $��1�����#202020.+��� -    ��Name$ ProfileImage��$/��)pack://application:,,,/Images/Profile.png��$	��100��$	��100��$��	0 30 0 10q��� -    ��Name$ LoginTextBlock��   BasicTextBlock#��[  $��
Логин��$��Center=�$��0 0 0 20q��� -    ��Name$ ProfessionListBox��$	��300������ .R�� $��0 0 0 10q�$��25�� �� $��33 ��� �� $
��auto �.+�A� $��20��$��20��$��0��$��0 0 5 0q�����  �� Path$ Color��� #��[  $��1��$��17(�$��Center=�$��5 0 5 0q����� $
 Name��� -     InvisibleButton#��[ $��25��$��25��$��2��$y�	Collapsed=�.���� $.��(pack://application:,,,/Images/Delete.png���� -    ��Name$ ProfessionsComboBox��  ProfessionComboBox#��[ $	��300��$y�	Collapsed=����� .R��� $��5 2 0 2q����� $
 Name�� -    �Name$ AddProfessionGrid��  BaseGrid#��[ $	��300��$y�	Collapsed=� �� $��35 ��� $	��230 �.+���   MenuIcon#��[ $��25��$��25��$+��%pack://application:,,,/Images/Add.png��$��Right=�$��0��$��0 0 10 0q��� #��[  $)��#Добавить профессию��$��1��$��18(�� $��1�����#202020 � RowDefinitions �� �� $��70 �.+��� $��0��.+��  �� �� .+��� $��Обо мне:��  	BaseLabel#��[ $��5 20 0 0q�$��18(�$��0��� $��1��.+��� -   	 ��Name$	 Description��  LoginTextBox#��[ 
 �� 	MaxLength$	
 200�� �� 
IsReadOnly$
 True��$
��Left=�$��10 5 0 0q�$	��100��$	��Top=�" �� VerticalScrollBarVisibility$
 Auto=� �� TextWrapping$
 Wrap=� �� AcceptsReturn$
 True����   	Watermark#��[ $"��Напишите о себе��$��13 6 0 0q���  �� Orientation$ 
Horizontal=�$��RightToLeft=�$��1��.+��� -	    ��Name$ CancelButton��  
ButtonBase#��[ $��Отмена��$��10q�$��35��$��90��$��15(�$y�	Collapsed=��� -
   $ 
EditButton��#��[ $ ��Редактировать��$��10q�$��35��$	��140��$��15(��� -   $ 
SaveButton��#��[ $��Сохранить��$��10q�$��35��$	��140��$y�	Collapsed=�$��15(�