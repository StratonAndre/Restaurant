   M S B A M L   `   `   `  ����L  HRestaurantManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:RestaurantManager.ViewsRestaurantManager.Views  5   	   K*clr-namespace:RestaurantManager.ViewModelsRestaurantManager.ViewModels  5   	   _4clr-namespace:RestaurantManager.Utilities.Converters&RestaurantManager.Utilities.Converters  5   	   (    "RestaurantManager.Views.MainWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:RestaurantManager.Views     5   	   9vm*clr-namespace:RestaurantManager.ViewModels     5   	   K
converters4clr-namespace:RestaurantManager.Utilities.Converters     5   	   $	��750��5   	   $
��1200��5   	     � WindowStartupLocation$  CenterScreen=�5   	    � Title 5
   	   �� *Title+ � 	Resources 5      I   CRestaurantManager.Utilities.Converters.BooleanToVisibilityConverter    BooleanToVisibilityConverterP   JRestaurantManager.Utilities.Converters.InverseBooleanToVisibilityConverter ' #InverseBooleanToVisibilityConverterH   BRestaurantManager.Utilities.Converters.NumberToVisibilityConverter  NumberToVisibilityConverter��%W   &	        &	      &	 &       5   
   62     5   
   69     5   
   61   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   � $��0��6   ���#2E3137 � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5       $��* �6#   �� 5!      $
��Auto �6#   .+�5%      �� $��0��6    �� Orientation$ 
Horizontal=�6)   $��10q�6B   .+�5&      �� $��🍽️��6   $��24(�6'   $��Center=�65   $��0,0,10,0q�6P   
�������6b   �� 5'      $��Restaurant Manager��6   $��Center=�66   $��20(�5(      $
��Bold"�6*   
�������6<   5)      �� 5,      $��1��6   $ 
Horizontal=�6)   $��Center=�6B   y�5-      �� *IsMenuVisible+ �� 	Converter# [  .+�5/      �� $
��Menu��6   $��5q�6(   $
��10,5q�63   $
��Menu��50   =   ��6   �� *NavigateCommand+6=   �� 51      $��Search��6   $��5q�6*   $
��10,5q�65   $��Search��52   =   ��6   �� *NavigateCommand+6=   �� 55      $��	My Orders��6   $��5q�6-   $
��10,5q�68   $��CustomerOrders��56   =   ��6   �� *NavigateCommand+y�57      �� *
IsClient+# [  �� 58      $��
My Profile��6   $��5q�6.   $
��10,5q�69   $��CustomerProfile��59   =   ��6   �� *NavigateCommand+y�5:      �� *
IsClient+# [  �� 5=      $��	Dashboard��6   $��5q�6-   $
��10,5q�68   $��AdminDashboard��5>   =   ��6   �� *NavigateCommand+y�5?      �� *
IsEmployee+# [  �� 5@      $��
Categories��6   $��5q�6.   $
��10,5q�69   $��CategoryManagement��5A   =   ��6   �� *NavigateCommand+y�5B      �� *
IsEmployee+# [  �� 5C      $��Dishes��6   $��5q�6*   $
��10,5q�65   $��DishManagement��5D   =   ��6   �� *NavigateCommand+y�5E      �� *
IsEmployee+# [  �� 5F      $��Menus��6   $��5q�6)   $
��10,5q�64   $��MenuManagement��5G   =   ��6   �� *NavigateCommand+y�5H      �� *
IsEmployee+# [  �� 5I      $��	Allergens��6   $��5q�6-   $
��10,5q�68   $��AllergenManagement��5J   =   ��6   �� *NavigateCommand+y�5K      �� *
IsEmployee+# [  �� 5L      $��Orders��6   $��5q�6*   $
��10,5q�65   $��OrderManagement��5M   =   ��6   �� *NavigateCommand+y�5N      �� *
IsEmployee+# [  5O      �� 5R      $��2��6   $ 
Horizontal=�6)   $��10q�6B   .+�5T      �� $��5q�6   $
��10,5q�6$   $
��Cart��5U   =   ��6   �� *NavigateCommand+y�5V      �� *
IsClient+# [  .��5W      �� $ 
Horizontal=�6!   .+�5X      �� $
��🛒��6$   $��14(�6.   $��Center=�6<   �� 5Y      $
��Cart��6$   $��Center=�60   $��5,0,0,0q�6K   �� 5Z      
���  �� �� CornerRadius$ 10��62   $��5,0,0,0q�6D   y�5[   !   �� *CartItemCount+# [ ._�5\      �� 
�������$	��5,2q�5]   (   $��Center=�65   $��Center=�6P   ��5\   (   �� *CartItemCount+5]   P   5^      5_      5`      �� 5c      $ 
Horizontal=�6   $��5q�66   .+�5e      �� $ 
Horizontal=�6!   y�5f   !   �� *IsUserAuthenticated+# [  .+�5g      �� $
��👤��6$   $��14(�6.   $��Center=�6<   
�������6W   �� 5h      $��Center=�6J   $	��5,0q�6e   
���������6$   �� *CurrentUser.FullName+6r   �� 5i      $��Logout��6!   $��5,0,0,0q�62   $
��10,5q�6C   ��6R   �� *LogoutCommand+5j      �� 5m      $ 
Horizontal=�6!   y�5n   !   �� *IsUserAuthenticated+# [ .+�5o      �� $��Login��6!   $��0q�61   $
��10,5q�6<   $��Login��5p   E   ��6!   �� *NavigateCommand+6E   �� 5q      $��Register��6!   $��5,0,0,0q�64   $
��10,5q�6E   $��Register��5r   E   ��6!   �� *NavigateCommand+6E   5s      5t      5u      5v      �� 5y   
   $��1��6   $��10q�6K   ��6&   �� *CurrentViewModel+6K   �� 5|   
   $��2��6   ���#2E3137.@�5}      �� $7��1© 2025 Restaurant Manager - WPF MVVM Application��6   
�������$
��10,5q�6c   5~      5      5�      