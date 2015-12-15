using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Contexts;
using Nancy;
using Nancy.Authentication.Token;
using Nancy.Diagnostics;
using Nancy.Security;
using Newtonsoft.Json;
using StudyBuddy.Core.Data;
using StudyBuddy.Core.Models;
using StudyBuddy.DbModule.DbHelpers;

namespace StudyBuddy.Core.Controllers
    {
    public class QuizzController : NancyModule
        {
        public QuizzController(ITokenizer tokenizer)
            {
            Post["/newquizz/"] = x =>
                {

                //this.RequiresAuthentication();
                var quizzData = ParseRequestData(Request.Body);
                var quizzModel = new QuizzModel();
                quizzModel.CreateNewQuizz(quizzData);
                return HttpStatusCode.OK;
                };

            Get["/singlequizz/{id}"] = x =>
                {
                var id = int.Parse(x["id"].Value);
                var quizzModel = new QuizzModel();
                var response = (Response) quizzModel.GetQuizz(id);
                response.ContentType = "application/json";
                response.StatusCode = HttpStatusCode.OK;
                return response;
                };

            Get["/allquizzes/"] = x =>
                {
                //this.RequiresAuthentication();
                var quizzModel = new QuizzModel();
                var response = (Response) quizzModel.GetAllQuizzes();
                response.ContentType = "application/json";
                response.StatusCode = HttpStatusCode.OK;
                return response;
                };

            Post["/addquestions/"] = x =>
                {
                var quizzData = ParseRequestData(Request.Body);
                var quizzModel = new QuizzModel();

                return HttpStatusCode.OK;
                };
            }

        public Quizz ParseRequestData(Stream requestStream)
            {
            using (var reader = new StreamReader(requestStream))
                {
                var strings =
                    reader.ReadToEnd();
                return JsonConvert.DeserializeObject<Quizz>(strings);
                }
            }
        }
    }