﻿using System.Collections.Generic;
using System.IO;
using Nancy;
using Nancy.Authentication.Token;
using Newtonsoft.Json;
using StudyBuddy.Core.Models;
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
                        StudyBuddyDbAssistant.CreateAuthenticationTables("AuthenticationDbCore");
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
                        StudyBuddyDbAssistant.CreateAuthenticationTables("AuthenticationDbCore");
                        }
                    }

                var identity = AuthenticationSingleton.AuthenticateUser(regData["username"],
                    regData["password"]);
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
                    case -5:
                        errorResponse.Add("status", "error");
                        errorResponse.Add("case", "Username or password is too short.");
                        break;
                    default:
                        break;
                }
            return errorResponse;
            }
        }
    }