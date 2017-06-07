using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ScanDownloaderV2
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Download
    {
        public int index;
        public int site;
        public String name;
        public String cover_path;

        public List<Chapters> chapters;

        public int nb_chapter;
        public int nb_tome;

        public int total_to_dl;
        public int total_to_dl_real;

        public Download()
        {
            index = -1;
            site = -1;
            name = "";
            cover_path = "";

            nb_chapter = 0;
            nb_tome = 0;

            chapters = new List<Chapters>();

            total_to_dl = 0;
            total_to_dl_real = 0;
        }

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

            total_to_dl = nb_chapter + nb_tome;
            total_to_dl_real = 0;
        }
    }
}
