using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;

namespace Tools
{
    public class LevensteinHelper
    {
        public static List<string> GetRanksForString(string key, int take = 5, int skip = 0)
        {
            var outp = new HashSet<KeywordPhraseReference>();
            
            using (var ctx = new TypeFineContext())
            {
                var keyword = ctx.Keywords.SingleOrDefault(x => x.Value == key);
                if (keyword != null)
                {
                    if (keyword.Phrase != null) return new List<string> { keyword.Phrase.Value };

                    var loveStories = ctx.KeywordPhraseReferences.Include("Phrase").Where(x => x.Keyword.Value == key);

                    if (!loveStories.Any())
                    {
                        var phrases = ctx.Phrases.ToList();
                        var lastRank = 0d;
                        foreach (var phrase in phrases)
                        {
                            var rank = StringMetrics.Levenstein(key, phrase.Value).Value;

                            if (outp.Count() < take)
                            {
                                outp.Add(new KeywordPhraseReference
                                    {
                                        Convergence = rank,
                                        UserWeight = 0,
                                        Keyword = keyword,
                                        Phrase = phrase
                                    });
                            }
                            else
                            {
                                if (!(rank > lastRank))
                                    continue;
                                outp.Add(new KeywordPhraseReference
                                    {
                                        Convergence = rank,
                                        UserWeight = 0,
                                        Keyword = keyword,
                                        Phrase = phrase
                                    });
                            }
                            var sort2 = outp.OrderByDescending(x => x.Convergence).Skip(skip).Take(take);
                            outp = new HashSet<KeywordPhraseReference>(sort2);
                            lastRank = outp.Last().Convergence;
                        }
                        ctx.KeywordPhraseReferences.AddRange(outp);
                        ctx.SaveChanges();
                        return outp.Select(x => x.Phrase.Value).ToList();
                    }
                    var result = loveStories.ToList().OrderBy(x => x.CompositeConvergance);
                    return result.Select(x => x.Phrase.Value).ToList();
                }
                throw new Exception("Missing keyword");
            }
        }
    }
}
