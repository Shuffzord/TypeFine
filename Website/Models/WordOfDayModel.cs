using System;
using TypeLite;

namespace Website.Models
{
    [TsClass(Module = "Models")]
    public class WordOfDayModel : AjaxResponseModel
    {
        public string Date { get; set; }
        public PhraseModel Phrase { get; set; }
    }
}