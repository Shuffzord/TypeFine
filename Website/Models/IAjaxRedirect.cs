namespace Website.Models
{
    public interface IAjaxRedirect
    {
        bool Redirect { get; }
        string UrlToRedirect { get; }
    }
}