using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;

/// <summary>
/// Summary description for Define
/// </summary>
public class Update : SlackBotHandler
{
    public override string[] TriggerWords { get { return new string[] { "@update" }; } }
    public override string BotName { get { return "Update Bot"; } }
    public override string Emoji { get { return ":arrow_up:"; } }
    public override string Process(string text)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
        {
            WorkingDirectory = "c:\\website\\directory",
            FileName = "c:\\git\\git.exe",
            Arguments = "pull",
            CreateNoWindow = true,
            UseShellExecute = true
        }).WaitForExit();
        return "Update from git complete";
    }
}