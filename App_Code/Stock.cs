using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;

/// <summary>
/// Summary description for Stock
/// </summary>
public class Stock : SlackBotHandler
{
    public override string TriggerWord { get { return "stock"; } }
    public override string Process(string text)
    {
        if (text == ":btc:")
        {
            var url = "https://btc-e.com/api/3/ticker/btc_usd";
            var quote = Json.Decode(new WebClient().DownloadString(url));
            return "Bitcon last traded on BTC-e for $" + quote.btc_usd.last;
        }
        else
        {
            var url = "http://dev.markitondemand.com/Api/v2/Quote/json";
            var wc = new WebClient();
            var quote = Json.Decode(wc.DownloadString(url + "/?symbol=" + text));

            bool positive = quote.ChangePercent >= 0;

            if (quote.LastPrice == null) quote.LastPrice = 0d;
            if (quote.MarketCap == null) quote.MarketCap = 0d;
            if (quote.ChangePercent == null) quote.ChangePercent = 0d;
            if (quote.ChangePercentYTD == null) quote.ChangePercentYTD = 0d;
            if (quote.High == null) quote.High = 0d;
            if (quote.Low == null) quote.Low = 0d;

            dynamic resp = new
            {
                text = "Information for " + quote.Name + " (" + quote.Symbol + ")",
                attachments = new[]
            {
                new
                {
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
                            title = "Market Cap",
                            value = (string)Math.Round((decimal)quote.MarketCap, 2).ToString("C"),
                            @short = "true"
                        },
                        new
                        {
                            title = "Change %",
                            value = (string)Math.Round((decimal)quote.ChangePercent, 2).ToString(),
                            @short = "true"
                        },
                        new
                        {
                            title = "Change % YTD",
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
            return respString;
        }
    }
}