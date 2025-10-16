using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyTool.Model
{
    public class QuestionResponse
    {
        public int Id { get; set; }
        public int SurveyResponseId {  get; set; }
        public int QuestionId { get; set; }
        public string ResponseText { get; set; }
        public decimal Score { get; set; }
        public bool IsVisible { get; set; }
    }
}
