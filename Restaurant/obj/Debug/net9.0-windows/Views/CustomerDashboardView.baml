   M S B A M L   `   `   `  ����L  HRestaurantManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:RestaurantManager.ViewsRestaurantManager.Views  5   	   K*clr-namespace:RestaurantManager.ViewModelsRestaurantManager.ViewModels  5   	   3    -RestaurantManager.Views.CustomerDashboardView   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:RestaurantManager.Views     5   	   9vm*clr-namespace:RestaurantManager.ViewModels     5   	   .��5      �   � RowDefinitions  5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6      	PageTitle#��[  5      $��
10,10,10,5q�5      ��5   !   �� *Title+5      �� 5   
   $��1��6     	CardPanel#��[ 6   $��
10,0,10,10q�6A   ._�5      �� .+�5      �� $��18(�5      $��SemiBold"�5      ��5      �� *CurrentUser.FullName+ �� StringFormat$ Welcome, {0}!��5      �� 5      $E��?View your orders below or browse the menu to place a new order.��6   $��0,5,0,0q�5      �� 5      $��Browse Menu��6   $
��Left=�5!      $��0,10,0,0q�5"      ��5       �� *BrowseMenuCommand+5"      5#      5$      �� 5'   
   $��2��6   $��
10,0,10,10q�6"   .@�5)      �� $��Active Orders��6   .��5*      � .+�5+      &   System.Windows.Controls.DataGrid     AutoGenerateColumns$ False��5-         
IsReadOnly$
 True��5.         HeadersVisibility$ Column=�5/         GridLinesVisibility$ 
Horizontal=�50      $��0q�51         RowHeaderWidth$ 0Q�52      ��5+      �� *ActiveOrders+ �� SelectedItem 5,      �� *SelectedOrder+   Columns 53      0  *System.Windows.Controls.DataGridTextColumn  54      ,  &System.Windows.Controls.DataGridColumn	   Header$	 Order ID��61   
   Width5  /System.Windows.Controls.DataGridLengthConverter$	
 100 6a   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6C   �� *	OrderCode+6a     55      $
	 Date��61   $	
 150 6u    6?   �� *	OrderDate+$ {0:g}��6u     56      $	 Status��61   $	
 120 6g    6A   �� *Status.StatusName+6g     57      $	 Est. Delivery��61   $	
 150 6�    6H   �� *EstimatedDeliveryTime+$ {0:g}��6�     58      $	 Total��61   $	
 100 6�    6@   �� *	TotalCost+  PriceConverter �� 	Converter# [ 6�   4  .System.Windows.Controls.DataGridTemplateColumn  59      $
 * 65      CellTemplate 5:   "   �� 5;   &   .R�5<   *   ��  �� Orientation$ 
Horizontal=�65   $��Right=�6N   .+�5=   .   �� $��View Details��65   $��5q�5>   5   ��5?   5   �� *%#DataContext.ViewOrderDetailsCommand+ �� RelativeSource ��  �� AncestorType DataGrid��5A   5   �� �� 5C   .   $��Cancel��65   $��5q�5D   5     DangerButton#��[ 5H   5   ��5E   5   �� * DataContext.CancelOrderCommand+ ��  DataGrid��5G   5   �� y�5I   5   �� *CanBeCancelled+   BooleanToVisibilityConverter# [ 5J   +   5K   '   5M      5O      �� 5R      $��You have no active orders��6    $��Center=�5S       $��Center=�5T       ���#999999$��16(�5V       y�5W       �� *ActiveOrders.Count+  NumberToVisibilityConverter# [  �� ConverterParameter$ inverse��5X      5Y      �� 5\      $��Order History��6   .��5]      � .+�5^        $ False��5`      $
 True��5a      $ Column=�5b      $ 
Horizontal=�5c      $��0q�5d      $ 0Q�5e      ��5^      �� *	AllOrders+ 5_      �� *SelectedOrder+ 5f        5g      $	 Order ID��61   $	
 100 6a    6C   �� *	OrderCode+6a     5h      $
	 Date��61   $	
 150 6u    6?   �� *	OrderDate+$ {0:g}��6u     5i      $	 Status��61   $	
 120 6g    6A   �� *Status.StatusName+6g     5j      $	 Total��61   $	
 100 6�    6@   �� *	TotalCost+# [ 6�     5k      $
 * 65    5l   "   �� 5m   &   .R�5n   *   �� $��View Details��61   $��Right=�5o   1   $��5q�5p   1   ��5q   1   �� *%#DataContext.ViewOrderDetailsCommand+ ��  DataGrid��5s   1   �� 5t   '   5v      5x      �� 5{      $'��!You haven't placed any orders yet��6    $��Center=�5|       $��Center=�5}       ���#999999$��16(�5       y�5�       �� *AllOrders.Count+# [ $ inverse��5�      5�      5�      �� 5�   
   $��0��6   $��3��6   ���	#80FFFFFFy�5�      �� *IsLoadingData+# [ ._�5�      �� $��Center=�6   $��Center=�64   .+�5�      �� $��
Loading...��6   $��18(�6.   $��Center=�6<   � 5�       � IsIndeterminate$
 True��6   $	��200��65   $��10��6A   $��0,10,0,0q�6M   5�      5�      �� 5�   
   $��0��6   $��3��6   ���	#FFEFE0E0���	#FFCC0000$��1q�5�   @    �� CornerRadius$ 4��6T    �� Padding$ 10q�5�      $��10q�6   $	��Top=�5�      $��Center=�6)   y�5�      �� *ErrorMessage+  NullToVisibilityConverter# [ ._�5�      �� ���	#FFCC0000$
��Wrap=�6M   ��6   �� *ErrorMessage+6M   5�      5�      5�      