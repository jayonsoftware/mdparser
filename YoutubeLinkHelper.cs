using System;
using System.IO;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Scriban;

namespace Mdparser12
{
	/// <summary>
	/// Youtube link helper.
	/// </summary>
	public class YoutubeLinkHelper
	{
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

			// find all youtube links
			var links = document.All.OfType<IHtmlAnchorElement>().ToList();
			foreach (var link in links)
			{
				// skip non-youtube links
				var source = PrepareYoutubeVideo(link);
				if (source == null)
				{
					// not a youtube link
					continue;
				}

				// generate HTML fragment using the templating engine
				var fragment = RenderYoutubeLink(source);
				var nodes = parser.ParseFragment(fragment, link);

				// replace the link with the rendered fragment
				link.ReplaceWith(nodes.ToArray());
			}

			// render the modified document
			return document.DocumentElement.OuterHtml;
		}

		private static StringComparison IgnoreCase => StringComparison.OrdinalIgnoreCase;

		/// <summary>
		/// Detects if the given HTML element is a youtube link and if it's so, produces a model class out of it.
		/// </summary>
		private static Source PrepareYoutubeVideo(IHtmlAnchorElement link)
		{
			if (link.Href.Contains("youtube.com", IgnoreCase) || link.Href.Contains("youtu.be", IgnoreCase))
			{
				// note: links don't have their own Ids
				var src = new Source(string.Empty);
				src.Description = link.Title ?? "Youtube video player";
				src.URL = GetYoutubeEmbedLink(link.Href);
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

		/// <summary>
		/// Lazy-evaluated Youtube iframe template.
		/// </summary>
		private static Lazy<Template> YoutubeTemplate { get; } = new Lazy<Template>(() =>
		{
			var lmTemplate = File.ReadAllText("YouTube.sbnhtml");
			return Template.Parse(lmTemplate);
		});

		/// <summary>
		/// Renders a Youtube iframe fragment.
		/// </summary>
		private static string RenderYoutubeLink(Source link)
		{
			return YoutubeTemplate.Value.Render(new
			{
				embedlink = link.URL,
				title = link.Description
			});
		}
	}
}
