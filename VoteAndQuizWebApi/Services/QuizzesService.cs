using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VoteAndQuizWebApi.Dto;
using VoteAndQuizWebApi.Models;
using VoteAndQuizWebApi.Repository.IRepository;
using VoteAndQuizWebApi.Services.Interfaces;

namespace VoteAndQuizWebApi.Services
{
    public class QuizzesService : IQuizzesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IQuizRepository _quizRepository;

        public QuizzesService(IUnitOfWork unitOfWork, IQuizRepository quizRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _quizRepository = quizRepository;
            _mapper = mapper;
        }

        public List<QuizForIndexMethodDTO> GetActiveQuizzes()
        {
            return _mapper.Map<List<QuizForIndexMethodDTO>>(
                _unitOfWork.Quiz.GetAll(q => q.IsActive).Include(q => q.Options));
        }

        public ServiceResult<QuizForIndexMethodDTO> GetQuizDetails(int id)
        {
            var quiz = _mapper.Map<QuizForIndexMethodDTO>(_unitOfWork.Quiz.Get(q => q.Id == id, "Options"));

            if (quiz == null)
            {
                return ServiceResult<QuizForIndexMethodDTO>.Fail(ServiceErrorType.NotFound, "Quiz not found.");
            }

            if (quiz.IsDeleted)
            {
                return ServiceResult<QuizForIndexMethodDTO>.Fail(ServiceErrorType.Validation, "This Quiz no longer exists");
            }

            return ServiceResult<QuizForIndexMethodDTO>.Success(quiz);
        }

        public ServiceResult CreateQuiz(QuizForCreateMethodDTO quizDto, string userId)
        {
            if (quizDto == null || quizDto.Options == null || quizDto.Options.Count < 2)
            {
                return ServiceResult.Fail(ServiceErrorType.Validation, "Quiz must have at least 2 options.");
            }

            var user = _unitOfWork.User.Get(u => u.Id == userId);
            if (user == null)
            {
                return ServiceResult.Fail(ServiceErrorType.Unauthorized, "Log in to create a quiz");
            }

            if (quizDto.QuizEndDate < DateTime.UtcNow.AddDays(1))
            {
                return ServiceResult.Fail(ServiceErrorType.Validation, "Quiz end date must be at least one day from today.");
            }

            var newQuiz = new Quiz
            {
                Name = quizDto.Name,
                UpdatedAt = DateTime.UtcNow.AddHours(3),
                DeletedAt = null,
                CreatedAt = DateTime.UtcNow.AddHours(3),
                CreatorId = userId,
                QuizEndDate = quizDto.QuizEndDate == DateTime.MinValue ? DateTime.UtcNow.AddDays(14) : quizDto.QuizEndDate,
                quizVotes = quizDto.quizVotes,
                Options = _mapper.Map<List<UserQuizAnswer>>(quizDto.Options),
                CorrectOption = _mapper.Map<WinnerQuizOption>(quizDto.CorrectOption),
                IsActive = true,
                IsDeleted = false,
                ShowQuiz = true,
            };

            var quizObj = _quizRepository.Get(q => q.Name.Trim().ToUpper() == quizDto.Name.TrimEnd().ToUpper());
            if (quizObj != null)
            {
                return ServiceResult.Fail(ServiceErrorType.Conflict, "Quiz already exists");
            }

            if (!_quizRepository.CreateQuiz(newQuiz))
            {
                return ServiceResult.Fail(ServiceErrorType.Failure, "Something went wrong while saving");
            }

            return ServiceResult.Success();
        }

        public ServiceResult DeleteQuiz(int quizId, string userId)
        {
            if (!_quizRepository.QuizExists(quizId))
            {
                return ServiceResult.Fail(ServiceErrorType.NotFound, "Quiz not found.");
            }

            var quizToDelete = _unitOfWork.Quiz.Get(q => q.Id == quizId);
            if (quizToDelete == null)
            {
                return ServiceResult.Fail(ServiceErrorType.NotFound, "Quiz not found.");
            }

            var user = _unitOfWork.User.Get(u => u.Id == userId);
            if (user == null)
            {
                return ServiceResult.Fail(ServiceErrorType.Unauthorized, "Log in to delete a quiz");
            }

            if (quizToDelete.IsDeleted)
            {
                return ServiceResult.Fail(ServiceErrorType.Validation, "This quiz is already deleted!");
            }

            if (quizToDelete.CreatorId != userId)
            {
                return ServiceResult.Fail(ServiceErrorType.Unauthorized, "You are not authorized to delete this quiz.");
            }

            if (!_quizRepository.DeleteQuiz(quizToDelete))
            {
                return ServiceResult.Fail(ServiceErrorType.Failure, "Something went wrong deleting quiz");
            }

            return ServiceResult.Success();
        }

        public ServiceResult VoteForQuiz(int quizId, int answerId, string userId)
        {
            var user = _unitOfWork.User.Get(u => u.Id == userId);
            if (user == null)
            {
                return ServiceResult.Fail(ServiceErrorType.Unauthorized, "Log in to vote for a quiz");
            }

            var quiz = _unitOfWork.Quiz.Get(q => q.Id == quizId);
            if (quiz == null || quiz.IsDeleted)
            {
                return ServiceResult.Fail(ServiceErrorType.NotFound, "Quiz not found or has been deleted.");
            }

            if (!quiz.IsActive || quiz.QuizEndDate < DateTime.UtcNow.AddHours(3))
            {
                return ServiceResult.Fail(ServiceErrorType.Validation, "Voting is closed for this quiz.");
            }

            var answer = _unitOfWork.UserQuizAnswer.Get(uqa => uqa.Id == answerId);
            if (answer == null)
            {
                return ServiceResult.Fail(ServiceErrorType.NotFound, "Quiz answer not found.");
            }

            answer.quizAnswerVotes += 1;
            quiz.quizVotes += 1;
            quiz.UpdatedAt = DateTime.UtcNow.AddHours(3);

            _unitOfWork.UserQuizAnswer.Update(answer);
            _unitOfWork.Quiz.Update(quiz);
            _unitOfWork.Save();

            return ServiceResult.Success();
        }

        public ServiceResult FinishQuiz(int quizId, string userId)
        {
            var user = _unitOfWork.User.Get(u => u.Id == userId);
            if (user == null)
            {
                return ServiceResult.Fail(ServiceErrorType.Unauthorized, "Log in to finish this quiz");
            }

            var quiz = _unitOfWork.Quiz.Get(q => q.Id == quizId);
            if (quiz == null)
            {
                return ServiceResult.Fail(ServiceErrorType.NotFound, "Quiz not found.");
            }

            if (quiz.CreatorId != userId)
            {
                return ServiceResult.Fail(ServiceErrorType.Unauthorized, "You are not authorized to finish this quiz.");
            }

            quiz.QuizEndDate = DateTime.UtcNow.AddHours(3);
            quiz.IsActive = false;
            quiz.UpdatedAt = DateTime.UtcNow.AddHours(3);

            _unitOfWork.Quiz.Update(quiz);
            _unitOfWork.Save();

            return ServiceResult.Success();
        }

        public ServiceResult<WinnerQuizOption> GetCorrectOption(int quizId)
        {
            var quiz = _unitOfWork.Quiz.Get(q => q.Id == quizId);
            if (quiz == null || quiz.IsDeleted)
            {
                return ServiceResult<WinnerQuizOption>.Fail(ServiceErrorType.NotFound, "Quiz not found or has been deleted.");
            }

            if (quiz.IsActive)
            {
                return ServiceResult<WinnerQuizOption>.Fail(ServiceErrorType.Validation, "Quiz is still active. The correct option is not available yet.");
            }

            var correctOption = _unitOfWork.WinnerQuizOption.Get(o => o.QuizId == quiz.Id);
            if (correctOption == null)
            {
                return ServiceResult<WinnerQuizOption>.Fail(ServiceErrorType.NotFound, "Correct option not found.");
            }

            return ServiceResult<WinnerQuizOption>.Success(correctOption);
        }
    }
}
