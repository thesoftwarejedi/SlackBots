<%@ WebHandler Language="C#" Class="SlackDriveTimeBot" %>

using System;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.Helpers;

public class SlackDriveTimeBot : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        var keyword = context.Request["text"].Substring(context.Request["trigger_word"].Length + 1);
        var key = ConfigurationManager.AppSettings["MapAPIKey"];
        var text = string.Empty;

        try
        {
            if (string.Equals(keyword, "ali", StringComparison.CurrentCultureIgnoreCase))
            {
                var url = string.Format("https://maps.googleapis.com/maps/api/distancematrix/json?origins=400%20E%20Pratt,%20Baltimore%20MD&destinations=Hunt%20Valley|MD&mode=driving&key={0}", key);
                var drive = Json.Decode(new WebClient().DownloadString(url));

                var minutes = (int)drive.rows[0].elements[0].duration.value;
                var time = drive.rows[0].elements[0].duration.text.ToString();

                if (minutes > (120 * 60))
                {
                    text = "83 SUCKS!  " + time + " is excessive, might as well just stay.";
                }
                else if (minutes > (45 * 60))
                {
                    text = "Ali's drive home is about average, just think what you could do with " + time + " extra in your day!";
                }
                else
                {
                    text = "Ali's drive home is " + time + ", stick around you have plenty of time to get home.";
                }
            }
            else if (string.Equals(keyword, "dana", StringComparison.CurrentCultureIgnoreCase))
            {
                context.Response.Write(Json.Encode(new
                {
                    icon_emoji = ":walking:",
                    text = "Your commute home should be around 6 minutes."
                }));

                return;
            }
            else
            {
                var url = string.Format("https://maps.googleapis.com/maps/api/distancematrix/json?origins=400%20E%20Pratt,%20Baltimore%20MD&destinations={1}|MD&mode=driving&key={0}", key, keyword);
                var drive = Json.Decode(new WebClient().DownloadString(url));

                var time = drive.rows[0].elements[0].duration.text.ToString();
                text = "Your commute home should be around " + time;
            }

        }
        catch (Exception)
        {
            text = "Your locations does not exist";
        }

        context.Response.Write(Json.Encode(new
        {
            text
        }));
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }

}