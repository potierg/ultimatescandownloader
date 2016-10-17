using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScansDownloader
{
    public class Japscan : IManga
    {
        String link_list_manga { get; set; }

        public Japscan()
        {
            list_chapter = new List<String[]>();
            link_list_manga = "http://www.japscan.com/mangas/";
        }

        public Japscan(Japscan org)
        {
            name = String.Copy(org.name);
            link = String.Copy(org.link);
            nb_tomes = org.nb_tomes;
            start = String.Copy(org.start);
            end = String.Copy(org.end);
            list_chapter = new List<String[]>(org.list_chapter);
            nb_chapter = org.nb_chapter;
            site_link = String.Copy(org.site_link);
            all_manga_page = String.Copy(org.all_manga_page);
            path_img = String.Copy(org.path_img);
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
                if (i.IndexOf("<div class=\"cell\"><a href=\"/mangas/") != -1 && i.IndexOf("blackout") == -1)
                {
                    String name = HtmlRequest.cut_str(i, "/\">", "</a></div>");
                    String link = HtmlRequest.cut_str(i, "<a href=\"", "/\">");
                    allManga.Add(new KeyValuePair<String, String>(name, "http://www.japscan.com" + link + "/"));
                }
            }
        }

        override public List<KeyValuePair<String, String>> get_all_mangas()
        {
            return allManga;
        }

        public override void load_chapters(KeyValuePair<String, String> infos)
        {
            String[] html = HtmlRequest.get_html(infos.Value);

            List<String[]> array_positions = new List<string[]>();
            int found;
            int found2;

            name = infos.Key;
            link = infos.Value;

            list_chapter.Clear();

            foreach (String i in html)
            {
                found = i.IndexOf("<a href=\"//www.japscan.com/lecture-en-ligne/");
                found2 = i.IndexOf("<h2>Volume");

                if (found == 0)
                {
                    int end_pos = i.IndexOf("/\">");

                    String link = i.Substring(11, end_pos - 11);
                    String desc = i.Substring(end_pos + 3, i.Length - end_pos - 7);

                    String[] tab = new String[3];
                    tab[0] = "1";
                    tab[1] = "http://" + link + "/";
                    tab[2] = desc;

                    array_positions.Insert(0, tab);

                }
                else if (found2 == 0)
                {
                    String data = i.Substring(4, i.Length - 14);

                    String nb_tome = Regex.Match(data, @"\d+").Value;

                    int pos_sep = data.IndexOf(":");
                    String title = "";
                    if (pos_sep != -1)
                        title = data.Substring(pos_sep + 2);
                    else
                        title = "";

                    String[] tab = new String[3];
                    tab[0] = "2";
                    tab[1] = nb_tome;
                    tab[2] = title;

                    array_positions.Insert(0, tab);
                }
            }

            int pos_prompt = 0;
            int pos_elem = 0;

            nb_tomes = 0;

            while (pos_prompt < array_positions.Count)
            {
                if (array_positions[pos_prompt][0] == "2")
                {
                    list_chapter.Add(array_positions[pos_prompt]);

                    while (pos_elem < pos_prompt)
                    {
                        list_chapter.Add(array_positions[pos_elem]);
                        pos_elem += 1;
                    }
                    pos_elem += 1;
                    nb_tomes += 1;
                }
                pos_prompt += 1;
            }

            if (pos_elem != array_positions.Count)
            {

                String[] tab = new String[3];
                tab[0] = "2";
                tab[1] = nb_tomes.ToString();
                tab[2] = "Last";

                list_chapter.Add(tab);

                while (pos_elem < array_positions.Count)
                {
                    list_chapter.Add(array_positions[pos_elem]);
                    pos_elem += 1;
                }
            }
        }

        public override void set_delimeter(int pos_start, int pos_end)
        {
            int pos = -1;
            int is_start = 0;
            nb_chapter = 0;

            foreach (String[] i in list_chapter)
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
            }
            return;
        }

        public override void download_one_scan(String link, String nb_page, int chapter, String path)
        {
            String[] content = HtmlRequest.get_html(link);

            foreach (String i in content)
            {
                if (i.IndexOf("<img data-img=") != -1)
                {
                    String link_img = HtmlRequest.cut_str(i, "src=\"", "\"/>");
                    int pos_point = link_img.LastIndexOf('.');
                    String ext = link_img.Substring(pos_point);

                    String nb_chapter = "";
                    if (chapter < 10)
                        nb_chapter = "0" + chapter.ToString();

                    if (Int32.Parse(nb_page) < 10)
                        nb_page = "0" + nb_page;

                    if (chapter != -1)
                        HtmlRequest.save_image(link_img, path + "chap" + nb_chapter + "p" + nb_page + ext);
                    else
                        HtmlRequest.save_image(link_img, path + nb_page + ext);
                }
            }
        }

        public override List<String[]> get_pages_details(String link, String path, int chap)
        {
            String[] content = HtmlRequest.get_html(link);

            List<String[]> ret_pages = new List<string[]>();

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


                ret_pages.Add(new String[4] { ((int)((cpt_page / nb_pages) * 100.0)).ToString(), link + cpt_page.ToString() + ".html", cpt_page.ToString(), chap.ToString() });
                cpt_page += 1;
            }

            return ret_pages;
        }


        public override List<String[]> get_chapters_details()
        {
            int is_start = 0;
            double cpt_chapter = 0;

            String title = "";

            List<String[]> ret_chapters = new List<String[]>();

            foreach (String[] i in list_chapter)
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
            }

            return (ret_chapters);
        }
    }
}
