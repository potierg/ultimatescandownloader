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

            String[] tab;
            int det = 0;
            int index = 0;

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

                if (i.IndexOf("<div id=\"mangaimg\">") == 0)
                    img_link = HtmlRequest.cut_str(i, "<div id=\"mangaimg\"><img src=\"", "\" alt=");

                if (found == 0 && is_start == 1)
                {
                    int end_pos = i.IndexOf("/\">");

                    String link = HtmlRequest.cut_str(i, "<a href=\"", "\">");
                    String desc = HtmlRequest.cut_str(i, "\">", "</a>") + " : " + HtmlRequest.cut_str(i, "</a> : ", "</td>");

                    tab = new String[3];

                    tab[0] = "1";
                    tab[1] = link;
                    tab[2] = desc;

                    Double number = TryParseDouble(HtmlRequest.cut_str(i, "\">", "</a>").Substring(name.Length));

                    String title = null;
                    if (desc.IndexOf(" : ") != -1)
                        title = HtmlRequest.cut_str(i, "</a> : ", "</td>");

                    list_chapter.Insert(0, new Chapters(index, true, number, "http://www.mangareader.net" + link + "/", null, title));
                    index++;
                }
            }
            nb_tomes = 0;
        }

        public override void set_delimeter(int pos_start, int pos_end)
        {
            int pos = -1;
            int is_start = 0;
            nb_chapter = 0;

            /*foreach (String[] i in list_chapter)
            {
                if (i[0] == "1")
                {
                    pos += 1;
                    if (pos == pos_start)
                    {
                        start = i[2];
                        is_start = 1;
                    }
                    if (pos == pos_end)
                    {
                        end = i[2];
                        is_start = 0;
                    }
                    if (is_start == 1)
                        nb_chapter += 1;
                }
            }*/
            return;
        }

        public override void download_one_scan(String link, String nb_page, int chapter, String path)
        {
            String[] content = HtmlRequest.get_html(link);

            foreach (String i in content)
            {
                if (i.IndexOf("<img id=\"img\"") != -1)
                {
                    String link_img = HtmlRequest.cut_str(i, "src=\"", "\" alt");
                    int pos_point = link_img.LastIndexOf('.');
                    String ext = link_img.Substring(pos_point);

                    String nb_chapter = "";
                    if (chapter < 10)
                        nb_chapter = "0" + chapter.ToString();

                    if (Int32.Parse(nb_page) < 10)
                        nb_page = "0" + nb_page;

                    HtmlRequest.save_image(link_img, path_img + "chap " + nb_chapter + " page " + nb_page + ext);
                }
            }
        }

        public override List<Chapters> get_pages_details(String link, String path, int chap)
        {
            String[] content = HtmlRequest.get_html("http://www.mangareader.net" + link);

            List<Chapters> ret_pages = new List<Chapters>();

            double nb_pages = 0;
            int is_start = 0;
            foreach (String i in content)
            {
                if (i.IndexOf("<div id=\"selectpage\">") != -1)
                    is_start = 1;
                if (i.IndexOf("<option value=") != -1 && is_start == 1)
                    nb_pages += 1;
            }

            int cpt_page = 1;
            while (cpt_page <= nb_pages)
            {
                String page = cpt_page.ToString();
                if (cpt_page < 10)
                    page = "0" + cpt_page.ToString();


                //ret_pages.Add(new String[4] { ((int)((cpt_page / nb_pages) * 100.0)).ToString(), "http://www.mangareader.net" + link + "/" + cpt_page.ToString(), cpt_page.ToString(), chap.ToString() });
                cpt_page += 1;
            }

            return ret_pages;
        }


        public override List<Chapters> get_chapters_details()
        {
            int is_start = 0;
            double cpt_chapter = 0;

            String title = "";

            List<Chapters> ret_chapters = new List<Chapters>();

            title = name;

            /*foreach (String[] i in list_chapter)
            {
                if (i[0] == "1")
                {
                    if (start == i[2])
                        is_start = 1;

                    if (is_start == 1)
                    {
                        int progressValue;

                        if (cpt_chapter > 0)
                            progressValue = (int)((cpt_chapter / nb_chapter) * 100.0);
                        else
                            progressValue = 0;

                        path_img = "./" + name + "/";

                        int chap = Int32.Parse(Regex.Match(i[2], @"\d+").Value);

                        ret_chapters.Add(new String[6] { progressValue.ToString(), title, path_img, i[1], i[2], chap.ToString() });

                        cpt_chapter++;
                    }

                    if (end == i[2])
                        is_start = 0;

                }
            }*/

            return (ret_chapters);
        }
    }
}
