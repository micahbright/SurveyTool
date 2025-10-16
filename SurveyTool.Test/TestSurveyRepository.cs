using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SurveyTool.Data;
using System.Security.Cryptography;

namespace SurveyTool.Test
{
    public class TestSurveyRepository
    {
        private readonly ISurveyRepository _repository;
        public TestSurveyRepository()
        {
            _repository = new SurveyRepository(new SurveyContext(new Microsoft.EntityFrameworkCore.DbContextOptions<SurveyContext>()));
        }

        [Fact]
        public void TestCreateAndLoadSurvey()
        {
            int sid = _repository.AddSurvey(new Survey()
            {
                Name = "Test",
            });

            int q1id = _repository.AddQuestion(new Question()
            {
                ParentId = null,
                SurveyId = sid,
                PercentWeight = 25,
                Text = "What color is the sky?",
                Type = QuestionType.FreeText,
            });

            int q2id = _repository.AddQuestion(new Question()
            {
                ParentId = null,
                SurveyId = sid,
                PercentWeight = 25,
                Text = "What type of car do you drive?",
                Type = QuestionType.SingleChoice,
            });

            _repository.AddQuestionOption(new QuestionOption() {
                QuestionId = q2id,
                Text = "Car"
            });
            _repository.AddQuestionOption(new QuestionOption()
            {
                QuestionId = q2id,
                Text = "Truck"
            });
            _repository.AddQuestionOption(new QuestionOption()
            {
                QuestionId = q2id,
                Text = "SUV"
            });
            _repository.AddQuestionOption(new QuestionOption()
            {
                QuestionId = q2id,
                Text = "None"
            });

            int q3id = _repository.AddQuestion(new Question()
            {
                ParentId = q2id,
                SurveyId= sid,
                Type = QuestionType.FreeText,
                Text = "What color is it?",
                PercentWeight = 25
            });

            // If Parent Question Response Equals "Truck", then this question is shown
            _repository.AddVisibilityRule(new QuestionVisibilityRule()
            {
                QuestionId = q3id,
                Type = VisibilityType.Equals,
                Value = "Truck",
                CustomType ="" 
            });
            var rules = _repository.GetVisibilityRules(sid);

            int q4id = _repository.AddQuestion(new Question()
            {
                ParentId = null,
                SurveyId = sid,
                Type = QuestionType.MultipleChoice,
                Text = "Which of these qualifies as a shape?",
                PercentWeight = 25
            });
            _repository.AddQuestionOption(new QuestionOption()
            {
                QuestionId = q4id,
                Text = "Square"
            });
            _repository.AddQuestionOption(new QuestionOption()
            {
                QuestionId = q4id,
                Text = "Circle"
            });
            _repository.AddQuestionOption(new QuestionOption()
            {
                QuestionId = q4id,
                Text = "Verb"
            });

            var testSurvey = Model.Survey.LoadFromDatabase(_repository, "Test");
        }

        [Fact]
        public void TestSaveAndLoadSurveyResponse()
        {
            TestCreateAndLoadSurvey();
            var testSurvey = Model.Survey.LoadFromDatabase(_repository, "Test");

            var sr = new SurveyResponse()
            {
                Name = "Test",
                SurveyId = testSurvey.Id,
            };
            int srid = _repository.AddSurveyResponse(sr);
            RecursiveAddQuestionResponse(testSurvey.Questions, srid);

            var surveyResponse = Model.SurveyResponse.LoadFromDatabase(_repository, srid);

            Model.SurveyResponse.UpdateSurveyResponse(_repository, surveyResponse);
            var scoreResponse = Model.SurveyResponse.LoadFromDatabase(_repository, srid);
        }

        private void RecursiveAddQuestionResponse(List<Model.Question> questions, int surveyResponseId)
        {
            foreach (var q in questions)
            {
                string responseText = "";
                switch (q.Type)
                {
                    case QuestionType.FreeText:
                        responseText = "Lorem ipsum";
                        break;
                    case QuestionType.MultipleChoice:
                        var choices = new Span<Model.QuestionOption>(q.QuestionOptions.ToArray());
                        RandomNumberGenerator.Shuffle(choices);

                        int num = RandomNumberGenerator.GetInt32(q.QuestionOptions.Count - 1) + 1;
                        var response = choices.Slice(0, num).ToArray().OrderBy(c => c.Id).Select(c => c.Text);
                        responseText = string.Join(',', response.ToArray());
                        break;
                    case QuestionType.SingleChoice:
                        // Force this for our example to be "Truck" in order to test the visibility rule
                        //responseText = "Truck";
                        //break;

                        // Random answer
                        int index = RandomNumberGenerator.GetInt32(q.QuestionOptions.Count);
                        responseText = q.QuestionOptions[index].Text;
                        break;
                }
                var qr1 = new QuestionResponse()
                {
                    QuestionId = q.Id,
                    ResponseText = responseText,
                    SurveyResponseId = surveyResponseId,
                    Score = RandomNumberGenerator.GetInt32(11),
                };
                _repository.AddQuestionResponse(qr1);
                RecursiveAddQuestionResponse(q.ChildQuestions, surveyResponseId);
            }
        }
    }
}