using System;
using System.Collections.Generic;
using System.Linq;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;

namespace api.Models.Orcid
{
    public class OrcidImportStatistics
    {
        private readonly List<List<object>> _items = new();

        public int Items => _items.Count + Failed;
        public int Unchanged { get; private set; }
        public int Failed { get; private set; }
        public int Inserts { get; private set; }
        public int Updates { get; private set; }
        public int Deletes { get; private set; }

        internal void TrackItem(FactFieldValue ffv)
        {
            var entities = new List<object> { ffv };
            Add(entities, ffv.DimName, ffv.DimWebLink, ffv.DimResearcherDescription,
                ffv.DimEmailAddrress, ffv.DimKeyword, ffv.DimPid, ffv.DimPidIdOrcidPutCodeNavigation,
                ffv.DimEducation, ffv.DimAffiliation, ffv.DimProfileOnlyPublication,
                ffv.DimProfileOnlyDataset, ffv.DimProfileOnlyFundingDecision,
                ffv.DimProfileOnlyResearchActivity, ffv.DimIdentifierlessData);
            if (ffv.DimIdentifierlessData != null)
                entities.AddRange(ffv.DimIdentifierlessData.InverseDimIdentifierlessData);
            if (ffv.DimProfileOnlyDataset != null)
                entities.AddRange(ffv.DimProfileOnlyDataset.DimWebLinks);
            if (ffv.DimProfileOnlyFundingDecision != null)
                entities.AddRange(ffv.DimProfileOnlyFundingDecision.DimWebLinks);
            if (ffv.DimProfileOnlyResearchActivity != null)
                entities.AddRange(ffv.DimProfileOnlyResearchActivity.DimWebLinks);
            _items.Add(entities);
        }

        internal void TrackFailedItem() => Failed++;

        private static void Add(List<object> entities, params object[] values)
        {
            entities.AddRange(values.Where(value => value != null));
        }

        public void CaptureChanges(TtvContext context, bool countItems = true)
        {
            context.ChangeTracker.DetectChanges();
            var entries = context.ChangeTracker.Entries().ToList();
            Inserts += entries.Count(entry => entry.State == EntityState.Added);
            Updates += entries.Count(entry => entry.State == EntityState.Modified);
            Deletes += entries.Count(entry => entry.State == EntityState.Deleted);

            if (countItems)
            {
                var changed = new HashSet<object>(
                    entries.Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                        .Select(entry => entry.Entity),
                    ReferenceEqualityComparer.Instance);
                Unchanged = _items.Count(item => item.All(entity => !changed.Contains(entity)));
            }
        }

        public string ToMessage(TimeSpan elapsed) =>
            FormattableString.Invariant($"seconds={elapsed.TotalSeconds:F0} items={Items} unchanged={Unchanged} failed={Failed} db_insert={Inserts} db_update={Updates} db_delete={Deletes}");
    }
}
