using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Website.PhraseService;

namespace Website.Extensions
{
    public interface IPhrase
    {
        string Comment { get; set; }
        string Right { get; set; }
    }

    public static class PhraseExtensions
    {
        internal class TransientPhrase : IPhrase
        {
            public string Comment { get; set; }
            public string Right { get; set; }

            public TransientPhrase(Phrase @this)
            {
                Comment = @this.Comment;
                Right = @this.Right;
            }

            public TransientPhrase(string comment, string right)
            {
                Comment = comment;
                Right = right;
            }
        }

        public static IPhrase ToTransient(this Phrase @this)
        {
            return new TransientPhrase(@this);
        }

        public static IPhrase ToTransient(string comment, string right)
        {
            return new TransientPhrase(comment, right);
        }

        public static IPhrase PreparePhrase(this IPhrase @this)
        {
            @this.NullToStringEmpty().TrimByKeyword().TrimByThingy().LineToMultiline();
            return @this;
        }

        public static IPhrase PreparePhrase(this Phrase @this)
        {
            var transient = @this.ToTransient().PreparePhrase();
            return transient;
        }

        private static IPhrase NullToStringEmpty(this IPhrase @this)
        {
            if (@this.Comment == null)
                @this.Comment = string.Empty;
            return @this;
        }

        private static string ToLi(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
                return string.Empty;
            return "<li>" + @this + "</li>";
        }

        private static string ToUl(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
                return string.Empty;
            return "<ul>" + @this + "</ul>";
        }

        private static string ToH6(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
                return string.Empty;
            if (@this.EndsWith(";"))
                @this = @this.Remove(@this.Length - 1, 1);
            return "<h6>" + @this + "</h6>";
        }

        private static string ProcessString(this string @this)
        {
            if (string.IsNullOrEmpty((@this)))
                return string.Empty;

            var matches = Regex.Split(@this, LetterParenthesisPattern);
            if (matches.Any())
            {
                var newMatches = new List<string>();
                for (var i = 0; i < matches.Length; i++)
                {
                    var match = matches[i];
                    if (!match.StartsWithLetterParenthesis())
                    {
                        newMatches.Add(match);
                    }
                }

                if (@this.StartsWithLetterParenthesis())
                {
                    @this = string.Join("", newMatches.Select(x => x.ToLi())).ToUl();
                }
                else
                {
                    @this = matches[0].ToH6();
                    @this += string.Join("", newMatches.Skip(1).Select(x => x.ToLi())).ToUl();
                }
               
            }

            return "<span class='search-definition'>" + @this + "</span>";
        }

        private const string NumberDotPattern = @"([0-9]\.)";
        private const string LetterParenthesisPattern = @"( [aA-zZ]\))";
        private const string StartsWithLetterParenthesisPattern = @"^( [aA-zZ]\))";
        private const string StartsWithNumberDotPattern = @"^([0-9]\.)";

        private static bool StartsWithLetterParenthesis(this string @this)
        {
            return Regex.IsMatch(@this, StartsWithLetterParenthesisPattern);
        }

        private static bool StartsWithNumberDot(this string @this)
        {
            return Regex.IsMatch(@this, StartsWithNumberDotPattern);
        }

        private static string ToH5(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
                return string.Empty;
            return "<h5>" + @this + "</h5>";
        }

        private static IPhrase LineToMultiline(this IPhrase @this)
        {
            var matches = Regex.Split(@this.Comment, NumberDotPattern);
            if (matches.Any() && matches.Length > 1)
            {
                var newMatches = new List<string>();
                for (var i = 0; i < matches.Length; i++)
                {
                    var match = matches[i];
                    if (match.StartsWithNumberDot())
                    {
                        if (i + 1 <= matches.Length)
                        {
                            matches[i + 1] = matches[i] + matches[i + 1];
                            newMatches.Add(matches[i+1]);
                            i++;
                        }
                    }
                    else
                    {
                        newMatches.Add(match);
                    }
                }

                @this.Comment = string.Empty;
                if (!@this.Comment.StartsWithNumberDot())
                {
                    @this.Comment = newMatches.First().ToH5();
                    @this.Comment += string.Join("", newMatches.Skip(1).Select(x => x.ProcessString()));
                }
                else
                    @this.Comment = string.Join("", newMatches.Select(x => x.ProcessString()));
            }
            else
            {
                @this.Comment = @this.Comment.ToH5();
            }
            return @this;
        }

        private static IPhrase TrimByThingy(this IPhrase @this)
        {
            @this.Comment = @this.Comment.Replace("„", "\"").Replace("”", "\"").Replace("\"\"", "\"");
            return @this;
        }

        private static IPhrase TrimByKeyword(this IPhrase @this)
        {
            if (@this.Comment.StartsWith(@this.Right))
                @this.Comment = @this.Comment.Remove(0, @this.Right.Length);
            return @this;
        }
    }
}