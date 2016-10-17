using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScansDownloader
{
    public class MangaHere : IManga
    {
        String link_list_manga { get; set; }

        public MangaHere()
        {
            list_chapter = new List<String[]>();
            link_list_manga = "http://www.mangahere.co/mangalist/";
        }

        public MangaHere(MangaHere org)
        {
            name = String.Copy(org.name);
            link = String.Copy(org.link);
            nb_tomes = org.nb_tomes;
            start = String.Copy(org.start);
            end = String.Copy(org.end);
            list_chapter = new List<String[]>(org.list_chapter);
            nb_chapter = org.nb_chapter;
            site_link = String.Copy(org.site_link);
            all_manga_page = String.Copy(org.all_manga_page);
            path_img = String.Copy(org.path_img);
        }

        public override void load_all_mangas()
        {
            allManga = new List<KeyValuePair<String, String>>();

            String[] content = HtmlRequest.get_html(link_list_manga);

            if (content == null)
            {
                allManga = null;
                return;
            }

            foreach (String i in content)
            {
                String[] stringSeparators = new String[] { "</li><li>" };
                if (i.IndexOf("<li><a class=\"manga_info\" rel=\"") != -1)
                {
                    String[] manga;
                    manga = i.Split(stringSeparators, StringSplitOptions.None);
                    foreach (String m in manga)
                    {
                        String name = HtmlRequest.cut_str(m, "rel=\"", "\" href=").Replace("&quot;", "\"").Replace("&amp;", "&");
                        String link = HtmlRequest.cut_str(m, "\" href=\"", "\"><span>");
                        allManga.Add(new KeyValuePair<String, String>(name, link));
                    }
                }
            }
        }

        override public List<KeyValuePair<String, String>> get_all_mangas()
        {
            return allManga;
        }

        public override void load_chapters(KeyValuePair<String, String> infos)
        {
            Debug.WriteLine("Load Chapter");
            String[] html = HtmlRequest.get_html(infos.Value);

            List<String[]> array_positions = new List<string[]>();

            this.name = infos.Key;
            this.link = infos.Value;

            list_chapter.Clear();
            int is_chapter_list = 0;
            int is_start = 0;

            String[] tab;

            String link = "";
            String desc;

            foreach (String i in html)
            {
                if (i.IndexOf("has been licensed") != -1)
                {
                    tab = new String[3];

                    tab[0] = "1";
                    tab[1] = "";
                    tab[2] = "Manga has been licensed";

                    list_chapter.Add(tab);
                    break;
                }

                if (i.IndexOf("<div class=\"detail_list\">") != -1)
                {
                    is_chapter_list = 1;
                    Debug.WriteLine(i);
                }

                if (i.IndexOf("<a class=\"color_0077\"") != -1 && is_chapter_list == 1)
                {
                    is_start = 1;
                    link = HtmlRequest.cut_str(i, "href=\"", "\" >");
                }
                if (i.IndexOf("</a>") != -1 && is_start == 1)
                {
                    is_start = 0;
                    desc = i.Substring(0, i.IndexOf("</a>"));

                    tab = new String[3];

                    tab[0] = "1";
                    tab[1] = link;
                    tab[2] = desc;

                    list_chapter.Insert(0, tab);
                }
                if (i.IndexOf("class=\"tab_comment clearfix\">") != -1 && is_chapter_list == 1)
                {
                    is_chapter_list = 0;
                }
            }
            nb_tomes = 0;
        }

        public override void set_delimeter(int pos_start, int pos_end)
        {
            int pos = -1;
            int is_start = 0;
            nb_chapter = 0;

            foreach (String[] i in list_chapter)
            {
                if (i[0] == "1")
                {
                    pos += 1;
                    if (pos == pos_start)
                    {
                        start = i[2];
                        is_start = 1;
                    }
                    if (pos == pos_end)
                    {
                        end = i[2];
                        is_start = 0;
                    }
                    if (is_start == 1)
                        nb_chapter += 1;
                }
            }
            return;
        }

        public override void download_one_scan(String link, String nb_page, int chapter, String path)
        {
            String[] content = HtmlRequest.get_html(link);

            foreach (String i in content)
            {
                if (i.IndexOf("<img id=\"img\"") != -1)
                {
                    String link_img = HtmlRequest.cut_str(i, "src=\"", "\" alt");
                    int pos_point = link_img.LastIndexOf('.');
                    String ext = link_img.Substring(pos_point);

                    String nb_chapter = "";
                    if (chapter < 10)
                        nb_chapter = "0" + chapter.ToString();

                    if (Int32.Parse(nb_page) < 10)
                        nb_page = "0" + nb_page;

                    HtmlRequest.save_image(link_img, path_img + "chap " + nb_chapter + " page " + nb_page + ext);
                }
            }
        }

        public override List<String[]> get_pages_details(String link, String path, int chap)
        {
            String[] content = HtmlRequest.get_html("http://www.mangareader.net" + link);

            List<String[]> ret_pages = new List<string[]>();

            double nb_pages = 0;
            int is_start = 0;
            foreach (String i in content)
            {
                if (i.IndexOf("<div id=\"selectpage\">") != -1)
                    is_start = 1;
                if (i.IndexOf("<option value=") != -1 && is_start == 1)
                    nb_pages += 1;
            }

            int cpt_page = 1;
            while (cpt_page <= nb_pages)
            {
                String page = cpt_page.ToString();
                if (cpt_page < 10)
                    page = "0" + cpt_page.ToString();


                ret_pages.Add(new String[4] { ((int)((cpt_page / nb_pages) * 100.0)).ToString(), "http://www.mangareader.net" + link + "/" + cpt_page.ToString(), cpt_page.ToString(), chap.ToString() });
                cpt_page += 1;
            }

            return ret_pages;
        }


        public override List<String[]> get_chapters_details()
        {
            int is_start = 0;
            double cpt_chapter = 0;

            String title = "";

            List<String[]> ret_chapters = new List<String[]>();

            title = name;

            foreach (String[] i in list_chapter)
            {
                if (i[0] == "1")
                {
                    if (start == i[2])
                        is_start = 1;

                    if (is_start == 1)
                    {
                        int progressValue;

                        if (cpt_chapter > 0)
                            progressValue = (int)((cpt_chapter / nb_chapter) * 100.0);
                        else
                            progressValue = 0;

                        path_img = "./" + name + "/";

                        int chap = Int32.Parse(Regex.Match(i[2], @"\d+").Value);

                        ret_chapters.Add(new String[6] { progressValue.ToString(), title, path_img, i[1], i[2], chap.ToString() });

                        cpt_chapter++;
                    }

                    if (end == i[2])
                        is_start = 0;

                }
            }

            return (ret_chapters);
        }
    }
}
