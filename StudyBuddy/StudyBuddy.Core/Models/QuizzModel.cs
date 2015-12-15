using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Newtonsoft.Json;
using StudyBuddy.Core.Data;
using StudyBuddy.DbModule.DbHelpers;

namespace StudyBuddy.Core.Models
    {
    public class QuizzModel
        {
        public string GetQuizz(int id)
            {
            var quizz = new Quizz();

            using (var dbWrapper = new DbWrapper("studybuddyquizzes"))
                {
                using (var command = new SQLiteCommand("SELECT QuizzName, Description FROM Quizzes WHERE QuizzID = @id")
                    )
                    {
                    command.Parameters.AddWithValue("@id", id);
                    var reader = dbWrapper.ExecuteReader(command);
                    while (reader.Read())
                        {
                        quizz.name = reader.GetString(0);
                        quizz.description = reader.GetString(1);
                        }
                    }
                }
            using (var dbWrapper = new DbWrapper ("studybuddyquizzes"))
                {
                using (var command = new SQLiteCommand ("SELECT QuestionID, Question FROM Questions WHERE QuizzID = @id"))
                    {
                    var questionId = 0;
                    var questions = new List<Quizz.Questions>();
                    command.Parameters.AddWithValue ("@id", id);
                    var reader = dbWrapper.ExecuteReader (command);
                    while (reader.Read ())
                        {
                        var singleQuestion = new Quizz.Questions();
                        var correctAnswersList = new List<int>();
                        questionId = reader.GetInt32 (0);
                        singleQuestion.question = reader.GetString (1);
                        var options = new List<Quizz.Options> ();
                        using (var internalWrapper = new DbWrapper("studybuddyquizzes"))
                            {
                            using (
                                var innerCommand =
                                    new SQLiteCommand(
                                        "SELECT AnswerID, Answer, Is_Correct FROM Answers WHERE QuestionID = @QId"))
                                {
                                
                                innerCommand.Parameters.AddWithValue("@QId", questionId);
                                var innerReader = internalWrapper.ExecuteReader(innerCommand);
                                while (innerReader.Read())
                                    {
                                    var option = new Quizz.Options ();
                                    option.id = innerReader.GetInt32(0);
                                    option.answer = innerReader.GetString(1);
                                    if (innerReader.GetInt32(2) == 1)
                                        {
                                        correctAnswersList.Add(option.id);
                                        }
                                    options.Add(option);
                                    }
                                }
                            }
                        singleQuestion.options = options.ToArray();
                        singleQuestion.rightAnswerIds = correctAnswersList.ToArray();
                        questions.Add(singleQuestion);
                        }
                    quizz.questions = questions.ToArray();
                    }
                }
            return JsonConvert.SerializeObject(quizz);
            }

        public string GetAllQuizzes()
            {
            var metaList = new List<QuizzMeta>();
            var quizzMeta = new Dictionary<string, List<QuizzMeta>> ();
            
            using (var dbWrapper = new DbWrapper("studybuddyquizzes"))
                {
                using (var command = new SQLiteCommand("SELECT QuizzID, QuizzName, Description FROM Quizzes"))
                    {
                    var reader = dbWrapper.ExecuteReader(command);
                    while (reader.Read())
                        {
                        var singleQuizzMeta = new QuizzMeta
                            {
                            id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            description = reader.GetString(2)
                            };
                        metaList.Add(singleQuizzMeta);
                        }
                    }
                }
            quizzMeta.Add("quizzes", metaList);
            return JsonConvert.SerializeObject(quizzMeta);
            }

        public void CreateNewQuizz(Quizz quizzData)
            {
            using (var dbWrapper = new DbWrapper("studybuddyquizzes"))
                {
                var quizzId = 0;
                if (!dbWrapper.DoesDbExist())
                    {
                    InstantiateQuizzDatabase();
                    }
                using (
                    var command =
                        new SQLiteCommand(
                            "INSERT INTO Quizzes (QuizzName, Description) values (@QuizzName, @Description)")
                    )
                    {
                    command.Parameters.AddWithValue("@QuizzName", quizzData.name);
                    command.Parameters.AddWithValue("@Description", quizzData.description);

                    dbWrapper.ExecuteNonQuery(command);
                    command.CommandText =
                            "SELECT QuizzID FROM Quizzes WHERE QuizzName = @QuizzName";
                    quizzId = int.Parse (dbWrapper.ExecuteScalar (command).ToString ());
                    }
                foreach (var question in quizzData.questions)
                    {
                    var questionId = -1;
                    using (
                        var command =
                            new SQLiteCommand(
                                "INSERT INTO Questions (Question, QuizzID) values (@Question, @QuizzID)")
                        )
                        {
                        command.Parameters.AddWithValue("@QuizzID", quizzId);
                        command.Parameters.AddWithValue("@Question", question.question);
                        dbWrapper.ExecuteNonQuery(command);
                        command.CommandText =
                            "SELECT QuestionID FROM Questions WHERE Question = @Question AND QuizzID = @QuizzID";
                        questionId = int.Parse(dbWrapper.ExecuteScalar(command).ToString());
                        }
                    foreach (var option in question.options)
                        {
                        using (
                            var command =
                                new SQLiteCommand(
                                    "INSERT INTO Answers (QuestionID, Answer, Is_Correct) values (@QuestionID, @Answer, @Is_Correct)")
                            )
                            {
                            command.Parameters.AddWithValue("@QuestionID", questionId);
                            command.Parameters.AddWithValue("@Answer", option.answer);
                            if (question.rightAnswerIds.Contains(option.id))
                                {
                                command.Parameters.AddWithValue("@Is_Correct", 1);
                                }
                            else
                                {
                                command.Parameters.AddWithValue("@Is_Correct", 0);
                                }
                            dbWrapper.ExecuteNonQuery(command);
                            }
                        }
                    }
                }
            }

        public void AddQuestions(Dictionary<string, string> quizzData)
            {
            var quizzId = quizzData["quizz_id"];
            using (var dbWrapper = new DbWrapper("studybuddyquizzes"))
                {
                if (!dbWrapper.DoesDbExist())
                    {
                    InstantiateQuizzDatabase();
                    }
                using (var command = new SQLiteCommand ("SELECT QuizzID from Quizzes WHERE QuizzID = @QuizzID"))
                    {
                    command.Parameters.AddWithValue ("@QuizzID", quizzId);
                    var found = dbWrapper.ExecuteScalar(command);

                    }
                }
            }

        static void InstantiateQuizzDatabase ()
            {
            StudyBuddyDbAssistant.CreateDatabase("studybuddyquizzes");
            StudyBuddyDbAssistant.CreateQuizzTables("studybuddyquizzes");
            }
        }
    }