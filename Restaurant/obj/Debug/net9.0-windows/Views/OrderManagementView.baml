   M S B A M L   `   `   `  ����L  HRestaurantManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:RestaurantManager.ViewsRestaurantManager.Views  5   	   K*clr-namespace:RestaurantManager.ViewModelsRestaurantManager.ViewModels  5   	   1    +RestaurantManager.Views.OrderManagementView   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:RestaurantManager.Views     5   	   9vm*clr-namespace:RestaurantManager.ViewModels     5   	   .��5      �   � RowDefinitions  5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6      	PageTitle#��[  5      $��
10,10,10,5q�5      ��5   !   �� *Title+5      � 5   
   $��1��6   $��
10,0,10,10q�6    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��* �6#   �� 5      $��* �6#   �� 5      $��* �6#   �� 5      $��* �6#   .+�5"      �� $��0��6     	CardPanel#��[ 6%   $��0,0,5,0q�6H   ._�5#      �� .+�5$      �� $��Total Orders��6    $��SemiBold"�64   �� 5%      $��24(�6=   ��6    �� *TotalOrders+6=   5&      5'      �� 5*      $��1��6   #��[ 6%   $	��5,0q�6H   ._�5+      �� .+�5,      �� $��Active Orders��6    $��SemiBold"�65   �� 5-      $��24(�6B   ��6    �� *ActiveOrderCount+6B   5.      5/      �� 52      $��2��6   #��[ 6%   $	��5,0q�6H   ._�53      �� .+�54      �� $��Today's Revenue��6    $��SemiBold"�67   �� 55      $��24(�6i   ��6    �� *TodayRevenue+  PriceConverter �� 	Converter# [ 6i   56      57      �� 5:      $��3��6   #��[ 6%   $	��5,0q�6H   ._�5;      �� .+�5<      �� $��	This Week��6    $��SemiBold"�61   �� 5=      $��24(�6h   ��6    �� *WeekRevenue+# [ 6h   5>      5?      �� 5B      $��4��6   #��[ 6%   $��5,0,0,0q�6H   ._�5C      �� .+�5D      �� $��
This Month��6    $��SemiBold"�62   �� 5E      $��24(�6i   ��6    �� *MonthRevenue+# [ 6i   5F      5G      5H      �� 5K   
   $��2��6   $��
10,0,10,10q�6"   .@�5M      �� $��Active Orders��6   .��5N      � .+�5O      &   System.Windows.Controls.DataGrid     AutoGenerateColumns$ False��5Q         
IsReadOnly$
 True��5R         HeadersVisibility$ Column=�5S         GridLinesVisibility$ 
Horizontal=�5T      $��0q�5U         RowHeaderWidth$ 0Q�5V      ��5O      �� *ActiveOrders+ �� SelectedItem 5P      �� *SelectedOrder+	   Columns	 5W      0  *System.Windows.Controls.DataGridTextColumn  5X      ,  &System.Windows.Controls.DataGridColumn
   Header$
 Order ID��61      Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 6a   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6C   �� *	OrderCode+6a     5Y      $
 Customer��61   $	 150 6e    6C   �� *User.FullName+6e     5Z      $

 Date��61   $	 150 6u    6?   �� *	OrderDate+ �� StringFormat$ {0:g}��6u     5[      $
 Status��61   $	 120 6g    6A   �� *Status.StatusName+6g     5\      $
 Total��61   $	 100 6�    6@   �� *	TotalCost+# [ 6�   4  .System.Windows.Controls.DataGridTemplateColumn  5]      $ * 65      CellTemplate 5^   "   �� 5_   &   .R�5`   *   ��  �� Orientation$ 
Horizontal=�65   $��Right=�6N   .+�5a   .   �� $��Update Status��65   $��5q�5b   5   ��5c   5   �� *%#DataContext.ViewOrderDetailsCommand+ �� RelativeSource ��  �� AncestorType DataGrid��5e   5   �� 5f   +   5g   '   5i      5k      �� 5n      $��No active orders��6    $��Center=�5o       $��Center=�5p       ���#999999$��16(�5r       y�5s       �� *ActiveOrders.Count+  NumberToVisibilityConverter# [  �� ConverterParameter$ inverse��5t      5u      �� 5x      $��
All Orders��6   .��5y      � .+�5z        $ False��5|      $
 True��5}      $ Column=�5~      $ 
Horizontal=�5      $��0q�5�      $ 0Q�5�      ��5z      �� *	AllOrders+ 5{      �� *SelectedOrder+	 5�        5�      $
 Order ID��61   $	 100 6a    6C   �� *	OrderCode+6a     5�      $
 Customer��61   $	 150 6e    6C   �� *User.FullName+6e     5�      $

 Date��61   $	 150 6u    6?   �� *	OrderDate+$ {0:g}��6u     5�      $
 Status��61   $	 120 6g    6A   �� *Status.StatusName+6g     5�      $
 Total��61   $	 100 6�    6@   �� *	TotalCost+# [ 6�     5�      $ * 65    5�   "   �� 5�   &   .R�5�   *   �� $ 
Horizontal=�65   $��Right=�6N   .+�5�   .   �� $��View Details��65   $��5q�5�   5   ��5�   5   �� *%#DataContext.ViewOrderDetailsCommand+ ��  DataGrid��5�   5   �� 5�   +   5�   '   5�      5�      �� 5�      $��No orders found��6    $��Center=�5�       $��Center=�5�       ���#999999$��16(�5�       y�5�       �� *AllOrders.Count+# [ $ inverse��5�      5�      5�      �� 5�   
   $��0��6   $��3��6   ���	#80FFFFFFy�5�      �� *IsLoadingData+   BooleanToVisibilityConverter# [ ._�5�      �� $��Center=�6   $��Center=�64   .+�5�      �� $��
Loading...��6   $��18(�6.   $��Center=�6<   � 5�       � IsIndeterminate$
 True��6   $	��200��65   $��10��6A   $��0,10,0,0q�6M   5�      5�      �� 5�   
   $��0��6   $��3��6   ���	#FFEFE0E0���	#FFCC0000$��1q�5�   @    �� CornerRadius$ 4��6T    �� Padding$ 10q�5�      $��10q�6   $	��Top=�5�      $��Center=�6)   y�5�      �� *ErrorMessage+  NullToVisibilityConverter# [ ._�5�      �� ���	#FFCC0000$
��Wrap=�6M   ��6   �� *ErrorMessage+6M   5�      5�      5�      