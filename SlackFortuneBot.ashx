<%@ WebHandler Language="C#" Class="SlackFortuneBot" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;

public class SlackFortuneBot : IHttpHandler
{
    private const string Response = "The answer to your question is {0}!";

    public void ProcessRequest(HttpContext context)
    {
        var question = context.Request["text"].Substring(context.Request["trigger_word"].Length + 1);

        if (!question.EndsWith("?"))
        {
            context.Response.Write("{ \"text\": \"" + "You must ask a YES or NO question!" + "\" }");
            return;
        }

        var result = (new Random().Next(0, 100)%2 == 0);
        context.Response.Write("{ \"text\": \"" + String.Format(Response, result ? "YES" : "NO") + "\" }");
    }

    public bool IsReusable { get { return true; } }
}