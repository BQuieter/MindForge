   M S B A M L   `   `   `  ����J  FMindForgeClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:MindForgeClientMindForgeClient  5      0    *MindForgeClient.Pages.OtherUserProfilePage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      /localclr-namespace:MindForgeClient     5      -   5          N� Title$  OtherUserProfilePage��5	      .��5      � -   5         �Name$ MainGrid��5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� -   5           CloseButton#��[  5      $��X��67   $��Right=�6C   $��0 3 5 0q�6_   $��15(�6�   �� 5   
   $��1��6   $��0 0 0 0q�6"   .+�5      �� -   5         ��Name$ ProfileImage��5      $/��)pack://application:,,,/Images/Profile.png��6*   $	��100��6]   $	��100��6i   $��	0 30 0 10q�6v   �� 5      -   5         ��Name$ LoginTextBlock��5        BasicTextBlock#��[ 60   $��
Логин��6X   $��Center=�6e   $��0 0 0 10q�6�   5      �� 5   
   $��2��6   $��Обо мне:��6     	BaseLabel#��[ 60   $��5 0 0 0q�6S   $��18(�6d   �� 5   
   -   5         ��Name$ Description��5      $��3��6'     LoginTextBox#��[ 64   $I��CПользователь ничего не указал о себе��6Z    �� 
IsReadOnly$
 True��6�   $
��auto��6�   $	��100��6�   $	��100��6�   $��Stretch=�6�   $��	10 5 10 0q�6�   $	��Top=�6�   " �� VerticalScrollBarVisibility$
 Auto=�6   �� TextWrapping$
 Wrap=�67  	 �� AcceptsReturn$
	 True��6K  �� 5   
   -   5        
 ��Name$
 ProfessionListBox��5      $��4��6-   $
��Auto=�6:   $��Hidden=�6j   $��Center=�6�   ��5      �� 5      .R�5      � $��0 0 0 10q�6   $��25��6-    � ColumnDefinitions 5      �� 5      $��33 �6/   �� 5       �� 5!      $
��auto �6/   .+�5#      A� $��20��6"   $��20��6.   $��0��6U   $��0 0 5 0q�6e   ��69   ��  �� Path$ Color�6e   �� 5$      #��[ 6$   $��1��6g   $��15(�6w   $��Center=�6�   $��5 0 5 0q�6�   ��6L   �� $
 Name�6�   5%      5&      5(      5)      5*      