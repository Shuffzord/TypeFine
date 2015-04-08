using TypeLite;

namespace Website.Models
{
    [TsClass(Module = "Models")]
    public class PhraseModel
    {
        public string Comment { get; set; }
        public string Right { get; set; }
    }
}