using ScanDownloaderV2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ScansDownloaderV2
{
    public class SiteJapscan : ISite
    {
        String link_list_manga { get; set; }

        public SiteJapscan()
        {
            link_list_manga = "http://www.japscan.com/mangas/";
        }

        public override void load_all_mangas()
        {
            allManga = new List<KeyValuePair<String, String>>();

            string[] content = HtmlRequest.get_html(link_list_manga);

            if (content == null)
            {
                allManga = null;
                return;
            }

            foreach (String i in content)
            {
                if (i.IndexOf("<div class=\"cell\"><a href=\"/mangas/") != -1 && i.IndexOf("blackout") == -1)
                {
                    String name = HtmlRequest.cut_str(i, "/\">", "</a></div>");
                    String link = HtmlRequest.cut_str(i, "<a href=\"", "/\">");
                    allManga.Add(new KeyValuePair<String, String>(name, "http://www.japscan.com" + link + "/"));
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
                if (i.IndexOf("data-img") != -1 && i.IndexOf("itemprop") == -1 && i.IndexOf("__Add__") == -1)
                {
                    if (nb_page > 0)
                        listlinkPage.Add(HtmlRequest.cut_str(i, "value=\"", "\">"));
                    nb_page++;
                }
                if (i.IndexOf("itemprop=\"image\"") != -1)
                    link_first_img = HtmlRequest.cut_str(i, "src=\"", "\"/>");
            }

            listPages.Add(new MyPage(0, nb_page, link_first_img, chapitre.getNumber(), chapitre.isChapter()));
            int p = 1;

            foreach (String l in listlinkPage)
            {
                string[] content2 = HtmlRequest.get_html("http://www.japscan.com" + l);
                foreach (String j in content2)
                {
                    if (j.IndexOf("itemprop=\"image\"") != -1)
                        listPages.Add(new MyPage(p, nb_page,  HtmlRequest.cut_str(j, "src=\"", "\"/>"), chapitre.getNumber(), chapitre.isChapter()));
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
