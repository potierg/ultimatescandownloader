using ScanDownloaderV2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ScansDownloaderV2
{
    public class SiteReadManga : ISite
    {
        String link_list_manga { get; set; }

        public SiteReadManga()
        {
            link_list_manga = "http://www.readmanga.today/manga-list";
            name_file = "ReadManga.json";
        }

        public override void load_all_mangas()
        {
            allManga = new List<KeyValuePair<String, String>>();

            for (int i = -1; i < 26; ++i)
            {
                String tmp_link = link_list_manga;
                if (i >= 0)
                    tmp_link = tmp_link + "/" + Convert.ToChar('a' + i);

                String[] content = HtmlRequest.get_html(tmp_link);
                if (content == null)
                {
                    allManga = null;
                    return;
                }

                foreach (String j in content)
                {
                    if (j.IndexOf("<span class=\"manga-item\">") != -1)
                    {
                        String str = HtmlRequest.cut_str(j, "<span class=\"icon-dot\"></span>", "</a>");

                        String link = HtmlRequest.cut_str(str, "<a href=\"", "\">");
                        String name = str.Substring(str.IndexOf("\">") + 2);
                        allManga.Add(new KeyValuePair<String, String>(name, link));
                    }
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

            int nb_list = 0;

            foreach (String i in content)
            {
                if (i.IndexOf("<select name=\"category_type\" class=\"form-control input-sm jump-menu\">") != -1)
                    nb_list++;

                if (i.IndexOf("<option value=\"") != -1 && nb_list == 2)
                {
                    if (nb_page > 0)
                        listlinkPage.Add(HtmlRequest.cut_str(i, "value=\"", "\">"));
                    else
                        listlinkPage.Add(HtmlRequest.cut_str(i, "value=\"", "\" selected"));
                    nb_page++;
                }
            }
            chapitre.setMax(nb_page);
            return listlinkPage;
        }


        public override void downloadScan(String link, int nb_page, Chapters chapitre, String path)
        {
            string[] content2 = HtmlRequest.get_html(link);
            foreach (String j in content2)
            {
                if (j.IndexOf("class=\"img-responsive-2\"") != -1)
                {
                    MyPage p = new MyPage(nb_page, chapitre.getMax(), HtmlRequest.cut_str(j, "src=\"", "?u=\" class="), chapitre.getNumber(), chapitre.isChapter());
                    p.download(path);
                    p = null;
                }
            }
        }
    }
}
