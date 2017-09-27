using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanDownloaderV2
{
    public class ISaga
    {
        protected virtual String name { get; set; } = "";
        protected virtual String auteur { get; set; } = "";
        protected virtual String année { get; set; } = "";
        protected virtual String resumé { get; set; } = "";

        protected virtual String img_link { get; set; } = null;

        protected virtual String link { get; set; } = "";
        protected virtual int nb_tomes { get; set; } = 0;

        protected virtual String start { get; set; } = "";
        protected virtual String end { get; set; } = "";

        protected virtual List<Chapters> list_chapter { get; set; } = new List<Chapters>();
        protected virtual int nb_chapter { get; set; } = 0;

        protected virtual String all_manga_page { get; set; } = "";

        protected virtual String path_img { get; set; } = "";

        public String getName()
        {
            return name;
        }

        public String getAuteur()
        {
            return auteur;
        }

        public String getAnnée()
        {
            return année;
        }

        public String getResumé()
        {
            return resumé;
        }

        public String getImgLink()
        {
            return img_link;
        }

        virtual public void load_chapters(System.Collections.Generic.KeyValuePair<String, String> list) { }

        public List<Chapters> get_chapters()
        {
            return list_chapter;
        }

        public void delete()
        {
            list_chapter = null;
        }

        protected double TryParseDouble(string input)
        {
            string Numbers = "0123456789.,";
            var numberBuilder = new StringBuilder();
            foreach (char c in input)
            {
                if (Numbers.IndexOf(c) > -1)
                    numberBuilder.Append(c);
            }
            String new_nb = numberBuilder.ToString();
            new_nb = new_nb.Replace(",", ".");
            try
            {
                Double ret = Convert.ToDouble(new_nb);
                return ret;
            }
            catch (Exception e)
            {
                return -666;
            }
        }

    }
}
