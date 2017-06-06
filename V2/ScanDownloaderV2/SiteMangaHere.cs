using ScanDownloaderV2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ScansDownloaderV2
{
    public class SiteMangaHere : ISite
    {
        String link_list_manga { get; set; }

        public SiteMangaHere()
        {
            link_list_manga = "http://www.mangahere.co/mangalist/";
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

        public override List<String> prepareDownload(Chapters chapitre)
        {
            String link = chapitre.getLink();

            string[] content = HtmlRequest.get_html(link);
            int nb_page = 0;

            List<String> listlinkPage = new List<string>();

            int stop = 0;

            foreach (String i in content)
            {
                if (i.IndexOf("class=\"next_page\">") != -1)
                    stop = 1;
                if (i.IndexOf("<option value=\"") != -1 && stop == 0)
                {
                    if (nb_page > 0)
                        listlinkPage.Add(HtmlRequest.cut_str(i, "value=\"", "\" >"));
                    else
                        listlinkPage.Add(HtmlRequest.cut_str(i, "value=\"", "\" selected"));
                    nb_page++;
                }
            }
            chapitre.setMax(nb_page);
            return (listlinkPage);
        }

        public override void downloadScan(String link, int nb_page, Chapters chapitre, String path)
        {
            string[] content2 = HtmlRequest.get_html(link);
            int reset = 0;
            while (content2.Length < 75 && reset < 5)
            {
                System.Threading.Thread.Sleep(1000);
                content2 = null;
                content2 = HtmlRequest.get_html(link);
                reset++;
            }
            foreach (String j in content2)
            {
                if (j.IndexOf("id=\"image\"") != -1)
                {
                    MyPage p = new MyPage(nb_page, chapitre.getMax(), HtmlRequest.cut_str(j, "src=\"", "\" onload="), chapitre.getNumber(), chapitre.isChapter());
                    p.download(path);
                    p = null;
                }
            }
       }
    }
}
