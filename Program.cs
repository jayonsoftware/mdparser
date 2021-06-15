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
            // display table of contents for the parsed markdown file
            var titles = astMarkdownDocument.Descendants<Markdig.Syntax.MarkdownObject>().ToList();
            foreach (var title in titles)
            {

                if (title is ParagraphBlock)
                {

                    var paragrap = (ParagraphBlock) title;

                    if (paragrap.Inline.FirstChild.GetType())
                    {
                        
                    }

                    
                }
            }

        }
    }
}
