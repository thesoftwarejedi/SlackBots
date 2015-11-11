using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SlackBotHandler
/// </summary>
public abstract class SlackBotHandler
{    
    public abstract string[] TriggerWords { get; }
    public virtual string BotName { get { return "R2i Bot"; } }
    public virtual string Emoji { get { return ":r2i:"; } }

    public abstract string Process(string text);
}