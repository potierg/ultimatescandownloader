using ScanDownloaderV2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ScansDownloaderV2
{
    public class SagaMangaHere : ISaga
    {
        public SagaMangaHere()
        {
            list_chapter = new List<Chapters>();
        }

        public SagaMangaHere(SagaMangaHere org)
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

            this.name = infos.Key;
            this.link = infos.Value;

            list_chapter.Clear();
            int is_chapter_list = 0;
            int is_start = 0;

            String[] tab;

            String link = "";
            String desc;

            int img = 0;
            int cpt_desc = 0;

            resumé = "";
            int index = 0;
            Tome current_tome = null;
            Chapters current_chapter = null;

            foreach (String i in html)
            {
                if (i.IndexOf("has been licensed") != -1)
                {
                    list_chapter.Add(new Chapters(-1, false, 0, "", null, "Manga has been licensed"));
                }

                if (i.IndexOf("<div class=\"detail_list\">") != -1)
                    is_chapter_list = 1;

                if (i.IndexOf("<a class=\"color_0077\"") != -1 && is_chapter_list == 1)
                {
                    is_start = 1;
                    current_chapter = new Chapters();
                    link = HtmlRequest.cut_str(i, "href=\"", "\" >");
                    if (link == "NOT FOUND")
                        link = HtmlRequest.cut_str(i, "href=\"", "\" name");
                    current_chapter.setLink(link);
                    current_chapter.SetisChapter(true);
                }
                if (i.IndexOf("</a>") != -1 && is_start == 1)
                {
                    desc = i.Substring(0, i.IndexOf("</a>"));

                    current_chapter.setNumber(TryParseDouble(desc.Substring(name.Length)));

                }

                if (i.IndexOf("<span class=\"mr6\"") != -1 && is_start == 1)
                {
                    is_start = 0;

                    int tome_nb = 0;
                    if (i.IndexOf("Vol ") != -1)
                        tome_nb = Int32.Parse(HtmlRequest.cut_str(i, "Vol ", "</span>"));

                    if (current_tome == null || current_tome.getNumber() != tome_nb)
                        current_tome = new Tome(null, tome_nb);

                    current_chapter.setIndex(index);
                    current_chapter.setTome(current_tome);
                    index++;

                    list_chapter.Add(current_chapter);
                }

                if (i.IndexOf("class=\"tab_comment clearfix\">") != -1 && is_chapter_list == 1)
                {
                    is_chapter_list = 0;
                }

                if (i.IndexOf("<li><label>Author(s):</label>") != -1)
                    auteur = HtmlRequest.cut_str(i, "\">", "</a></li>");

                if (img == 1)
                {
                    img_link = HtmlRequest.cut_str(i, "<img src=\"", "\" onerror");
                    img = 0;
                }

                if (i.IndexOf("<div class=\"manga_detail_top clearfix\">") != -1)
                    img = 1;

                if (i.IndexOf("<p id=\"show\" style=\"display:none;\">") != -1)
                    cpt_desc = 1;


                if (cpt_desc == 1)
                {
                    if (i.IndexOf("<p id=\"show\" style=\"display:none;\">") != -1 && i.IndexOf("&nbsp;<a href=") != -1)
                        resumé = HtmlRequest.cut_str(i, "style=\"display:none;\">", "&nbsp;<a href=");
                    else if (i.IndexOf("<p id=\"show\" style=\"display:none;\">") != -1)
                    {
                        int pos = i.IndexOf("<p id=\"show\" style=\"display:none;\">") + 35;
                        resumé += i.Substring(pos, i.Length - pos);
                    }
                    else if (i.IndexOf("&nbsp;<a href=") != -1)
                    {
                        resumé += i.Substring(0, i.IndexOf("&nbsp;<a href="));
                    }
                    else
                        resumé += i;
                }

                if (i.IndexOf("&nbsp;<a href=") != -1 && cpt_desc == 1)
                    cpt_desc = 0;
            }
            resumé = resumé.Replace("&quot;", "\"");
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
