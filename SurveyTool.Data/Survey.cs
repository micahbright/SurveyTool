using System.ComponentModel.DataAnnotations;

namespace SurveyTool.Data
{
    public class Survey
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
