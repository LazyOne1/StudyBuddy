using System.Data.SQLite;

namespace StudyBuddy.DbModule.DbHelpers
    {
    public class StudyBuddyDbAssistant
        {
        public static void CreateDatabase(string database)
            {
            SQLiteConnection.CreateFile(database + ".sqlite");
            }

        public static void CreateQuizzTables(string database)
            {
            using (var sqLiteConnection = new SQLiteConnection("DataSource=" + database + ".sqlite;Version=3;"))
                {
                using (
                    var sqLiteCommand =
                        new SQLiteCommand(
                            "create table Quizzes (QuizzID INTEGER PRIMARY KEY AUTOINCREMENT, QuizzName varchar(25) UNIQUE, Description varchar(50))")
                    )
                    {
                    sqLiteCommand.Connection = sqLiteConnection;
                    sqLiteConnection.Open ();
                    sqLiteCommand.ExecuteNonQuery ();

                    sqLiteCommand.CommandText =
                        "create table Questions (QuestionID INTEGER PRIMARY KEY AUTOINCREMENT, 'QuizzID' INTEGER REFERENCES 'Quizzes' ('QuizzID'), 'Question' varchar(25))";
                    sqLiteCommand.ExecuteNonQuery ();

                    sqLiteCommand.CommandText =
                       "create table Answers (AnswerID INTEGER PRIMARY KEY AUTOINCREMENT, 'QuestionID' INTEGER REFERENCES 'Questions' ('QuestionID'), 'Answer' varchar(25), Is_Correct INTEGER default 0)";
                    sqLiteCommand.ExecuteNonQuery ();
                    }
                }
            }

        public static void CreateAuthenticationTables(string database)
            {
            using (var sqLiteConnection = new SQLiteConnection("DataSource=" + database + ".sqlite;Version=3;"))
                {
                using (
                    var sqLiteCommand =
                        new SQLiteCommand(
                            "create table UserGroups (GroupID INTEGER PRIMARY KEY AUTOINCREMENT, GroupName varchar(20) UNIQUE, GroupClass varchar(20) UNIQUE)")
                    )
                    {
                    sqLiteCommand.Connection = sqLiteConnection;
                    sqLiteConnection.Open();
                    sqLiteCommand.ExecuteNonQuery();

                    sqLiteCommand.CommandText =
                        "create table UserRoles (RoleID INTEGER PRIMARY KEY AUTOINCREMENT, RoleName varchar(20) UNIQUE, PermissionLevel INTEGER default 0)";
                    sqLiteCommand.ExecuteNonQuery();

                    sqLiteCommand.CommandText =
                        "create table 'Users' ('UserID' INTEGER PRIMARY KEY, 'GroupID' INTEGER REFERENCES 'UserGroups' ('GroupID'), 'RoleID' " +
                        "INTEGER REFERENCES 'UserRoles' ('RoleID'), UserName varchar(20) UNIQUE, UserPassword varchar(20), RegistrationDate datetime)";
                    sqLiteCommand.ExecuteNonQuery();

                    sqLiteCommand.CommandText =
                        "INSERT into UserGroups (GroupName, GroupClass) values ('Not Set', 'Default')";
                    sqLiteCommand.ExecuteNonQuery();

                    sqLiteCommand.CommandText = "INSERT into UserRoles (RoleName) values ('Standard')";
                    sqLiteCommand.ExecuteNonQuery();

                    sqLiteCommand.CommandText =
                        "INSERT into UserRoles (RoleName, PermissionLevel) values ('Admin', 9001)";
                    sqLiteCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }