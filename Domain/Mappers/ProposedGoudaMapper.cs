using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Models;
using System;

namespace Domain.Mappers
{
    public static class ProposedGoudaMapper
    {
        public static ProposedGoudaModel Convert(this ProposedPhrase phrase)
        {
            var goudaModel = new ProposedGoudaModel
                {
                    Right = phrase.Right,
                    Comment = phrase.Comment,
                    Id = phrase.Id,
                    IsOk = false
                };
            return goudaModel;
        }

        public static List<ProposedGoudaModel> Convert(this List<ProposedPhrase> goudas)
        {
            return goudas.Select(x => x.Convert()).ToList();
        }
    }

    public static class GoudaMapper
    { 
        public static GoudaModel Convert(this Phrase @this)
        {
            var gouda = new GoudaModel
                {
                    Id = @this.Id,
                    Value = @this.Value,
                    AddDate = @this.AddDate,
                    Comment = @this.Comment,
                    Interesting = @this.Interesting,
                    Source = @this.Source.ConvertToString()
                };

            return gouda;
        }

        public static WordOfTheDayModel Convert(this WordOfTheDay @this)
        {
            var wordOfTheDayModel = new WordOfTheDayModel
                {
                    GoudaModel = @this.Phrase.Convert(),
                    ActiveIn = @this.ActiveIn
                };
            return wordOfTheDayModel;
        }

        public static IEnumerable<GoudaModel> Convert(this IEnumerable<Phrase> @this)
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