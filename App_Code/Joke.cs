using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for Joke
/// </summary>
public class Joke : SlackBotHandler
{
    public override string[] TriggerWords { get { return new string[] { "@joke" }; } }
    public override string BotName { get { return "A Joke for Everyday"; } }
    public override string Emoji { get { return ":joy:"; } }

    string feedURL = "";
    XmlDocument JokesFeedData
    {
        get
        {
            var rssXmlDoc = HttpContext.Current.Cache.Get("jokesfeed.rss") as XmlDocument;
            if (rssXmlDoc==null)
            {
                var url = "http://www.jokebadger.com/rss/";
                rssXmlDoc = new XmlDocument();

                // Load the RSS file from the RSS URL
                rssXmlDoc.Load(url);

                // Parse the Items in the RSS file
                

                // Load the RSS file from the RSS URL
                rssXmlDoc.Load(url);
                HttpContext.Current.Cache.Add("jokesfeed.rss", rssXmlDoc, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);
            }
            return rssXmlDoc;
        }
    }

    public override string Process(string text)
    {
        try
        {
            
            var jokes = JokesFeedData.SelectNodes("rss/channel/item");

            Random rnd = new Random();

            int rnd1 = rnd.Next(jokes.Count);
            
            var jokeDescriptionNode = jokes[rnd1].SelectSingleNode("description");
            string description = jokeDescriptionNode != null ? jokeDescriptionNode.InnerText : "";
            description=description.Replace("<div style=\"background: #FAFAFA; border: 1px solid #DDD; padding: 4px; margin-bottom: 5px; font-family: 'Lucida Grande', Tahoma, Verdana, sans-serif; font-size: 14px;\"><div style=\"margin-left: 5px;\">","");
            description = description.Replace("<br />", "\n");
            description = StripHTML(description.Substring(0, description.IndexOf("</div>"))); //StripHTML(description.Substring(description.IndexOf("<div>")+5, description.IndexOf("</div>")).Replace("<br />","char(13)")); //Regex.Replace(description, @"<br[^>]*>", " "); //string.Format("{0}", HttpUtility.HtmlDecode(description));
			return string.Format("{0}", HttpUtility.HtmlDecode(description));


        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
    public static string StripHTML(string source)
    {
        return Regex.Replace(source, "<.*?>", string.Empty);
    }
}