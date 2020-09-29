using System;
using System.Collections.Generic;

namespace DiscordBot
{
    public class Config
    {
        public string SubscriptionsFullPath { get; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/FullRetardDiscordBot/subs.csv";
        public List<string> ValidChoices { get; } = new List<string>()
        {
            "evga",
            "amazon",
            "newegg"
        };
        public Dictionary<string, string> ProperNames { get; } = new Dictionary<string, string>()
        {
            { "evga", "EVGA" },
            { "amazon", "Amazon" },
            { "newegg", "Newegg" },
            { "test", "Test" }
        };
        public Dictionary<string, string> Urls { get; } = new Dictionary<string, string>()
        {
            { "evga", "https://www.evga.com/products/product.aspx?pn=10G-P5-3897-KR" },
            { "amazon", "https://www.amazon.com/EVGA-10G-P5-3897-KR-GeForce-Technology-Backplate/dp/B08HR3Y5GQ" },
            { "newegg", "https://www.newegg.com/evga-geforce-rtx-3080-10g-p5-3897-kr/p/N82E16814487518" },
            { "test", "https://google.com" }
        };
    }
}
