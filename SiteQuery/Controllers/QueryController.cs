using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using SiteQuery.Extensions;

namespace SiteQuery.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Query([FromQuery] string sitemap, [FromQuery] string xpath,
            [FromQuery] string query)
        {
            if (!ValidXpath(xpath)) return BadRequest("Invalid Xpath");

            var client = new HttpClient();
            var response = await client.GetAsync(sitemap);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound("Invalid Url");
            }

            var body = await response.Content.ReadAsStringAsync();
            client.Dispose();

            var xxx = await QueryXml(body, xpath, query);
            return xxx is not null ? Ok(xxx) : BadRequest("Invalid Xml");
        }

        private static bool ValidXpath(string xpath)
        {
            try
            {
                XPathExpression.Compile(xpath);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private static async Task<Dictionary<string, List<string>>?> QueryXml(string sitemapString, string xpath,
            string query)
        {
            var xml = new XmlDocument();
            xml.LoadXml(sitemapString);
            var sites = xml.DocumentElement?.GetElementsByTagName("loc").Cast<XmlNode>().Select(x => x.InnerText);
            var client = new HttpClient();


            var sitesHtml = new Dictionary<string, List<string>>();
            if (sites != null)
            {
                await Parallel.ForEachAsync(sites,
                    async (site, cancellationToken) =>
                    {
                        var result = (await client.GetStringAsync(site, cancellationToken)).ReplaceLineEndings();

                        //const string xpathTest = "//div[contains(@class,'react-component')]";

                        var html = new HtmlDocument();
                        html.LoadHtml(result);
                        var body = html.DocumentNode.ChildNodes.SingleOrDefault(node => node.Name == "html")?.ChildNodes
                            .SingleOrDefault(node => node.Name == "body")?
                            .SelectNodes(xpath: xpath);

                        //TODO Throw error to outside function
                        if (body is null) return;
                        var divs = body.Where(div => div.OuterHtml.Contains(query)).Select(div => div.OuterHtml)
                            .ToList();
                        sitesHtml.Add(site, divs);
                    });
            }

            return sitesHtml;
        }
    }
}