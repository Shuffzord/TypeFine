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
            : base("TypeFineDB")
        {
        }

        public DbSet<Phrase> Phrases { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<ProposedPhrase> ProposedPhrases { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<NotificationUser> NotificationUsers { get; set; }
        public DbSet<NotificationHistory> NotificationHistory { get; set; }
        public DbSet<KeywordPhraseReference> KeywordPhraseReferences { get; set; }
        public DbSet<DeveloperMessage> DeveloperMessages { get; set; }
        public DbSet<WordOfTheDay> WordsOfTheDay { get; set; }
        public DbSet<RequestInfo> RequestInfos { get; set; }

        public IEnumerable<Phrase> GetPhrase(string keyword)
        {
            throw new NotImplementedException();
        }

        private struct PhraseIdWithRankModel
        {
            public long PhraseId;
            public double Rank;
        }

        private class PhraseWithRankModelComparer : IComparer<PhraseIdWithRankModel>
        {
            public int Compare(PhraseIdWithRankModel x, PhraseIdWithRankModel y)
            {
                return y.Rank.CompareTo(x.Rank);
            }
        }

        private IEnumerable<PhraseIdWithRankModel> GetTopTen(string keyword)
        {
            var outp = new List<PhraseIdWithRankModel>(10);
            var i = 0;
            foreach (var phrase in Phrases.Select(x => new { x.Id, x.Value }))
            {
                var rank = StringMetrics.Levenstein(keyword, phrase.Value).Value;
                if (i < 10)
                {
                    outp.Add(new PhraseIdWithRankModel
                    {
                        PhraseId = phrase.Id,
                        Rank = rank
                    });

                    i++;
                    continue;
                }
                if (i == 10)
                {
                    outp.Sort(new PhraseWithRankModelComparer());
                }
                if (!(rank > outp[9].Rank))
                    continue;

                outp.RemoveAt(9);
                outp.Add(new PhraseIdWithRankModel
                {
                    PhraseId = phrase.Id,
                    Rank = rank
                });
                outp.Sort(new PhraseWithRankModelComparer());
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
                    var phrase = Phrases.Single(x => x.Id == top.PhraseId);
                    var reference = new KeywordPhraseReference
                    {
                        Convergence = top.Rank,
                        Phrase = phrase,
                        Keyword = keywordEntity
                    };
                    KeywordPhraseReferences.Add(reference);
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
            ICollection<KeywordPhraseReference> weakReferences, IEnumerable<PhraseIdWithRankModel> tops)
        {
            var sortedWeakReferences = new List<KeywordPhraseReference>(weakReferences);
            foreach (var top in tops)
            {
                if (sortedWeakReferences.Count < 10)
                {
                    sortedWeakReferences.Add(new KeywordPhraseReference
                    {
                        Convergence = top.Rank,
                        Phrase = Phrases.Single(x => x.Id == top.PhraseId)
                    });
                    continue;
                }
                sortedWeakReferences.Sort(new WeakReferenceComparer());
                if (!(top.Rank > sortedWeakReferences[9].CompositeConvergance))
                    continue;

                if (sortedWeakReferences.Any(x => x.Phrase.Value.Equals(Phrases.Single(g => g.Id == top.PhraseId).Value)))
                    continue;

                var referenceToRemove = sortedWeakReferences.ElementAt(9);
                KeywordPhraseReferences.Remove(referenceToRemove);
                sortedWeakReferences.RemoveAt(9);

                var referenceToAdd = new KeywordPhraseReference
                    {
                        Convergence = top.Rank,
                        Phrase = Phrases.Single(x => x.Id == top.PhraseId)
                    };
                KeywordPhraseReferences.Add(referenceToAdd);
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