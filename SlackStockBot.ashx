<%@ WebHandler Language="C#" Class="SlackStockBot" %>

using System;
using System.Dynamic;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Collections.Generic;

public class SlackStockBot : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        var stock = context.Request["text"].Substring(context.Request["trigger_word"].Length + 1);
        var url = "http://dev.markitondemand.com/Api/v2/Quote/json";
        var wc = new WebClient();
        wc.Headers["Content-type"] = "application/x-www-form-urlencoded";
        var quote = Json.Decode(wc.UploadString(url, "symbol=" + stock));

        bool positive = quote.ChangePercent >= 0;

        dynamic resp = new
        {
            attachments = new[]
            {
                new
                {
                    text = "Summary for " + quote.Name + " (" + quote.Symbol + ")",
                    color = positive ? "good" : "danger",
                    fields = new[]
                    {
                        new
                        {
                            title = "Price",
                            value = (string)Math.Round((decimal)quote.LastPrice, 2).ToString(),
                            @short = "true"
                        },
                        new
                        {
                            title = "Change",
                            value = (string)Math.Round((decimal)quote.ChangePercent, 2).ToString(),
                            @short = "true"
                        }
                    }
                },
                new {
                    text = "Details for " + quote.Name + " (" + quote.Symbol + ")",
                    color = "",
                    fields = new[]
                    {
                        new
                        {
                            title = "Market Cap",
                            value = (string)Math.Round((decimal)quote.MarketCap, 2).ToString(),
                            @short = "true"
                        },
                        new
                        {
                            title = "Change YTD",
                            value = (string)Math.Round((decimal)quote.ChangePercentYTD, 2).ToString(),
                            @short = "true"
                        },
                        new
                        {
                            title = "High",
                            value = (string)quote.High.ToString(),
                            @short = "true"
                        },
                        new
                        {
                            title = "Low",
                            value = (string)quote.Low.ToString(),
                            @short = "true"
                        }
                    }
                }
            }
        };

        var respString = Json.Encode(resp);
        context.Response.Write(respString);
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }

}