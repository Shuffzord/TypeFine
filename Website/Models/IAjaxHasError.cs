namespace Website.Models
{
    public interface IAjaxHasError : IAjaxRedirect
    {
        bool HasError { get; }
        string ErrorMessage { get;}
    }
}