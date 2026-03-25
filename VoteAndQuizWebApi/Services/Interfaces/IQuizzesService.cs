using VoteAndQuizWebApi.Dto;
using VoteAndQuizWebApi.Models;

namespace VoteAndQuizWebApi.Services.Interfaces
{
    public interface IQuizzesService
    {
        List<QuizForIndexMethodDTO> GetActiveQuizzes();
        ServiceResult<QuizForIndexMethodDTO> GetQuizDetails(int id);
        ServiceResult CreateQuiz(QuizForCreateMethodDTO quizDto, string userId);
        ServiceResult DeleteQuiz(int quizId, string userId);
        ServiceResult VoteForQuiz(int quizId, int answerId, string userId);
        ServiceResult FinishQuiz(int quizId, string userId);
        ServiceResult<WinnerQuizOption> GetCorrectOption(int quizId);
    }
}
