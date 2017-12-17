using System;
using System.Web;
using System.IO;
using System.Text;
using System.Linq;
using HtmlAgilityPack;

namespace gunits
{
	class GUnits
	{
		static string[] Lines(string input) {
			return input.Split('\n');
		}

		static HtmlNode SearchNode(HtmlDocument doc) {
			return doc.DocumentNode
				.SelectSingleNode("//html/body/table/tbody/tr/td/div/div/div[@id='search']");
		}

		static HtmlNode SearchCardNode(HtmlNode searchNode) {
			return searchNode
				.SelectSingleNode("./div/ol/div/div/div");
		}

		static string FirstEquals(string txt) {
			string[] lines = Lines(txt);
			foreach(string line in lines) {
				if(line.Contains(" = ") || line.Contains("equals")) {
					return line;
				}
			}
			return null as string;
		}

		static void Main(string[] args)
		{
			bool verbose = false;
			var query = new ArraySegment<string>(args);
			if(args[0] == "--verbose") {
				verbose = true;
				query = new ArraySegment<string>(args, 1, args.Length - 1);
			}
			string encoded = HttpUtility.UrlEncode(
				String.Join(" ", query.ToArray())
			);
			string url = "https://google.com/search?q=" + encoded;
			HtmlWeb web = new HtmlWeb();
			var html = web.Load(url);
			if(verbose) {
				var sw = new FileStream("debug.html", FileMode.Create);
				html.Save(sw, Encoding.UTF8);
			}
			// looking for:
			// div#search > div > ol > div > div > div
			// the built-in google search "card"
			// yikes!!
			var search = SearchNode(html);
			var card = SearchCardNode(search);
			if(card != null && card.InnerText.Contains(" = ")) {
				Console.WriteLine(card.InnerText);
			} else {
				// uh oh...
				Console.WriteLine(
					"WARNING: No Google conversion found!"
				);
				string conversion = FirstEquals(search.InnerText);
				if(conversion != null) {
					Console.WriteLine(conversion);
				} else {
					Console.WriteLine(
						"ERROR: No conversions found!"
					);
				}
			}
		}
	}
}
