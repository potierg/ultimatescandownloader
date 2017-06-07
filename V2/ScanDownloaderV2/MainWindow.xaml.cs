using Newtonsoft.Json;
using ScansDownloaderV2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ScanDownloaderV2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int selectedSite = 0;
        private List<ISite> allSites;

        private ISaga currentSaga;

        private List<KeyValuePair<String, String>> allMangas;
        private List<KeyValuePair<String, String>> printMangas;
        public List<Download> listDownload;

        private ProgressBar pg1;
        private ProgressBar pg2;

        public MainWindow()
        {
            InitializeComponent();
            initLetters();

            listDownload = new List<Download>();

            printMangas = new List<KeyValuePair<String, String>>();

            allSites = new List<ISite>();
            allSites.Add(new SiteJapscan());
            allSites.Add(new SiteMangaReader());
            allSites.Add(new SiteMangaHere());
            allSites.Add(new SiteReadManga());

            loadDownload();
        }

        private void initLetters()
        {
            Button b = new Button();
            b.Content = "0-9";
            b.Margin = new Thickness(0, 0, 0, 3);
            b.HorizontalAlignment = HorizontalAlignment.Left;
            b.Width = 27;
            b.Height = 20;
            b.Click += LetterButton_Click;

            GridLetter.Children.Add(b);

            for (int i = 1; i < 27; ++i)
            {
                char c = (char)(65 + i - 1);
                b = new Button();
                b.Content = c;
                b.Margin = new Thickness(32 * i, 0, 0, 3);
                b.HorizontalAlignment = HorizontalAlignment.Left;
                b.Width = 27;
                b.Height = 20;
                b.Click += LetterButton_Click;

                GridLetter.Children.Add(b);
            }
        }

        private void LetterButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSite <= 0)
                return;
            String content = (sender as Button).Content.ToString();
            int i = 0;
            int pos;
            foreach (KeyValuePair<String, String> manga in allMangas)
            {
                if (content == "0-9" && manga.Key[0] >= '0' && manga.Key[0] <= '9')
                {
                    pos = i;
                    if (i + 15 < allMangas.Count)
                        pos = i + 10;
                    listSagas.ScrollIntoView(listSagas.Items.GetItemAt(pos));
                    break;
                }
                else if ((manga.Key[0] >= 'a' && manga.Key[0] <= 'z') || (manga.Key[0] >= 'A' && manga.Key[0] <= 'Z'))
                {
                    if (manga.Key[0] == content[0])
                    {
                        pos = i;
                        if (i + 15 < allMangas.Count)
                            pos = i + 10;
                        if (pos < listSagas.Items.Count)
                            listSagas.ScrollIntoView(listSagas.Items.GetItemAt(pos));
                        break;
                    }
                }
                i++;
            }
        }

        private void ListSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListSite.SelectedIndex > 0)
            {
                this.selectedSite = ListSite.SelectedIndex;
                gridOneSaga.Visibility = Visibility.Hidden;
                if (allSites[selectedSite - 1].get_all_mangas() == null)
                    reloadSite(1, null);
                else
                    reloadSite(0, null);
                gridSagas.Visibility = Visibility.Visible;
            }
            else
                ListSite.SelectedIndex = this.selectedSite;
        }

        private void reloadSite(int is_reload, String search) // is_reload 0 - no, 1 - once, 2 - button
        {
            try
            {
                if (selectedSite <= 0)
                    return;
                if (is_reload > 0)
                {
                    allSites[selectedSite - 1].delete();
                    if (File.Exists(allSites[selectedSite - 1].getNameFile()) && is_reload != 2)
                        allSites[selectedSite - 1].loadFile();
                    else
                        allSites[selectedSite - 1].load_all_mangas();
                }
                allMangas = allSites[selectedSite - 1].get_all_mangas();

                listSagas.Items.Clear();

                Debug.WriteLine(is_reload + " - " + search);
                printMangas.Clear();

                foreach (KeyValuePair<String, String> pair in allMangas)
                {
                    if (search == null || pair.Key.ToLower().IndexOf(search.ToLower()) != -1)
                    {
                        printMangas.Add(pair);

                        ListBoxItem itm = new ListBoxItem();
                        itm.Content = pair.Key;

                        listSagas.Items.Add(itm);
                    }
                }
            } catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show("Erreur : "+e.Message + " Veut tu réeasayer", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (result == MessageBoxResult.Yes)
                    reloadSite(1, null);
            }
        }

        private ISaga getCurrentSaga()
        {
            currentSaga = null;
            switch (selectedSite)
            {
                case 1:
                    currentSaga = new SagaJapscan();
                    break;
                case 2:
                    currentSaga = new SagaMangaReader();
                    break;
                case 3:
                    currentSaga = new SagaMangaHere();
                    break;
                case 4:
                    currentSaga = new SagaReadManga();
                    break;
            }
            return currentSaga;
        }

        private void loadChapter()
        {
            if (listSagas.SelectedIndex < 0)
                return;

            gridSagas.Visibility = Visibility.Hidden;
            gridOneSaga.Visibility = Visibility.Visible;

            currentSaga = getCurrentSaga();

            int index_saga = listSagas.SelectedIndex;
            String value_saga = ((ListBoxItem)listSagas.SelectedItem).Content.ToString();

            Debug.WriteLine(value_saga);

            currentSaga.load_chapters(printMangas[listSagas.SelectedIndex]);
            List<Chapters> allBooks = currentSaga.get_chapters();

            if (allBooks.Count > 0 && allBooks[0].getIndex() == -1)
            {
                TitleSaga.Text = currentSaga.getName() + " - Attention, la série est licensié !";
                buttonDownloadSaga.IsEnabled = false;
            }
            else
            {
                buttonDownloadSaga.IsEnabled = true;
                TitleSaga.Text = currentSaga.getName();
            }


            DetailsSaga.Text = currentSaga.getAuteur() + " - " + currentSaga.getAnnée();
            ResumeSaga.Text = currentSaga.getResumé();

            if (currentSaga.getImgLink() != null)
            {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(currentSaga.getImgLink(), UriKind.RelativeOrAbsolute);
                bi3.CacheOption = BitmapCacheOption.OnLoad;
                bi3.EndInit();

                CoverSaga.Source = bi3;
            }
            else
                CoverSaga.Source = null;

            listBook.Items.Clear();

            foreach (Chapters c in allBooks)
            {
                if (c.getIndex() != -1)
                {
                    if (c.isChapter() == true)
                    {
                        if (c.getTitle() == null)
                            listBook.Items.Add("Chapitre " + c.getNumber() + " ");
                        else
                            listBook.Items.Add("Chapitre " + c.getNumber() + " : " + c.getTitle());
                    }
                    else
                    {
                        if (c.getTome().getTitle() != null)
                            listBook.Items.Add("Tome " + c.getNumber() + " : " + c.getTome().getTitle());
                        else
                            listBook.Items.Add("Tome " + c.getNumber() + " ");
                    }
                }
            }

        }

        private void ListSaga_MouseDown(object sender, MouseButtonEventArgs e)
        {
            loadChapter();
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            gridOneSaga.Visibility = Visibility.Hidden;
            gridSagas.Visibility = Visibility.Visible;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            reloadSite(2, null);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            gridOneSaga.Visibility = Visibility.Hidden;
            gridSagas.Visibility = Visibility.Visible;

            reloadSite(0, SearchBox.Text.ToString());
        }

        private void buttonDownloadSaga_Click(object sender, RoutedEventArgs e)
        {
            List<Chapters> allBooks = currentSaga.get_chapters();
            List<Chapters> newList = new List<Chapters>();
            for (int i = allBooks.Count - 1; i >= 0; --i)
                newList.Add(new Chapters(allBooks[i]));

            int index;
            if (listDownload.Count <= 0)
                index = 0;
            else
                index = listDownload.Last().index + 1;

            Download dl = new Download(index, selectedSite, currentSaga.getName(), currentSaga.getImgLink(), newList);
            listDownload.Add(dl);

            saveDownload();

            int id = listBoxDownload.Items.Count;
            addDownloadListBox(dl, id);
        }

        private void loadDownload()
        {

            if (!File.Exists("/tmp/Json/Download.json"))
                return;
            string json = File.ReadAllText("/tmp/Json/Download.json");

            listDownload = JsonConvert.DeserializeObject<List<Download>>(json);
            int i = 0;
            foreach (Download dl in listDownload)
            {
                addDownloadListBox(dl, i);
                i++;
            }
        }

        private void addDownloadListBox(Download dl, int index)
        {
            Grid g = new Grid();
            g.Height = 70;

            Image img = new Image();
            img.HorizontalAlignment = HorizontalAlignment.Left;
            img.Width = 70;

            if (dl.cover_path != null)
            {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(dl.cover_path, UriKind.RelativeOrAbsolute);
                bi3.CacheOption = BitmapCacheOption.OnLoad;
                bi3.EndInit();

                img.Source = bi3;
            }
            else
                img.Source = null;

            g.Children.Add(img);

            TextBlock tb1 = new TextBlock();
            tb1.Margin = new Thickness(75, 0, 0, 0);
            tb1.TextWrapping = TextWrapping.Wrap;
            tb1.Text = dl.name;
            tb1.VerticalAlignment = VerticalAlignment.Top;
            g.Children.Add(tb1);

            TextBlock tb2 = new TextBlock();
            tb2.Margin = new Thickness(75, 19, 0, 0);
            tb2.TextWrapping = TextWrapping.Wrap;
            tb2.Text = dl.nb_tome + " Tomes - " + dl.nb_chapter + " Chapitres";
            tb2.VerticalAlignment = VerticalAlignment.Top;
            g.Children.Add(tb2);

            if (index == 0)
            {
                pg1 = new ProgressBar();
                pg1.Margin = new Thickness(75, 0, 10, 20);
                pg1.Height = 10;
                pg1.VerticalAlignment = VerticalAlignment.Bottom;

                pg2 = new ProgressBar();
                pg2.Margin = new Thickness(75, 0, 10, 0);
                pg2.Height = 10;
                pg2.VerticalAlignment = VerticalAlignment.Bottom;

                g.Children.Add(pg1);
                g.Children.Add(pg2);
            }

            Button bt1 = new Button();
            bt1.Content = "E";
            bt1.HorizontalAlignment = HorizontalAlignment.Right;
            bt1.Margin = new Thickness(0, 1, 38, 0);
            bt1.VerticalAlignment = VerticalAlignment.Top;
            bt1.Width = 23;
            bt1.Tag = dl.index;
            bt1.Click += buttonEditDL_Click;

            g.Children.Add(bt1);

            Button bt2 = new Button();
            bt2.Content = "X";
            bt2.HorizontalAlignment = HorizontalAlignment.Right;
            bt2.Margin = new Thickness(0, 1, 10, 0);
            bt2.VerticalAlignment = VerticalAlignment.Top;
            bt2.Width = 23;
            bt2.Tag = dl.index;
            bt2.Click += buttonDeleteDL_Click;

            g.Children.Add(bt2);

            listBoxDownload.Items.Add(g);
        }

        private void buttonEditDL_Click(object sender, RoutedEventArgs e)
        {
            saveDownload();
        }

        private void buttonDeleteDL_Click(object sender, RoutedEventArgs e)
        {
            int index = (int)((Button)sender).Tag;

            for (int i = 0; i < listDownload.Count; ++i)
            {
                if (listDownload[i].index == index)
                    listDownload.RemoveAt(i);
            }

            saveDownload();

            listBoxDownload.Items.Clear();
            int id = 0;
            foreach (Download d in listDownload)
            {
                addDownloadListBox(d, id);
                id++;
            }
        }

        private void ButtonAddDownload_Click(object sender, RoutedEventArgs e)
        {
            List<Chapters> newList = new List<Chapters>();

            foreach (String item in listBook.SelectedItems)
            {
                Debug.WriteLine(item);
                foreach (Chapters chap in currentSaga.get_chapters())
                {
                    if (item.IndexOf("Tome " + chap.getNumber() + " ") != -1 || item.IndexOf("Chapitre " + chap.getNumber() + " ") != -1)
                    {
                        newList.Add(new Chapters(chap));
                    }
                }
            }

            int index;
            if (listDownload.Count <= 0)
                index = 0;
            else
                index = listDownload.Last().index + 1;

            Download dl = new Download(index, selectedSite, currentSaga.getName(), currentSaga.getImgLink(), newList);
            listDownload.Add(dl);

            saveDownload();

            addDownloadListBox(dl, listBoxDownload.Items.Count);
        }

        private void ButtonExplorer_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                textPath.Text = dialog.SelectedPath + "\\";
        }

        private async void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (textPath.Text == "")
                return;
            while (listDownload.Count > 0)
            {
                Download d = listDownload[0];
                d.name = d.name.Replace("/", "-");
                d.name = d.name.Replace("\\", "-");
                d.name = d.name.Replace(":", "-");
                d.name = d.name.Replace("*", ".");
                d.name = d.name.Replace("?", "!");
                d.name = d.name.Replace("\"", "-");
                d.name = d.name.Replace("<", "-");
                d.name = d.name.Replace(">", "-");
                d.name = d.name.Replace("|", "-");
                String path = textPath.Text + d.name + "\\";
                System.IO.Directory.CreateDirectory(path);
                int nb_chap_real = 0;
                foreach (Chapters c in d.chapters)
                {
                    String path2 = path;
                    if (c.getTome() != null)
                    {
                        if (c.getTome().getTitle() != null && c.getTome().getTitle() != "")
                        {
                            c.getTome().title = c.getTome().title.Replace(":", "-");
                            c.getTome().title = c.getTome().title.Replace("*", ".");
                            c.getTome().title = c.getTome().title.Replace("?", "!");
                            c.getTome().title = c.getTome().title.Replace("<", "-");
                            c.getTome().title = c.getTome().title.Replace(">", "-");
                            c.getTome().title = c.getTome().title.Replace("|", "-");

                            path2 = path2 + "Tome " + c.getTome().getNumber() + " - " + c.getTome().getTitle() + "\\";
                        }
                        else
                            path2 = path2 + "Tome " + c.getTome().getNumber() + "\\";

                        System.IO.Directory.CreateDirectory(path2);
                    }

                    List<String> pages_link = null;
                    int error = 0;

                        pages_link = allSites[d.site - 1].prepareDownload(c);

                    if (error == 0)
                    {
                        int nb_page_real = 0;
                        int nb_page_max = c.getMax();

                        foreach (String l in pages_link)
                        {
                            try
                            {
                                nb_page_real++;
                                allSites[d.site - 1].downloadScan(l, nb_page_real - 1, c, path2);
                                (pg1).Value = (int)((double)((double)(nb_page_real) / (double)(nb_page_max)) * 100.0);
                                await Task.Delay(100);
                            }
                            catch (Exception g)
                            {
                                listBoxErrors.Items.Add("Error : " + d.name + " " + c.getNumber() + " page " + nb_page_real);
                            }
                        }
                        nb_chap_real++;
                        (pg2).Value = (int)((double)((double)(nb_chap_real) / (double)(d.chapters.Count())) * 100.0);
                        await Task.Delay(10);
                    }

                    d.total_to_dl_real++;
                    saveDownload();
                }

                int pos = 0;
                for (int i = 0; i < listDownload.Count; ++i)
                {
                    if (listDownload[i].index == d.index)
                        listDownload.RemoveAt(pos);
                    pos++;
                }

                saveDownload();

                listBoxDownload.Items.Clear();
                foreach (Download d2 in listDownload)
                {
                    addDownloadListBox(d2, listBoxDownload.Items.Count);
                }
            }
        }

        private void buttonDownloadSelect_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                foreach (ListBoxItem o in listSagas.SelectedItems)
                {
                    foreach (KeyValuePair<String, String> m in printMangas)
                    {
                        if (o.Content.ToString() == m.Key)
                        {
                            ISaga newSaga = getCurrentSaga();

                            currentSaga.load_chapters(m);
                            List<Chapters> allBooks = currentSaga.get_chapters();

                            if (allBooks.Count > 0 && allBooks[0].getIndex() == -1)
                            {
                                newSaga = null;
                            }
                            else
                            {
                                List<Chapters> allChaps = newSaga.get_chapters();
                                List<Chapters> newList = new List<Chapters>();
                                for (int i = allBooks.Count - 1; i >= 0; --i)
                                    newList.Add(new Chapters(allBooks[i]));

                                int index;
                                if (listDownload.Count <= 0)
                                    index = 0;
                                else
                                    index = listDownload.Last().index + 1;

                                Download dl = new Download(index, selectedSite, currentSaga.getName(), currentSaga.getImgLink(), newList);
                                listDownload.Add(dl);

                                addDownloadListBox(dl, listBoxDownload.Items.Count);
                            }
                            break;
                        }
                    }
                }
                saveDownload();
            }));
        }

        public void saveDownload()
        {
            String json = JsonConvert.SerializeObject(listDownload);
            System.IO.Directory.CreateDirectory("/tmp/Json/");
            File.WriteAllText("/tmp/Json/Download.json", json);
            json = null;
        }
    }
}
