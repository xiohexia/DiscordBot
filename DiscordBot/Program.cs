using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace DiscordBot
{
    class Program
    {
        static public List<string> ValidChoices = new List<string>()
        {
            "evga",
            "amazon",
            "newegg"
        };
        static public Dictionary<string, string> ProperNames = new Dictionary<string, string>()
        {
            { "evga", "EVGA" },
            { "amazon", "Amazon" },
            { "newegg", "Newegg" },
            { "test", "Test" }
        };
        static public Dictionary<string, string> Urls = new Dictionary<string, string>()
        {
            { "evga", "https://www.evga.com/products/product.aspx?pn=10G-P5-3897-KR" },
            { "amazon", "https://www.amazon.com/EVGA-10G-P5-3897-KR-GeForce-Technology-Backplate/dp/B08HR3Y5GQ" },
            { "newegg", "https://www.newegg.com/evga-geforce-rtx-3080-10g-p5-3897-kr/p/N82E16814487518" },
            { "test", "https://google.com" }
        };

        static public DiscordClient discord;
        static CommandsNextModule commands;
        static async Task Main(string[] args)
        {
            await MainAsync(args).ConfigureAwait(false);
            await CheckLoop().ConfigureAwait(false);
        }

        static async Task CheckLoop()
        {
            int delay = 1 * 60 * 1000;
            while (true)
            {
                try
                {
                    var results = PageChecker.CheckAll();
                    await PublishToSubscribers(results).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    discord.DebugLogger.LogMessage(LogLevel.Critical, "PageChecker", e.Message, DateTime.Now);
                }
                await Task.Delay(delay).ConfigureAwait(false);
            }
        }

        static async Task MainAsync(string[] args)
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = app.Default.BotAuth,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = app.Default.Prefix
            });
            commands.RegisterCommands<BotCommands>();
            await discord.ConnectAsync().ConfigureAwait(false);
            //await Task.Delay(-1).ConfigureAwait(false);
        }

        static private async Task PublishToSubscribers(Dictionary<string, bool> results)
        {
            if(results.ContainsValue(true))
            {
                List<Sub> subs = SubHandler.GetSubs();
                foreach (var sub in subs)
                {
                    DiscordGuild guild = await discord.GetGuildAsync(sub.GuildId).ConfigureAwait(false);
                    DiscordMember member = await guild.GetMemberAsync(sub.UserId).ConfigureAwait(false);
                    await member.SendMessageAsync(ResultConverter(results)).ConfigureAwait(false);
                }
            }
        }

        static private string ResultConverter(Dictionary<string, bool> results)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in results)
            {
                if(line.Value)
                {
                    string site = ProperNames[line.Key];
                    string stock = "Available";
                    string url = Urls[line.Key];
                    sb.Append($"**{site}**: **{stock}** -- {url}\n");
                }
                discord.DebugLogger.LogMessage(LogLevel.Info, "PageChecker", $"{line.Key}: {line.Value}", DateTime.Now);
            }
            return sb.ToString();
        }
    }
}
