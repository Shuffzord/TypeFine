using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Website.Extensions;
using Website.Models;
using Website.PhraseService;

namespace Website.Services
{
    public interface IPhraseService
    {
        List<PhraseModel> GetPhrases(string value, string deviceId = null, string accountId = null, bool skip = false);
        List<PhraseModel> GetPhrases(GetPhrasesRequest request);
        void Match(string keyword, string phrase, string deviceId = null, string accountId = null);
        WordOfDayModel GetWordOfDay();
    }

    public class PhraseService : IPhraseService
    {
        public List<PhraseModel> GetPhrases(string value, string deviceId = null, string accountId = null, bool skip = false)
        {
            return GetPhrases(new GetPhrasesRequest(){Keyword = value, Skip = skip, AccountId = accountId, DeviceId = deviceId});
        }

        public List<PhraseModel> GetPhrases(GetPhrasesRequest request)
        {
            using (var client = new ApiServiceClient())
            {
                var response = client.GetPhrase(request);
                return response.Phrases == null ? new List<PhraseModel>() : response.Phrases.ToList().Select(x => x.PreparePhrase()).Select(x=>x.ToModel()).ToList();
            }
        }

        public void Match(string keyword, string phrase, string deviceId = null, string accountId = null)
        {
            using (var client = new ApiServiceClient())
            {
                client.Match(new MatchRequest(){AccountId = accountId, DeviceId = deviceId, Phrase = phrase, Keyword = keyword});
            }
        }

        public WordOfDayModel GetWordOfDay()
        {
            using (var client = new ApiServiceClient())
            {
               return client.GetLastWordOfTheDay().ToModel();
            }
        }
    }
}