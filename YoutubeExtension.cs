using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;
using Scriban;

namespace Mdparser12
{
	/// <summary>
	/// Replaces links to Youtube videos with iframe code.
	/// </summary>
	public class YoutubeExtension : IMarkdownExtension
	{
		/// <summary>
		/// Sets up the parsing pipeline.
		/// </summary>
		/// <param name="pipeline">The pipeline to set up.</param>
		public void Setup(MarkdownPipelineBuilder pipeline)
		{
		}

		/// <summary>
		/// Sets up the rendering pipeline.
		/// </summary>
		/// <param name="pipeline">The pipeline to set up.</param>
		/// <param name="renderer">The renderer to set up.</param>
		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
		{
			renderer.ObjectRenderers.TryRemove<LinkInlineRenderer>();
			renderer.ObjectRenderers.AddIfNotAlready<YoutubeLinkInlineRenderer>();
		}

		/// <summary>
		/// Renders customized links for Youtube videos.
		/// </summary>
		internal class YoutubeLinkInlineRenderer : LinkInlineRenderer
		{
			protected override void Write(HtmlRenderer renderer, LinkInline link)
			{
				if (link.Url.Contains("youtube.com", StringComparison.OrdinalIgnoreCase) ||
					link.Url.Contains("youtu.be", StringComparison.OrdinalIgnoreCase))
				{
					// extract video identifier portion
					var embedLink = link.Url;
					var url = new UriBuilder(link.Url);

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

					// display optional title
					var title = link.Title;
					if (string.IsNullOrWhiteSpace(title) && link.LastChild is LiteralInline text)
					{
						title = text.Content.ToString();
					}

					// fall back to the default title
					if (string.IsNullOrWhiteSpace(title))
					{
						title = "Youtube video player";
					}

                    
                    renderer.Write(LinkRenderer("YouTube", title, embedLink));
                }
				else
				{
					base.Write(renderer, link);
				}
			}

            public string LinkRenderer(String linkType, String title, String embedlink)
            {
                if (linkType == "YouTube")
                {
					var lmTemplate = File.ReadAllText("YouTube.sbnhtml");
                    var template = Template.Parse(lmTemplate);
                    var result = template.Render(new { embedlink, title});
                    return result;
                }
                else
                {
                    return "<a href=\""+ embedlink + "\">"+ title +"link text" + "</a>";

                }
            }
		}
	}
}
