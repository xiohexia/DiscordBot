using System;
using System.Collections.Generic;
using System.Linq;
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
        private SubHandler subHandler;
        private PageChecker pageChecker;
        private Config config;
        private DiscordClient discord;
        private CommandsNextModule commands;
        
        private Program()
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = app.Default.BotAuth,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });
            config = new Config();
            subHandler = new SubHandler(config);
            pageChecker = new PageChecker(discord.DebugLogger, config);
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = app.Default.Prefix,
                Dependencies = BuildDependancies()
            });
            commands.RegisterCommands<BotCommands>();
        }

        static async Task Main(string[] args)
        {
            await new Program().Run().ConfigureAwait(false);
        }

        private async Task Run()
        {
            await discord.ConnectAsync().ConfigureAwait(false);
            await CheckLoop().ConfigureAwait(false);
        }

        async Task CheckLoop()
        {
            while (true)
            {
                try
                {
                    var results = pageChecker.CheckAll();
                    await PublishToSubscribers(results).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    discord.DebugLogger.LogMessage(LogLevel.Critical, "PageChecker", e.Message, DateTime.Now);
                }
                await Task.Delay(new Random().Next(35, 85) * 1000).ConfigureAwait(false);
            }
        }

        private async Task PublishToSubscribers(Dictionary<string, bool> results)
        {
            if(results.ContainsValue(true))
            {
                foreach (var sub in subHandler.Subs)
                {
                    DiscordGuild guild = await discord.GetGuildAsync(sub.GuildId).ConfigureAwait(false);
                    DiscordMember member = await guild.GetMemberAsync(sub.UserId).ConfigureAwait(false);
                    await member.SendMessageAsync(ResultConverter(results)).ConfigureAwait(false);
                }
            }
        }

        private string ResultConverter(Dictionary<string, bool> results)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in results)
            {
                if(line.Value)
                {
                    string site = config.ProperNames[line.Key];
                    string stock = "Available";
                    string url = config.Urls[line.Key];
                    sb.Append($"**{site}**: **{stock}** -- {url}\n");
                }
                discord.DebugLogger.LogMessage(LogLevel.Info, "PageChecker", $"{line.Key}: {line.Value}", DateTime.Now);
            }
            return sb.ToString();
        }

        private DependencyCollection BuildDependancies()
        {
            using var dc = new DependencyCollectionBuilder();
            dc.AddInstance(subHandler)
              .AddInstance(pageChecker)
              .AddInstance(config);
            return dc.Build();
        }

        ~Program()
        {
            discord.Dispose();
            discord = null;
        }
    }
}
