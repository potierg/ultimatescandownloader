using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScansDownloader
{
    class ChapJapscan : IChapter
    {
        public ChapJapscan(IManga manga, String selected) : base(manga, selected)
        {
        }
        public override void download_one_scan(String link, String nb_page, String chapter, String path)
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
                    if (float.Parse(chapter, CultureInfo.InvariantCulture.NumberFormat) < 10)
                        nb_chapter = "0" + chapter.ToString();

                    if (Int32.Parse(nb_page) < 10)
                        nb_page = "0" + nb_page;

                    if (chapter != "")
                        HtmlRequest.save_image(link_img, path + "chap" + nb_chapter.Replace('.', ' ') + " page" + nb_page + ext);
                    else
                        HtmlRequest.save_image(link_img, path + "page " + nb_page + ext);
                }
            }
        }

        public override List<String[]> get_pages_details(String link, String path, String chap)
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


                ret_pages.Add(new String[4] { ((int)((cpt_page / nb_pages) * 100.0)).ToString(), link + cpt_page.ToString() + ".html", cpt_page.ToString(), chap });
                cpt_page += 1;
            }

            return ret_pages;
        }


        public override String[] get_chapters_details()
        {

            String path_img = manga_name + "/" + path + "/";

            String chap;
            if (description.IndexOf("Tome") == -1)                
            {
                Match m = Regex.Match(description, @"[0-9]+\.[0-9]+");
                chap = m.Value;

                if (chap == "")
                {
                    m = Regex.Match(description, @"[0-9]+");
                    chap = m.Value;
                }
            } else
                chap = "";

            return (new String[5] { path, path_img, link, description, chap.ToString() });

        }
    }
}
