   M S B A M L   `   `   `  ����J  FMindForgeClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:MindForgeClientMindForgeClient  5      7    1MindForgeClient.Pages.Chats.Group.CreateGroupPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      /localclr-namespace:MindForgeClient     5      -   5          N� Title$  CreateGroupPage��5	      .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $��* �6   �� 5      $��50 �6   .+�5   
   � $��0��6   $��Center=�6   $��0 0 0 30q�69    5      �� 5      $
��auto �6    �� 5      $
��auto �6    .+�5      �� $��0��6      BasicTextBlock#��[  6%   $��Фото группы��6M   $��17(�6`   $��Center=�6n   $��0 0 0 10q�6�   �� 5      -   5         ��Name$ 
GroupImage��5      $��1��6(   $I��Cpack://application:,,,/Images/MenuIcons/Chats/BaseGroupImageBig.png��65   $��96��6�   $��96��6�   6�   5      � 5   
   $��1��6   $��Center=�6   $��0 0 0 30q�69    5      �� 5      $
��auto �6    �� 5      $
��auto �6    .+�5!      �� $��0��6   #��[  6%   $#��Название группы��6M   $��17(�6d   $��Center=�6r   $��0 0 0 10q�6�   � 5"      $��1��6   .+�5#      �� -   5         ��Name$ GroupNameBox��5#        LoginTextBox#��[ 60    �� 	MaxLength$ 50��6V   6e   �� 5$        	Watermark#��[ 6   $4��.Напишите название группы��6?   $��3 1 0 0q�6_   5%      5&      � 5(   
   $��2��6    5)      �� 5*      $
��auto �6    �� 5+      $
��auto �6    �� 5,      $��* �6    .+�5.      �� #��[  6   $+��%Добавить участников��6A   $��Center=�6\   $��17(�6y   $��0 0 0 10q�6�   � 5/      $��1��6   $��Center=�6     � ColumnDefinitions 50      �� 51      $��37 �6'   �� 52      $	��315 �6'   .+�54      �� $��0��6   $>��8pack://application:,,,/Images/MenuIcons/FilterSearch.png��6(   $��32��6j   $��32��6u   $��0 0 5 0q�6�   �� 55      #��[ 6   $��1��6?   $,��&Напишите логин друга��6O   $��14(�6k   $�� 3 0 0 0q�6y   $��Center=�6�   �� 56      -   5        #��[ 56      $��1��6@   $��Stretch=�6P   $
��auto��6n   $��20��6{   $��Center=�6�   $��0q�6�   $��14(�6�   6�   57      �� 58      -   5        $��2��58      $	��500��6(   $
��Auto=�64   $��Disabled=�6W   .��59      �� -   5         ��Name$ FriendsListBox��59      ���#222222$
��Left=�6G   $��Disabled=�6b    %� IsVirtualizing$
 True��6�    %� CacheLength@  :System.Windows.Controls.VirtualizationCacheLengthConverter$ 30 6�   	 %� CacheLengthUnit$
	 Item=�6�   ��5:      �� 5;      .R�5<         MindForgeClient.MenuGrid     MindForgeClient.MenuGrid
   MouseLeftButtonDown 
 MenuGrid_MouseLeftButtonDown6O      Loaded Friend_Loaded6�     MenuGrid#��[ 6-   $��2q�6�   $	��500��6�   .+�5=   "   ��  ��Name$ Template��6-   $��5q�6?    �� Orientation$ 
Horizontal=�6J   .+�5>   &   �� -   5        $/��)pack://application:,,,/Images/Profile.png��5>   ,   $��48��6_   $��48��6j   $��0 0 10 0q�6v   6�   �� 5?   &   #��[  60   $��Center=�6t   ��6X   ��  �� Path$ Login�6t   5@   #   5A      5B      5D      5E      5F      � 5H   
   $��3��6    5I      �� 5J      �� 5K      .+�5M      �� -   5        $��0��5M        
ButtonBase#��[ 6%   $��Отмена��6I   $	��150��6Z   $��35��6f   $��Right=�6r   $��0 0 10 0q�6�   6�   �� 5N      -	   5        $��1��5N      #��[ 6%   $��Создать��6I   $	��150��6[   $��35��6g   $
��Left=�6s   $��10 0 0 0q�6�   6�   5O      5P      5Q      