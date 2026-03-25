using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoteAndQuizWebApi.Dto;
using VoteAndQuizWebApi.Services;
using VoteAndQuizWebApi.Services.Interfaces;

namespace VoteAndQuizWebApi.Controllers
{
    [Route("api/Quizzes")]
    [ApiController]
    [Authorize]
    public class QuizzesController : Controller
    {
        private readonly IQuizzesService _quizzesService;

        public QuizzesController(IQuizzesService quizzesService)
        {
            _quizzesService = quizzesService;
        }

        [HttpGet]
        public IActionResult Index() //Lists all quizzes on the main quiz page
        {
            var quizzes = _quizzesService.GetActiveQuizzes();
            return Json(quizzes);
        }

        [HttpGet("Details/{id}")]
        [ProducesResponseType(200)]
        public IActionResult Details(int? id) //You should be able to access Details page to vote for a quiz option.
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = _quizzesService.GetQuizDetails(id.Value);
            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            return Json(result.Data);
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] QuizForCreateMethodDTO quizDto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Log in to create a quiz");
            }

            var result = _quizzesService.CreateQuiz(quizDto, userId);
            return ToActionResult(result, "Successfully created");
        }

        [HttpDelete("Delete/{quizId}")]
        public IActionResult DeleteQuiz(int quizId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Log in to delete a quiz");
            }

            var result = _quizzesService.DeleteQuiz(quizId, userId);
            return ToActionResult(result, "Successfully deleted");
        }

        [HttpPut("{quizId}/vote/{answerId}")]
        public IActionResult VoteForQuiz(int quizId, int answerId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Log in to vote for a quiz");
            }

            var result = _quizzesService.VoteForQuiz(quizId, answerId, userId);
            return ToActionResult(result, "Successfully voted for the quiz answer.");
        }

        [HttpPost("Finish/{quizId}")]
        public IActionResult FinishQuiz(int quizId) //only creator can finish it
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Log in to finish this quiz");
            }

            var result = _quizzesService.FinishQuiz(quizId, userId);
            return ToActionResult(result, "Successfully finished quiz");
        }

        [HttpGet("CorrectOption/{quizId}")]
        public IActionResult GetCorrectOption(int quizId)
        {
            var result = _quizzesService.GetCorrectOption(quizId);
            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            return Ok(result.Data);
        }

        private IActionResult ToActionResult(ServiceResult result, string? successMessage = null)
        {
            if (result.Succeeded)
            {
                return Ok(successMessage ?? "Success");
            }

            return result.ErrorType switch
            {
                ServiceErrorType.Validation => BadRequest(result.ErrorMessage),
                ServiceErrorType.NotFound => NotFound(result.ErrorMessage),
                ServiceErrorType.Unauthorized => Unauthorized(result.ErrorMessage),
                ServiceErrorType.Conflict => StatusCode(422, result.ErrorMessage),
                _ => StatusCode(500, result.ErrorMessage)
            };
        }
    }
}