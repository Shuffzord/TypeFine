using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using HtmlAgilityPack;

namespace Tools
{
    public static class HtmlScrapper
    {
        public static void GetSjpDataFromHtml()
        {
            //HttpUtility.UrlEncode("a discrétion", Encoding.GetEncoding("ISO-8859-1"));
            const string link = "http://sjp.pl/slownik/lp.phtml?page=";
            int i = 1;
            var request = WebRequest.Create(link + i);

            using (var fs = File.OpenWrite(@"C:\\Users\\Public\\" + Guid.NewGuid() + ".csv"))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    while (i < 3901)
                    {
                        var response = request.GetResponse();

                        using (response)
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                if (responseStream != null)
                                {
                                    HtmlNodeCollection nodes;
                                    using (var rdr = new StreamReader(responseStream, Encoding.GetEncoding("ISO-8859-2")))
                                    {
                                        var html = rdr.ReadToEnd();
                                        var doc = new HtmlDocument();
                                        html = HttpUtility.UrlDecode(html, Encoding.GetEncoding("ISO-8859-2"));
                                        doc.LoadHtml(html);
                                        nodes = doc.DocumentNode.SelectNodes("//td/a");
                                    }

                                    foreach (var item in nodes)
                                    {
                                        sw.WriteLine(item.InnerHtml);
                                    }
                                }

                                i++;
                                request = WebRequest.Create(link + i);
                            }
                        }
                    }
                }
            }
        }

        public static void GetSjpWords()
        {
            const string link = "http://sjp.pl/";
            using (var reader = new StreamReader(@"C:\\Users\\Public\\sjp lista.csv", Encoding.UTF8))
            {
                var fs = File.Create(@"C:\\Users\\Public\\" + Guid.NewGuid() + ".csv");
                using (var writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    var line = reader.ReadLine();
                    while (!reader.EndOfStream && !string.IsNullOrWhiteSpace(line))
                    {
                        line = HttpUtility.UrlEncode(line.Trim(), Encoding.GetEncoding("ISO-8859-2"));
                        Console.WriteLine(line);
                        var request = WebRequest.Create(link + line);
                        WebResponse response;

                        try
                        {
                            response = request.GetResponse();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                            Thread.Sleep(300000);
                            continue;
                        }

                        using (response)
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                if (responseStream != null)
                                {
                                    using (
                                        var rdr = new StreamReader(responseStream,
                                                                   Encoding.GetEncoding("ISO-8859-2")))
                                    {
                                        var html = rdr.ReadToEnd();

                                        var doc = new HtmlDocument();
                                        doc.LoadHtml(html);
                                        var titleNode = doc.DocumentNode.SelectSingleNode("//h1");
                                        var descNode = doc.DocumentNode.SelectSingleNode("//p[3]");
                                        try
                                        {
                                            writer.WriteLine(titleNode.InnerText + "$$" +
                                                             descNode.InnerText.Replace("<br>", Environment.NewLine));
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                                        }

                                    }
                                }
                            }
                        }


                        line = reader.ReadLine();
                    }
                }
            }
        }

        public static void GetWordOfTheDayFromHtml()
        {
            var request = WebRequest.Create("http://slowodnia.wordpress.com/2014/03/28/dessous/");

            using (var fs = File.Create("C:\\Users\\Public\\" + Guid.NewGuid() + ".csv"))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    while (true)
                    {
                        var response = request.GetResponse();

                        using (response)
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                var rdr = new StreamReader(responseStream);
                                var html = rdr.ReadToEnd();

                                var doc = new HtmlDocument();
                                doc.LoadHtml(HttpUtility.HtmlDecode(html));

                                var titleNode = doc.DocumentNode.SelectSingleNode("//div[@class=\"posttitle\"]/h2");
                                var entryNode = doc.DocumentNode.SelectSingleNode("//div[@class=\"entry\"]");
                                var linkNode = doc.DocumentNode.SelectSingleNode("//div[@class=\"postnav\"]/div[@class=\"alignleft\"]//a");

                                var meaningNode = entryNode.SelectSingleNode("//strong[text()=\"ZNACZENIE\"]/..");
                                var usageNode = entryNode.SelectSingleNode("//strong[text()=\"UŻYCIE\"]/..");
                                var exampleNode = entryNode.SelectSingleNode("//strong[text()=\"PRZYKŁAD\"]/..");
                                var etymologyNode = entryNode.SelectSingleNode("//strong[text()=\"ETYMOLOGIA\"]/..");

                                var t = new Tuple<string, string, string, string, string>(
                                    titleNode != null ? titleNode.InnerText.Trim() : string.Empty,
                                    meaningNode != null ? meaningNode.InnerText.Trim() : string.Empty,
                                    usageNode != null ? usageNode.InnerText.Trim() : string.Empty,
                                    exampleNode != null ? exampleNode.InnerText.Trim() : string.Empty,
                                    etymologyNode != null ? etymologyNode.InnerText.Trim() : string.Empty);
                                sw.WriteLine(string.Concat(t.Item1, ";", t.Item2, ";", t.Item3, ";", t.Item4, ";", t.Item5));

                                if (linkNode == null)
                                    break;
                                var link = linkNode.Attributes["href"].Value;

                                request = WebRequest.Create(link);
                            }
                        }
                    }
                }
            }
        }

        public static string NormalizeKeyword(string keyword)
        {
            return keyword
                .Trim()
                .ToLower(CultureInfo.InvariantCulture)
                .Replace(' ', '_')
                .Replace('ą', 'a')
                .Replace('ć', 'c')
                .Replace('ę', 'e')
                .Replace('ń', 'n')
                .Replace('ł', 'l')
                .Replace('ó', 'o')
                .Replace('ś', 's')
                .Replace('ź', 'z')
                .Replace('ż', 'z');
        }


    }
}
