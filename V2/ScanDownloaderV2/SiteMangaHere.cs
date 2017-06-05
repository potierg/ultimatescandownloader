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

        public override List<MyPage> prepareDownload(Chapters chapitre)
        {
            String link = chapitre.getLink();

            string[] content = HtmlRequest.get_html(link);
            int nb_page = 0;
            String link_first_img = "";

            List<String> listlinkPage = new List<string>();
            List<MyPage> listPages = new List<MyPage>();

            int stop = 0;

            foreach (String i in content)
            {
                if (i.IndexOf("class=\"next_page\">") != -1)
                    stop = 1;
                if (i.IndexOf("<option value=\"") != -1 && stop == 0)
                {
                    if (nb_page > 0)
                        listlinkPage.Add(HtmlRequest.cut_str(i, "value=\"", "\" >"));
                    nb_page++;
                }
                if (i.IndexOf("onload=\"loadImg(this)\"") != -1)
                    link_first_img = HtmlRequest.cut_str(i, "src=\"", "\" onload=");
            }

            listPages.Add(new MyPage(0, nb_page, link_first_img, chapitre.getNumber(), chapitre.isChapter()));
            int p = 1;

            foreach (String l in listlinkPage)
            {
                string[] content2 = HtmlRequest.get_html(l);
                foreach (String j in content2)
                {
                    if (j.IndexOf("onload=\"loadImg(this)\"") != -1)
                        listPages.Add(new MyPage(p, nb_page, HtmlRequest.cut_str(j, "src=\"", "\" onload="), chapitre.getNumber(), chapitre.isChapter()));
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
