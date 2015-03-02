<%@ WebHandler Language="C#" Class="SlackDefineBot" %>

using System;
using System.Net;
using System.Web;
using System.Web.Helpers;

public class SlackDefineBot : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.Write("{ \"text\": \"" + Json.Decode(new WebClient().DownloadString("http://api.wordnik.com:80/v4/word.json/" + context.Request["text"].Substring(context.Request["trigger_word"].Length + 1) + "/definitions?limit=1&includeRelated=true&useCanonical=false&includeTags=false&api_key=a2a73e7b926c924fad7001ca3111acd55af2ffabf50eb4ae5"))[0].text + "\" }");
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}