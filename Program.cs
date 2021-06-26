using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using Scriban;
using Scriban.Syntax;

namespace Mdparser12
{
	class Program
	{
		static void NodeParser(INode node)
		{
			Console.WriteLine(node.NodeType);
			var nextNode = node.NextSibling;
			if (nextNode == null)
			{
				return;
			}
			NodeParser(nextNode);
		}

		static void Main(string[] _)
		{
			var source = File.ReadAllText("demo.html");
			var result = ReplaceYoutubeLinks(source);
			File.WriteAllText("result.html", result);
			
			Console.WriteLine(result);
			Console.ReadLine();
		}

		private static string ReplaceYoutubeLinks(string html)
		{
			// parse the document
			var config = Configuration.Default;
			var context = BrowsingContext.New(config);
			var parser = context.GetService<IHtmlParser>();
			var document = parser.ParseDocument(html);

			// find all youtube links
			var links = document.All.Where(n => n.LocalName == "a").ToList();
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

				// replace the link with the rendered
				link.ReplaceWith(nodes.ToArray());
			}

			// render the modified document
			return document.DocumentElement.OuterHtml;
		}

		private static Source PrepareYoutubeVideo(IElement element)
		{
			var ignoreCase = StringComparison.OrdinalIgnoreCase;
			if (element is IHtmlAnchorElement link)
			{
				if (link.Href.Contains("youtube.com", ignoreCase) ||
					link.Href.Contains("youtu.be", ignoreCase))
				{
					// note: links don't have their own Ids
					var src = new Source(string.Empty);
					src.Description = link.Title ?? "Youtube video player";
					src.URL = GetYoutubeEmbedLink(link.Href);
					return src;
				}
			}

			return null;
		}

		private static string GetYoutubeEmbedLink(string linkUrl)
		{
			// extract video identifier portion
			var embedLink = linkUrl;
			var url = new UriBuilder(linkUrl);

			// assume something like https://youtu.be/HeQX2HjkcNo
			if (url.Host.Contains("youtu.be", StringComparison.OrdinalIgnoreCase))
			{
				embedLink = $"https://www.youtube.com/embed{url.Path}";
			}
			else // assume something like https://www.youtube.com/watch?v=HeQX2HjkcNo
			{
				var queryParts = url.Query.Trim('?').Split('&');
				foreach (var part in queryParts)
				{
					if (part.StartsWith("v=", StringComparison.OrdinalIgnoreCase))
					{
						embedLink = $"https://www.youtube.com/embed/{part[2..].Trim()}";
					}
				}
			}

			return embedLink;
		}

		// Given an html, parse it and return a List<Node> 
		public static List<Node> GetNotionNodes(string html)
		{
			var notionNodes = new List<Node>();

			//Use the default configuration for AngleSharp
			var config = Configuration.Default;

			//Create a new context for evaluating webpages with the given config
			var context = BrowsingContext.New(config);
			var parser = context.GetService<IHtmlParser>();
			var document = parser.ParseDocument(html);

			var elements = document.QuerySelector("div");

			foreach (var elementsChildNode in elements.ChildNodes)
			{
				if (elementsChildNode.NodeType == NodeType.Element)
				{
					var elementNode = (Element)elementsChildNode;
					Console.WriteLine("> " + elementNode.Id);
					Console.WriteLine(">> " + elementNode.ClassName);
					foreach (var chileNode in elementNode.Children)
					{
						Console.WriteLine(">>> " + chileNode.NodeType);
						Console.WriteLine(">>>> " + chileNode.ClassName);
						if (chileNode.ClassName == "source")
						{
							var newSource = new Source(elementNode.Id);
							notionNodes.Add(newSource);
						}
					}

				}
			}

			return notionNodes;
		}

		public string ReplaceHtml(string html, string nodeId, string htmlBlock)
		{
			//Todo : Append the given html to the correct postion. 
			return html;
		}


		// All Link is managed here, returns an html back
		public static string RenderYoutubeLink(Source link)
		{
			var lmTemplate = File.ReadAllText("YouTube.sbnhtml");
			var template = Template.ParseLiquid(lmTemplate);
			var result = template.Render(new
			{
				test = "Cool",
				embedlink = link.URL,
				title = link.Description
			});

			return result;
		}
	}
}
