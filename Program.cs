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
            var mdFile = File.ReadAllText("test.md");
            var pipeline = new MarkdownPipelineBuilder()
                .Use<YoutubeExtension>()
                .UseAdvancedExtensions()
                .Build();

            var ast = Markdown.Parse(mdFile, pipeline);
            var html = Markdown.ToHtml(ast, pipeline);

            Console.WriteLine(html);
            //Console.ReadLine();
        }
    }
}
