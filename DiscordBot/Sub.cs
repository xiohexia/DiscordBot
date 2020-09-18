using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public class Sub : IEquatable<Sub>
    {
        public Sub(ulong userId, ulong guildId)
        {
            UserId = userId;
            GuildId = guildId;
        }
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Sub sub = (Sub)obj;
            return sub.UserId == UserId && sub.GuildId == GuildId;
        }

        public bool Equals(Sub sub)
        {
            if (sub == null) return false;
            return sub.UserId == UserId && sub.GuildId == GuildId;
        }
    }
}
