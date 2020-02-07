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

        [Command("emoji"), Description("Usage: .emoji :eggplant:")]
        public async Task Emoji(CommandContext ctx, [Description("Emoji!")] DiscordEmoji emoji)
        {
            await ctx.RespondAsync($"{ctx.User.Mention}, {emoji}");
        }

        [Command("emoji2"), Description("Usage: .emoji :eggplant:")]
        public async Task Emoji2(CommandContext ctx, [Description("Emoji!")] DiscordGuildEmoji emoji)
        {
            await ctx.RespondAsync($"{ctx.User.Mention}, {emoji}");
        }
    }
}
