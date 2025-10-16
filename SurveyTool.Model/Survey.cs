using SurveyTool.Data;

namespace SurveyTool.Model
{
    public class Survey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Question> Questions { get; set; }
        public IEnumerable<Question> AllQuestions { get {
                return RecursiveEnumerateQuestions(Questions);
            } }

        private IEnumerable<Question> RecursiveEnumerateQuestions(List<Question> questions)
        {
            foreach (var question in questions)
            {
                yield return question;
                foreach(var x in RecursiveEnumerateQuestions(question.ChildQuestions))
                {
                    yield return x;
                }
            }
        }

        public static void DeleteFromDatabase(ISurveyRepository repository, string name)
        {
            repository.DeleteSurvey(name);
        }

        public static void UpdateSurvey(ISurveyRepository repository, Survey survey)
        {
            repository.UpdateSurvey(new Data.Survey()
            {
                Id = survey.Id,
                Name = survey.Name
            });

            UpdateChildQuestions(repository, survey.Questions, survey.Id);

            repository.DeleteSurvey(survey.Name);
            survey.CreateInDatabase(repository);
        }

        private static void UpdateChildQuestions(ISurveyRepository repository, List<Question> questions, int surveyId)
        {
            foreach (Question question in questions)
            {
                repository.UpdateQuestion(new Data.Question()
                {
                    Id = question.Id,
                    ParentId = question.ParentId,
                    PercentWeight = question.PercentWeight,
                    Text = question.Text,
                    Type = question.Type,
                    SurveyId = surveyId
                });
                foreach (var qOption in question.QuestionOptions)
                {
                    repository.UpdateQuestionOption(new Data.QuestionOption()
                    {
                        Id = qOption.Id,
                        QuestionId = qOption.QuestionId,
                        Text = qOption.Text,
                    });
                }
                foreach( var rules in question.VisibilityRules)
                {
                    repository.UpdateVisibilityRule(new Data.QuestionVisibilityRule()
                    {
                        Value = rules.Value,
                        Type = rules.Type,
                        CustomType = rules.CustomType,
                        Id = rules.Id,
                        QuestionId = question.Id
                    });
                }
            }
        }

        public static Survey LoadFromDatabase(ISurveyRepository repository, string name)
        {
            var survey = repository.GetSurvey(name);
            var questions = repository.GetQuestions(survey.Id);
            var questionOptions = repository.GetQuestionOptions(survey.Id);
            var visibilityRules = repository.GetVisibilityRules(survey.Id);
            var surveyOutput = new Survey()
            {
                Id = survey.Id,
                Name = survey.Name,
                Questions = new List<Question>()
            };

            foreach (var question in questions.ToList())
            {
                if (question.ParentId == null)
                {
                    var newQuestion = new Question()
                    {
                        ParentId = null,
                        ChildQuestions = new List<Question>(),
                        Id = question.Id,
                        PercentWeight = question.PercentWeight,
                        Text = question.Text,
                        Type = question.Type,
                        QuestionOptions = new List<QuestionOption>(),
                        VisibilityRules = new List<QuestionVisibilityRule>()
                    };
                    surveyOutput.Questions.Add(newQuestion);
                    LoadChildQuestions(newQuestion, question, surveyOutput, questionOptions, questions, visibilityRules);
                }
            }

            return surveyOutput;
        }

        private static void LoadChildQuestions(Question newQuestion, Data.Question question, Survey surveyOutput, IEnumerable<Data.QuestionOption> questionOptions, IEnumerable<Data.Question> questions, IEnumerable<Data.QuestionVisibilityRule> visibilityRules)
        {
            newQuestion.QuestionOptions
                .AddRange(questionOptions
                    .Where(c => c.QuestionId == question.Id)
                    .Select(c => new QuestionOption() {
                        Id = c.Id,
                        QuestionId = question.Id,
                        Text = c.Text
                    }));
            newQuestion.VisibilityRules
                .AddRange(visibilityRules
                    .Where(c=>c.QuestionId==question.Id)
                    .Select(c=> new QuestionVisibilityRule() {
                        Id = c.Id,
                        QuestionId = question.Id,
                        CustomType = c.CustomType,
                        Type = c.Type,
                        Value = c.Value
                    }));
            foreach (var q in questions)
            {
                if (q.ParentId == question.Id)
                {
                    var newChildQuestion = new Question() {
                        ParentId = q.ParentId,
                        ChildQuestions = new List<Question>(),
                        Id = q.Id,
                        PercentWeight = q.PercentWeight,
                        Text = q.Text,
                        Type = q.Type,
                        QuestionOptions = new List<QuestionOption>(),
                        VisibilityRules = new List<QuestionVisibilityRule>()
                    };
                    newQuestion.ChildQuestions.Add(newChildQuestion);
                    LoadChildQuestions(newChildQuestion, q, surveyOutput, questionOptions, questions, visibilityRules);
                }
            }
        }

        public void CreateInDatabase(ISurveyRepository repository)
        {
            int sid = repository.AddSurvey(new SurveyTool.Data.Survey()
            {
                Name = Name,
            });

            foreach(var question in Questions)
            {
                AddChildQuestion(repository, question, null, sid);
            }
            Id = sid;
        }

        private void AddChildQuestion(ISurveyRepository repository, Question question, int?parentQuestionId, int surveyId)
        {
            var qid = repository.AddQuestion(new SurveyTool.Data.Question()
            {
                ParentId = parentQuestionId,
                PercentWeight = question.PercentWeight,
                SurveyId = surveyId,
                Text = question.Text,
                Type = question.Type
            });

            foreach (var option in question.QuestionOptions)
            {
                repository.AddQuestionOption(new Data.QuestionOption()
                {
                    QuestionId = qid,
                    Text = option.Text
                });
            }

            foreach (var childQuestion in question.ChildQuestions)
            {
                AddChildQuestion(repository, childQuestion, qid, surveyId);
            }
        }

    }
}
