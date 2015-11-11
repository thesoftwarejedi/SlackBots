using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SlackBotHandler
/// </summary>
public abstract class SlackBotHandler
{    
    public abstract string TriggerWord { get; }
    public abstract string Process(string text);
}