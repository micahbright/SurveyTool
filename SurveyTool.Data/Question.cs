using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyTool.Data
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int?ParentId { get; set; }
        public QuestionType Type { get; set; }
        public string Text { get; set; }
        public decimal PercentWeight { get; set; }

    }
}
