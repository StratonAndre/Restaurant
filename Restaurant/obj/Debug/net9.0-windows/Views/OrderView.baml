   M S B A M L   `   `   `  ����L  HRestaurantManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:RestaurantManager.ViewsRestaurantManager.Views  5   	   K*clr-namespace:RestaurantManager.ViewModelsRestaurantManager.ViewModels  5   	   '    !RestaurantManager.Views.OrderView   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:RestaurantManager.Views     5   	   9vm*clr-namespace:RestaurantManager.ViewModels     5   	   .��5      �   � RowDefinitions  5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $��
10,10,10,5q�6;   .+�5      �� $��Order #��6      	PageTitle#��[  6'   $��0q�6J   �� 5      #��[  69   $��5,0,0,0q�6\   ��6   �� *Order.OrderCode+6\   5      �� 5   
   $��1��6     	CardPanel#��[ 6   $��
10,0,10,10q�6A   ._�5      �  � ColumnDefinitions 5      �� 5      $��* �6'   �� 5      $��* �6'   �� 5      $��* �6'   .+�5#      �� $��0��6   .+�5$      �� $��Status��6    $��SemiBold"�6.   �� 5%      $��16(�6I   ��6    �� *Order.Status.StatusName+6I   5&      �� 5)      $��1��6   .+�5*      �� $��
Order Date��6    $��SemiBold"�62   �� 5+      ��6    �� *Order.OrderDate+ �� StringFormat$ {0:g}���� 5,      $��Estimated Delivery��6    $��SemiBold"�6:   $��0,10,0,0q�6P   �� 5-      ��6    �� *Order.EstimatedDeliveryTime+$ {0:g}��5.      �� 51      $��2��6   .+�52      �� $��Order Total��6    $��SemiBold"�63   �� 53      $��16(�6l   ��6    �� *Order.TotalCost+  PriceConverter �� 	Converter# [ 6l   54      55      56      �� 59   
   $��2��6   #��[ 6   $��
10,0,10,10q�6A   ._�5:      �   5;      �� 5<      $
��Auto �6$   �� 5=      $��* �6$   .+�5A      �� $��0��6   $��Order Items��6)     SectionTitle#��[ 5B      &   System.Windows.Controls.DataGrid  5E      $��1��6      AutoGenerateColumns$ False��5G         
IsReadOnly$
 True��5H         HeadersVisibility$ Column=�5I         GridLinesVisibility$ 
Horizontal=�5J      $��0q�5K      	   RowHeaderWidth$	 0Q�5L      ��5F      �� *OrderDetails+
   Columns
 5M      0  *System.Windows.Controls.DataGridTextColumn  5N      ,  &System.Windows.Controls.DataGridColumn   Header$
 Item��6-      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6X   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6;   �� *
ItemName+6X     5O      $
 Type��6-   $	 100 6X    6;   �� *
ItemType+6X     5P      $ Quantity��6-   $ 80 6\    6?   �� *
Quantity+6\     5Q      $ 
Unit Price��6-   $	 100 6�    6A   �� *	UnitPrice+# [ 6�     5R      $ Total��6-   $	 100 6�    6<   �� *	LineTotal+# [ 6�   5T      5U      5V      �� 5Y   
   $��3��6   #��[ 6   $��
10,0,10,10q�6A   ._�5Z      �  5[      �� 5\      $��* �6'   �� 5]      $
��Auto �6'   .+�5a      �� $��0��6   $ 
Horizontal=�6-   y�5b      �� *CanUpdateStatus+   BooleanToVisibilityConverter# [ .+�5c      �� $��Update Status:��6    $��Center=�66   $��0,0,10,0q�6Q   �� 5d       �� DisplayMemberPath$ 
StatusName��5f      $	��150��5g      $��0,0,10,0q�5h      ��5d      �� *AvailableStatuses+ �� SelectedItem 5e      �� *SelectedStatus+5h      �� 5i      $��Update��6   ��5j      �� *UpdateStatusCommand+��5k      �� *SelectedStatus+5l      �� 5o      $��1��6   $��Cancel Order��5p        DangerButton#��[ 5s      ��5q      �� *CancelOrderCommand+y�5r      �� *CanCancelOrder+# [ 5s      5t      5u      �� 5x   
   $��0��6   $��4��6   ���	#80FFFFFFy�5z      �� *IsLoadingData+# [ ._�5{      �� $��Center=�6   $��Center=�64   .+�5|      �� $��
Loading...��6   $��18(�6.   $��Center=�6<   � 5}       � IsIndeterminate$
 True��6   $	��200��65   $��10��6A   $��0,10,0,0q�6M   5~      5      �� 5�   
   $��0��6   $��4��6   ���	#FFEFE0E0���	#FFCC0000$��1q�5�   @    �� CornerRadius$ 4��6T    �� Padding$ 10q�5�      $��10q�6   $	��Top=�5�      $��Center=�6)   y�5�      �� *ErrorMessage+  NullToVisibilityConverter# [ ._�5�      �� ���	#FFCC0000$
��Wrap=�6M   ��6   �� *ErrorMessage+6M   5�      5�      5�      