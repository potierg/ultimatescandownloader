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

                    current_chapter.setNumber(TryParseDouble(desc.Replace(name, "")));

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
    }
}
