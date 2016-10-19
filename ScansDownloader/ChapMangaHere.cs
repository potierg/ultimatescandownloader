using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScansDownloader
{
    class ChapMangaHere : IChapter
    {
        public ChapMangaHere(IManga manga, String selected) : base(manga, selected)
        {
        }

        public override void download_one_scan(String link, String nb_page, String chapter, String path)
        {
            String[] content = HtmlRequest.get_html(link);

            foreach (String i in content)
            {
                if (i.IndexOf("onerror=\"javascript:rerender(this);\"") != -1)
                {
                    String link_img = HtmlRequest.cut_str(i, "<img src=\"", "\" onerror");
                    int pos = link_img.LastIndexOf(".");
                    int pos2 = link_img.LastIndexOf("?");
                    String ext = link_img.Substring(pos, pos2 - pos);

                    String nb_chapter = chapter.ToString();
                    if (float.Parse(chapter, CultureInfo.InvariantCulture.NumberFormat) < 10)
                        nb_chapter = "0" + nb_chapter;

                    if (Int32.Parse(nb_page) < 10)
                        nb_page = "0" + nb_page;

                    if (link.IndexOf("javascript") == -1)
                        HtmlRequest.save_image(link_img, path + "chap" + nb_chapter.Replace('.', ' ') + " page" + nb_page + ext);

                    return;
                }
            }
        }

        public override List<String[]> get_pages_details(String link, String path, String chap)
        {
            String[] content = HtmlRequest.get_html(link);

            List<String[]> ret_pages = new List<string[]>();

            double nb_pages = 0;
            foreach (String i in content)
            {
                if (i.IndexOf("<option value=") != -1)
                    nb_pages += 1;
            }

            int cpt_page = 1;
            while (cpt_page <= nb_pages)
            {
                String page = cpt_page.ToString();
                if (cpt_page < 10)
                    page = "0" + cpt_page.ToString();


                ret_pages.Add(new String[4] { ((int)((cpt_page / nb_pages) * 100.0)).ToString(), link + cpt_page.ToString() + ".html", cpt_page.ToString(), chap });
                cpt_page += 1;
            }

            return ret_pages;
        }


        public override String[] get_chapters_details()
        {
            List<String[]> ret_chapters = new List<String[]>();


            String path_img = "./" + manga_name + "/";

            String chap = "";

            Match m = Regex.Match(description, @"[0-9]+\.[0-9]+");
            chap = m.Value;

            if (chap == "")
            {
                m = Regex.Match(description, @"[0-9]+");
                chap = m.Value;
            }
            return (new String[5] { "Chapter " + chap, path_img, link, description.Trim(), chap.ToString() });
        }
    }
}