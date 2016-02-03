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
    public override string Emoji { get { return ":joy: :smiley: "; } }

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
            
            return Regex.Replace(description, @"<br[^>]*>", " "); //string.Format("{0}", HttpUtility.HtmlDecode(description));


        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
    private string GetAlbumRSS(SyndicationItem album)
    {

        string url = "";
        foreach (SyndicationElementExtension ext in album.ElementExtensions)
            if (ext.OuterName == "itemRSS") url = ext.GetObject<string>();
        return (url);

    }
}