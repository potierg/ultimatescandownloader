using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScansDownloader
{
    public class IManga
    {
        protected virtual List<KeyValuePair<String, String>> allManga { get; set; }

        protected virtual String name { get; set; } = "";
        protected virtual String link { get; set; } = "";
        protected virtual int nb_tomes { get; set; } = 0;

        protected virtual String start { get; set; } = "";
        protected virtual String end { get; set; } = "";

        protected virtual List<String[]> list_chapter { get; set; } = new List<string[]>();
        protected virtual int nb_chapter { get; set; } = 0;

        protected virtual String site_link { get; set; } = "";
        protected virtual String all_manga_page { get; set; } = "";

        protected virtual String path_img { get; set; } = "";

        public String getName()
        {
            return name;
        }
        virtual public void load_all_mangas()
        {
        }

        virtual public List<KeyValuePair<String, String>> get_all_mangas()
        {
            return allManga;
        }

        virtual public void load_chapters(System.Collections.Generic.KeyValuePair<String, String> list) { }

        public List<String[]> get_chapters()
        {
            return list_chapter;
        }

        public String get_description(int val)
        {
            switch (val)
            {
                case 0:
                    return (name);
                case 1:
                    return (start);
                case 2:
                    return (end);

            }
            return "";
        }

        public void delete() {
            allManga = null;
            list_chapter = null;
        }
    }
}
