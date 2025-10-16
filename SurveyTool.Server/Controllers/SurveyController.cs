using Microsoft.AspNetCore.Mvc;
using SurveyTool.Data;
using SurveyTool.Model;

namespace SurveyTool.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SurveyController : Controller
    {
        private readonly ILogger<SurveyController> _logger;
        private readonly ISurveyRepository _repository;
        public SurveyController(ILogger<SurveyController> logger, ISurveyRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpPost]
        public void CreateSurvey(SurveyTool.Model.Survey survey)
        {
            survey.CreateInDatabase(_repository);
        }

        [HttpGet]
        public Model.Survey GetSurvey(string name)
        {
            return Model.Survey.LoadFromDatabase(_repository, name);
        }

        [HttpPut]
        public void UpdateSurvey(SurveyTool.Model.Survey survey)
        {
            Model.Survey.UpdateSurvey(_repository, survey);
        }

        [HttpDelete]
        public void DeleteSurvey(string name)
        {
            Model.Survey.DeleteFromDatabase(_repository, name);
        }
    }
}
