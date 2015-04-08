using System.Collections.Generic;
using TypeLite;

namespace Website.Models
{
    [TsClass(Module = "Models")]
    public class SearchResponseModel : AjaxResponseModel
    {
        public List<PhraseModel> Phrases { get; set; }
        public string Keyword { get; set; }

        public SearchResponseModel(string keyword)
        {
            Keyword = keyword;
            Phrases = new List<PhraseModel>();
        }
    }
}