using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyTool.Data
{
    public class QuestionCondition
    {
        [Key]
        public int Id { get; set; }
        public int QuestionId { get; set; }

        //Allows for alternate parent questions(this condition can be on a question which is not a child question)
        public int ParentQuestionId { get; set; }
    }
}
