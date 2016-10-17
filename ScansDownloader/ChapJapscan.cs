using System;
using System.Collections.Generic;
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
                        HtmlRequest.save_image(link_img, path + "chap" + nb_chapter + "page" + nb_page + ext);
                    else
                        HtmlRequest.save_image(link_img, path + "page " + nb_page + ext);
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


        public override String[] get_chapters_details()
        {

            String path_img = manga_name + "/" + path + "/";

            int chap;
            if (description.IndexOf("Tome") == -1)
                chap = Int32.Parse(Regex.Match(description, @"\d+").Value);
            else
                chap = -1;

            return (new String[5] { path, path_img, link, description, chap.ToString() });

        }
    }
}
