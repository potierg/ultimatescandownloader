using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanDownloaderV2
{
    public class Chapters
    {
        private bool is_chapter;

        private int index;
        private double number;
        private String link;
        private Tome tome;
        private String title;

        public Chapters()
        {

        }

        public Chapters(Chapters old)
        {
            this.index = old.index;
            this.is_chapter = old.is_chapter;
            this.number = old.number;
            this.link = old.link;
            if (old.tome != null)
                this.tome = new Tome(old.tome);
            else
                this.tome = null;
            this.title = old.title;
        }

        public Chapters(int id, bool is_chapter, double nb, String l, Tome t, String title)
        {
            this.index = id;
            this.is_chapter = is_chapter;
            this.number = nb;
            this.link = l;
            this.tome = t;
            this.title = title;
        }

        public void setIndex(int id)
        {
            index = id;
        }

        public void setNumber(double nb)
        {
            number = nb;
        }

        public void setLink(String li)
        {
            link = li;
        }

        public void setTome(Tome t)
        {
            tome = t;
        }

        public void SetisChapter(bool is_chap)
        {
            is_chapter = is_chap;
        }

        public void setTitle(String title)
        {
            this.title = title;
        }

        public int getIndex()
        {
            return index;
        }

        public double getNumber()
        {
            return number;
        }

        public String getLink()
        {
            return link;
        }

        public Tome getTome()
        {
            return tome;
        }

        public bool isChapter()
        {
            return is_chapter;
        }

        public String getTitle()
        {
            return title;
        }
    }
}
