using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using Microsoft.Owin.Security;
using Website.Models;
using Website.Services;

namespace Website.Controllers
{
    public class SearchController : ControllerBase
    {
        private readonly IPhraseService _phraseService;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public SearchController(IPhraseService phraseService)
        {
            _phraseService = phraseService;
        }

        private string GetDeviceId()
        {
            return Request.ServerVariables["REMOTE_ADDR"];
        }

        private string GetAccountId()
        {
            var login = AuthenticationManager.User;
            if (login == null || !login.Identity.IsAuthenticated)
                return null;
            return login.Claims.First().Value;
        }

        [ActionName("Q")]
        public ActionResult Query(string value, bool skip = false)
        {
            var deviceId = GetDeviceId();
            var accountId = GetAccountId();
            
            var response = new SearchResponseModel(value);
            try
            {
                response.Phrases = _phraseService.GetPhrases(value, deviceId, accountId, skip);
                response.Success = true;
            }
            catch (SoapException e)
            {
                response.SetError(e);
            }
            catch (Exception e)
            {
                response.SetError(e);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [ActionName("M")]
        public ActionResult ConfirmMatch(string keyword, string phrase)
        {
              var response = new MatchResponseModel();
              var deviceId = GetDeviceId();
              var accountId = GetAccountId();

              try
              {
                 _phraseService.Match(keyword, phrase, deviceId, accountId);
                  response.Success = true;
              }
              catch (SoapException e)
              {
                  response.SetError(e);
              }
              catch (Exception e)
              {
                  response.SetError(e);
              }
              return Json(response, JsonRequestBehavior.AllowGet);
        }

        [ActionName("W")]
        public ActionResult WordOfDay()
        {
            var response = new WordOfDayModel();
            try
            {
                response =  _phraseService.GetWordOfDay();
                response.Success = true;
            }
            catch (SoapException e)
            {
                response.SetError(e);
            }
            catch (Exception e)
            {
                response.SetError(e);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}