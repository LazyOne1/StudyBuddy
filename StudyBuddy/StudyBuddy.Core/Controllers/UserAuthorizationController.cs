using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Runtime.InteropServices.ComTypes;
using Nancy;
using Nancy.Authentication.Token;
using Nancy.Conventions;
using Nancy.Security;
using Newtonsoft.Json;
using StudyBuddy.Core.Models;
using StudyBuddy.DbModule;
using StudyBuddy.DbModule.DbHelpers;

namespace StudyBuddy.Core.Controllers
    {
    public class UserAuthorizationController : NancyModule
        {
        public static AuthenticationModel AuthenticationSingleton = new AuthenticationModel();

        public UserAuthorizationController(ITokenizer tokenizer)
            {
            Post["/login/"] = x =>
                {
                using (var dbWrapper = new DbWrapper("AuthenticationDbCore"))
                    {
                    if (!dbWrapper.DoesDbExist())
                        {
                        StudyBuddyDbAssistant.CreateDatabase("AuthenticationDbCore");
                        StudyBuddyDbAssistant.CreateTables("AuthenticationDbCore");
                        }
                    }
                var loginData = ParseAuthData(Request.Body);
                var identity = AuthenticationSingleton.AuthenticateUser(loginData["username"],
                    loginData["password"]);
                if (identity == null)
                    {
                    var response = (Response) JsonConvert.SerializeObject(FormErrorResponse(-1));
                    response.ContentType = "application/json";
                    response.StatusCode = HttpStatusCode.NotAcceptable;
                    return response;
                    }
                else
                    {
                    var token = tokenizer.Tokenize(identity, Context);
                    return new
                        {
                        Token = token
                        };
                    }
                };

            Get["/validatetoken/"] = x =>
                {
                this.RequiresAuthentication();
                return "Yay!";
                };

            Post["/register/"] = x =>
                {
                var regData = ParseAuthData(Request.Body);
                var authenticationStatus = AuthenticationSingleton.RegisterUser(regData["username"], regData["password"]);
                if (authenticationStatus != 0)
                    {
                    var response = (Response) JsonConvert.SerializeObject(FormErrorResponse(authenticationStatus));
                    response.ContentType = "application/json";
                    response.StatusCode = HttpStatusCode.NotAcceptable;
                    return response;
                    }
                using (var dbWrapper = new DbWrapper("AuthenticationDbCore"))
                    {
                    if (!dbWrapper.DoesDbExist())
                        {
                        StudyBuddyDbAssistant.CreateDatabase("AuthenticationDbCore");
                        StudyBuddyDbAssistant.CreateTables("AuthenticationDbCore");
                        }
                    }
                return HttpStatusCode.OK;
                };
            }

        public Dictionary<string, string> ParseAuthData(Stream requestStream)
            {
            using (var reader = new StreamReader(requestStream))
                {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
                }
            }

        public Dictionary<string, string> FormErrorResponse(int error)
            {
            var errorResponse = new Dictionary<string, string>();
            switch (error)
                {
                    case -1:
                        errorResponse.Add("status", "error");
                        errorResponse.Add("case", "Bad username or password.");
                        break;
                    case -3:
                        errorResponse.Add("status", "error");
                        errorResponse.Add("case", "User with that name already exists.");
                        break;
                    case -4:
                        errorResponse.Add("status", "error");
                        errorResponse.Add("case", "Incorrect characters in username or password.");
                        break;
                    default:
                        break;
                }
            return errorResponse;
            }
        }
    }