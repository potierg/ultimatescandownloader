using Newtonsoft.Json;
using ScanDownloaderV2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace ScansDownloaderV2
{
    public class ISite
    {
        protected virtual List<KeyValuePair<String, String>> allManga { get; set; } = null;
        protected String name_file;

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

        public void createFile()
        {
            String json = JsonConvert.SerializeObject(allManga);

            System.IO.Directory.CreateDirectory("/tmp/Json/");

            File.WriteAllText("/tmp/Json/" + name_file, json);
        }

        public void loadFile()
        {
            string json = File.ReadAllText("/tmp/Json/" + name_file);

            allManga = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(json);

            return;
        }

        public String getNameFile()
        {
            return "/tmp/Json/" + name_file;
        }

        public void delete()
        {
            allManga = null;
        }
    }
}
