   M S B A M L   `   `   `  ����J  FMindForgeClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:MindForgeClientMindForgeClient  5   	        MindForgeClient.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:MindForgeClient     5   	   -   5           BasicWindowStyle#��[  5   	     � WindowStartupLocation$  CenterScreen=�5	   	   '  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      ResizeBorderThickness$ 10q�6   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��140 �6   $��45��6+   $	��140��69   �� 5      $
��Auto �6   �� 5      �� 5      -   5         ��Name$ ProfileFrameColumn��5   8   $��0 �6   $	��350��6)   68    � RowDefinitions 5   
   �� 5      $��25 �6   �� 5      .+�5   
   �� ���#252525$��1��6,   $��1��6<    �� ShowsPreview$ False��6I   $��5��6^   $��Center=�6h   $��Stretch=�6�   � 5   
   $��4��6   $��0��6#   ���#171717.+�5      ��  �� Orientation$ 
Horizontal=�6   $��RightToLeft=�62   .+�5      �� -   5        $��X��5        HeaderButton#��[ 6%      IsHitTestVisibleInChrome$
 True��6K   6x   �� 5      -   5        $��U��5      #��[ 6%   $
 True��6K   6x   �� 5      -   5        $��-��5      #��[ 6%   $
 True��6K   6x   5       �� 5!      $ 
Horizontal=�6   $��LeftToRight=�62   .+�5"      ��   Logo#��[ 6   �� 5#        HeaderAppName#��[ 6   5$      5%      � 5&   
   $��0��6   $��1��6   ���#252525 5'      �� 5(      �� 5)      $��42 �6    �� 5*      $��70 �6    .+�5,      �� $��0��6   .+�5-         MindForgeClient.MenuGrid  -   5        	  Name$	 Friends��5-   !      MindForgeClient.MenuGrid
   MouseLeftButtonDown$
 	MenuClick��6T     MenuGrid#��[ 62    � ToolTip$ Друзья��6t   .+�5.      ��   MenuIcon#��[ 6   $9��3pack://application:,,,/Images/MenuIcons/Friends.png��6>    K� ZIndex$ 1��6{   �� 5/        	MenuLabel#��[ 6   $��Друзья��6?   50        52      -   5        $	 Chats��52   !   $
 	MenuClick��6R   #��[ 60   $ Чаты��6r   .+�53      �� #��[ 6   $7��1pack://application:,,,/Images/MenuIcons/Chats.png��6>   $ 1��6y   �� 54      #��[ 6   $��Чаты��6?   55        57      -   5        $	 Groups��57   !   $
 	MenuClick��6S   #��[ 61   $ Группы��6s   .+�58      �� #��[ 6   $8��2pack://application:,,,/Images/MenuIcons/Groups.png��6>   $ 1��6z   �� 59      #��[ 6   $��Группы��6?   5:      5;      � 5<      -	   5          BaseGrid#��[ 5<      $��1��65   $ 
Выход��6_   .+�5=      �� #��[ 6   $8��2pack://application:,,,/Images/MenuIcons/Logout.png��6:   $ 1��6v   $��1 0 0 0q�6�   �� 5>      #��[ 6   $��
Выход��6;   5?        5@      -
   5        $	 Profile��5@      $
 	MenuClick��6e     ProfileMenuGrid#��[ 6.   $��2��6X   $ Профиль��6�    5A      �� 5B      $��45 �6'   �� 5C      .+�5E      �� -   5         ��Name$ ProfileImage��5E      #��[ 6.   $/��)pack://application:,,,/Images/Profile.png��6P   $��Center=�6�   $��38��6�   $��38��6�   $��4 0 0 0q�6�   � 5F      $��1��6    5G      �� 5H      �� 5I      .+�5K      �� -   5         ��Name$ 
LoginLabel��5K       	 	BaseLabel#��[	 60   $��
Логин��6S   $��0��6c   $��Bottom=�6p   ���#AAA$��1 0 0 0q�6�   �� 5L      #��[	 6   $��Профиль��6?   $��1��6Q   $��1 0 0 0q�6^   5M      5N      5O      �� 5P   
   $ 
Horizontal=�6   $��LeftToRight=�6.   $��2��6J   5R       � 5S   
   -   5          �Name$ 	MainFrame��5S      $��2��6#   $��2��63     � NavigationUIVisibility$ Hidden=�6@    � 5T   
   -   5        $ ProfileFrame��5T      $��3��6&   $��2��66   $ Hidden=�6C   ���#222222$y�	Collapsed=�6x   6�   5U      5V      