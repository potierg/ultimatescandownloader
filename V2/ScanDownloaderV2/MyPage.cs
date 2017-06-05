using ScansDownloaderV2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanDownloaderV2
{
    public class MyPage
    {
        public int num_page;
        public int max_page;
        public String link;
        public double chapter;
        public bool is_chapter;

        public MyPage(int num, int max, String l, double c, bool ic)
        {
            num_page = num;
            max_page = max;
            link = l;
            chapter = c;
            is_chapter = ic;
        }

        public String absolute_number(int nb, String nb_bit)
        {
            var new_number = new StringBuilder(nb_bit);
            String old_number = nb.ToString();

            int j = new_number.Length - 1;
            for (int i = old_number.Length - 1; i >= 0; i--)
            {
                new_number[j] = old_number[i];
                j--;
            }

            return new_number.ToString();
        }

        public void download(String path)
        {
            String name;
            String page = "";
            String ext;

            if (max_page >= 100)
                page = absolute_number(num_page, "000");
            else if (max_page >= 10)
                page = absolute_number(num_page, "00");
            else if (max_page < 1000)
                page = absolute_number(num_page, "0");

            if (is_chapter)
                name = "chapter " + chapter + " page " + page;
            else
                name = "page " + page;

            if (link.IndexOf("?token") != -1)
                ext = link.Substring(link.LastIndexOf('.'), link.IndexOf("?token") - link.LastIndexOf('.'));
            else
                ext = link.Substring(link.LastIndexOf('.'));

            HtmlRequest.save_image(link, path + name + ext);
        }
    }
}
