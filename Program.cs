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
		static void Main(string[] _)
		{
			// parse demo.html, save result.html
			var source = File.ReadAllText("in/notion/demo.html");
			var result = YoutubeLinkHelper.ReplaceYoutubeLinks(source);
			File.WriteAllText("out/html/demo.html", result);
		}

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
	}
}
