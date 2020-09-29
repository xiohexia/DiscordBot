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
        private SubHandler subHandler;
        private PageChecker pageChecker;
        private Config config;

        public BotCommands(SubHandler subHandler, PageChecker pageChecker, Config config)
        {
            this.subHandler = subHandler;
            this.pageChecker = pageChecker;
            this.config = config;
        }
        
        //[Command("hi"), Description("Hello There...")]
        //public async Task Hi(CommandContext ctx)
        //{
        //    await ctx.RespondAsync($"Hello There... {ctx.User.Mention}");
        //}

        //[Command("emoji"), Description("Usage: .emoji :eggplant:")]
        //public async Task Emoji(CommandContext ctx, [Description("Emoji!")] DiscordEmoji emoji)
        //{
        //    await ctx.RespondAsync($"{ctx.User.Mention}, {emoji}");
        //}

        //[Command("spam"), Description("Usage: .spam @person")]
        //public async Task Spam(CommandContext ctx, [Description("The person you want to spam!")] DiscordMember discordMember)
        //{
        //    //await ctx.RespondAsync($"{discordMember.Mention}");
        //}

        [Command("check"), Description("Usage: \n.check evga \n.check amazon \n.check newegg")]
        public async Task Check(CommandContext ctx, [Description("Site you want to check.\nUse: evga, amazon, newegg")]string site)
        {
            if (config.ValidChoices.Contains(site))
            {
                bool isAvailable = pageChecker.Check(site);
                string isAvailableStr = isAvailable ? "Available" : "Out of Stock";
                string properSiteName = config.ProperNames[site];
                string url = config.Urls[site];
                await ctx.RespondAsync($"{ctx.User.Mention} **{properSiteName}** is **{isAvailableStr}** -- {url}").ConfigureAwait(false);
            }
            else await ctx.RespondAsync($"{ctx.User.Mention} \"{site}\" is not a valid choice. See .help check").ConfigureAwait(false);
        }

        //TODO: Add CheckAll BotCommand

        [Command("sub"), Description("Usage: .sub")]
        public async Task Sub(CommandContext ctx)
        {
            if (subHandler.Add(ctx)) await ctx.RespondAsync($"{ctx.Member.Mention}, You are now subscribed.").ConfigureAwait(false);
            else await ctx.RespondAsync($"{ctx.Member.Mention}, You were already subscribed.").ConfigureAwait(false);
        }

        [Command("unsub"), Description("Usage: .unsub")]
        public async Task Unsub(CommandContext ctx)
        {
            if (subHandler.Remove(ctx)) await ctx.RespondAsync($"{ctx.Member.Mention}, You are no longer subscribed.").ConfigureAwait(false);
            else await ctx.RespondAsync($"{ctx.Member.Mention}, You were not subscribed.").ConfigureAwait(false);
        }
    }
}
