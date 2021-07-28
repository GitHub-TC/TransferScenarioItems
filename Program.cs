using CommandLine;
using Fastenshtein;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TransferScenarioItems
{
    class Program
    {
        static void Main(string[] args) => Parser.Default.ParseArguments<Options>(args).WithParsed(TransferIcons);

        private static void TransferIcons(Options options)
        {
            var nameIdMapping = JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllText(options.NameIdMappingFile));

            var foundIds = new List<int>();

            var matcher = nameIdMapping.Keys.Select(key => new Tuple<string, Levenshtein>(key, new Fastenshtein.Levenshtein(key))).ToList();

            Directory.GetFiles(options.ScenarioIconsFolder)
                .ToList()
                .ForEach(icon =>
                {
                    if (nameIdMapping.TryGetValue(Path.GetFileNameWithoutExtension(icon), out var id)) { CopyToIdItem(foundIds, options, icon, id); return; }

                    Console.WriteLine($"not found: {Path.GetFileName(icon)}");
                });

            if (options.EAHIconsFolder != null) Directory.GetFiles(options.EAHIconsFolder)
                 .ToList()
                 .ForEach(icon =>
                 {
                     if (nameIdMapping.TryGetValue(Path.GetFileNameWithoutExtension(icon), out var id)) { CopyToIdItem(foundIds, options, icon, id); return; }

                     Console.WriteLine($"not found: {Path.GetFileName(icon)}");
                 });


            Directory.GetFiles(options.ScenarioIconsFolder)
                .ToList()
                .ForEach(icon =>
                {
                    var pureName = Path.GetFileNameWithoutExtension(icon);
                    var nearestMatch = matcher.Aggregate(new Tuple<int, string>(int.MaxValue, string.Empty), (r, m) =>
                    {
                        int testDiff = m.Item2.DistanceFrom(pureName);
                        return testDiff < r.Item1 ? new Tuple<int, string>(testDiff, m.Item1) : r;
                    });
                    if (nameIdMapping.TryGetValue(nearestMatch.Item2, out var id)) { CopyToIdItem(foundIds, options, icon, id); return; }

                    Console.WriteLine($"not found: {Path.GetFileName(icon)}");
                });

            if (options.EAHIconsFolder != null)
            {
                var files = Directory.GetFiles(options.EAHIconsFolder).ToDictionary(k => k, k => k);

                foreach (var item in matcher)
                {
                    var nearestMatch = files.Aggregate(new Tuple<int, string>(int.MaxValue, string.Empty), (r, m) =>
                    {
                        int testDiff = item.Item2.DistanceFrom(Path.GetFileNameWithoutExtension(m.Key));
                        return testDiff < r.Item1 ? new Tuple<int, string>(testDiff, m.Key) : r;
                    });
                    if (nameIdMapping.TryGetValue(item.Item1, out var id)) CopyToIdItem(foundIds, options, nearestMatch.Item2, id); 
                    else                                                   Console.WriteLine($"not found: {item.Item1}");
                }
            }

        }

        private static void CopyToIdItem(List<int> foundIds, Options options, string icon, int id)
        {
            if (foundIds.Contains(id)) return;

            try { File.Copy(icon, Path.Combine(options.TargetFolder, id.ToString() + Path.GetExtension(icon)), true); }
            catch (Exception error) { Console.WriteLine($"{icon}: {error}"); }
        }
    }
}
