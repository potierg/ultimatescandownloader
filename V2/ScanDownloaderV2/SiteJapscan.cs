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
            name_file = "Japscan.json";
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
            this.createFile();
        }

        public override List<String> prepareDownload(Chapters chapitre)
        {
            String link = chapitre.getLink();

            string[] content = HtmlRequest.get_html(link);
            int nb_page = 0;

            List<String> listlinkPage = new List<string>();

            foreach (String i in content)
            {
                if (i.IndexOf("data-img") != -1 && i.IndexOf("itemprop") == -1 && i.IndexOf("__Add__") == -1)
                {
                    listlinkPage.Add(HtmlRequest.cut_str(i, "value=\"", "\">"));
                    nb_page++;
                }
            }
            chapitre.setMax(nb_page);
            return listlinkPage;
        }

        public override void downloadScan(String link, int nb_page, Chapters chapitre, String path)
        {
            string[] content = HtmlRequest.get_html("http://www.japscan.com" + link);
            foreach (String j in content)
            {
                if (j.IndexOf("itemprop=\"image\"") != -1)
                {
                    MyPage p = new MyPage(nb_page, chapitre.getMax(), HtmlRequest.cut_str(j, "src=\"", "\"/>"), chapitre.getNumber(), chapitre.isChapter());
                    p.download(path);
                    p = null;
                }
            }
        }
    }
}
