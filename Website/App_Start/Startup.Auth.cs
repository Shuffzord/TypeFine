using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Website
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {

            /*
             * Jak ja kurwa nienawidzę tych chui z MS i ich jebaną konfigurację i dokumentację, 
             * która zakłada, że ja KONIECZNIE muszę używać bazy użyszkodników.
             * Godziny zajęło mi ustawienie odpowiedniej konfiguracji ciasteczek.
             * O ile nic nie płonie - nie ruszać.
             * 
             * Ponieważ nie tworzymy własnego użyszkodnika nad zewnętrznym cookie, a mimo to chcielibyśmy
             * by użyszkodnik zalogowany zewnętrznym providerem traktowany był jak zalogowany providerem aplikacji
             * musimy zadeklarować ciasteczko ApplicationCookie jako identyfikujące użyszkodnika
             * a potem wrzucać w nie ciasteczko zewnętrzne. 
             * 
             * I to wystarcza, wy jebane chuje piszące dokumentację.
            */

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ApplicationCookie);

            app.UseFacebookAuthentication(
               appId: "654303034605750",
               appSecret: "4bafadb2756f2cf7917c6c387dfdd206");

        }
    }
}