using SurveyTool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyTool.Model
{
    public class SurveyResponse
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public List<QuestionResponse> QuestionResponses { get; set; }
        public decimal Score { get; set; }

        public void TabulateScore(ISurveyRepository repository)
        {
            decimal totalScore = 0m;
            var survey = Model.Survey.LoadFromDatabase(repository, Name);

            foreach (var q in QuestionResponses)
            {
                decimal pct = survey.AllQuestions.Single(c => c.Id == q.QuestionId).PercentWeight / 100M;
                decimal weightedScore = q.Score * pct;
                totalScore += weightedScore;
            }
            Score = totalScore;
        }

        public int CreateInDatabase(ISurveyRepository repository)
        {
            int srid = repository.AddSurveyResponse(new SurveyTool.Data.SurveyResponse() { Name = Name });
            foreach(var qr in QuestionResponses)
            {
                var questionResponse = new SurveyTool.Data.QuestionResponse()
                {
                    QuestionId = qr.QuestionId,
                    ResponseText = qr.ResponseText,
                    SurveyResponseId = srid,
                    Score = qr.Score
                };
                repository.AddQuestionResponse(questionResponse);
            }
            Id = srid;
            TabulateScore(repository);
            return Id;
        }

        public static SurveyResponse LoadFromDatabase(ISurveyRepository _repository, int id)
        {
            var surveyResponse = new SurveyResponse();
            surveyResponse.Id = id;
            var sr = _repository.GetSurveyResponse(id);
            var qrs = _repository.GetQuestionResponses(id);
            surveyResponse.Name = sr.Name;
            surveyResponse.Score = sr.Score;
            surveyResponse.SurveyId = sr.SurveyId;
            surveyResponse.QuestionResponses = qrs.Select(c => new QuestionResponse()
            {
                QuestionId = c.QuestionId,
                ResponseText = c.ResponseText,
                Id = c.Id,
                SurveyResponseId = sr.Id,
                Score = c.Score
            }).ToList();
            return surveyResponse;
        }

        public static void UpdateSurveyResponse(ISurveyRepository _repository, SurveyResponse surveyResponse)
        {
            var rules = _repository.GetVisibilityRules(surveyResponse.SurveyId);
            surveyResponse.TabulateScore(_repository);
            _repository.UpdateSurveyResponse(new Data.SurveyResponse()
            {
                Id = surveyResponse.Id,
                Name = surveyResponse.Name,
                Score = surveyResponse.Score,
                SurveyId = surveyResponse.SurveyId
            });
            var survey = Survey.LoadFromDatabase(_repository, surveyResponse.Name);
            ValidateVisibilityRules(survey, rules, surveyResponse);
            foreach (var qr in surveyResponse.QuestionResponses)
            {
                if (qr.IsVisible)
                {
                    _repository.UpdateQuestionResponse(new Data.QuestionResponse()
                    {
                        Id = qr.Id,
                        QuestionId = qr.QuestionId,
                        Score = qr.Score,
                        ResponseText = qr.ResponseText,
                        SurveyResponseId = qr.SurveyResponseId
                    });
                }
                else
                {
                    _repository.DeleteQuestionResponse(qr.Id);
                }
            }
        }

        public static void ValidateVisibilityRules(Survey survey, IEnumerable<Data.QuestionVisibilityRule> rules, SurveyResponse surveyResponse)
        {
            foreach(var qr in surveyResponse.QuestionResponses)
            {
                qr.IsVisible = true;
            }
            foreach(var rule in rules)
            {
                var qr = surveyResponse.QuestionResponses.FirstOrDefault(c => c.QuestionId == rule.QuestionId);
                if (qr != null)
                {
                    switch (rule.Type)
                    {
                        case VisibilityType.Contains:
                            if (int.Parse(rule.Value) > int.Parse(qr.ResponseText))
                            {
                                qr.IsVisible = false;
                                RecursiveSetAllChildrenInvisible(survey, surveyResponse, qr.QuestionId);
                            }
                            break;
                        case VisibilityType.IntGreaterThan:
                            if (int.Parse(rule.Value) < int.Parse(qr.ResponseText))
                            {
                                qr.IsVisible = false;
                                RecursiveSetAllChildrenInvisible(survey, surveyResponse, qr.QuestionId);
                            }
                            break;
                        case VisibilityType.IntLessThan:
                            if (int.Parse(rule.Value) == int.Parse(qr.ResponseText))
                            {
                                qr.IsVisible = false;
                                RecursiveSetAllChildrenInvisible(survey, surveyResponse, qr.QuestionId);
                            }
                            break;
                        case VisibilityType.Equals:
                            if (qr.ResponseText != rule.Value)
                            {
                                qr.IsVisible = false;
                                RecursiveSetAllChildrenInvisible(survey, surveyResponse, qr.QuestionId);
                            }
                            break;
                        case VisibilityType.Custom:
                            // Add custom logic here based on rule.CustomType
                            // This could be pluggable for example - we could
                            // load up an assembly based on the value in rule.CustomType
                            break;
                    }
                }
            }

        }

        private static void RecursiveSetAllChildrenInvisible(Survey survey, SurveyResponse surveyResponse, int qid)
        {
            var immediateChildren = survey.AllQuestions.Where(c => c.ParentId == qid);
            if (immediateChildren.Any())
            {
                var ids = immediateChildren.Select(c => c.Id);
                var sr = surveyResponse.QuestionResponses.Where(c => ids.Contains(c.QuestionId));
                foreach (var r in sr)
                {
                    r.IsVisible = false;
                }
                foreach (var child in immediateChildren)
                {
                    RecursiveSetAllChildrenInvisible(survey, surveyResponse, child.Id);
                }
            }
        }

        public static void DeleteFromDatabase(ISurveyRepository _repository, int id)
        {
            _repository.DeleteSurveyResponse(id);
        }


    }
}
