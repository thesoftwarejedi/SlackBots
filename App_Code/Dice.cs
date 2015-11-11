using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Dice
/// </summary>
public class Dice : SlackBotHandler
{
    const int Sides = 6;
    const int MaxRolls = 100;
    public override string TriggerWord { get { return "dice"; } }
    public override string Process(string text)
    {
        uint max = 1;
        if (!uint.TryParse(text, out max) || max == 0 || max > MaxRolls)
            return String.Format("Try again with a whole number between 1 and {0}.", MaxRolls);

        var rand = new Random();
        var rolls = new List<int>();

        for (var i = 0; i < max; i++)
            rolls.Add(rand.Next(1, Sides + 1));

        return String.Format("Avg {0} of {1} total rolls ({2}).", rolls.Average(), rolls.Count, String.Join(", ", rolls.Select(e => e.ToString())));
    }
}