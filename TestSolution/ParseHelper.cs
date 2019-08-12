using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace TestSolution
{
    class ParseHelper
    {
        //Function to parse HTML and add img sources to the list
        static public List<Uri> GetImages(string htmlSource, string url, int max = 10)
        {
            List<Uri> links = new List<Uri>();

            //regexp to find src and assign to "url" varible 
            string regexImgSrc = @"<img.*?src=""(?<url>.*?)"".*?>";
            MatchCollection matchesImgSrc = Regex.Matches(htmlSource, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            //Loop to get all urls out from regexp
            foreach (Match m in matchesImgSrc)
            {
                string href = m.Groups["url"].Value;

                //Check if url has no domain at the beginning
                if (href[0] == '/' && href[1] == '/')
                {
                    href = url + href.Substring(2);
                }
                else if (href[0] == '/')
                {
                    href = url + href.Substring(1);
                }

                //Check if filename contains extention of file 
                if (!Path.GetFileName(href).Contains('.'))
                {
                    continue;
                }
                try
                {
                    links.Add(new Uri(href));
                }

                //Except if can't create Uri
                catch
                {
                    continue;
                }
            }
            return links;

        }

        //Function to get HTML code of site
        static public string GetHTML(string url)
        {
            string html = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    html = sr.ReadToEnd();
                }
            }

            //Close app if got WebException
            catch (System.Net.WebException)
            {
                Console.WriteLine("Error accessing url");
                Console.ReadKey();
                Environment.Exit(0);
            }
            return html;
        }


        //Function to create txt file and format text
        static public void Savetxt(List<Uri> links)
        {
            var dict = new Dictionary<string, List<string>>();

            //temporary list to fill dictionary
            List<string> existing;

            //Dictionary formatted as "Domain:List of filenames"
            foreach (System.Uri uri in links)
            {
                string domain = uri.Host;
                if (!dict.TryGetValue(domain, out existing))
                {
                    existing = new List<string>();
                    dict.Add(domain, existing);
                }
                string name = Path.GetFileName(uri.AbsolutePath.ToString());
                existing.Add(name);
            }
            string path = @".\output.txt";         
            try
            {
                using (FileStream fstream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {

                    //Get data from formed dictionary, transform to byte and write for keys and items
                    foreach (var dom in dict.Keys)
                    {
                        byte[] arrayDom = System.Text.Encoding.Default.GetBytes(dom + "\n");
                        fstream.Write(arrayDom, 0, arrayDom.Length);
                        foreach (var value in dict[dom])
                        {
                            byte[] arrayVal = System.Text.Encoding.Default.GetBytes("- " + value + "\n");
                            fstream.Write(arrayVal, 0, arrayVal.Length);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error writing file");
            }
        }

        //Function to download images using their urls
        static public void Download(List<Uri> links, int maxNum)
        {
            Console.WriteLine("-----------------");
            Console.WriteLine("Download started");
            //Use List length if it's shorter
            if (maxNum > links.Count)
            {
                maxNum = links.Count;
            }
            using (var client = new WebClient())
            {

                //Throw an exception if can't download
                for (int i = 0; i < maxNum; i++)
                {
                    string directory = @".\images\";
                    if (!File.Exists(directory)) Directory.CreateDirectory(directory);
                    try
                    {
                        client.DownloadFile(links[i].ToString(), directory + Path.GetFileName(links[i].ToString()));
                    }
                    catch
                    {
                        Console.WriteLine("Download Error for " + Path.GetFileName(links[i].ToString()));
                        continue;
                    }
                }
            }
        }
    }
}
