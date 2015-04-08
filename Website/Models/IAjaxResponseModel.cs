namespace Website.Models
{
    public interface IAjaxResponseModel : IAjaxHasError
    {
        bool Success { get; set; }
    }
}