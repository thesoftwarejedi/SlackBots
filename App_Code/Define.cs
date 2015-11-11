using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;

/// <summary>
/// Summary description for Define
/// </summary>
public class Define : SlackBotHandler
{
    public override string[] TriggerWords { get { return new string[] { "@define" }; } }
    public override string BotName { get { return "Dictionary"; } }
    public override string Emoji { get { return ":book:"; } }
    public override string Process(string text)
    {
        try
        {
            return Json.Decode(new WebClient().DownloadString(string.Format("http://api.wordnik.com:80/v4/word.json/{0}/definitions?limit=1&includeRelated=true&useCanonical=false&includeTags=false&api_key=a2a73e7b926c924fad7001ca3111acd55af2ffabf50eb4ae5", HttpUtility.UrlEncode(text))))[0].text;
        }
        catch(Exception ex)
        {
            return ex.ToString();
        }
    }
}