using ScanDownloaderV2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ScansDownloaderV2
{
    public class SagaMangaReader : ISaga
    {
        public SagaMangaReader()
        {
            list_chapter = new List<Chapters>();
        }

        public SagaMangaReader(SagaMangaReader org)
        {
            name = String.Copy(org.name);
            link = String.Copy(org.link);
            nb_tomes = org.nb_tomes;
            start = String.Copy(org.start);
            end = String.Copy(org.end);
            list_chapter = new List<Chapters>(org.list_chapter);
            nb_chapter = org.nb_chapter;
            all_manga_page = String.Copy(org.all_manga_page);
            path_img = String.Copy(org.path_img);
        }

        public override void load_chapters(KeyValuePair<String, String> infos)
        {
            String[] html = HtmlRequest.get_html(infos.Value);

            int found;

            name = infos.Key;
            link = infos.Value;

            list_chapter.Clear();
            int is_start = 0;
            int is_line_start = 0;

            int det = 0;
            int index = 0;

            Double tmp_number = 0;
            String tmp_link = "";
            String tmp_title = "";

            foreach (String i in html)
            {
                found = i.IndexOf("<a href=\"/");

                if (det == 1)
                {
                    année = HtmlRequest.cut_str(i, "<td>", "</td>");
                    det = 0;
                }

                if (det == 2)
                {
                    auteur = HtmlRequest.cut_str(i, "<td>", "</td>");
                    det = 0;
                }

                if (i.IndexOf("<td class=\"propertytitle\">Year of Release:</td>") == 0)
                    det = 1;

                if (i.IndexOf("<td class=\"propertytitle\">Author:</td>") == 0)
                    det = 2;

                if (i.IndexOf("<th class=\"leftgap\">Chapter Name</th>") != -1)
                    is_start = 1;

                if (i.IndexOf("<a href=\"") != -1 && is_start == 1)
                    is_line_start = 1;

                if (i.IndexOf("<div class=\"clear\"></div>") != -1 && is_start == 1)
                    is_start = 0;

                if (i.IndexOf("<div id=\"mangaimg\">") == 0)
                    img_link = HtmlRequest.cut_str(i, "<div id=\"mangaimg\"><img src=\"", "\" alt=");

                if (found == 0 && is_start == 1)
                {
                    String link = HtmlRequest.cut_str(i, "<a href=\"", "\">");
                    String desc = HtmlRequest.cut_str(i, "\">", "</a>") + " : " + HtmlRequest.cut_str(i, "</a> : ", "</td>");

                    tmp_number = TryParseDouble(HtmlRequest.cut_str(i, "\">", "</a>").Substring(name.Length));

                    tmp_title = null;
                    if (desc.IndexOf(" : ") != -1)
                        tmp_title = HtmlRequest.cut_str(i, "</a> : ", "</td>");

                    tmp_link = "http://www.mangareader.net" + link + "/";
                }

                if (i.IndexOf("<td>") != -1 && is_line_start == 1)
                {
                    String date = HtmlRequest.cut_str(i, "<td>", "</td>");
                    list_chapter.Insert(0, new Chapters(index, true, tmp_number, tmp_link, null, tmp_title, date));
                    index++;
                    is_line_start = 0;
                }
            }
            nb_tomes = 0;
        }        
    }
}
