namespace StudyBuddy.Core.Data
    {
    public struct Quizz
        {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Questions[] questions { get; set; }

        public struct Questions
            {
            public string question;
            public Options[] options;
            public int[] rightAnswerIds;
            }

        public struct Options
            {
            public int id { get; set; }
            public string answer { get; set; }
            }
        }
    }