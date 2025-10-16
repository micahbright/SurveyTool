using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyTool.Data
{
    public class SurveyResponse
    {
        [Key]
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string Name { get; set; }
        public decimal Score { get; set; }

    }
}
