using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;

/// <summary>
/// Summary description for DriveTime
/// </summary>
public class DriveTime : SlackBotHandler
{
    public override string TriggerWord { get { return "traffic"; } }
    public override string Process(string text)
    {
        var key = ConfigurationManager.AppSettings["MapAPIKey"];
        var ret = string.Empty;

        try
        {
            if (string.Equals(text, "ali", StringComparison.CurrentCultureIgnoreCase))
            {
                var url = string.Format("https://maps.googleapis.com/maps/api/distancematrix/json?origins=400%20E%20Pratt,%20Baltimore%20MD&destinations=Hunt%20Valley|MD&mode=driving&key={0}", key);
                var drive = Json.Decode(new WebClient().DownloadString(url));

                var minutes = (int)drive.rows[0].elements[0].duration.value;
                var time = drive.rows[0].elements[0].duration.text.ToString();

                if (minutes > (120 * 60))
                {
                    ret = "83 SUCKS!  " + time + " is excessive! You might as well just stay.";
                }
                else if (minutes > (45 * 60))
                {
                    ret = "Ali's drive home is about average. Just think what you could do with " + time + " extra in your day!";
                }
                else
                {
                    ret = "Ali's drive home is " + time + ". Stick around! You have plenty of time to get home.";
                }
            }
            else if (string.Equals(text, "dana", StringComparison.CurrentCultureIgnoreCase))
            {
                ret = Json.Encode(new
                {
                    icon_emoji = ":walking:",
                    text = "Your commute home should be around 6 minutes."
                });                
            }
            else
            {
                var url = string.Format("https://maps.googleapis.com/maps/api/distancematrix/json?origins=400%20E%20Pratt,%20Baltimore%20MD&destinations={1}|MD&mode=driving&key={0}", key, text);
                var drive = Json.Decode(new WebClient().DownloadString(url));

                var time = drive.rows[0].elements[0].duration.text.ToString();
                ret = "Your commute home should be around " + time;
            }

        }
        catch (Exception)
        {
            ret = "Your location does not exist";
        }

        return ret;
    }
}