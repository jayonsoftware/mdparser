using System;
using System.IO;
using System.Linq;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Mdparser12
{
    class Program
    {
        static void Main(string[] args)
        {
            var mdFile = File.ReadAllText("demo.md");
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var astMarkdownDocument = Markdown.Parse(mdFile, pipeline);

            // display table of contents
            var titles = astMarkdownDocument.Descendants<HeadingBlock>().ToList();
            foreach (var title in titles)
            {
                if (title.Inline.FirstChild is LiteralInline inline)
                {
                    // use Level property to display indent
                    Console.Write(new string('-', title.Level) + ">");

                    // node text
                    Console.WriteLine(inline.Content);
                }
            }

            Console.ReadLine();
        }
    }
}
