using Website.Models;

namespace Website.Extensions
{
    public static class PhraseModelExtensions
    {
        public static PhraseModel ToModel(this IPhrase phrase)
        {
            return new PhraseModel { Comment = phrase.Comment, Right = phrase.Right };
        }
    }
}