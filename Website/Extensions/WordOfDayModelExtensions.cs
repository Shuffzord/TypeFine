using Website.Models;
using Website.PhraseService;

namespace Website.Extensions
{
    public static class WordOfDayModelExtensions
    {
        public static WordOfDayModel ToModel(this WordOfTheDayResponse @this)
        {
            var model = new WordOfDayModel();
            model.Date = @this.Date.ToShortDateString();
            model.Phrase = @this.Phrase.ToTransient().ToModel();

            return model;
        }
    }
}