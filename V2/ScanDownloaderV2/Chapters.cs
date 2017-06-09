using Newtonsoft.Json;
using System;

namespace ScanDownloaderV2
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Chapters
    {
        public bool is_chapter;

        public int index;
        public double number;
        public String link;
        public Tome tome;
        public String title;
        public int total_page;
        public String date;

        public Chapters()
        {
            index = 0;
            number = 0;
            link = "";
            tome = new Tome();
            title = "";
            total_page = 0;
            date = null;
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
            this.total_page = old.total_page;
            this.date = old.date;
        }

        public Chapters(int id, bool is_chapter, double nb, String l, Tome t, String title, String date)
        {
            this.index = id;
            this.is_chapter = is_chapter;
            this.number = nb;
            this.link = l;
            this.tome = t;
            this.title = title;
            this.total_page = 0;
            this.date = date;
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

        public void setMax(int max)
        {
            total_page = max;
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

        public int getMax()
        {
            return total_page;
        }
    }
}
