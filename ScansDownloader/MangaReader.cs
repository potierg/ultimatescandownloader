using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScansDownloader
{
    public class MangaReader : IManga
    {
        String link_list_manga { get; set; }

        public MangaReader()
        {
            list_chapter = new List<String[]>();
            link_list_manga = "http://www.mangareader.net/alphabetical";
        }

        public MangaReader(MangaReader org)
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
            int is_start = 0;

            if (content == null)
            {
                allManga = null;
                return;
            }

            foreach (String i in content)
            {
                if (i.IndexOf("<div class=\"series_col\"><a name=\" \"></a>") != -1)
                    is_start = 1;

                if (i.IndexOf("<li><b>Network</b></li>") != -1)
                    is_start = 0;

                if (i.IndexOf("<li><a href=\"/") != -1 && is_start == 1)
                {
                    String str;
                    if (i.IndexOf("<ul class=\"series_alpha\">") != -1)
                        str = i.Substring(25);
                    else
                        str = i;

                    String name = HtmlRequest.cut_str(str, "\">", "</a>");
                    String link = HtmlRequest.cut_str(str, "<li><a href=\"", "\">");
                    allManga.Add(new KeyValuePair<String, String>(name, "http://www.mangareader.net" + link));
                }
            }
        }

        override public List<KeyValuePair<String, String>> get_all_mangas()
        {
            return allManga;
        }

        public override void load_chapters(KeyValuePair<String, String> infos)
        {
            String[] html = HtmlRequest.get_html(infos.Value);

            List<String[]> array_positions = new List<string[]>();
            int found;

            name = infos.Key;
            link = infos.Value;

            list_chapter.Clear();
            int is_start = 0;

            String[] tab;

            foreach (String i in html)
            {
                found = i.IndexOf("<a href=\"/");


                if (i.IndexOf("<th class=\"leftgap\">Chapter Name</th>") != -1)
                    is_start = 1;

                if (found == 0 && is_start == 1)
                {
                    int end_pos = i.IndexOf("/\">");

                    String link = HtmlRequest.cut_str(i, "<a href=\"", "\">");
                    String desc = HtmlRequest.cut_str(i, "\">", "</a>") + " : " + HtmlRequest.cut_str(i, "</a> : ", "</td>");

                    tab = new String[3];

                    tab[0] = "1";
                    tab[1] = link;
                    tab[2] = desc;

                    list_chapter.Add(tab);

                }
            }
            nb_tomes = 0;
        }
    }
}
