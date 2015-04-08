using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Service.Contracts;
using Service.Inspection;

namespace Service
{
    //[RequestInfoSaving]
    public class ApiService : IApiService
    {

        private static void SaveInfo(IBaseRequest request, TypeFineContext ctx, RequestType type, bool save = false)
        {
            ctx.RequestInfos.Add(new RequestInfo
            {
                RequestType = type,
                AccountId = request.AccountId,
                DeviceId = request.DeviceId
            });
            if (save)
                ctx.SaveChanges();
        }

        /// <summary>
        /// Pobiera 5 pierwszych (lub 6-10) frazy pasujące do słowa kluczowego.
        /// </summary>
        public GetPhrasesResponse GetPhrase(GetPhrasesRequest request)
        {
            var response = new GetPhrasesResponse();
            using (var ctx = new TypeFineContext())
            {
                //SaveInfo(request, ctx, RequestType.GetPhrase, true);

                var phrases = ctx.TheAlgorithm(request.Keyword).ToList();
                if (phrases.Count() == 1)
                    response.Phrases = phrases.Select(x =>
                                         new ContractPhrase
                                             {
                                                 Keyword = request.Keyword,
                                                 Comment = x.Comment,
                                                 Right = x.Value
                                             }).ToList();

                if (request.Skip)
                    response.Phrases = phrases.Skip(5).Select(x =>
                    new ContractPhrase
                        {
                            Keyword = request.Keyword,
                            Comment = x.Comment,
                            Right = x.Value
                        }).ToList();
                else
                    response.Phrases = phrases.Take(5).Select(x =>
                    new ContractPhrase
                    {
                        Keyword = request.Keyword,
                        Comment = x.Comment,
                        Right = x.Value
                    }).ToList();
            }
            return response;
        }

        public GetPhrasesResponse GetPhrase2(string keyword)
        {
            throw new NotImplementedException();
        }

        public void AddPhrase(AddPhraseRequest request)
        {
            using (var ctx = new TypeFineContext())
            {
                SaveInfo(request, ctx, RequestType.AddPhrase);
                ctx.ProposedPhrases.Add(new ProposedPhrase
                    {
                        Comment = request.Comment,
                        Right = request.Right
                    });
                ctx.SaveChanges();
            }
        }


        public CheckResponse Check(CheckRequest request)
        {
            using (var context = new TypeFineContext())
            {
                SaveInfo(request, context, RequestType.Check, true);
                return new CheckResponse
                    {
                        NewPhrasesCount = context.Phrases.Count(x => x.AddDate > request.LastUpdateDate && x.Interesting),
                    };
            }
        }

        public UpdateResponse Update(UpdateRequest request)
        {
            using (var context = new TypeFineContext())
            {
                SaveInfo(request, context, RequestType.Update, true);
                return new UpdateResponse
                    {
                        Phrases = context.Phrases.Where(x => x.AddDate > request.LastUpdateDate && x.Interesting).Select(x => x.Value).ToList(),
                    };
            }
        }

        public void RegisterOrUpdateUserNotification(NotificationRequest request)
        {
            using (var context = new TypeFineContext())
            {
                SaveInfo(request, context, RequestType.RegisterOrUpdateUserNotification);

                var notificationUser = context.NotificationUsers.SingleOrDefault(
                    x => x.User.DeviceId == request.DeviceId && x.User.AccountId == request.AccountId);
                if (notificationUser != null)
                {
                    if (notificationUser.ChannelName != null && notificationUser.ChannelName != request.ChannelUrl)
                    {
                        notificationUser.ChannelName = request.ChannelUrl;
                    }
                }
                else
                {
                    var user =
                        context.Users.SingleOrDefault(
                            x => x.AccountId == request.AccountId && x.DeviceId == request.DeviceId);
                    if (user != null)
                    {
                        AddNotificationUser(request, context, user);

                    }
                    else
                    {
                        var newUser = context.Users.Add(new User
                            {
                                DeviceId = request.DeviceId,
                                AccountId = request.AccountId
                            });
                        AddNotificationUser(request, context, newUser);
                    }
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Zwiększa "wagę" słowa wybranego z propozycji fraz.
        /// </summary>
        public void Match(MatchRequest request)
        {
            using (var ctx = new TypeFineContext())
            {
                SaveInfo(request, ctx, RequestType.Match);
                KeywordPhraseReference reference;
                try
                {
                    reference = ctx.KeywordPhraseReferences.Include(x => x.Keyword).Include(x => x.Phrase).Single(x =>
                                                                      x.Keyword.Value == request.Keyword &&
                                                                      x.Phrase.Value == request.Phrase);
                }
                catch (Exception)
                {
                    throw new FaultException("nie znaleziono złączenia pomiędzy podanym słowem kluczowym i frazą", new FaultCode("Match"));
                }
                reference.UserWeight += 0.02;
                if (reference.CompositeConvergance > 1)
                {
                    reference.Keyword.Phrase = reference.Phrase;
                    var toRemove = reference.Keyword.WeakReferences.ToList();
                    ctx.KeywordPhraseReferences.RemoveRange(toRemove);
                    ctx.SaveChanges();
                }
                ctx.SaveChanges();
            }
        }

        public GetMessagesResponse GetMessages(GetMessagesRequest request)
        {
            var response = new GetMessagesResponse();
            using (var context = new TypeFineContext())
            {
                var requestDate = request.Date;
                response.Messages =
                    context.DeveloperMessages.Where(x => x.Date >= requestDate).Select(y => new Message
                        {
                            Header = y.Header,
                            Content = y.Content,
                            Date = y.Date
                        }).ToList();
            }
            return response;
        }

        public GetPhrasesResponse GetRandomPhrase(GetRandomPhraseRequest request)
        {
            var response = new GetPhrasesResponse();
            using (var ctx = new TypeFineContext())
            {
                var item = ctx.Phrases.Where(x => !String.IsNullOrEmpty(x.Comment)).OrderBy(r => Guid.NewGuid()).Take(1);
                var itm = item.Single();
                response.Phrases = new List<ContractPhrase>(new[]
                    {
                        new ContractPhrase
                            {
                                Comment = itm.Comment,
                                Right = itm.Value
                            }
                    });
            }
            return response;
        }

        public WordOfTheDayResponse GetLastWordOfTheDay()
        {
            var response = new WordOfTheDayResponse();
            using (var context = new TypeFineContext())
            {
                var last = context.WordsOfTheDay.OrderByDescending(x => x.ActiveIn).Take(1);
                if (!last.Any()) return response;
                var word = last.First();
                response = new WordOfTheDayResponse
                    {
                        Date = word.ActiveIn,
                        ContractPhrase = new ContractPhrase
                            {
                                Right = word.Phrase.Value,
                                Comment = word.Phrase.Comment
                            }
                    };
            }
            return response;
        }

        public WordOfTheDayResponse GetWordOfTheDay(WordOfTheDayRequest request)
        {
            return GetLastWordOfTheDay();
        }

        public CrosswordResponse GenerateCrossword(CrosswordRequest request)
        {

            var cw = new Tools.Crossword(request.DimX, request.DimY);
            List<Phrase> words;
            using (var ctx = new TypeFineContext())
            {
                words = ctx.Phrases.Where(x => !String.IsNullOrEmpty(x.Comment)).ToList();

            }
            cw.AddWords(words.Take(100).Select(x => x.Value).ToList());
            return new CrosswordResponse { Board = cw.GetBoard };
        }

        public void LogError(LogErrorRequest request)
        {
            //dostuff
        }

        public void LogTime(LogTimeRequest request)
        {
            //do stuff
        }

        //public void Setup()
        //{
        //    MultithreadedGenerateServiceDatabase();
        //}

        public static void MultithreadedGenerateServiceDatabase()
        {
            const int threshold = 2000;
            const string path = "Domain.Migration.Data.Phrases2.csv";

            using (var ctx = new TypeFineContext())
            {
                Database.SetInitializer(new PhraseDatabaseInitializer());
                if (ctx.Database.Exists())
                {
                    ctx.Database.Initialize(true);
                }

            }

            var bc = new BlockingCollection<List<string>>();
            var readingTask = Task.Run(() =>
            {
                var phrases =
                    Assembly.GetAssembly(typeof(PhraseDatabaseInitializer))
                            .GetManifestResourceStream(path);

                if (phrases == null)
                    throw new Exception(string.Format("There is no ebedded resource under path {0}", path));
                var i = 1;

                var bulk = new List<string>();
                using (var sr = new StreamReader(phrases))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        bulk.Add(line);
                        if (i % threshold == 0)
                        {
                            bc.Add(bulk);
                            bulk = new List<string>();
                        }
                        i++;
                    }

                    bc.Add(bulk);
                    bc.CompleteAdding();
                }
            });

            var savingTask = Task.Run(() =>
                {
                    Parallel.ForEach(bc.GetConsumingEnumerable(), bulk =>
                        {

                            using (var ctx = new TypeFineContext())
                            {
                                var keywords = bulk.Select(x =>
                                    {
                                        var values = x.Split(new[] { "&&" }, StringSplitOptions.None);
                                        var phrase = new Phrase
                                            {
                                                Value = values[0],
                                                Comment = values[1],
                                                Interesting = int.Parse(values[2]) == 1,
                                                Source = (SourceType)int.Parse(values[3]),
                                                AddDate = DateTime.Parse(values[4], CultureInfo.InvariantCulture)
                                            };
                                        var keyword = new Keyword
                                            {
                                                Phrase = phrase,
                                                Value = phrase.Value
                                            };
                                        return keyword;
                                    });
                                ctx.Keywords.AddRange(keywords);
                                ctx.SaveChanges();
                            }
                        });

                });

            Task.WaitAll(readingTask, savingTask);
        }

        private static void AddNotificationUser(NotificationRequest request, TypeFineContext context, User user)
        {
            context.NotificationUsers.Add(new NotificationUser
                {
                    ChannelName = request.ChannelUrl,
                    User = user,
                    LastUpdate = DateTime.Now
                });
        }
    }
}
