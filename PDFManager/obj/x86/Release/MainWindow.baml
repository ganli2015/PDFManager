   M S B A M L   `   `   `  ����   
PDFManager    PDFManager.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Gdxco6http://schemas.devexpress.com/winfx/2008/xaml/controls     -     � Title$  
MainWindow��$	��350��$	��628��
������� � 
ResizeMode$ NoResize=� � 	Resources    bookTemplate��%4   &	        �� .R��� 
���i������ *Name+.��� $��0q� � ColumnDefinitions �� $
��242* ��� $
��364* �.+��� -   $��listBox_book��#���   $	��311��$	��Top=�
���i��$��1q� �� SelectionMode$ Extended=� � ForceCursor . |�.  $��1����Z� � -   $��grid1��$	��309��$
��Left=�$��1,2,0,0q�$	��Top=�$	��241�� � RowDefinitions �� $	��36* ��� $
��232* ��� $	��41* �.+��� -   $��comboBox_category��$��23��$
��Left=�$��20,6,0,0q�$	��Top=�$	��120��
��������� -   $��button_initialize��$��设置目录��$��2��$��23��$
��Left=�$��0,12,0,0q�$	��Top=�$��57��
��������� -   $��button_setLabel��$��设定标签��$��2��$��23��$
��Left=�$��
117,12,0,0q�$	��Top=�$��57��
��������� -   $��button_addLabel��$��新增标签��$��2��$��23��$
��Left=�$��
178,12,0,0q�$	��Top=�$��57��
��������� -   $��button_refreshDir��$��刷新目录��$��2��$��23��$
��Left=�$��	58,12,0,0q�$	��Top=�$��57��
��������� -	   $��border1��
���i��$��1q�$��1��$	��231��$
��Left=�$��20,1,0,0q�$	��Top=�$	��167����Z� ._��� -
   $��scrollViewer1��$��1��$��2��$	��230��$
��Left=�$��0,0,0,0q�$	��Top=�$	��167��
���i��$��1q�$
��Auto=���. .��� -   $��wrapPanel_label������  �� ElementName$ scrollViewer1�� �� Path$ Width�	 �� Mode$	 OneWay=���Z� 