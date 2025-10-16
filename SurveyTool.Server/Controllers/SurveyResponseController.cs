using Microsoft.AspNetCore.Mvc;
using SurveyTool.Data;

namespace SurveyTool.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SurveyResponseController : Controller
    {
        private readonly ILogger<SurveyController> _logger;
        private readonly ISurveyRepository _repository;

        public SurveyResponseController(ILogger<SurveyController> logger, ISurveyRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpPost]
        public int CreateSurveyResponse(SurveyTool.Model.SurveyResponse surveyResponse)
        {
            return surveyResponse.CreateInDatabase(_repository);
        }

        [HttpGet]
        public Model.SurveyResponse GetSurveyResponse(int id)
        {
            return Model.SurveyResponse.LoadFromDatabase(_repository, id);
        }

        [HttpPut]
        public void UpdateSurveyResponse(SurveyTool.Model.SurveyResponse surveyResponse)
        {
            Model.SurveyResponse.UpdateSurveyResponse(_repository, surveyResponse);
        }

        [HttpDelete]
        public void DeleteSurveyResponse(int id)
        {
            Model.SurveyResponse.DeleteFromDatabase(_repository, id);
        }
    }
}
