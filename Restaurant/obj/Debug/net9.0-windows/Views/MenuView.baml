   M S B A M L   `   `   `  ����L  HRestaurantManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:RestaurantManager.ViewsRestaurantManager.Views  5   	   K*clr-namespace:RestaurantManager.ViewModelsRestaurantManager.ViewModels  5   	   &     RestaurantManager.Views.MenuView   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:RestaurantManager.Views     5   	   9vm*clr-namespace:RestaurantManager.ViewModels     5   	   .��5      �   � RowDefinitions  5   
   �� 5      $
��Auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $	��250 �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��6!   $��2��61      	PageTitle#��[  5      $��
10,10,10,5q�5      ��5      �� *Title+5      �� 5   
   $��1��6   $��0��6     	CardPanel#��[ 6.   $��10q�6Q   ._�5      �   5      �� 5       $
��Auto �6$   �� 5!      $��* �6$   .+�5$      �� $��0��6   $��
Categories��6)     SectionTitle#��[ 5%      $��0,0,0,10q�5&      �� 5(      $��1��6    �� DisplayMemberPath$
 Name��5+      $��0q�5,      
������ ��5)      �� *
Categories+ �� SelectedItem 5*      �� *SelectedCategory+5-      5.      5/      � 52   
   $��1��6   $��1��6     53      �� 54      $
��Auto �6    �� 55      $��* �6    .+�59      �� $��0��6   #��[ 5:      $��
10,10,10,5q�5;      ��59   %   �� *SelectedCategory.Name+ �� StringFormat$ Menu Items - {0}��5;      �� 5>      $��1��6   $
��Auto=�6(   .��5?      �� ��6   �� *	MenuItems+��5@      �� 5A      .R�5B      �  � Orientation$ 
Horizontal=�6(   5C      ��5E      �� 5F      .R�5G      �� #��[ 6%   $	��300��6H   $��5q�6T   ._�5H   "   �   5I   &   �� 5J   *   $
��Auto �68   �� 5K   *   $
��Auto �68   �� 5L   *   $
��Auto �68   �� 5M   *   $
��Auto �68   �� 5N   *   $
��Auto �68   �� 5O   *   $
��Auto �68   .+�5S   &   �� $��0��60   $��16(�5T   0   $
��Bold"�6>   $��0,0,0,5q�6P   ��5S   =   �� *Name+5T   P   �� 5W   &   $��1��6-   $	��0,5q�6:   y�5X   -   �� *Images.Count+  NumberToVisibilityConverter �� 	Converter# [ ._�5Y   *   �� $	��120��5Z   0   $��Uniform=�6=   ��5Y   0   �� *Images[0].ImagePath+5Z   =   5[   '   �� 5^   &   $��2��61    �� Orientation$ 
Horizontal=�6>   $	��0,5q�6W   .+�5_   *   �� $��Portion Size: ��64   �� 5`   *   ��64   �� *PortionSize+$
 {0}g��5a   '   �� 5d   &   $��3��61   $ 
Horizontal=�6>   $	��0,5q�6W   .+�5e   *   �� $��Price: ��64   �� 5f   *   $��SemiBold"�5g   4   ��5f   4   �� *Price+  PriceConverter# [ 5g   4   5h   '   �� 5k   &   $��4��61   $ Vertical=�6>   $	��0,5q�6U   y�5l   1   �� *Allergens.Count+# [ .+�5m   *   �� $��
Allergens:��64   $��SemiBold"�6F   �� 5n   *   ��67   �� *	Allergens+��5o   .   �� 5p   2   .R�5q   6   � 5r   3   ��5t   .   �� 5u   2   .R�5v   6   �� ���	#FFF0F0F0$��2q�6T    �� Padding$	 4,2q�6_   	 �� CornerRadius$	 3��6m   ._�5w   :   �� $��11(�6Z   ��6D   �� *Name+6Z   5x   7   5y   3   5{   +   5|   '   � 5   &   $��5��6+   $��0,10,0,0q�68    5�   *   �� 5�   .   $��* �6?   �� 5�   .   $
��Auto �6?   .+�5�   *   �� $��0��64   $��Out of Stock��6D   
���  ��$
��Bold"�5�   E   $��Center=�5�   4   y�5�   4   �� *IsAvailable+ ' #InverseBooleanToVisibilityConverter# [ �� 5�   *   $��1��61   $��Add to Cart��6A   y�5�   1   �� *DataContext.IsClient+
 �� RelativeSource
 ��  �� AncestorType ItemsControl   BooleanToVisibilityConverter# [ ��5�   1   �� *DataContext.AddToCartCommand+
 ��  ItemsControl��5�   1   �� {�5�   1   �� *IsAvailable+5�   '   5�   #   5�      5�      5�      5�      �� 5�      $��1��6   $*��$Select a category to view menu items��5�      $��Center=�5�      $��Center=�5�      ���#999999$��16(�5�      y�5�      �� *MenuItems.Count+# [  �� ConverterParameter$ inverse���� 5�      $��1��6   ���	#80FFFFFFy�5�      �� *IsLoadingData+# [ ._�5�      �� $��Center=�6   $��Center=�68   .+�5�      �� $��
Loading...��6    $��18(�62   $��Center=�6@   � 5�       � IsIndeterminate$
 True��6"   $	��200��69   $��10��6E   $��0,10,0,0q�6Q   5�      5�      5�      �� 5�   
   $��1��6   $��0��6   $��2��6.   ���	#FFEFE0E0���	#FFCC0000$��1q�5�   @   $	 4��6T   $ 10q�5�      $��10q�6   $	��Top=�5�      $��Center=�6)   y�5�      �� *ErrorMessage+  NullToVisibilityConverter# [ ._�5�      �� ���	#FFCC0000$
��Wrap=�6M   ��6   �� *ErrorMessage+6M   5�      5�      5�      