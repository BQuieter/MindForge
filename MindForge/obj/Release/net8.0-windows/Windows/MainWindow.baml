   M S B A M L   `   `   `  ���� J  FMindForgeClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:MindForgeClientMindForgeClient       MindForgeClient.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     /localclr-namespace:MindForgeClient     -      BasicWindowStyle#��[    � WindowStartupLocation$  CenterScreen=�'  !System.Windows.Shell.WindowChrome   WindowChrome      ResizeBorderThickness$ 10q�.���  � ColumnDefinitions �� $	��140 �$��45��$	��140���� $
��Auto ���  � RowDefinitions �� $��25 ��� .+��� 
������ $��1��$��1�� �� ShowsPreview$ False��$��3��$��Center=�$��Stretch=�� $��3��$��0�����#171717.+���  �� Orientation$ 
Horizontal=�$��RightToLeft=�.+��� -   $��X��  HeaderButton#��[    IsHitTestVisibleInChrome$
 True���� -   $��U��#��[ $
 True���� -   $��-��#��[ $
 True���� $ 
Horizontal=�$��LeftToRight=�.+���   Logo#��[ ��   HeaderAppName#��[ � $��0��$��1�����#252525 �� �� $��42 ��� $��70 �.+��� $��0��.+�   MindForgeClient.MenuGrid  -     Name$ Friends��   MindForgeClient.MenuGrid	   MouseLeftButtonDown$	 	MenuClick��  MenuGrid#��[ .+���   MenuIcon#��[ $9��3pack://application:,,,/Images/MenuIcons/Friends.png��
 K� ZIndex$
 1����   	MenuLabel#��[ $��Друзья��  -   $ Chats��$	 	MenuClick��#��[ .+��� #��[ $7��1pack://application:,,,/Images/MenuIcons/Chats.png��$
 1���� #��[ $��Чаты��  -   $ Groups��$	 	MenuClick��#��[ .+��� #��[ $8��2pack://application:,,,/Images/MenuIcons/Groups.png��$
 1���� #��[ $��Группы��  $	 	MenuClick��#��[ .+��� #��[ $=��7pack://application:,,,/Images/MenuIcons/Communities.png��$
 1���� #��[ $��Сообщества��  -   $ Settings��$	 	MenuClick��#��[ .+��� #��[ $:��4pack://application:,,,/Images/MenuIcons/Settings.png��$
 1���� #��[ $��Настройки��� -	     BaseGrid#��[ $��1��.+��� #��[ $8��2pack://application:,,,/Images/MenuIcons/Logout.png��$
 1��$��1 0 0 0q��� #��[ $��
Выход��  -
   $ Profile��$	 	MenuClick��  ProfileMenuGrid#��[ $��2�� �� $��45 ��� .+��� -    ��Name$ ProfileImage��#��[ $/��)pack://application:,,,/Images/Profile.png��$��Center=�$��38��$��38��$��4 0 0 0q�� $��1�� �� �� .+��� -    ��Name$ 
LoginLabel�� 	 	BaseLabel#��[	 $��
Логин��$��0��$��Bottom=����#AAA$��1 0 0 0q��� #��[	 $��Профиль��$��1��$��1 0 0 0q� � -     �Name$ 	MainFrame��$��2��$��2��  � NavigationUIVisibility$ Hidden=�