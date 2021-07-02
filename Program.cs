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
	}
}
