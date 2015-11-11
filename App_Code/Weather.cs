using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;

/// <summary>
/// Summary description for Weather
/// </summary>
public class Weather : SlackBotHandler
{
    public override string TriggerWord { get { return "weather"; } }
    public override string Process(string text)
    {
        if (string.IsNullOrEmpty(text))
            text = "Baltimore, MD";

        text = DoKeywordReplacements(text);

        var weatherUrl = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&units=imperial", HttpUtility.UrlEncode(text));
        var weatherIcon = "http://openweathermap.org/img/w/";
        var weather = Json.Decode(new WebClient().DownloadString(weatherUrl));

        var title = "Current conditions for " + weather.name;
        var icon_url = weatherIcon + weather.weather[0].icon + ".png";
        var positive = true;

        return Json.Encode(new
        {
            text = title,
            icon_url = icon_url,
            attachments = new[]
            {
                new
                {
                    color = positive ? "good" : "danger",
                    fields = new []
                    {
                        new
                        {
                            title = "Conditions",
                            value = string.Format("*{0}*\n{1}", weather.weather[0].main, weather.weather[0].description),
                            @short = "true"
                        },
                        new
                        {
                            title = "Current Temp",
                            value = weather.main.temp + "\u00B0F",
                            @short = "true"
                        },
                        new
                        {
                            title = "High",
                            value = weather.main.temp_max + "\u00B0F",
                            @short = "true"
                        },
                        new
                        {
                            title = "Low",
                            value = weather.main.temp_min + "\u00B0F",
                            @short = "true"
                        }
                    }
                }
            }
        });
    }

    private string DoKeywordReplacements(string location)
    {
        var testLocation = location.ToLower().Trim();
        if (testLocation == "stl" || testLocation == "eric") location = "st louis, mo";
        else if (testLocation == "bmore" || testLocation == "balt") location = "baltimore, md";
        return location;
    }
}