using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core.Client;

namespace Uchu.World
{
    public class LootContainerComponent : Component
    {
        private LootContainerComponent()
        {
            Random = new Random();

            Entries = new List<LootMatrix>();

            Currencies = new List<CurrencyMatrix>();
        }

        private Random Random { get; }

        private List<LootMatrix> Entries { get; }

        private List<CurrencyMatrix> Currencies { get; }

        private int Level { get; set; }

        private int LootIndex { get; set; }

        private int CurrencyIndex { get; set; }

        private bool Searched { get; set; }

        public async Task CollectDetailsAsync()
        {
            if (Searched) return;

            await FindMatrixIndicesAsync();

            await QueryLootMatrixAsync();

            await QueryCurrencyMatrixAsync();

            Searched = true;
        }

        public IEnumerable<Lot> GenerateLootYields()
        {
            foreach (var matrix in Entries)
            {
                var count = Random.Next(matrix.Minimum, matrix.Maximum);

                var entries = matrix.Entries.ToList();
                
                for (var i = 0; i < count; i++)
                {
                    if (entries.Count == default) break;
                    
                    var entry = entries[Random.Next(default, entries.Count)];
                    
                    entries.Remove(entry);

                    var result = Random.NextDouble();

                    if (result <= matrix.Percentage)
                    {
                        yield return entry.Lot;
                    }
                }
            }
        }

        public int GenerateCurrencyYields()
        {
            var yield = 0;

            foreach (var currency in Currencies)
            {
                if (currency.Level > Level) continue;

                var result = Random.Next(currency.Minimum, currency.Maximum);

                yield += result;
            }

            return yield;
        }

        private async Task QueryLootMatrixAsync()
        {
            await using var ctx = new CdClientContext();

            var matrices = await ctx.LootMatrixTable.Where(
                m => m.LootMatrixIndex == LootIndex
            ).ToArrayAsync();

            foreach (var matrix in matrices)
            {
                var tables = await ctx.LootTableTable.Where(
                    t => t.LootTableIndex == matrix.LootTableIndex
                ).ToArrayAsync();

                var items = tables.Select(t => new LootMatrixEntry
                {
                    Lot = t.Itemid ?? 0,
                    Priority = t.SortPriority ?? 0,
                    Mission = t.MissionDrop ?? false
                }).ToList();

                items.Sort((a, b) => a.Priority - b.Priority);

                var entry = new LootMatrix
                {
                    Minimum = matrix.MinToDrop ?? 1,
                    Maximum = matrix.MaxToDrop ?? 1,
                    Percentage = matrix.Percent ?? 0,
                    Entries = items.ToArray()
                };

                Entries.Add(entry);
            }
        }

        private async Task QueryCurrencyMatrixAsync()
        {
            await using var ctx = new CdClientContext();

            var matrices = await ctx.CurrencyTableTable.Where(
                c => c.CurrencyIndex == CurrencyIndex
            ).ToArrayAsync();

            foreach (var matrix in matrices)
            {
                var entry = new CurrencyMatrix
                {
                    Level = matrix.Npcminlevel ?? 0,
                    Minimum = matrix.Minvalue ?? 0,
                    Maximum = matrix.Maxvalue ?? 0
                };

                Currencies.Add(entry);
            }
        }

        private async Task FindMatrixIndicesAsync()
        {
            await using var ctx = new CdClientContext();

            var destructible = GameObject.Lot.GetComponentId(ComponentId.DestructibleComponent);

            if (destructible != default)
            {
                var component = await ctx.DestructibleComponentTable.FirstOrDefaultAsync(
                    c => c.Id == destructible
                );

                LootIndex = component.LootMatrixIndex ?? 0;

                CurrencyIndex = component.CurrencyIndex ?? 0;

                Level = component.Level ?? 0;

                return;
            }

            var package = GameObject.Lot.GetComponentId(ComponentId.PackageComponent);

            if (package != default)
            {
                var component = await ctx.PackageComponentTable.FirstOrDefaultAsync(
                    c => c.Id == package
                );

                LootIndex = component.LootMatrixIndex ?? 0;

                CurrencyIndex = default;

                return;
            }

            LootIndex = default;

            CurrencyIndex = default;
        }
    }
}