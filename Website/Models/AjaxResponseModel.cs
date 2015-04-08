using System;
using System.Web.Services.Protocols;

namespace Website.Models
{
    public class AjaxResponseModel : IAjaxResponseModel
    {
        public bool Success { get; set; }
        public bool Redirect { get; private set; }
        public string UrlToRedirect { get; private set; }
        public bool HasError { get; private set; }
        public string ErrorMessage { get; private set; }


        private void SetHasError()
        {
            HasError = true;
        }

        public void SetError(SoapException e)
        {
            SetHasError();
            ErrorMessage = "Przepraszamy. Serwis chwilowo niedostępny.";
        }

        public void SetError(Exception e)
        {
            SetHasError();
            ErrorMessage = "Zostaliśmy zaatakowani przez dzikie małpy! Inwazja wkrótce zostanie odparta.";
        }

        public void RedirectTo(string url)
        {
            Redirect = true;
            UrlToRedirect = url;
        }
    }
}