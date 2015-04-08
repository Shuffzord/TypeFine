using System.Web.Mvc;
using Website.Attributes;

namespace Website.Models
{
    [ModelBinder(typeof(AliasModelBinder))]
    public class SearchModel
    {
        [BindAlias("Q")]
        public string Query { get; set; }
    }
}