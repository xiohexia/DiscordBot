using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using DSharpPlus.CommandsNext;

namespace DiscordBot
{
    static public class SubHandler
    {
        static string fileName = "subs.csv";

        static public bool Add(CommandContext ctx)
        {
            Sub sub = new Sub(ctx.User.Id, ctx.Guild.Id);
            List<Sub> subs = GetSubs();
            if (!subs.Contains(sub))
            {
                SaveSub(sub);
                return true;
            }
            else return false;
        }

        static public bool Remove(CommandContext ctx)
        {
            Sub sub = new Sub(ctx.User.Id, ctx.Guild.Id);
            List<Sub> subs = GetSubs();
            if (subs.Contains(sub))
            {
                subs.Remove(sub);
                OverwriteSubs(subs);
                return true;
            }
            else return false;
        }

        static public List<Sub> GetSubs()
        {
            CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = false,
                HeaderValidated = null
            };
            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                return csv.GetRecords<Sub>().ToList();
            }
        }

        static private void SaveSub(Sub sub)
        {
            CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                //HasHeaderRecord = !File.Exists(fileName)
                HasHeaderRecord = false,
                HeaderValidated = null
            };
            using (FileStream fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    using (var csv = new CsvWriter(streamWriter, csvConfig))
                    {
                        csv.WriteRecords(new List<Sub>() { sub });
                    }
                }
            }
        }

        static private void OverwriteSubs(List<Sub> subs)
        {
            CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                //HasHeaderRecord = true,
                HasHeaderRecord = false,
                HeaderValidated = null
            };
            using (var writer = new StreamWriter(fileName))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                csv.WriteRecords(subs);
            }
        }
    }
}
