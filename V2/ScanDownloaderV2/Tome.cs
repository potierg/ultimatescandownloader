using Newtonsoft.Json;
using System;

namespace ScanDownloaderV2
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Tome
    {
        public String title;
        public int number;

        public Tome()
        {
            title = "";
            number = 0;
        }

        public Tome(Tome t)
        {
            this.title = t.title;
            this.number = t.number;
        }

        public Tome(String title, int number)
        {
            this.title = title;
            this.number = number;
        }

        public String getTitle()
        {
            return title;
        }

        public int getNumber()
        {
            return number;
        }
    }
}
