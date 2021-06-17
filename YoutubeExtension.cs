using System;
using System.Collections.Generic;
using System.Text;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

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

					// <iframe width="560" height="315" src="embedLink" title="title" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
					renderer.Write("<iframe width=\"560\" height=\"315\" src=\"");
					renderer.WriteEscapeUrl(embedLink);
					renderer.Write("\" title=\"");
					renderer.WriteEscape(title);
					renderer.Write("\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>");
				}
				else
				{
					base.Write(renderer, link);
				}
			}
		}
	}
}
