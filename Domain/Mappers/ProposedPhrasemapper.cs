using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Models;
using System;

namespace Domain.Mappers
{
    public static class ProposedPhraseMapper
    {
        public static ProposedPhraseModel Convert(this ProposedPhrase phrase)
        {
            var phraseModel = new ProposedPhraseModel
                {
                    Right = phrase.Right,
                    Comment = phrase.Comment,
                    Id = phrase.Id,
                    IsOk = false
                };
            return phraseModel;
        }

        public static List<ProposedPhraseModel> Convert(this List<ProposedPhrase> phrases)
        {
            return phrases.Select(x => x.Convert()).ToList();
        }
    }

    public static class PhraseMapper
    { 
        public static PhraseModel Convert(this Phrase @this)
        {
            var phrase = new PhraseModel
                {
                    Id = @this.Id,
                    Value = @this.Value,
                    AddDate = @this.AddDate,
                    Comment = @this.Comment,
                    Interesting = @this.Interesting,
                    Source = @this.Source.ConvertToString()
                };

            return phrase;
        }

        public static WordOfTheDayModel Convert(this WordOfTheDay @this)
        {
            var wordOfTheDayModel = new WordOfTheDayModel
                {
                    PhraseModel = @this.Phrase.Convert(),
                    ActiveIn = @this.ActiveIn
                };
            return wordOfTheDayModel;
        }

        public static IEnumerable<PhraseModel> Convert(this IEnumerable<Phrase> @this)
        {
            return @this.Select(x => x.Convert());
        }
        public static IEnumerable<WordOfTheDayModel> Convert(this IEnumerable<WordOfTheDay> @this)
        {
            return @this.Select(x => x.Convert());
        }

        public static String ConvertToString(this Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }

    }

    
}