using ScanDownloaderV2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ScansDownloaderV2
{
    public class SagaJapscan : ISaga
    {
        public SagaJapscan()
        {
            list_chapter = new List<Chapters>();
        }

        public SagaJapscan(SagaJapscan org)
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

            List<String[]> array_positions = new List<string[]>();
            int found;
            int found2;
            int found3;

            name = infos.Key;
            link = infos.Value;

            int nb_cell = 0;
            int synopsis = 0;

            list_chapter.Clear();

            Tome current_tome = null;
            int index = 0;

            foreach (String i in html)
            {
                found = i.IndexOf("<a href=\"//www.japscan.com/lecture-en-ligne/");
                found2 = i.IndexOf("<h2>Volume");
                found3 = i.IndexOf("<div class=\"cell\">");

                if (found3 == 0)
                    nb_cell++;

                if (found3 == 0 && nb_cell == 6)
                    auteur = i.Substring(18, i.Length - 24);

                if (found3 == 0 && nb_cell == 7)
                    année = i.Substring(18, i.Length - 24);

                if (synopsis == 1)
                {
                    resumé = i.Substring(0, i.Length - 6);
                    synopsis = 0;
                }

                if (i.IndexOf("<div id=\"synopsis\">") == 0)
                    synopsis = 1;

                if (found == 0)
                {
                    int end_pos = i.IndexOf("/\">");

                    String link = i.Substring(11, end_pos - 11);
                    String desc = i.Substring(end_pos + 3, i.Length - end_pos - 7);

                    String desc_det;
                    String title = null;

                    if (current_tome != null && i.IndexOf("Tome " + current_tome.getNumber()) != -1)
                        list_chapter.Add(new Chapters(index, false, current_tome.getNumber(), "http://" + link + "/", current_tome, title));
                    else
                    {
                        if (desc.IndexOf(":") != -1)
                        {
                            desc_det = desc.Substring(desc.IndexOf(name) + name.Length, desc.IndexOf(":") - (desc.IndexOf(name) + name.Length));
                            title = desc.Substring(desc.IndexOf(": ") + 2, desc.Length - (desc.IndexOf(": ") + 2));
                        }
                        else
                            desc_det = desc.Substring(desc.IndexOf(name) + name.Length, desc.Length - (desc.IndexOf(name) + name.Length));

                        Double number = TryParseDouble(desc_det);

                        list_chapter.Add(new Chapters(index, true, number, "http://" + link + "/", current_tome, title));

                        index++;
                    }
                }
                else if (found2 == 0)
                {
                    String data = i.Substring(4, i.Length - 14);

                    String nb_tome = Regex.Match(data, @"\d+").Value;

                    int pos_sep = data.IndexOf(":");
                    String title;
                    if (pos_sep != -1)
                        title = data.Substring(pos_sep + 2);
                    else
                        title = null;

                    current_tome = new Tome(title, Int32.Parse(nb_tome));
                }
            }
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
            string[] content = HtmlRequest.get_html(link);

            foreach (String i in content)
            {
                if (i.IndexOf("<img data-img=") != -1)
                {
                    string link_img = HtmlRequest.cut_str(i, "src=\"", "\"/>");
                    int pos_point = link_img.LastIndexOf('.');
                    String ext = link_img.Substring(pos_point);

                    String nb_chapter = "";
                    if (chapter < 10)
                        nb_chapter = "0" + chapter.ToString();

                    if (Int32.Parse(nb_page) < 10)
                        nb_page = "0" + nb_page;

                    Debug.WriteLine(path + "chap" + nb_chapter + "p" + nb_page + ext);
                    if (chapter != -1)
                        HtmlRequest.save_image(link_img, path + "chap" + nb_chapter + "p" + nb_page + ext);
                    else
                        HtmlRequest.save_image(link_img, path + nb_page + ext);
                }
            }
        }

        public override List<Chapters> get_pages_details(String link, String path, int chap)
        {
            String[] content = HtmlRequest.get_html(link);

            List<Chapters> ret_pages = new List<Chapters>();

            double nb_pages = 1;
            foreach (String i in content)
            {
                if (i.IndexOf("<option data-img=") != -1)
                    nb_pages += 1;
            }

            int cpt_page = 1;
            while (cpt_page <= nb_pages)
            {
                String page = cpt_page.ToString();
                if (cpt_page < 10)
                    page = "0" + cpt_page.ToString();


                //ret_pages.Add(new String[4] { ((int)((cpt_page / nb_pages) * 100.0)).ToString(), link + cpt_page.ToString() + ".html", cpt_page.ToString(), chap.ToString() });
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

                        path_img = "./" + name + "/" + title + "/";

                        int chap;
                        if (i[2].IndexOf("Tome") == -1)
                            chap = Int32.Parse(Regex.Match(i[2], @"\d+").Value);
                        else
                            chap = -1;

                        ret_chapters.Add(new String[6] { progressValue.ToString(), title, path_img, i[1], i[2], chap.ToString() });

                        cpt_chapter++;
                    }

                    if (end == i[2])
                        is_start = 0;

                }
                else
                {
                    title = "Tome ";
                    if (Int32.Parse(i[1]) < 10)
                        title = title + "0";
                    title = title + i[1];
                    if (i[2] != "")
                        title = title + " " + i[2];
                }
            }*/

            return (ret_chapters);
        }
    }
}
