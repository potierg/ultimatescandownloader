using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScansDownloader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IManga all_mangas;
        List<IChapter> list_mangas = new List<IChapter>();
        bool cancel = false;
        int last_page = 1;
        int selectedSite = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void refreshListManga(String sort)
        {           
            listMangas.Items.Clear();

            foreach (KeyValuePair<string, string> i in all_mangas.get_all_mangas())
            {
                String new_i = i.Key.ToLower();
                if (sort == "" || new_i.IndexOf(sort.ToLower()) != -1)
                    listMangas.Items.Add(i.Key);
            }
        }
        private void listSites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int select = listSites.SelectedIndex;
            selectedSite = select;
            if (select < 0)
                return;

            switch (select)
            {
                case 0:
                    all_mangas = new Japscan();
                    break;
                case 1:
                    all_mangas = new MangaReader();
                    break;
                case 2:
                    all_mangas = new MangaHere();
                    break;
                case 3:
                    all_mangas = new ReadManga();
                    break;
            }


            all_mangas.load_all_mangas();

            GridSites.Visibility = Visibility.Hidden;
            GridMangas.Visibility = Visibility.Visible;
            last_page = 2;

            refreshListManga("");
        }

        private void listMangas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Object obj = listMangas.SelectedItem;
            if (obj == null)
                return;
            String item = obj.ToString();

            int id_index = -1;
            int cpt = 0;
            foreach (KeyValuePair<String, String> i in all_mangas.get_all_mangas())
            {
                if (i.Key == item)
                {
                    id_index = cpt;
                    break;
                }
                cpt++;
            }

            if (id_index < 0)
                return;
            all_mangas.load_chapters(all_mangas.get_all_mangas()[id_index]);
            List<string[]> all_datas = all_mangas.get_chapters();

            GridMangas.Visibility = Visibility.Hidden;
            GridChapters.Visibility = Visibility.Visible;
            last_page = 3;

            listChapters.Items.Clear();


            foreach (string[] i in all_datas)
            {
                if (i[0] == "1")
                {
                    listChapters.Items.Add(i[2]);
                }
            }
        }

        private void RefreshWaitList()
        {
            if (list_mangas == null)
                return;
            WaitList.Items.Clear();
            foreach (IChapter i in list_mangas)
            {
                if (i != null)
                    WaitList.Items.Add(i.getDescription());
            }
            GridWaitList.Visibility = Visibility.Visible;
        }
        private void showWaitList_Click(object sender, RoutedEventArgs e)
        {
            RefreshWaitList();
            GridSites.Visibility = Visibility.Hidden;
            GridMangas.Visibility = Visibility.Hidden;
            GridChapters.Visibility = Visibility.Hidden;
        }

        private void returnSites_Click(object sender, RoutedEventArgs e)
        {
            GridSites.Visibility = Visibility.Visible;
            GridMangas.Visibility = Visibility.Hidden;
            last_page = 1;
            listSites.SelectedIndex = -1;
            all_mangas.delete();
        }

        private void returnMangas_Click(object sender, RoutedEventArgs e)
        {
            GridMangas.Visibility = Visibility.Visible;
            GridChapters.Visibility = Visibility.Hidden;
            last_page = 2;
            listMangas.SelectedIndex = -1;
            Search.Text = "";
        }

        private void addWaitList_Click(object sender, RoutedEventArgs e)
        {
            foreach (String item in listChapters.SelectedItems)
            {
                IChapter chap = null;
                switch (selectedSite)
                {
                    case 0:
                        chap = new ChapJapscan(all_mangas, item);
                        break;
                    case 1:
                        chap = new ChapMangaReader(all_mangas, item);
                        break;
                    case 2:
                        chap = new ChapMangaHere(all_mangas, item);
                        break;
                    case 3:
                        chap = new ChapReadManga(all_mangas, item);
                        break;
                }
                list_mangas.Add(chap);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GridWaitList.Visibility = Visibility.Hidden;
            switch (last_page)
            {
                case 1:
                    GridSites.Visibility = Visibility.Visible;
                    break;
                case 2:
                    GridMangas.Visibility = Visibility.Visible;
                    break;
                case 3:
                    GridChapters.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void RemoveWaitList_Click(object sender, RoutedEventArgs e)
        {
            foreach (String item in WaitList.SelectedItems)
            {
                int index = 0;
                foreach (IChapter i in list_mangas)
                {
                    if (i.getDescription() == item)
                    {
                        list_mangas.RemoveAt(index);
                        break;
                    }
                    index++;
                }
            }
            RefreshWaitList();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Int16 timeout;
            Boolean hasBeenDownloaded;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            BackButton.IsEnabled = false;

            String path = dialog.SelectedPath + "/";

            StopButton.Visibility = Visibility.Visible;
            StartButton.Visibility = Visibility.Hidden;
            cancel = false;

            while (list_mangas.Count > 0)
            {
                IChapter i = list_mangas.First();
                timeout = 0;
                hasBeenDownloaded = false;
                while (!hasBeenDownloaded)
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(path + i.getMangaName());
                        String[] chapters = i.get_chapters_details();

                        System.IO.Directory.CreateDirectory(path + chapters[1]);
                        CurrentElem.Content = chapters[0];
                        List<String[]> pages = i.get_pages_details(chapters[2], path + chapters[1], chapters[4]);

                        foreach (String[] details_page in pages)
                        {
                            if (cancel == true)
                            {
                                cancel = false;
                                return;
                            }
                            ProgressPage.Value = Int32.Parse(details_page[0]);
                            CurrentPage.Content = "Page " + details_page[2];
                            i.download_one_scan(details_page[1], details_page[2], details_page[3], path + chapters[1]);
                            await Task.Delay(10);
                        }
                        hasBeenDownloaded = true;
                    }
                    catch (System.Net.WebException)
                    {
                        System.Threading.Thread.Sleep(30000);
                        ++timeout;
                    }
                    if (timeout == 5)
                    {
                        stop_action();
                        return;
                    }

                }


                if (list_mangas.Count > 0)
                    list_mangas.RemoveAt(0);
                RefreshWaitList();
            }
            stop_action();
        }

        private void stop_action()
        {
            BackButton.IsEnabled = true;
            StopButton.Visibility = Visibility.Hidden;
            StartButton.Visibility = Visibility.Visible;

            CurrentElem.Content = "";
            CurrentPage.Content = "";
            ProgressPage.Value = 0;

            cancel = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            stop_action();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            String text = Search.Text;
            refreshListManga(text);
        }
    }
}
