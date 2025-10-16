using SurveyTool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyTool.Model
{
    public class QuestionVisibilityRule
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public VisibilityType Type { get; set; }
        public string Value { get; set; }
        public string CustomType { get; set; }
    }
}
