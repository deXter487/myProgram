using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CodeVS
{
    class Program
    {
        static void Main(string[] args)
        {
           // Console.WriteLine("Hello World!");
           GetHtmlSync();
           Console.ReadLine();

        }

        private static async void GetHtmlSync()
        {
            var url = "https://www.ebay.com/sch/i.html?_nkw=XBox&_in_kw=1&_ex_kw=&_sacat=0&_udlo=&_udhi=&_ftrt=901&_ftrv=1&_sabdlo=&_sabdhi=&_samilow=&_samihi=&_sadis=15&_stpos=&_sargn=-1%26saslc%3D1&_salic=1&_sop=12&_dmd=1&_ipg=50&_fosrp=1";

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            // grab all listings
            var ProductsHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("ListViewInner")).ToList();

            var productListItems = ProductsHtml[0].Descendants("li")
                .Where(node => node.GetAttributeValue("id", "")
                .Contains("item")).ToList();

            
            foreach(var productListItem in productListItems)
            {
                // ProductListingId
                Console.WriteLine(productListItem.GetAttributeValue("listingid", ""));

                // Product Name
                Console.WriteLine(productListItem.Descendants("h3")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvtitle")).FirstOrDefault().InnerText.Trim('\r','\n','\t')
                );

                //Price
                Console.WriteLine(
                     Regex.Match(
                     productListItem.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvprice prc")).FirstOrDefault().InnerText.Trim('\r','\n','\t')
                    ,@"\d+.\d+")
                );

                //Listing Type   
                Console.WriteLine(
                    productListItem.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvformat")).FirstOrDefault().InnerText.Trim('\r','\n','\t')                    
                );
              
                // url
                Console.WriteLine(
                    productListItem.Descendants("a").FirstOrDefault().GetAttributeValue("href","")
                );

                Console.WriteLine();
            }


        }
    }
}
