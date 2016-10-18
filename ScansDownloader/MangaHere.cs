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
                        String name = HtmlRequest.cut_str(m, "rel=\"", "\" href=").Replace("&quot;", "-").Replace("&amp;", "&");
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
    }
}
