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
    public class ReadManga : IManga
    {
        String link_list_manga { get; set; }

        public ReadManga()
        {
            list_chapter = new List<String[]>();
            link_list_manga = "http://www.readmanga.today/manga-list";
        }

        public ReadManga(ReadManga org)
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

        private void load_page_manga(String link)
        {
            String[] content = HtmlRequest.get_html(link);

            foreach (String i in content)
            {
                if (i.IndexOf("<span class=\"manga-item\">") != -1)
                {
                    String str = i.Trim();
                    String n_link = HtmlRequest.cut_str(str, "<a href=\"", "\">");
                    String name = HtmlRequest.cut_str(str, n_link + "\">", "</a>");
                    allManga.Add(new KeyValuePair<String, String>(name, n_link));
                }
            }
        }

        public override void load_all_mangas()
        {
            allManga = new List<KeyValuePair<String, String>>();
            load_page_manga(link_list_manga);

            for (char c = 'a'; c <= 'z'; c++)
            {
                load_page_manga(link_list_manga + "/" + c);
            }
            return;
        }

        override public List<KeyValuePair<String, String>> get_all_mangas()
        {
            return allManga;
        }

        public override void load_chapters(KeyValuePair<String, String> infos)
        {
            String[] html = HtmlRequest.get_html(infos.Value);

            List<String[]> array_positions = new List<string[]>();

            list_chapter.Clear();
            int is_start = 0;

            name = infos.Key;
            link = infos.Value;

            String[] tab;

            String n_link = "";
            String desc = "";

            foreach (String i in html)
            {
                String str = i.Trim();

                Debug.WriteLine(i);

                if (str.IndexOf("<ul class=\"chp_lst\">") != -1)
                    is_start = 1;

                if (is_start == 1 && str.IndexOf("<a href=\"") != -1)
                    n_link = HtmlRequest.cut_str(str, "<a href=\"", "\">");

                if (is_start == 1 && str.IndexOf("<span class=\"val\">") != -1)
                {
                    desc = HtmlRequest.cut_str(str, "\"></span>", " </span>");

                    tab = new String[3];

                    tab[0] = "1";
                    tab[1] = String.Copy(n_link);
                    tab[2] = String.Copy(desc);

                    list_chapter.Add(tab);
                }                
            }
            nb_tomes = 0;
        }
    }
}
