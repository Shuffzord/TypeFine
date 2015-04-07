using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace Domain.Entities
{
    public class TypeFineContext : DbContext
    {
        public TypeFineContext()
            : base("GoudaDb")
        {
        }

        public DbSet<Phrase> Goudas { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<ProposedPhrase> ProposedGoudas { get; set; }
        public DbSet<User> GoudaUsers { get; set; }
        public DbSet<Notifications> GoudaNotifications { get; set; }
        public DbSet<NotificationUser> GoudaNotificationUsers { get; set; }
        public DbSet<NotificationHistory> NotificationHistory { get; set; }
        public DbSet<KeywordPhraseReference> KeywordGoudaWeakReferences { get; set; }
        public DbSet<DeveloperMessage> GoudaMessages { get; set; }
        public DbSet<WordOfTheDay> WordsOfTheDay { get; set; }
        public DbSet<RequestInfo> RequestInfos { get; set; }

        public IEnumerable<Phrase> GetGouda(string keyword)
        {
            throw new NotImplementedException();
        }

        private struct GoudaIdWithRankModel
        {
            public long GoudaId;
            public double Rank;
        }

        private class GoudaWithRankModelComparer : IComparer<GoudaIdWithRankModel>
        {
            public int Compare(GoudaIdWithRankModel x, GoudaIdWithRankModel y)
            {
                return y.Rank.CompareTo(x.Rank);
            }
        }

        private IEnumerable<GoudaIdWithRankModel> GetTopTen(string keyword)
        {
            var outp = new List<GoudaIdWithRankModel>(10);
            var i = 0;
            foreach (var gouda in Goudas.Select(x => new { x.Id, x.Value }))
            {
                var rank = StringMetrics.Levenstein(keyword, gouda.Value).Value;
                if (i < 10)
                {
                    outp.Add(new GoudaIdWithRankModel
                    {
                        GoudaId = gouda.Id,
                        Rank = rank
                    });

                    i++;
                    continue;
                }
                if (i == 10)
                {
                    outp.Sort(new GoudaWithRankModelComparer());
                }
                if (!(rank > outp[9].Rank))
                    continue;

                outp.RemoveAt(9);
                outp.Add(new GoudaIdWithRankModel
                {
                    GoudaId = gouda.Id,
                    Rank = rank
                });
                outp.Sort(new GoudaWithRankModelComparer());
            }
            return outp;
        }

        public IEnumerable<Phrase> TheAlgorithm(string keyword)
        {
            var keywordEntity = Keywords.Include(x => x.WeakReferences.Select(r => r.Phrase)).SingleOrDefault(x => x.Value == keyword) ?? new Keyword { Value = keyword };
            keywordEntity.Count++;
            if (keywordEntity.Phrase != null)
            {
                SaveChanges();
                return new[] { keywordEntity.Phrase };
            }
            if (keywordEntity.WeakReferences == null)
                keywordEntity.WeakReferences = new Collection<KeywordPhraseReference>();
            var weakReferences = keywordEntity.WeakReferences;

            if (!weakReferences.Any())
            {
                var outp = new List<Phrase>();
                foreach (var top in GetTopTen(keyword))
                {
                    var gouda = Goudas.Single(x => x.Id == top.GoudaId);
                    var reference = new KeywordPhraseReference
                    {
                        Convergence = top.Rank,
                        Phrase = gouda,
                        Keyword = keywordEntity
                    };
                    KeywordGoudaWeakReferences.Add(reference);
                    outp.Add(reference.Phrase);
                }
                SaveChanges();
                if (outp.Count != 10)
                    throw new Exception("Powinno być dokładnie 10 luźnych referencji!");
                return outp;
            }

            var mergedReferences = MergeWeakReferences(weakReferences, GetTopTen(keyword));
            SaveChanges();
            return mergedReferences.Select(x => x.Phrase);
        }

        private class WeakReferenceComparer : IComparer<KeywordPhraseReference>
        {
            public int Compare(KeywordPhraseReference x, KeywordPhraseReference y)
            {
                return y.CompositeConvergance.CompareTo(x.CompositeConvergance);
            }
        }
        private IEnumerable<KeywordPhraseReference> MergeWeakReferences(
            ICollection<KeywordPhraseReference> weakReferences, IEnumerable<GoudaIdWithRankModel> tops)
        {
            var sortedWeakReferences = new List<KeywordPhraseReference>(weakReferences);
            foreach (var top in tops)
            {
                if (sortedWeakReferences.Count < 10)
                {
                    sortedWeakReferences.Add(new KeywordPhraseReference
                    {
                        Convergence = top.Rank,
                        Phrase = Goudas.Single(x => x.Id == top.GoudaId)
                    });
                    continue;
                }
                sortedWeakReferences.Sort(new WeakReferenceComparer());
                if (!(top.Rank > sortedWeakReferences[9].CompositeConvergance))
                    continue;

                if (sortedWeakReferences.Any(x => x.Phrase.Value.Equals(Goudas.Single(g => g.Id == top.GoudaId).Value)))
                    continue;

                var referenceToRemove = sortedWeakReferences.ElementAt(9);
                KeywordGoudaWeakReferences.Remove(referenceToRemove);
                sortedWeakReferences.RemoveAt(9);

                var referenceToAdd = new KeywordPhraseReference
                    {
                        Convergence = top.Rank,
                        Phrase = Goudas.Single(x => x.Id == top.GoudaId)
                    };
                KeywordGoudaWeakReferences.Add(referenceToAdd);
                sortedWeakReferences.Add(referenceToAdd);
                sortedWeakReferences.Sort(new WeakReferenceComparer());
            }
            weakReferences = sortedWeakReferences;
            if (weakReferences.Count != 10)
                throw new WeakReferenceCountException();
            return weakReferences;
        }
    }
}