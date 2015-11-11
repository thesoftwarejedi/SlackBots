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
    public override string[] TriggerWords { get { return new string[] { "@drive", "@drivetime", "@traffic" }; } }
    public override string BotName { get { return "Traffic"; } }
    public override string Emoji { get { return ":traffic_light:"; } }
    public override string Process(string text)
    {
        var key = ConfigurationManager.AppSettings["MapAPIKey"];
        var ret = string.Empty;
        var resp = string.Empty;

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
                var url = string.Format("https://maps.googleapis.com/maps/api/distancematrix/json?origins=400%20E%20Pratt,%20Baltimore%20MD&destinations={0}&mode=driving&units=imperial", HttpUtility.UrlEncode(text));
                resp = new WebClient().DownloadString(url);
                var drive = Json.Decode(resp);

                var time = drive.rows[0].elements[0].duration.text.ToString();
                var distance = drive.rows[0].elements[0].distance.text.ToString();
                ret = string.Format("Your commute of {0} should be around {1}", distance, time);
                /*
                ret = Json.Encode(new
                {
                    username = this.BotName,
                    icon_emoji = this.Emoji,
                    text = string.Format("Your commute of {0} should be around {1}", distance, time),
                    attachments = new[]
                    {
                        new
                        {
                            fields = new[]
                            {
                                new
                                {
                                    title = "Trigger Word",
                                    value = HttpContext.Current.Request["trigger_word"],
                                    @short = false
                                },
                                new
                                {
                                    title = "Trigger Text",
                                    value = HttpContext.Current.Request["text"],
                                    @short = false
                                },
                                new
                                {
                                    title = "Text",
                                    value = text,
                                    @short = false
                                },
                                new
                                {
                                    title = "Response",
                                    value = resp,
                                    @short = false
                                }
                            }
                        }
                    }
                });
                */
            }

        }
        catch (Exception ex)
        {
            ret = Json.Encode(new
            {
                username = this.BotName,
                icon_emoji = ":boom:",
                text = "ERROR - " + ex.Message,
                attachments = new[]
                {
                    new
                    {
                        color = "danger",
                        fields = new []
                        {
                            new
                            {
                                title = "Response",
                                value = resp,
                                @short = "false"
                            }
                        }
                    }
                }
            });                
        }

        return ret;
    }
}