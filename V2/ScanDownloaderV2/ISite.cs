using ScanDownloaderV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScansDownloaderV2
{
    public class ISite
    {
        protected virtual List<KeyValuePair<String, String>> allManga { get; set; } = null;

        virtual public void load_all_mangas()
        {
        }

        public List<KeyValuePair<String, String>> get_all_mangas()
        {
            return allManga;
        }

        public virtual List<String> prepareDownload(Chapters chapitre)
        {
            return null;
        }

        public virtual void downloadScan(String link, int nb_page, Chapters chapitre, String path)
        {

        }

        public void delete()
        {
            allManga = null;
        }
    }
}
