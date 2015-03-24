<%@ WebHandler Language="C#" Class="SlackDiceBot" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;

public class SlackDiceBot : IHttpHandler
{
    const int Sides = 6;
    const int MaxRolls = 100;

    public void ProcessRequest(HttpContext context)
    {
        uint max = 1;
        if (!uint.TryParse(context.Request["text"].Substring(context.Request["trigger_word"].Length + 1), out max) || max == 0 || max > MaxRolls)
        {
            context.Response.Write("{ \"text\": \"" + String.Format("Try again with a whole number between 1 and {0}.", MaxRolls) + "\" }");
            return;
        }

        var rand = new Random();
        var rolls = new List<int>();

        for (var i = 0; i < max; i++)
            rolls.Add(rand.Next(1, Sides+1));

        var result = String.Format("Avg {0} of {1} total rolls ({2}).", rolls.Average(), rolls.Count, String.Join(", ", rolls.Select(e => e.ToString())));

        context.Response.Write("{ \"text\": \"" + result + "\" }");
    }

    public bool IsReusable { get { return true; } }
}
