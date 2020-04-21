using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
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

        [Command("emoji"), Description("Usage: .emoji :eggplant:")]
        public async Task Emoji(CommandContext ctx, [Description("Emoji!")] DiscordEmoji emoji)
        {
            await ctx.RespondAsync($"{ctx.User.Mention}, {emoji}");
        }

        [Command("spam"), Description("Usage: .spam @User")]
        public async Task Spam(CommandContext ctx, [Description("The person you want to spam!")] DiscordMember discordMember)
        {
            await discordMember.SendMessageAsync("Testing!!!");
            //await ctx.RespondAsync($"{discordMember.Mention}");
        }
    }
}
