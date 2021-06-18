using System;
using System.IO;
using System.Linq;
using AngleSharp;
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

            var elements = document.QuerySelectorAll("figure");
            foreach (var element in elements)
            {
                var html = element.InnerHtml;
                if (html.Contains("class=\"source\""))
                {
                    //ToDo: Find the link and text
                    //ToDo: call LinkRenderer method below, get the text and replace this element ?.
               
                }
                else if (html.Contains("class=\"bookmark-info\"")) {

                    foreach (var elementChildNode in element.ChildNodes)
                    {
                       // Console.WriteLine(elementChildNode.NodeType);
                       // Console.WriteLine(elementChildNode.TextContent);
                    }

                    //Console.WriteLine("Its A book mark");
                }
            }

            //Todo: Save the html...for now save to another file other then the source file
         

            //var mdFile = File.ReadAllText("test.md");
            //var pipeline = new MarkdownPipelineBuilder()
            //    .Use<YoutubeExtension>()
            //    .UseAdvancedExtensions()
            //    .Build();

            //var ast = Markdown.Parse(mdFile, pipeline);
            //var html = Markdown.ToHtml(ast, pipeline);

            //Console.WriteLine(html);
            Console.ReadLine();
        }

        public static string LinkRenderer(String linkType, String title, String embedlink)
        {
            if (linkType == "YouTube")
            {
                var lmTemplate = File.ReadAllText("YouTube.sbnhtml");
                var template = Template.Parse(lmTemplate);
                var result = template.Render(new { embedlink, title });
                return result;
            }
            else
            {
                return "<a href=\"" + embedlink + "\">" + title + "link text" + "</a>";

            }
        }
    }
}
