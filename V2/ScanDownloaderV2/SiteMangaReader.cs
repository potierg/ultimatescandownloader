using ScanDownloaderV2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ScansDownloaderV2
{
    public class SiteMangaReader : ISite
    {
        String link_list_manga { get; set; }

        public SiteMangaReader()
        {
            link_list_manga = "http://www.mangareader.net/alphabetical";
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

        public override List<MyPage> prepareDownload(Chapters chapitre)
        {
            String link = chapitre.getLink();

            string[] content = HtmlRequest.get_html(link);
            int nb_page = 0;
            String link_first_img = "";

            List<String> listlinkPage = new List<string>();
            List<MyPage> listPages = new List<MyPage>();

            foreach (String i in content)
            {
                if (i.IndexOf("<option value=\"") != -1)
                {
                    if (nb_page > 0)
                        listlinkPage.Add("http://www.mangareader.net" + HtmlRequest.cut_str(i, "value=\"", "\">"));
                    nb_page++;
                }
                if (i.IndexOf("<img") != -1)
                    link_first_img = HtmlRequest.cut_str(i, "src=\"", "\" alt=");
            }

            listPages.Add(new MyPage(0, nb_page, link_first_img, chapitre.getNumber(), chapitre.isChapter()));
            int p = 1;

            foreach (String l in listlinkPage)
            {
                string[] content2 = HtmlRequest.get_html(l);
                foreach (String j in content2)
                {
                    if (j.IndexOf("<img") != -1)
                        listPages.Add(new MyPage(p, nb_page, HtmlRequest.cut_str(j, "src=\"", "\" alt="), chapitre.getNumber(), chapitre.isChapter()));
                }
                content2 = null;
                p++;
            }
            content = null;
            listlinkPage = null;


            return listPages;
        }
    }
}
