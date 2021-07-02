using System.IO;

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
