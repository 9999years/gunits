using System;
using System.Web;
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
			string encoded = HttpUtility.UrlEncode(
				String.Join(" ", args)
			);
			string url = "https://google.com/search?q=" + encoded;
			HtmlWeb web = new HtmlWeb();
			var html = web.Load(url);
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
