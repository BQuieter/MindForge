   M S B A M L   `   `   `  ����J  FMindForgeClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:MindForgeClientMindForgeClient  5      K GMindForgeClasses, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM8clr-namespace:MindForgeClasses;assembly=MindForgeClassesMindForgeClasses 5      *    $MindForgeClient.Pages.Chats.ChatPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      /localclr-namespace:MindForgeClient     5      Lclasses8clr-namespace:MindForgeClasses;assembly=MindForgeClasses     5      -   5          N� Title$  ChatPage��5
      .��5      �  � RowDefinitions 5   
   �� 5      $��60 �6   �� 5      $
��auto �6   �� 5      $��* �6   �� 5      $
��auto �6   $��30��6*   $	��200��69   .+�5   
      MindForgeClient.MenuGrid     MindForgeClient.MenuGrid   MouseLeftButtonDown$ OpenInformation��6`      InformClickGridInChat#��[  6   $��0��6H   $��0q�6U    � ColumnDefinitions 5      �� 5      $��65 �6#   �� 5      �� 5      $
��auto �6#   .+�5      �� -   5         ��Name$ ChatPartnerImage��5      $��0��6.   $��48��6>   $��48��6I   $
��Left=�6U   $��5 0 0 0q�6p   �� 5      -   5         ��Name$ ChatPartnerLogin��5      $��1��62     BasicTextBlock#��[ 6B    �� TextAlignment$
 Left=�6j   $��Center=�6   �� 5      -   5          
ButtonBase#��[ 5      $��2��69   $��40��6I   $��40��6T   $��Center=�6`    � ToolTip$ Позвонить��6{   $��0 0 10 0q�6�   .��5      �� $;��5pack://application:,,,/Images/MenuIcons/Call/Call.png��6   $��30��6W   $��30��6b   5      5       � 5   
   -   5          �Name$ 	MainFrame��5      $��1��6#   $	��200��60   ���#222222$y�	Collapsed=�6R   �� 5    
   -   5        $ WarnTextBlock��5    +   $y�	Collapsed=�6   $��2��6B   #��[ 6O   $;��5У вас нет ни одного сообщения��6w   $��Center=�6�   $��Center=�6�   $��25(�6�   �� 5!   
   -   5        	 ��Name$	 ScrollViewer��5!      $��2��6-   $
��Auto=�6:   $��Disabled=�6^   $��False��6�   .��5"      �� -   5        
 ��Name$
 MessageList��5"      $��Disabled=�6+    %� IsVirtualizing$
 True��6a    %� VirtualizationMode$ 	Recycling=�6�    %� CacheLength@  :System.Windows.Controls.VirtualizationCacheLengthConverter$ 30 6�    %� CacheLengthUnit$
 Item=�6�   $��False��6  ��5#      �� 5$       �� DataType#  MindForgeClasses.MessageGroup"  6#   .R�5%      �� .+�5&      �� -	   5        #��[ 5&   (   $��Center=�6l   $��4q�6�   $��13(�6�   ��6P   �� *
DateString+6�   �� 5'       �� Orientation$ 
Horizontal=�6)   $��0 3 0 3q�6B   .+�5(   "   �� -
   5        $��30��5(   (   $��30��63   $��5q�6?   $@��:pack://application:,,,/Images/MenuIcons/Chats/GoToChat.png��6J   6�   �� 5)   "   #��[ 6,   $
��Bold"�6p   $��5 0 0 0q�6�   $��Center=�6�   ��6T   �� *
SenderName+6�   5*      �� 5+      ��6+   �� *
Messages+��5,   "   �� 5-   &   )  #MindForgeClasses.MessageInformation"  63   .R�5.   *   � ���#252525$
��Left=�6D   $��0 2 0 2q�6_    5/   .   �� 50   2   �� 51   2   $��50 �6C   .+�53   .   �� $��0��66     Message#��[ 6F   $
��Left=�6�   $��5 5 0 5q�6�   ��6g   �� *	Message+6�   �� 54   .   $��1��68   #��[ 6H   $��0 0 0 2q�6p   $��12(�6�   $��Bottom=�6�   $��Center=�6�   ��6�   �� *Time+6�   55   +   56   '   58      59      5:      5<      5=      � 5>   
   $��3��6   ���#232323 5?      �� 5@      �� 5A      $��60 �6#   .+�5C      �� -   5         ��Name$ MessageTextBox��5C      $��0��6.     MessageTextBox#��[ 6>   $��Hidden=�6�   �� 5D      $��0��6     	Watermark#��[ 6(   $,��&Написать сообщение...��6K   $��3 0 0 0q�6h   $��16(�6y   �� 5E      -   5        $��1��5E        
SendButton#��[ 6%   $��36��6I   $��36��6T   $��Bottom=�6`   $��0 0 0 5q�6{   .��5F      �� $6��0pack://application:,,,/Images/MenuIcons/Send.png��6   $��24��6R   $��24��6]   5G      5H      5I      5J      