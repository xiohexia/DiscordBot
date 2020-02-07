using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace DiscordBot
{
    class Program
    {
        static DiscordClient discord;
        static CommandsNextModule commands;
        static void Main(string[] args)
        {
            //https://discordapp.com/oauth2/authorize?client_id=675157130742988830&scope=bot&permissions=383040

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = "Njc1MTU3MTMwNzQyOTg4ODMw.XjzILg.gmAA66iALCV2p5_prPQSLh88HjI",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "."
            });
            commands.RegisterCommands<BotCommands>();
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
