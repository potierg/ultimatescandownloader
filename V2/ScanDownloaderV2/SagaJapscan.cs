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
                        list_chapter.Add(new Chapters(index, false, current_tome.getNumber(), "http://" + link + "/", current_tome, title, null));
                    else
                    {
                        if (desc.Substring(desc.IndexOf(name) + name.Length).IndexOf(":") != -1)
                        {
                            desc_det = desc.Substring(desc.IndexOf(name) + name.Length, desc.IndexOf(":") - (desc.IndexOf(name) + name.Length));
                            title = desc.Substring(desc.IndexOf(": ") + 2, desc.Length - (desc.IndexOf(": ") + 2));
                        }
                        else
                            desc_det = desc.Substring(desc.IndexOf(name) + name.Length, desc.Length - (desc.IndexOf(name) + name.Length));

                        Double number = TryParseDouble(desc_det);

                        list_chapter.Add(new Chapters(index, true, number, "http://" + link + "/", current_tome, title, null));

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
    }
}
