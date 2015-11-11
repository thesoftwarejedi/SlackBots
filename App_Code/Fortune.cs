using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Fortune
/// </summary>
public class Fortune : SlackBotHandler
{
    private const string Response = "The answer to your question is {0}!";

    public override string TriggerWord { get { return "fortune"; } }
    public override string Process(string text)
    {
        if (!text.EndsWith("?"))
            return "You must ask a YES or NO question!";

        var result = (new Random().Next(0, 100) % 2 == 0);
        return String.Format(Response, result ? "YES" : "NO");
    }
}