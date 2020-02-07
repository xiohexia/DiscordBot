using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DiscordBot
{
    public class BotCommands
    {
        [Command("hi"), Description("Hello There...")]
        public async Task Hi(CommandContext ctx)
        {
            await ctx.RespondAsync($"Hello There... {ctx.User.Mention}");
        }

        [Command("eggplant"), Description("Eggplant Emoji")]
        public async Task Email(CommandContext ctx)
        {
            DiscordEmoji emoji = DiscordEmoji.FromName(ctx.Client, ":eggplant:");
            await ctx.RespondAsync($"{emoji}");
        }
    }
}
