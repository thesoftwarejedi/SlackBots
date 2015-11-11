<%@ WebHandler Language="C#" Class="SlackBotListener" %>

using System;
using System.Web;
using System.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text.RegularExpressions;

public class SlackBotListener : IHttpHandler
{
    Dictionary<string, SlackBotHandler> _handlers;
    Dictionary<string, SlackBotHandler> Handlers
    {
        get
        {
            if (_handlers == null)
            {
                _handlers = new Dictionary<string, SlackBotHandler>();

                var types = System.Reflection.Assembly.Load("App_Code").GetTypes();
                foreach(var type in types.Where(t => t.BaseType == typeof(SlackBotHandler) )) // ((t.BaseType != null) && (t.BaseType.Name == "SlackBotHandler")) )) // !t.IsAbstract && t.IsAssignableFrom(typeof(SlackBotHandler))))
                {
                    var handler = Activator.CreateInstance(type) as SlackBotHandler;
                    if (handler != null)
                    {
                        foreach(var word in handler.TriggerWords)
                            _handlers.Add(word.ToLower(), handler);
                    }
                }

            }
            return _handlers;
        }
    }

    public void ProcessRequest (HttpContext context)
    {
        var token = context.Request["token"];
        if (token != ConfigurationManager.AppSettings["SlackToken"])
        {
            context.Response.Clear();
            context.Response.StatusCode = 403;
            return;
        }

        var trigger = context.Request["trigger_word"].ToLower();
        var team_id = context.Request["team_id"];
        var team_domain = context.Request["team_domain"];
        var channel_id = context.Request["channel_id"];
        var channel_name = context.Request["channel_name"];
        var user_id = context.Request["user_id"];
        var user_name = context.Request["user_name"];

        var resp = string.Empty;
        var text = string.Empty;

        var handler = (SlackBotHandler)null;

        if (Handlers.TryGetValue(trigger, out handler))
        {
            if (context.Request["text"] != trigger)
            {
                var rx = string.Join("|", handler.TriggerWords.Select(w => w + "\\s"));
                text = Regex.Replace(context.Request["text"], rx, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }

            resp = handler.Process(text);
            if (resp.StartsWith("{"))
                context.Response.Write(resp);
            else
                context.Response.Write(Json.Encode(new
                {
                    username = handler.BotName,
                    icon_emoji = handler.Emoji,
                    text = resp
                }));
        }
        else
        {
            context.Response.Write(Json.Encode(new
            {
                username = "R2i Bot",
                icon_emoji = ":r2i:",
                text = string.Format("Invalid trigger word '{0}'. Only valid for [{1}]", trigger, string.Join(", ", Handlers.Keys))
            }));
        }
    }

    public bool IsReusable { get { return false; } }

}