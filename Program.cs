using System;
using System.IO;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Newtonsoft.Json;
using Scriban;

namespace Mdparser12
{
    class Program
    {
        static void Main(string[] _)
        {
            //Use the default configuration for AngleSharp
            var config = Configuration.Default;

            //Create a new context for evaluating webpages with the given config
            var context = BrowsingContext.New(config);

            var source = File.ReadAllText("demo.html");
            var parser = context.GetService<IHtmlParser>();
            var document = parser.ParseDocument(source);

            var elements = document.QuerySelector("div");
            Console.WriteLine(elements.Id);

            foreach (var elementsChildNode in elements.ChildNodes)
            {
                Console.WriteLine(elementsChildNode);
            }

            Console.WriteLine();
            //foreach (var element in elements)
            //{
            //    var html = element.InnerHtml;
            //    Console.WriteLine(html);
            //    Console.WriteLine("*******************************************************");

            //    //if (html.Contains("class=\"source\""))
            //{
            //    //ToDo: Find the link and text
            //    //ToDo: call LinkRenderer method below, get the text and replace this element ?.

            //}
            //else if (html.Contains("class=\"bookmark-info\"")) {

            //    foreach (var elementChildNode in element.ChildNodes)
            //    {
            //       // Console.WriteLine(elementChildNode.NodeType);
            //       // Console.WriteLine(elementChildNode.TextContent);
            //    }

            //    //Console.WriteLine("Its A book mark");
            //}
            //}

            //Todo: Save the html...for now save to another file other then the source file



            //
            Console.ReadLine();
        }


        // All Link is managed here, returnes an html back
        public static string LinkRenderer(Link link)
        {
            var linkType = "YouTube";
            if (linkType == "YouTube")
            {
                var lmTemplate = File.ReadAllText("YouTube.sbnhtml");
                var template = Template.Parse(lmTemplate);
                var result = template.Render(new { link.URL, link.Description }); ;
                return result;
            }
            else
            {
                return "<a href=\"" + link.URL + "\">" + link.Description + "link text" + "</a>";

            }
        }
    }
}
