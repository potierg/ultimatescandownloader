using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ScansDownloaderV2
{
    public class SiteReadManga : ISite
    {
        String link_list_manga { get; set; }

        public SiteReadManga()
        {
            link_list_manga = "http://www.mangareader.net/alphabetical";
        }

        public override void load_all_mangas()
        {
            allManga = new List<KeyValuePair<String, String>>();

            String[] content = HtmlRequest.get_html(link_list_manga);
            int is_start = 0;

            if (content == null)
            {
                allManga = null;
                return;
            }

            foreach (String i in content)
            {
                if (i.IndexOf("<div class=\"series_col\"><a name=\" \"></a>") != -1)
                    is_start = 1;

                if (i.IndexOf("<li><b>Network</b></li>") != -1)
                    is_start = 0;

                if (i.IndexOf("<li><a href=\"/") != -1 && is_start == 1)
                {
                    String str;
                    if (i.IndexOf("<ul class=\"series_alpha\">") != -1)
                        str = i.Substring(25);
                    else
                        str = i;

                    String name = HtmlRequest.cut_str(str, "\">", "</a>");
                    String link = HtmlRequest.cut_str(str, "<li><a href=\"", "\">");
                    allManga.Add(new KeyValuePair<String, String>(name, "http://www.mangareader.net" + link));
                }
            }
        }
    }
}
