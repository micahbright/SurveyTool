using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SurveyTool.Data
{
    public interface ISurveyRepository
    {
        Task<Survey> GetSurveyByNameAsync(string name);

        void DeleteSurvey(string name);

        int AddSurveyResponse(SurveyResponse surveyResponse);
        void UpdateSurveyResponse(SurveyResponse surveyResponse);
        void DeleteSurveyResponse(int Id);
        int AddQuestionResponse(QuestionResponse questionResponse);
        void UpdateQuestionResponse(QuestionResponse qResponse);
        void DeleteQuestionResponse(int qrid);
        int AddSurvey(Survey survey);
        void UpdateSurvey(Survey survey);
        int AddQuestion(Question question);
        void UpdateQuestion(Question question);
        int AddQuestionOption(QuestionOption questionOption);
        void UpdateQuestionOption(QuestionOption qOption);

        Survey GetSurvey(string name);
        IEnumerable<Question> GetQuestions(int surveyId);
        IEnumerable<QuestionOption> GetQuestionOptions(int surveyId);

        Task SaveChangesAsync();
        SurveyResponse GetSurveyResponse(int id);
        IEnumerable<QuestionResponse> GetQuestionResponses(int surveyResponseId);
        IEnumerable<QuestionVisibilityRule> GetVisibilityRules(int surveyId);
        void UpdateVisibilityRule(QuestionVisibilityRule rule);
        int AddVisibilityRule(QuestionVisibilityRule rule);
        void DeleteVisibilityRule(QuestionVisibilityRule rule);
    }
}
