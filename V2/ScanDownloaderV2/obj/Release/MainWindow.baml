   M S B A M L   `   `   `  ���� K  GScanDownloaderV2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:ScanDownloaderV2ScanDownloaderV2  !    ScanDownloaderV2.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     0localclr-namespace:ScanDownloaderV2       � Title$  UltimateScanDownloader��$	��628��$��1016.5��.��� .+��� -    ��Name$ 
tabControl��.@��� $��	Navigator��.��� ���	#FFE5E5E5.+�� $��27��$	��Top=�$��0,4,0,0q�.+��� -    ��Name$ ListSite��$
��Left=�$	��Top=�$	��120��$��23��.@��� $��Choose Site�� �� 
IsSelected . $
��Left=�$	��118���� $��Japscan��$
��Left=�$	��118���� $��MangaReader��$
��Left=�$	��118���� $��	MangaHere��$
��Left=�$	��118���� $��	ReadManga��$
��Left=�$	��118���� -    ��Name$ RefreshButton��$��Refresh��$
��Left=�$��	125,0,0,0q�$	��Top=�$��80��$��23���� -    ��Name$ 	SearchBox��$��23��$��
212,0,98,0q� �� TextWrapping$
 Wrap=�$�� ��$	��Top=��� -   $ buttonDownloadSelect��$��Download��$��0,0,10,4q�$��Bottom=� J� RenderTransformOrigin$ 0,-0.1&�$��Right=�$��83��$��23��� -    �Name$ 
GridLetter��$��	0,32,10,0q�$��26��$	��Top=�� -   $ 	gridSagas��$��0,58,0,0q�.+��� -   	 ��Name$	 	listSagas��
 �� SelectionMode$
 Extended=�����  �� 
TargetType" �� �� BasedOn# [B��.�-	   � -
   $ gridOneSaga��$��0,58,0,0q�$y�Hidden=�.+�� $	��164��$	��Top=�.+��� -    ��Name$ 	CoverSaga��$
��Left=�$	��144��$��	10,10,0,0q�$	��Top=�$	��121���� -    ��Name$ 	TitleSaga��$��151,10,103,0q�$
��Wrap=�$	��Top=�$��39��$��20(��� $�Title���� �� �� �� �� �� �� -   $ DetailsSaga��$��151,49,103,0q�$
��Wrap=�$��Détail��$	��Top=�$��15(��� -   $ 
buttonBack��$
��Back��$��	0,10,10,0q�$	��Top=�$��39��$��Right=�$��88���� -   $ buttonDownloadSaga��$��Download Saga��$��	0,54,10,0q�$	��Top=�$��39��$��Right=�$��88���� $��80��$��151,74,117,0q�$	��Top=�$
��Auto=�$��Disabled=�.���� -   $ 
ResumeSaga��$��Stretch=�$
��Wrap=�$��15(�� $��10,169,10,10q�.+��� -   $	 listBook��$
 Extended=�$��0,0,0,53q��� -   $ ButtonAddDownload��$��Add to Download��$
��Left=�$��0,0,0,10q�$	��150��$��38��$��Bottom=��� $��
Downloader��.��� ���	#FFE5E5E5.+�� $��28��$	��Top=�$��0,5,0,0q�.+��� -   $ textPath��$��28��$
 Wrap=�$�� ��$	��Top=�$��	0,0,237,0q� �� 
IsReadOnly . �� -   $ ButtonExplorer��$	��...��$��Right=�$��	0,0,194,0q�$	��Top=�$��38��$��28��$ 3.632,0.929&��� -   $ ButtonStart��$��Start��$��Right=�$��	0,0,102,0q�$	��Top=�$��87��$��28���� -   $ 
Buttonstop��$
��Stop��$��0,0,10,0q�$	��Top=�$��28��$��Right=�$��87���� -   $	 listBoxDownload��$��58,38,0,248.4q�$��Stretch=�$��Visible=��� -   $ 	textBlock��$
��Left=�$��
10,0,0,227q�$
��Wrap=�$��Erreurs��$��16��$��Bottom=��� -   $	 listBoxErrors��$	��212��$��0,0,0,10q�$��Bottom=�� $
��Left=�$��6,38,0,248.4q�$��47��$��Center=�.+��� -   $ ButtonUpDownload��$
��Left=�$��47��$��47��$��0,-33,0,33.2q�   MyButton#��[  ����  �� ImageSource$ Ressources/ArrowUp.png���� -   $ ButtonDownDownload��$
��Left=�$��47��$��47��$��0,19,0,-18.8q�#��[  ���� $ Ressources/ArrowDown.png��