using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.Contracts
{
    [DataContract]
    public class ContractPhrase
    {
        [DataMember]
        public string Keyword { get; set; }

        [DataMember]
        public string Right { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }

    public interface IBaseRequest
    {
        string DeviceId { get; set; }

        string AccountId { get; set; }
    }

    [DataContract]
    public abstract class BaseRequest : IBaseRequest
    {
        [DataMember]
        public string DeviceId { get; set; }

        [DataMember]
        public string AccountId { get; set; }
    }

    [DataContract]
    public class GetPhrasesRequest : BaseRequest
    {
        [DataMember]
        public string Keyword { get; set; }

        [DataMember]
        public bool Skip { get; set; }
    }

    [DataContract]
    public class GetPhrasesResponse
    {
        [DataMember]
        public List<ContractPhrase> Phrases { get; set; }
    }

    [DataContract]
    public class AddPhraseRequest : BaseRequest
    {
        [DataMember]
        public string Right { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }

    [DataContract]
    public class CheckRequest : BaseRequest
    {
        [DataMember]
        public DateTime LastUpdateDate { get; set; }
    }

    [DataContract]
    public class CheckResponse
    {
        [DataMember]
        public int NewPhrasesCount { get; set; }
    }

    [DataContract]
    public class UpdateRequest : BaseRequest
    {
        [DataMember]
        public DateTime LastUpdateDate { get; set; }
    }

    [DataContract]
    public class NotificationRequest : BaseRequest
    {
        [DataMember]
        public string ChannelUrl { get; set; }
    }

    [DataContract]
    public class UpdateResponse
    {
        [DataMember]
        public DateTime UpdateDate { get; set; }

        [DataMember]
        public IList<string> Phrases { get; set; }
    }


    [DataContract]
    public class MatchRequest : BaseRequest
    {
        [DataMember]
        public string Keyword { get; set; }

        [DataMember]
        public string Phrase { get; set; }
    }

    [DataContract]
    public class GetMessagesRequest : BaseRequest
    {
        [DataMember]
        public DateTime Date { get; set; }
    }

    [DataContract]
    public class GetRandomPhraseRequest : BaseRequest
    {
        
    }

    [DataContract]
    public class Message
    {
        [DataMember]
        public string Header { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public DateTime Date { get; set; }
    }

    [DataContract]
    public class GetMessagesResponse
    {
        [DataMember]
        public List<Message> Messages { get; set; }
    }

    [DataContract]
    public class WordOfTheDayRequest : BaseRequest
    {

    }

    [DataContract]
    public class CrosswordResponse
    {
        public char[,] Board { get; set; }
    }

    [DataContract]
    public class CrosswordRequest : BaseRequest
    {
        [DataMember]
        public int DimX { get; set; }

        [DataMember]
        public int DimY { get; set; }
    }

    [DataContract]
    public class LogTimeRequest : BaseRequest
    {
        public long Miliseconds { get; set; }
    }

    [DataContract]
    public class LogErrorRequest : BaseRequest
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Exception { get; set; }

        [DataMember]
        public LogErrorType LogErrorType { get; set; }

    }

    [DataContract]
    public class WordOfTheDayResponse
    {
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public ContractPhrase ContractPhrase { get; set; }
    }
}
