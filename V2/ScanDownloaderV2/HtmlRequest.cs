using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ScansDownloaderV2
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
                if (response == null)
                    throw new Exception("Load " + link);
            }
            catch (System.Exception ex)
            {
                throw new Exception("Load " + link);
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
            else
            {
                throw new Exception("Access " + link);
            }

            return html_res.Split('\n');
        }

        static public void save_image(String link, String path, int nb_try = 0)
        {
            try
            {
                Debug.WriteLine(link);
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(link), @path);
                }
            } catch (WebException we)
            {
                Debug.WriteLine("Fail " + link);
                if (nb_try < 3)
                    HtmlRequest.save_image(link, path, nb_try + 1);
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
                return ("NOT FOUND");
            }
            return (str.Substring(pos1 + start_part.Length, pos2 - pos1 - start_part.Length));
        }
    }
}
