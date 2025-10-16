using SurveyTool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyTool.Model
{
    public class Question
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public QuestionType Type { get; set; }
        public string Text { get; set; }
        public decimal PercentWeight { get; set; }
        public List<QuestionOption> QuestionOptions { get; set; }
        public List<Question> ChildQuestions { get; set; }
        public List<QuestionVisibilityRule> VisibilityRules { get; set; }
    }
}
