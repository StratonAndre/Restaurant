   M S B A M L   `   `   `  ����L  HRestaurantManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:RestaurantManager.ViewsRestaurantManager.Views  5   	   K*clr-namespace:RestaurantManager.ViewModelsRestaurantManager.ViewModels  5   	   _4clr-namespace:RestaurantManager.Utilities.Converters&RestaurantManager.Utilities.Converters  5   	   4    .RestaurantManager.Views.AllergenManagementView   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:RestaurantManager.Views     5   	   9vm*clr-namespace:RestaurantManager.ViewModels     5   	   K
converters4clr-namespace:RestaurantManager.Utilities.Converters     5   	     � 	Resources  5      I   CRestaurantManager.Utilities.Converters.BooleanToVisibilityConverter    BooleanToVisibilityConverterP   JRestaurantManager.Utilities.Converters.InverseBooleanToVisibilityConverter ' #InverseBooleanToVisibilityConverterF   @RestaurantManager.Utilities.Converters.NullToVisibilityConverter  NullToVisibilityConverterC   =RestaurantManager.Utilities.Converters.BooleanToTextConverter  BooleanToTextConverter;   5RestaurantManager.Utilities.Converters.PriceConverter  PriceConverter��%�   &	        &	      &	 &     &	 9     &	 L       5   
   62     5   
   69     5   
   6/     5   
   6,     5   
   6$   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $	��300 �6   �� 5      $��* �6   .+�5   
   �� $��0��6     	CardPanel#��[ 6!   $��10q�6D   ._�5      �  � RowDefinitions 5      �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5       $��* �6$   .+�5$      � $��0��6    5%      �� 5&      $��* �6+   �� 5'      $
��Auto �6+   .+�5*      �� $��0��6    $��	Allergens��60     SectionTitle#��[ 5+       �� 5-      $��1��6   $��Add New��6-   ��5.      �� *AddNewAllergenCommand+5/      � 52      $��1��6   $
��0,10q�6$   .+�53      �� $��5q�54      $��1q�55      ���#CCCCCC��53      �� *
SearchTerm+ �� UpdateSourceTrigger$ PropertyChanged=�56      �� 58      $��Search allergens...��6    ���#999999$��7,0,0,0q�5:       $��Center=�5;       y�5<       �� *
SearchTerm+ �� 	Converter# [  �� ConverterParameter$ inverse��5=      �� 5@      $��2��6   $��0q�5C      
������ ��5A      �� *	Allergens+ �� SelectedItem 5B      �� *SelectedAllergen+��5E      �� 5F      .R�5G      � $	��0,5q�6#    5H   "   �� 5I   &   $��* �67   �� 5J   &   $
��Auto �67   .+�5M   "   �� $��0��6,   $��Center=�5N   ,   ��5M   <   �� *Name+5N   ,   �� 5P   "   $��1��6,   ���#666666$��Center=�5S   ,   ��5Q   ,   �� *	DishCount+ �� StringFormat$ ({0})��5S   ,   5T      5U      5W      5X      5Y      �� 5\   
   $��1��6   #��[ 6!   $��
0,10,10,10q�6D   ._�5]      � .+�5_      � y�6   �� *IsInEditMode+# [   5`      �� 5a      $
��Auto �6(   �� 5b      $��* �6(   �� 5c      $
��Auto �6(   .+�5g      �� $��0��6    #��[ 5h       ��5g   -   �� *IsAddingNew+# [ $$ Add New Allergen|Edit Allergen��5h       �� 5k      $��1��6!   $��0,20,0,0q�6.   .+�5m      �� $��0,0,0,15q�6%   .+�5n      �� $��Allergen Name *��6(   $��0,0,0,5q�6?   �� 5o      $��5q�5p   &   ��5o   &   �� *EditAllergen.Name+$ PropertyChanged=�5p   &   5q      �� 5t      $��0,0,0,15q�6%   .+�5u      �� $��Description��6(   $��0,0,0,5q�6;   �� 5v       �� AcceptsReturn$
 True��5w   &   	 �� TextWrapping$
	 Wrap=�5x   &   $	��100��5y   &   $��5q�5z   &   ��5v   &   �� *EditAllergen.Description+$ PropertyChanged=�5z   &   5{      5|      �� 5      $��2��6!   
 �� Orientation$
 
Horizontal=�6.   $��Right=�6G   $��0,15,0,0q�6c   .+�5�      �� $��Cancel��6!     OutlineButton#��[ 5�   !   $��0,0,10,0q�5�   !   $
��20,5q�5�   !   ��5�   !   �� *CancelEditCommand+5�   !   �� 5�      $
��Save��6!     PrimaryButton#��[ 5�   !   $
��20,5q�5�   !   ��5�   !   �� *SaveAllergenCommand+5�   !   5�      5�      � 5�      y�6   �� *IsInEditMode+# [  5�      �� 5�      $
��Auto �6(   �� 5�      $
��Auto �6(   �� 5�      $��* �6(   .+�5�      �� $��0��6    $��3��6-   $6��0Select an allergen from the list to view details��5�       $��Center=�5�       $��Center=�5�       ���#999999$��16(�5�       y�5�       �� *SelectedAllergen+# [ $ inverse��� 5�      $��0��6   y�5�      �� *SelectedAllergen+# [  5�      �� 5�      $��* �6/   �� 5�      $
��Auto �6/   .+�5�      �� $��0��6$   #��[ 5�   $   ��5�   $   �� *SelectedAllergen.Name+5�   $   �� 5�      $��1��6%   $
 
Horizontal=�65   .+�5�      �� $
��Edit��6%   #��[ 5�   %   $��0,0,10,0q�5�   %   ��5�   %   �� *EditAllergenCommand+5�   %   �� 5�      $��Delete��6%    	 DangerButton#��[	 5�   %   ��5�   %   �� *DeleteAllergenCommand+5�   %   5�      5�      �� 5�      $��1��6    $
��Wrap=�5�       $
��0,10q�5�       ��5�       �� *SelectedAllergen.Description+y�5�       �� *SelectedAllergen.Description+# [ � 5�      $��2��6   y�5�      �� *SelectedAllergen+# [  5�      �� 5�      $
��Auto �6,   �� 5�      $��* �6,   .+�5�      �� $��0��6$   $��Dishes with this allergen��61    
 SubSectionTitle#��[
 5�   $   $��	0,10,0,10q�5�   $   &   System.Windows.Controls.DataGrid  5�      $��1��6#      AutoGenerateColumns$ False��5�   #      
IsReadOnly$
 True��5�   #      HeadersVisibility$ Column=�5�   #      GridLinesVisibility$ 
Horizontal=�5�   #   $��0q�5�   #      RowHeaderWidth$ 0Q�5�   #   ��5�   #   �� *AllergenDishes+   Columns 5�      0  *System.Windows.Controls.DataGridTextColumn  5�   "   ,  &System.Windows.Controls.DataGridColumn   Header$
 Name��65      Width5	  /System.Windows.Controls.DataGridLengthConverter$ *	 6\   1
  +System.Windows.Controls.DataGridBoundColumn 
  Binding 6C   �� *Name+6\     5�   "   $ Category��65   $	 150	 6i    6G   �� *Category.Name+6i     5�   "   $ Price��65   $	 100	 6�    6D   �� *Price+# [ 6�     5�   "   $ Status��65   $	 100	 6�    6E   �� *IsAvailable+# [ $ Available|Out of Stock��6�   5�      5�      5�      5�      5�      �� 5�   
   $��0��6   $��2��6!   ���	#80FFFFFFy�5�      �� *IsLoadingData+# [  ._�5�      �� $��Center=�6   $��Center=�64   .+�5�      �� $��
Loading...��6   $��18(�6.   $��Center=�6<   � 5�       � IsIndeterminate$
 True��6   $	��200��65   $��10��6A   $��0,10,0,0q�6M   5�      5�      �� 5�   
   $��0��6   $��2��6!   ���	#FFEFE0E0���	#FFCC0000$��1q�5�   @    �� CornerRadius$ 4��6T    �� Padding$ 10q�5�      $��10q�6   $	��Top=�5�      $��Center=�6)   y�5�      �� *ErrorMessage+# [ ._�5�      �� ���	#FFCC0000$
��Wrap=�6M   ��6   �� *ErrorMessage+6M   5�      5�      5�      