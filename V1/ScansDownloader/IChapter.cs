using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScansDownloader
{
    class IChapter
    {
        protected virtual String manga_name { get; set; } = "";
        protected virtual int nb_tome { get; set; } = 0;
        protected virtual String name_tome { get; set; } = "";
        protected virtual String path { get; set; } = "";
        protected virtual String description { get; set; } = "";
        protected virtual String link { get; set; } = "";
        public IChapter(IManga manga, String selected)
        {
            manga_name = manga.getName().Trim();
            foreach (string[] i in manga.get_chapters())
            {
                if (i[0] == "2")
                {
                    nb_tome = Int32.Parse(i[1]);
                    name_tome = i[2];
                }
                if (i[0] == "1" && i[2] == selected)
                {
                    Debug.WriteLine(nb_tome.ToString() + " - " + name_tome);
                    Debug.WriteLine(i[0] + " - " + i[1] + " - " + i[2]);

                    link = i[1];
                    description = i[2];
                    if (name_tome != "")
                        path = "Tome " + nb_tome + " - " + name_tome;
                    else
                        path = "Tome " + nb_tome;
                    return;
                }
            }
        }

        public String getMangaName()
        {
            return (manga_name);
        }

        public String getDescription()
        {
            return (description);
        }

        virtual public String[] get_chapters_details()
        {
            return null;
        }

        virtual public List<String[]> get_pages_details(String link, String path, String chap)
        {
            return null;
        }

        virtual public void download_one_scan(String link, String nb_page, String chapter, String path) { }
    }
}
