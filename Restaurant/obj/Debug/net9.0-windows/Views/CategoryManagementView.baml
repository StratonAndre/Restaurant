   M S B A M L   `   `   `  ����L  HRestaurantManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:RestaurantManager.ViewsRestaurantManager.Views  5   	   K*clr-namespace:RestaurantManager.ViewModelsRestaurantManager.ViewModels  5   	   _4clr-namespace:RestaurantManager.Utilities.Converters&RestaurantManager.Utilities.Converters  5   	   4    .RestaurantManager.Views.CategoryManagementView   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:RestaurantManager.Views     5   	   9vm*clr-namespace:RestaurantManager.ViewModels     5   	   K
converters4clr-namespace:RestaurantManager.Utilities.Converters     5   	     � 	Resources  5      I   CRestaurantManager.Utilities.Converters.BooleanToVisibilityConverter    BooleanToVisibilityConverterP   JRestaurantManager.Utilities.Converters.InverseBooleanToVisibilityConverter ' #InverseBooleanToVisibilityConverterF   @RestaurantManager.Utilities.Converters.NullToVisibilityConverter  NullToVisibilityConverterC   =RestaurantManager.Utilities.Converters.BooleanToTextConverter  BooleanToTextConverter;   5RestaurantManager.Utilities.Converters.PriceConverter  PriceConverterH   BRestaurantManager.Utilities.Converters.NumberToVisibilityConverter  NumberToVisibilityConverter��%�   &	        &	      &	 &     &	 9     &	 L     &	 _       5   
   62     5   
   69     5   
   6/     5   
   6,     5   
   6$     5   
   61   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $	��300 �6   �� 5      $��* �6   .+�5   
   �� $��0��6     	CardPanel#��[ 6!   $��10q�6D   ._�5      �  � RowDefinitions 5      �� 5      $
��Auto �6$   �� 5       $
��Auto �6$   �� 5!      $��* �6$   .+�5%      � $��0��6    5&      �� 5'      $��* �6+   �� 5(      $
��Auto �6+   .+�5+      �� $��0��6    $��
Categories��60     SectionTitle#��[ 5,       �� 5.      $��1��6   $��Add New��6-   ��5/      �� *AddNewCategoryCommand+50      � 53      $��1��6   $
��0,10q�6$   .+�54      �� $��5q�55      $��1q�56      ���#CCCCCC��54      �� *
SearchTerm+ �� UpdateSourceTrigger$ PropertyChanged=�57      �� 59      $��Search categories...��6    ���#999999$��7,0,0,0q�5;       $��Center=�5<       y�5=       �� *
SearchTerm+ �� 	Converter# [  �� ConverterParameter$ inverse��5>      �� 5A      $��2��6   $��0q�5D      
������ ��5B      �� *
Categories+ �� SelectedItem 5C      �� *SelectedCategory+��5F      �� 5G      .R�5H      � $	��0,5q�6#    5I   "   �� 5J   &   $��* �67   �� 5K   &   $
��Auto �67   .+�5N   "   �� $��0��6,   $��Center=�5O   ,   ��5N   <   �� *Name+5O   ,   �� 5Q   "   $��1��6,   ���#666666$��Center=�5T   ,   ��5R   ,   �� *	ItemCount+ �� StringFormat$ ({0})��5T   ,   5U      5V      5X      5Y      5Z      �� 5]   
   $��1��6   #��[ 6!   $��
0,10,10,10q�6D   ._�5^      � .+�5`      � y�6   �� *IsInEditMode+# [   5a      �� 5b      $
��Auto �6(   �� 5c      $��* �6(   �� 5d      $
��Auto �6(   .+�5h      �� $��0��6    #��[ 5i       ��5h   -   �� *IsAddingNew+# [ $$ Add New Category|Edit Category��5i       �� 5l      $��1��6!   $��0,20,0,0q�6.   .+�5n      �� $��0,0,0,15q�6%   .+�5o      �� $��Category Name *��6(   $��0,0,0,5q�6?   �� 5p      $��5q�5q   &   ��5p   &   �� *EditCategory.Name+$ PropertyChanged=�5q   &   5r      �� 5u      $��0,0,0,15q�6%   .+�5v      �� $��Description��6(   $��0,0,0,5q�6;   �� 5w       �� AcceptsReturn$
 True��5x   &   	 �� TextWrapping$
	 Wrap=�5y   &   $	��100��5z   &   $��5q�5{   &   ��5w   &   �� *EditCategory.Description+$ PropertyChanged=�5{   &   5|      5}      �� 5�      $��2��6!   
 �� Orientation$
 
Horizontal=�6.   $��Right=�6G   $��0,15,0,0q�6c   .+�5�      �� $��Cancel��6!     OutlineButton#��[ 5�   !   $��0,0,10,0q�5�   !   $
��20,5q�5�   !   ��5�   !   �� *CancelEditCommand+5�   !   �� 5�      $
��Save��6!    	 PrimaryButton#��[	 5�   !   $
��20,5q�5�   !   ��5�   !   �� *SaveCategoryCommand+5�   !   5�      5�      � 5�      y�6   �� *IsInEditMode+# [  5�      �� 5�      $
��Auto �6(   �� 5�      $
��Auto �6(   �� 5�      $��* �6(   .+�5�      �� $��0��6    $��3��6-   $5��/Select a category from the list to view details��5�       $��Center=�5�       $��Center=�5�       ���#999999$��16(�5�       y�5�       �� *SelectedCategory+# [ $ inverse��� 5�      $��0��6   y�5�      �� *SelectedCategory+# [  5�      �� 5�      $��* �6/   �� 5�      $
��Auto �6/   .+�5�      �� $��0��6$   #��[ 5�   $   ��5�   $   �� *SelectedCategory.Name+5�   $   �� 5�      $��1��6%   $
 
Horizontal=�65   .+�5�      �� $
��Edit��6%   #��[	 5�   %   $��0,0,10,0q�5�   %   ��5�   %   �� *EditCategoryCommand+5�   %   �� 5�      $��Delete��6%    
 DangerButton#��[
 5�   %   ��5�   %   �� *DeleteCategoryCommand+5�   %   5�      5�      �� 5�      $��1��6    $
��Wrap=�5�       $
��0,10q�5�       ��5�       �� *SelectedCategory.Description+y�5�       �� *SelectedCategory.Description+# [ � 5�      $��2��6   y�5�      �� *SelectedCategory+# [  5�      �� 5�      $
��Auto �6,   �� 5�      $��* �6,   .+�5�      �� $��0��6$   $��Items in this category��61     SubSectionTitle#��[ 5�   $   $��	0,10,0,10q�5�   $   �� 5�      $��1��6%   .@�5�      �� $��Dishes��6&   .��5�   "   &   System.Windows.Controls.DataGrid     AutoGenerateColumns$ False��5�   +      
IsReadOnly$
 True��5�   +      HeadersVisibility$ Column=�5�   +      GridLinesVisibility$ 
Horizontal=�5�   +   $��0q�5�   +      RowHeaderWidth$ 0Q�5�   +   ��5�   +   �� *CategoryDishes+   Columns 5�   &   0  *System.Windows.Controls.DataGridTextColumn  5�   *   ,	  &System.Windows.Controls.DataGridColumn 	  Header$
 Name��6=    	  Width5
  /System.Windows.Controls.DataGridLengthConverter$ *
 6d   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6K   �� *Name+6d     5�   *   $ Price��6=   $	 100
 6�    6L   �� *Price+# [ 6�     5�   *   $ Portion��6=   $	 100
 6�    6N   �� *PortionSize+$
 {0}g��6�     5�   *   $ Status��6=   $	 100
 6�    6M   �� *IsAvailable+# [ $ Available|Out of Stock��6�   5�   #   5�      �� 5�      $��Menus��6&   .��5�   "     $ False��5�   +   $
 True��5�   +   $ Column=�5�   +   $ 
Horizontal=�5�   +   $��0q�5�   +   $ 0Q�5�   +   ��5�   +   �� *CategoryMenus+ 5�   &     5�   *   $
 Name��6=   $ *
 6d    6K   �� *Name+6d     5�   *   $ Price��6=   $	 100
 6�    6L   �� *Price+# [ 6�     5�   *   $ Dishes��6=   $	 100
 6r    6M   �� *MenuDishes.Count+6r     5�   *   $ Status��6=   $	 100
 6�    6M   �� *IsAvailable+# [ $ Available|Out of Stock��6�   5�   #   5�      5�      5�      5�      5�      5�      �� 5�   
   $��0��6   $��2��6!   ���	#80FFFFFFy�5�      �� *IsLoadingData+# [  ._�5�      �� $��Center=�6   $��Center=�64   .+�5�      �� $��
Loading...��6   $��18(�6.   $��Center=�6<   � 5�       � IsIndeterminate$
 True��6   $	��200��65   $��10��6A   $��0,10,0,0q�6M   5�      5�      �� 5  
   $��0��6   $��2��6!   ���	#FFEFE0E0���	#FFCC0000$��1q�5  @    �� CornerRadius$ 4��6T    �� Padding$ 10q�5     $��10q�6   $	��Top=�5     $��Center=�6)   y�5     �� *ErrorMessage+# [ ._�5     �� ���	#FFCC0000$
��Wrap=�6M   ��6   �� *ErrorMessage+6M   5     5     5	     