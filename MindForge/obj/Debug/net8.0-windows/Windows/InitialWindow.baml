   M S B A M L   `   `   `  ����J  FMindForgeClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MindForge	MindForge  5   	       MindForge.InitialWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   U QWpfAnimatedGif, Version=2.0.0.0, Culture=neutral, PublicKeyToken=9e7cd3b544a090dc,gif"http://wpfanimatedgif.codeplex.com  5   	   #localclr-namespace:MindForge  5   	   -   5          �Name$  	MindForge��5   	      BasicWindowStyle#��[  5	   	    � WindowStartupLocation$ CenterScreen=�5
   	   '  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      ResizeBorderThickness$ 10q�6   .��5      � -   5         �Name$ MainGrid��5       � ColumnDefinitions 5   
   �� 5      $
��0.5* �6   �� 5      $	��600 �6   �� 5      $
��0.5* �6    � RowDefinitions 5   
   �� 5      $��25 �6   �� 5      .+�5   
   � $��3��6   $��0��6#   ���#171717.+�5      ��  �� Orientation$ 
Horizontal=�6   $��RightToLeft=�62   .+�5      �� -   5        $��X��5        HeaderButton#��[ 6%      IsHitTestVisibleInChrome$
 True��6K   6x   �� 5      -   5        $��U��5      #��[ 6%   $
 True��6K   6x   �� 5      -   5        $��-��5      #��[ 6%   $
 True��6K   6x   5      �� 5       $ 
Horizontal=�6   $��LeftToRight=�62   .+�5!      ��   Logo#��[ 6   �� 5"        HeaderAppName#��[ 6   5#      5$      � 5%   
   $��1��6   $��1��6    5&      �� 5'      $	��200 �6    �� 5(      .+�5*      �� $0��*pack://application:,,,/Images/FullLogo.jpg��6   $	��180��6H   $��	0 20 0 20q�6U    � 5+      -   5        	  �Name$	 InitialFrame��5+      $��1��6*   
  � NavigationUIVisibility$
 Hidden=�67   �� 5,      -   5         ��Name$ LoadingPanel��5,      $��1��6/   .+�5-      ��   
LoadingGif#��[ 6   "  WpfAnimatedGif.ImageBehavior   AnimatedSource$ ..\Images\Loading.gif��6<   $��0 50 0 0q�6v   $y�Visible=�6�   �� 5.        BasicTextBlock#��[ 6   $��Загрузка...��6D   $��25(�6W   $��Center=�6e   $��0 30 0 0q�6�   5/      50      �� 51   
   ���#202020$��0��6#   $��1��63   �� 52   
   ���#202020$��2��6#   $��1��63   53      54      