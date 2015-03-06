<%@ WebHandler Language="C#" Class="SlackWeatherBot" %>

using System;
using System.Net;
using System.Web;
using System.Web.Helpers;

public class SlackWeatherBot : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        var location = "Baltimore, MD";
        if (context.Request["text"] != context.Request["trigger_word"])
            location = context.Request["text"].Substring(context.Request["trigger_word"].Length + 1);
        var weatherUrl = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&units=imperial", HttpUtility.UrlEncode(location));
        var weatherIcon = "http://openweathermap.org/img/w/";
        var weather = Json.Decode(new WebClient().DownloadString(weatherUrl));

        var text = "Current conditions for " + weather.name;
        var icon_url = weatherIcon + weather.weather[0].icon + ".png";
        var positive = true;

        context.Response.Write(Json.Encode(new
        {
            text = text,
            icon_url = icon_url,
            attachments = new[]
            {
                new
                {
                    color = positive ? "good" : "danger",
                    fields = new[]
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
                            value = weather.main.temp + "&deg;F",
                            @short = "true"
                        },
                        new
                        {
                            title = "High",
                            value = weather.main.temp_max + "&deg:F",
                            @short = "true"
                        },
                        new
                        {
                            title = "Low",
                            value = weather.main.temp_min + "&deg:F",
                            @short = "true"
                        }
                    }
                }
            }
        }));
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}