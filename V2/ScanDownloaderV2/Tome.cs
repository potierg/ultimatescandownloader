using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanDownloaderV2
{
    public class Tome
    {
        private String title;
        private int number;

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
