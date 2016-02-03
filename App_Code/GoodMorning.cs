using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Helpers;
using System.Web;

/// <summary>
/// Summary description for GoodMorning
/// </summary>
public class GoodMorning : SlackBotHandler
{

    string GoodMorningHTML
    {
        get
        {
            var ret = HttpContext.Current.Cache.Get("GoodMorning.htm") as string;
            if (string.IsNullOrEmpty(ret))
            {
                var url = "http://www.omniglot.com/language/phrases/goodmorning.htm";

                using (var web = new WebClient())
                {
                    ret = web.DownloadString(url);
                }
                HttpContext.Current.Cache.Add("GoodMorning.htm", ret, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);
            }
            return ret;
        }
    }
    public override string[] TriggerWords { get { return new string[] { "good morning" }; } }
    public override string BotName { get { return "Good Morning To You Too"; } }
    public override string Emoji { get { return ":sunrise:"; } }
    public override string Process(string text)
    {
        var html = new HtmlDocument();
        var random = new Random();

        html.LoadHtml(GoodMorningHTML);

        var rows = html.DocumentNode.QuerySelector("#unicode").QuerySelectorAll("tr");
        var rnd = random.Next(1, rows.Count() - 1);        
        var row = rows.Skip(rnd).First();
        var languageCell = row.QuerySelector("td:first-child");
        var language = string.Empty;

        if (languageCell.QuerySelector("a") != null)
            language = Regex.Replace(HttpUtility.HtmlDecode(languageCell.QuerySelector("a").InnerHtml).Replace("\n", "").Trim(), @"<br[^>]*>", " ");
        else
            language = Regex.Replace(HttpUtility.HtmlDecode(languageCell.InnerHtml).Replace("\n", "").Trim(), @"<br[^>]*>", " ");
        if (string.IsNullOrEmpty(language))
            return null;

        var translationsCell = row.QuerySelector("td:last-child");
        var translation = string.Empty;

        if (translationsCell.QuerySelector("br") == null)   // only one entry
        {
            if (translationsCell.QuerySelector("a") != null)
                translation = translationsCell.QuerySelector("a").InnerText;
            else
                translation = translationsCell.InnerText;
        }
        else
        {
            var cellHtml = translationsCell.InnerHtml;
            var translations = Regex.Replace(cellHtml, @"<br[^>]*>", "|").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            rnd = random.Next(translations.Length - 1);
            var fragment = new HtmlDocument();
            fragment.LoadHtml(translations[rnd].Replace("\n", "").Trim());
            var translationCell = fragment.DocumentNode;
            if (translationCell.QuerySelector("a") != null)
                translation = Regex.Replace(translationCell.QuerySelector("a").InnerHtml, @"<br[^>]*>", " ");
            else
                translation = Regex.Replace(translationCell.InnerHtml, @"<br[^>]*>", " ");

            if (!string.IsNullOrEmpty(translation))
            {
                fragment = new HtmlDocument();
                fragment.LoadHtml(translation);
                translation = fragment.DocumentNode.InnerText;
            }

        }
        if (string.IsNullOrEmpty(translation))
            return null;

        return string.Format("{0} (_{1}_)", HttpUtility.HtmlDecode(translation), language);
    }
}