using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ScansDownloader
{
    class HtmlRequest
    {
        static public String[] get_html(String link)
        {
            HttpWebResponse response;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                return (null);
            }

            String html_res = "";

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                html_res = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }

            return html_res.Split('\n');
        }

        static public void save_image(String link, String path)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(link), @path);
            }
        }

        static public String cut_str(String str, String start_part, String end_part)
        {
            int pos1;
            int pos2;
            pos1 = str.IndexOf(start_part);
            pos2 = str.IndexOf(end_part);

            if (pos1 == -1 || pos2 == -1 || pos1 > pos2)
            {
                Console.WriteLine("==> " + str);
                return ("NOT FOUND");
            }
            return (str.Substring(pos1 + start_part.Length, pos2 - pos1 - start_part.Length));
        }
    }
}
