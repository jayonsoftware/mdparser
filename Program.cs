using System;
using System.IO;
using Markdig;
using Markdig.Syntax;

namespace Mdparser12
{
    class Program
    {
        static void Main(string[] args)
        {
            var mdFile = File.ReadAllText("demo.md");
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var astMarkdownDocument = Markdown.Parse(mdFile, pipeline);
            foreach (var block in astMarkdownDocument)
            {
                if (block is Markdig.Syntax.ParagraphBlock)
                {
                    var paragrap = (ParagraphBlock) block;
                    var lines = paragrap.Lines;
                    Console.WriteLine(paragrap.Inline.FirstChild);
                }
            }

        }
    }
}
