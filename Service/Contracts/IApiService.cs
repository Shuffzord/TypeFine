using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.Mvc;
using System.Web.Script.Services;

namespace Service.Contracts
{
    [ServiceContract]
    public interface IApiService
    {
        [OperationContract]
        [WebInvoke(
         UriTemplate = "/GetPhrase",
         Method = "*",
         RequestFormat = WebMessageFormat.Json,
         ResponseFormat = WebMessageFormat.Json,
         BodyStyle = WebMessageBodyStyle.Wrapped
         )]
        GetPhrasesResponse GetPhrase(GetPhrasesRequest request);
        /*
         * JAK TO KURWA WYWOŁAć z browsera. Diablo ważne jest JSON.stringify.
         * $.ajax({
                dataType: 'json',
                method: 'POST',
                url: "http://localhost/Service/ApiService.svc/json/GetPhrase",
                contentType: 'application/json',
                data: JSON.stringify({
                  request: {
                        Keyword: "S"
                        }
                })
            }).done(function() {
                debugger;
            });

        */

        [OperationContract]
        CrosswordResponse GenerateCrossword(CrosswordRequest request);

        [OperationContract]
        void AddPhrase(AddPhraseRequest request);

        [OperationContract]
        CheckResponse Check(CheckRequest request);

        [OperationContract]
        UpdateResponse Update(UpdateRequest request);

        [OperationContract]
        void RegisterOrUpdateUserNotification(NotificationRequest request);

        [OperationContract]
        [WebInvoke(
         UriTemplate = "/Match",
         Method = "*",
         RequestFormat = WebMessageFormat.Json,
         ResponseFormat = WebMessageFormat.Json,
         BodyStyle = WebMessageBodyStyle.Wrapped
         )]
        void Match(MatchRequest request);

        [OperationContract]
        GetMessagesResponse GetMessages(GetMessagesRequest request);


        [OperationContract]
        GetPhrasesResponse GetRandomPhrase(GetRandomPhraseRequest request);

        [OperationContract]
        [WebInvoke(
          UriTemplate = "/GetLastWordOfTheDay",
          Method = "*",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Wrapped
        )]
        WordOfTheDayResponse GetLastWordOfTheDay();

        [OperationContract]
        [WebInvoke(
          UriTemplate = "/GetWordOfTheDay",
          Method = "*",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Wrapped
        )]
        WordOfTheDayResponse GetWordOfTheDay(WordOfTheDayRequest request);

        [OperationContract]
        void LogError(LogErrorRequest request);

        [OperationContract]
        void LogTime(LogTimeRequest request);

        //[OperationContract]
        //void Setup();

    }

}
