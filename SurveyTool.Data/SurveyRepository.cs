using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SurveyTool.Data
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly SurveyContext _context;

        public SurveyRepository(SurveyContext context)
            => _context = context;

        public int AddSurveyResponse(SurveyResponse surveyResponse)
        {
            _context.SurveyResponses.Add(surveyResponse);
            _context.SaveChanges();
            return surveyResponse.Id;
        }

        public int AddQuestionResponse(QuestionResponse questionResponse)
        {
            _context.QuestionResponses.Add(questionResponse);
            _context.SaveChanges();
            return questionResponse.Id;
        }

        public void UpdateSurveyResponse(SurveyResponse surveyResponse)
        {
            var sr = _context.SurveyResponses.Single(c=>c.Id==surveyResponse.Id);
            sr.SurveyId = surveyResponse.SurveyId;
            sr.Name = surveyResponse.Name;
            sr.Score = surveyResponse.Score;
            _context.SaveChanges();
        }

        public void UpdateQuestionResponse(QuestionResponse qResponse)
        {
            var qr = _context.QuestionResponses.Single(c=>c.Id == qResponse.Id);
            qr.QuestionId = qResponse.QuestionId;
            qr.Score = qResponse.Score;
            qr.ResponseText = qResponse.ResponseText;
            qr.SurveyResponseId = qResponse.SurveyResponseId;
            _context.SaveChanges();
        }

        public void DeleteQuestionResponse(int qrid)
        {
            _context.QuestionResponses.Remove(_context.QuestionResponses.First(c => c.Id == qrid));
            _context.SaveChanges();
        }

        public void DeleteSurveyResponse(int Id)
        {
            var surveyResponse = _context.SurveyResponses.First(c => c.Id == Id);
            var questionResponses = _context.QuestionResponses.Where(c => c.SurveyResponseId == Id);
            _context.RemoveRange(questionResponses);
            _context.Remove(surveyResponse);
            _context.SaveChanges();
        }

        public void DeleteSurvey(string name)
        {
            var survey = _context.Surveys.First(s => s.Name == name);
            var questions = _context.Questions.Where(q => q.SurveyId == survey.Id);
            var surveyResponses = _context.SurveyResponses.Where(c=>c.SurveyId == survey.Id);
            var questionResponses = _context.QuestionResponses.Where(c => surveyResponses.Select(d => d.Id).Contains(c.SurveyResponseId));
            _context.RemoveRange(questionResponses);
            _context.Remove(surveyResponses);
            _context.RemoveRange(_context.QuestionOptions.Where(qo => questions.Select(q => q.Id).Contains(qo.QuestionId)));
            _context.RemoveRange(questions);
            _context.Surveys.Remove(survey);
            _context.SaveChanges();
        }

        public int AddSurvey(Survey survey)
        {
            _context.Surveys.Add(survey);
            _context.SaveChanges();
            return survey.Id;
        }

        public void UpdateSurvey(Survey survey)
        {
            var s = _context.Surveys.Single(c=>c.Id==survey.Id);
            s.Name = survey.Name;
            _context.SaveChanges();
        }

        public int AddQuestion(Question question)
        {
            _context.Questions.Add(question);
            _context.SaveChanges();
            return question.Id;
        }

        public void UpdateQuestion(Question question)
        {
            var q = _context.Questions.Single(c=>c.Id==question.Id);
            q.Text = question.Text;
            q.PercentWeight = question.PercentWeight;
            q.ParentId = question.ParentId;
            q.SurveyId = question.SurveyId;
            _context.SaveChanges();
        }

        public int AddQuestionOption(QuestionOption questionOption)
        {
            _context.QuestionOptions.Add(questionOption);
            _context.SaveChanges();
            return questionOption.Id;
        }

        public void UpdateQuestionOption(QuestionOption qOption)
        {
            var qo = _context.QuestionOptions.Single(c=>c.Id == qOption.Id);
            qo.QuestionId = qOption.QuestionId;
            qo.Text = qOption.Text;
            _context.SaveChanges();
        }

        public async Task<Survey> GetSurveyByNameAsync(string name)
            => await _context.Surveys.FirstOrDefaultAsync(b => b.Name == name);

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        Survey ISurveyRepository.GetSurvey(string name)
        {
            return _context.Surveys.FirstOrDefault(c => c.Name == name);
        }

        IEnumerable<Question> ISurveyRepository.GetQuestions(int surveyId)
        {
            return _context.Questions.Where(c=>c.SurveyId == surveyId);
        }

        IEnumerable<QuestionOption> ISurveyRepository.GetQuestionOptions(int surveyId)
        {
            return _context.QuestionOptions.Where(c => _context.Questions.Where(d => d.SurveyId == surveyId).Select(d => d.Id).Contains(c.QuestionId));
        }

        public SurveyResponse GetSurveyResponse(int id)
        {
            return _context.SurveyResponses.Single(c => c.Id == id);
        }

        public IEnumerable<QuestionResponse> GetQuestionResponses(int surveyResponseId)
        {
            return _context.QuestionResponses.Where(c=>c.SurveyResponseId == surveyResponseId);
        }
        public IEnumerable<QuestionVisibilityRule> GetVisibilityRules(int surveyId)
        {
            return _context.QuestionVisibilityRules
                .Where(q => _context.Questions
                    .Where(c => c.SurveyId == surveyId)
                    .Select(c => c.Id)
                    .Contains(q.QuestionId)).ToArray();
        }
        public void UpdateVisibilityRule(QuestionVisibilityRule rule)
        {
            var dbRule = _context.QuestionVisibilityRules.Single(c=>c.Id== rule.Id);
            dbRule.QuestionId = rule.QuestionId;
            dbRule.Value = rule.Value;
            dbRule.CustomType = rule.CustomType;
            dbRule.Type = rule.Type;
            _context.SaveChanges();
        }

        public int AddVisibilityRule(QuestionVisibilityRule rule)
        {
            _context.QuestionVisibilityRules.Add(rule);
            _context.SaveChanges();
            return rule.Id;
        }
        public void DeleteVisibilityRule(QuestionVisibilityRule rule)
        {
            _context.QuestionVisibilityRules.Remove(rule);
            _context.SaveChanges();
        }
    }
}
