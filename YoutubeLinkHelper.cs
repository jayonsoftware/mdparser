using System;
using System.IO;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using Scriban;

namespace Mdparser12
{
	/// <summary>
	/// Youtube link helper.
	/// </summary>
	public class YoutubeLinkHelper
	{
		private static StringComparison IgnoreCase => StringComparison.OrdinalIgnoreCase;

		/// <summary>
		/// Replaces links to Youtube videos with iframe code.
		/// </summary>
		public static string ReplaceYoutubeLinks(string html)
		{
			// parse the document
			var config = Configuration.Default;
			var context = BrowsingContext.New(config);
			var parser = context.GetService<IHtmlParser>();
			var document = parser.ParseDocument(html);

			var allLinks = document.All;
			foreach (var link in allLinks)
			{
				if (!string.IsNullOrEmpty(link.Id) )
				{
					if (link.FirstElementChild != null && link.FirstElementChild.ClassName == "source" && link.TagName == "FIGURE")
					{
						var element = (IHtmlAnchorElement) link.FirstElementChild.FirstElementChild;
						if (element != null)
						{
							var source = new Source(link.Id) {URL = element.Href, Description = element.InnerHtml};

							Console.WriteLine(JsonConvert.SerializeObject(source, Formatting.Indented));

							if (element.Href.Contains("youtube.com", IgnoreCase) || element.Href.Contains("youtu.be", IgnoreCase))
							{
								var lmTemplate = File.ReadAllText("in/sbnhtml/youtube.sbnhtml");
								var template = Template.Parse(lmTemplate);
								var result = template.Render(new {Id = source.Id, URL = source.URL, Description = source.Description});
								//Console.WriteLine(result);

								var nodes = parser.ParseFragment(result, link);
								link.ReplaceWith(nodes.ToArray());
							} else if (element.Href.Contains("www.google.com/maps/", IgnoreCase))
							{

								var lmTemplate = File.ReadAllText("in/sbnhtml/googlemaps.sbnhtml");
								var template = Template.Parse(lmTemplate);
								var result = template.Render(new { Id = source.Id, URL = source.URL, Description = source.Description });
								//Console.WriteLine(result);

								var nodes = parser.ParseFragment(result, link);
								link.ReplaceWith(nodes.ToArray());
							}
						}
						
					}
				}
			}
			return document.DocumentElement.OuterHtml;
		}

		/// <summary>
		/// Detects if the given HTML element is a youtube link and if it's so, produces a model class out of it.
		/// </summary>
		private static Source PrepareYoutubeVideo(IHtmlAnchorElement link)
		{
			if (link.Href.Contains("youtube.com", IgnoreCase) || link.Href.Contains("youtu.be", IgnoreCase))
			{
				// note: links don't have their own Ids
				var src = new Source(string.Empty)
				{
					Description = link.Title ?? "Youtube video player", URL = GetYoutubeEmbedLink(link.Href)
				};
				return src;
			}

			return null;
		}

		/// <summary>
		/// Transforms youtube shareable video link into an embeddable link.
		/// </summary>
		private static string GetYoutubeEmbedLink(string linkUrl)
		{
			// extract video identifier portion
			var embedLink = linkUrl;
			var url = new UriBuilder(linkUrl);

			// assume something like https://youtu.be/HeQX2HjkcNo
			if (url.Host.Contains("youtu.be", IgnoreCase))
			{
				embedLink = $"https://www.youtube.com/embed{url.Path}";
			}
			else // assume something like https://www.youtube.com/watch?v=HeQX2HjkcNo
			{
				var queryParts = url.Query.Trim('?').Split('&');
				foreach (var part in queryParts)
				{
					if (part.StartsWith("v=", IgnoreCase))
					{
						embedLink = $"https://www.youtube.com/embed/{part[2..].Trim()}";
					}
				}
			}

			return embedLink;
		}
	}
}
