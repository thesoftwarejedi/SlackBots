<%@ WebHandler Language="C#" Class="SlackBotListener" %>

using System;
using System.Web;
using System.Web.Helpers;
using System.Collections.Generic;
using System.Linq;

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
                        _handlers.Add(handler.TriggerWord, handler);
                }

            }
            return _handlers;
        }
    }

    public void ProcessRequest (HttpContext context)
    {
        var trigger = context.Request["trigger_word"];
        var resp = string.Empty;
        var text = string.Empty;

        if (string.IsNullOrEmpty(trigger))
            resp = "missing trigger";
        else
        {
            var handler = (SlackBotHandler)null;

            if (Handlers.TryGetValue(trigger, out handler))
            {
                if (context.Request["text"] != context.Request["trigger_word"])
                    text = context.Request["text"].Substring(context.Request["trigger_word"].Length + 1);

                resp = handler.Process(text);
            }
            else
            {
                resp = string.Format("Invalid trigger word '{0}'. Only valid for [{1}]", trigger, string.Join(", ", Handlers.Keys));
            }
        }
        context.Response.Write(Json.Encode(new { text = resp })); //, trigger = trigger, keyword = text }));
    }

public bool IsReusable { get { return false; } }

}