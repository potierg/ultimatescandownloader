using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScanDownloaderV2
{
    /// <summary>
    /// Logique d'interaction pour EditDownload.xaml
    /// </summary>
    public partial class EditDownload : Window
    {
        private Download current_dl;
        private MainWindow ptr;

        public EditDownload(MainWindow ptr)
        {
            InitializeComponent();
            this.ptr = ptr;
        }

        public void setDownload(Download dl)
        {
            current_dl = dl;

            if (dl.cover_path != null)
            {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(dl.cover_path, UriKind.RelativeOrAbsolute);
                bi3.CacheOption = BitmapCacheOption.OnLoad;
                bi3.EndInit();

                imgCover.Source = bi3;
            }

            this.Title = "Edit " + dl.name;

            nameDL.Text = current_dl.name;            

            setDlChapList();
        }

        private void setDlChapList()
        {
            DetailsDL.Text = current_dl.nb_tome + " Tomes - " + current_dl.nb_chapter + " Chapitres";

            listChaptersDL.Items.Clear();
            foreach (Chapters c in current_dl.chapters)
            {
                if (c.getIndex() != -1)
                {
                    String tmp_content = "";
                    if (c.isChapter() == true)
                    {
                        if (c.getTitle() == null)
                            tmp_content = "Chapitre " + c.getNumber() + " ";
                        else
                            tmp_content = "Chapitre " + c.getNumber() + " : " + c.getTitle();
                    }
                    else
                    {
                        if (c.getTome().getTitle() != null)
                            tmp_content = "Tome " + c.getNumber() + " : " + c.getTome().getTitle();
                        else
                            tmp_content = "Tome " + c.getNumber() + " ";
                    }
                    if (c.date == null)
                        listChaptersDL.Items.Add(tmp_content);
                    else
                        listChaptersDL.Items.Add(c.date + " - " + tmp_content);
                }
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            current_dl = null;
            this.Close();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            IList selectItems = listChaptersDL.SelectedItems;
            List<String> newListItems = new List<String>();

            foreach (String item in selectItems)
                newListItems.Add(item);

            while (newListItems.Count > 0)
            {
                int i = 0;
                foreach (Chapters chap in current_dl.chapters)
                {
                    if (newListItems[0].IndexOf("Tome " + chap.getNumber() + " ") != -1 || newListItems[0].IndexOf("Chapitre " + chap.getNumber() + " ") != -1)
                    {
                        newListItems.RemoveAt(0);
                        if (current_dl.chapters[i].isChapter())
                            current_dl.nb_chapter--;
                        else
                            current_dl.nb_tome--;
                        current_dl.chapters.RemoveAt(i);
                        break;
                    }
                    ++i;
                }
            }
            ptr.saveDownload();
            ptr.isDownloadRefresh = true;
            setDlChapList();
        }
    }
}
