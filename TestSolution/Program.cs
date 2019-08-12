using System;
using System.Collections.Generic;


namespace TestSolution
{
    class Program
    {         
        static void Main()
        {
            int maxNum = 10;
            Console.WriteLine("Input URL:");
            string url = Console.ReadLine();
            if (!url.Contains("http"))
            {
                url = "http://" + url;
            }

            //If input is incorrect(not numbers) using default value 10           
            try
            {
                    Console.WriteLine("Input maximum number of images to download (default = 10):");
                    maxNum = Convert.ToInt32(Console.ReadLine());
            }
            catch (System.FormatException)
            {
                    Console.WriteLine("Your input is wrong. Using 10.");
            }

            //Get source html of page
            string html = ParseHelper.GetHTML(url);

            //Get list of image urls
            List<Uri> links = ParseHelper.GetImages(html, url);

            //Output urls
            foreach (System.Uri n in links)
            {
                Console.WriteLine(n.ToString());
            }
            ParseHelper.Savetxt(links);
            ParseHelper.Download(links, maxNum);
            Console.WriteLine("Finished. Press any key...");
            Console.ReadKey();
        }
    }
}


