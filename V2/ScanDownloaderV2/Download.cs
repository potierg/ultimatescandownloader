using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScanDownloaderV2
{
    class Download
    {
        public int index;
        public int site;
        public String name;
        public String cover_path;

        public List<Chapters> chapters;

        public int nb_chapter;
        public int nb_tome;

        public ProgressBar pb1;
        public ProgressBar pb2;

        public Download(int id, int s, String n, String c, List<Chapters> lc)
        {
            index = id;
            site = s;
            name = n;
            cover_path = c;
            chapters = lc;

            nb_chapter = 0;
            nb_tome = 0;

            foreach(Chapters ch in lc)
            {
                if (ch.isChapter())
                    nb_chapter++;
                else
                    nb_tome++;
            }
        }

        public void setPg(ProgressBar pb1, ProgressBar pb2)
        {
            this.pb1 = pb1;
            this.pb2 = pb2;
        }
    }
}
