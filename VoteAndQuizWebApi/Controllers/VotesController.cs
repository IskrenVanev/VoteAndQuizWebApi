using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoteAndQuizWebApi.Dto.VoteDtos;
using VoteAndQuizWebApi.Services;
using VoteAndQuizWebApi.Services.Interfaces;

namespace VoteAndQuizWebApi.Controllers
{
    [Route("api/Votes")]
    [ApiController]
    [Authorize]
    public class VotesController : Controller
    {
        private readonly IVotesService _votesService;

        public VotesController(IVotesService votesService)
        {
            _votesService = votesService;
        }

        [HttpGet]
        public IActionResult Index() //Lists all votes on the main vote page
        {
            var votes = _votesService.GetActiveVotes();
            return Json(votes);
        }

        [HttpGet("Details/{id}")]
        [ProducesResponseType(200)]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = _votesService.GetVoteDetails(id.Value);
            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            return Json(result.Data);
        }

        [HttpPost("Create")]
        [Authorize]
        public IActionResult Create([FromBody] VoteForCreateMethodDTO vote)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Log in to create a vote");
            }

            var result = _votesService.CreateVote(vote, userId);
            return ToActionResult(result, "Successfully created");
        }

        [HttpGet("Result/{id}")]
        public IActionResult GetVoteResult(int? id)
        {
            if (id == null)
            {
                return BadRequest("Vote ID cannot be null.");
            }

            var result = _votesService.GetVoteResult(id.Value);
            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            return Ok(result.Data);
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Log in to delete a vote");
            }

            var result = _votesService.DeleteVote(id.Value, userId);
            return ToActionResult(result, "Successfully deleted");
        }

        
        [HttpPost("Finish/{id}")]
        [Authorize]
        public IActionResult Finish(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Log in to finish a vote");
            }

            var result = _votesService.FinishVote(id.Value, userId);
            return ToActionResult(result, "Successfully finished the vote!");
        }

        [HttpPost("Vote/{id}/{voteOptionId}")]
        [Authorize]
        public IActionResult Vote(int? id, int voteOptionId)
        {
            if (id == null)
            {
                return BadRequest();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Log in to vote");
            }

            var result = _votesService.Vote(id.Value, voteOptionId, userId);
            return ToActionResult(result, "Successfully voted");
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
