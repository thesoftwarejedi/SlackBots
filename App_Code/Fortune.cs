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

    public override string[] TriggerWords { get { return new string[] { "@fortune", "@ask" }; } }
    public override string BotName { get { return "Fortune"; } }
    public override string Emoji { get { return ":crystal_ball:"; } }
    public override string Process(string text)
    {
        if (!text.EndsWith("?"))
            return "You must ask a YES or NO question!";

        var result = (new Random().Next(0, 100) % 2 == 0);
        return String.Format(Response, result ? "YES" : "NO");
    }
}