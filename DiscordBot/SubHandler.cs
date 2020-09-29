using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using CsvHelper;
using CsvHelper.Configuration;
using DSharpPlus.CommandsNext;

namespace DiscordBot
{
    public class SubHandler
    {
        private readonly Config config;
        private List<Sub> subs;
        private CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = false,
            HeaderValidated = null
        };

        public SubHandler(Config config)
        {
            this.config = config;
            subs = SyncFromFile();
        }

        public List<Sub> Subs => subs;

        public bool Add(CommandContext ctx)
        {
            Sub sub = new Sub(ctx.User.Id, ctx.Guild.Id);
            if (!subs.Contains(sub))
            {
                subs.Add(sub);
                AppendSubToFile(sub);
                return true;
            }
            else return false;
        }

        public bool Remove(CommandContext ctx)
        {
            Sub sub = new Sub(ctx.User.Id, ctx.Guild.Id);
            if (subs.Contains(sub))
            {
                subs.Remove(sub);
                OverwriteSubsFile(subs);
                return true;
            }
            else return false;
        }

        private List<Sub> SyncFromFile()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(config.SubscriptionsFullPath));
            using (var fileStream = new FileStream(config.SubscriptionsFullPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    using (var csv = new CsvReader(streamReader, csvConfig))
                    {
                        return csv.GetRecords<Sub>().ToList();
                    }
                }
            }
        }

        private void AppendSubToFile(Sub sub)
        {
            FileWriter(FileMode.Append, new List<Sub>() { sub });
        }

        private void OverwriteSubsFile(List<Sub> subs)
        {
            FileWriter(FileMode.Truncate, subs);
        }

        private void FileWriter(FileMode fileMode, List<Sub> subs)
        {
            using (var fileStream = new FileStream(config.SubscriptionsFullPath, fileMode, FileAccess.Write, FileShare.None))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    using (var csv = new CsvWriter(streamWriter, csvConfig))
                    {
                        csv.WriteRecords(subs);
                    }
                }
            }
        }


    }
}
