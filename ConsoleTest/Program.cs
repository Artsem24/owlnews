using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;

namespace ConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var parser = new Parser();
			parser.Work();
		}

		public class Parser
		{
			public async void Work()
			{
				const string url = "https://citydog.by/allposts/category/people/";

				var loader = new HtmlLoader(url);

				var source = await loader.GetSourceByPageId(1);

				var parser = new HtmlParser();

				var document = parser.Parse(source);

				var itemsText = document.QuerySelectorAll(
					"div > div.headingArticle > h2 > a");
				var itemsImg = document.QuerySelectorAll(
					"div > div.imageWrapper > a > img");

				var listText = itemsText.Select(item => item.TextContent).ToList();

				var listImg = itemsImg.Select(item => item.GetAttribute("src")).ToList();

				var dic = listText.Zip(listImg, (k, v) => new { k, v })
					.ToDictionary(x => x.k, x => x.v);

				foreach (var res in dic)
				{
					Console.WriteLine($"Заголовок: {res.Key}");
					Console.WriteLine($"Ссылка на изображение: {res.Value}");
					Console.WriteLine($"-----------------------------------------------------------");
				}
			}
		}

		public class HtmlLoader
		{
			readonly HttpClient _client;
			private readonly string _url;

			public HtmlLoader(string url)
			{
				_client = new HttpClient();
				_url = url;
			}

			public async Task<string> GetSourceByPageId(int id)
			{
				var currentUrl = _url.Replace("{CurrentId}", id.ToString());
				var response = _client.GetAsync(currentUrl);
				string source = null;

				if (response != null && response.Result.StatusCode == HttpStatusCode.OK)
				{
					source = await response.Result.Content.ReadAsStringAsync();
				}

				return source;
			}
		}
	}
}
