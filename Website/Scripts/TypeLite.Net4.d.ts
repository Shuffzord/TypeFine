
 
 

 

/// <reference path="Enums.ts" />

declare module Models {
interface PhraseModel {
  Comment: string;
  Right: string;
}
interface SearchResponseModel extends Website.Models.AjaxResponseModel {
  Phrases: Models.PhraseModel[];
  Keyword: string;
}
interface WordOfDayModel extends Website.Models.AjaxResponseModel {
  Date: string;
  Phrase: Models.PhraseModel;
}
}
declare module Website.Models {
interface AjaxResponseModel {
  Success: boolean;
  Redirect: boolean;
  UrlToRedirect: string;
  HasError: boolean;
  ErrorMessage: string;
}
}


