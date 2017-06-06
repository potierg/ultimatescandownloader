using ScanDownloaderV2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ScansDownloaderV2
{
    public class SagaReadManga : ISaga
    {
        public SagaReadManga()
        {
            list_chapter = new List<Chapters>();
        }

        public SagaReadManga(SagaReadManga org)
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

            List<Chapters> array_positions = new List<Chapters>();
            int found;

            name = infos.Key;
            link = infos.Value;

            list_chapter.Clear();

            int synopsis = 0;
            int author = 0;
            int chapter = 0;
            int picture = 0;

            String tmp_link = "";

            int index = 0;


            foreach (String i in html)
            {

                if (synopsis == 1)
                {
                    synopsis = 0;
                    resumé = HtmlRequest.cut_str(i, "\">", "</span></p>");
                    if (resumé == "NOT FOUND")
                        resumé = HtmlRequest.cut_str(i, "<p>", "</p>");
                    if (resumé == "NOT FOUND")
                        resumé = "";
                }

                if (i.IndexOf("<li class=\"list-group-item movie-detail\">") != -1)
                    synopsis = 1;

                if (author == 1 && i.IndexOf("<li><a href=\"") != -1)
                {
                    author = 0;
                    auteur = HtmlRequest.cut_str(i, "\">", "</a></li>");
                }

                if (i.IndexOf("<ul class=\"cast-list clearfix\">") != -1)
                    author = 1;

                if (picture == 1 && i.IndexOf("<img src=\"") != -1)
                {
                    picture = 0;
                    img_link = HtmlRequest.cut_str(i, "<img src=\"", "\" alt=\"");
                }

                if (i.IndexOf("<div class=\"col-md-3\">") != -1)
                    picture = 1;

                if (i.IndexOf("<ul class=\"chp_lst\">") != -1)
                    chapter = 1;

                if (i.IndexOf("<a href=\"") != -1 && chapter == 1)
                    tmp_link = HtmlRequest.cut_str(i, "<a href=\"", "\">");

                if (i.IndexOf("<span class=\"val\"><span class=\"icon-arrow-2\"></span>") != -1)
                {
                    String desc = HtmlRequest.cut_str(i, "\"></span>", " </span>");

                    Double number = TryParseDouble(desc.Substring(name.Length + 3));

                    if (number != -666)
                    {
                        list_chapter.Insert(0, new Chapters(index, true, number, tmp_link + "/", null, null));
                        index++;
                    }
                }
            }
            nb_tomes = 0;
        }
    }
}
