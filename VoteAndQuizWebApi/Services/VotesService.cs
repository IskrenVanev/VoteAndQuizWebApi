using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VoteAndQuizWebApi.Dto;
using VoteAndQuizWebApi.Dto.VoteDtos;
using VoteAndQuizWebApi.Models;
using VoteAndQuizWebApi.Repository.IRepository;
using VoteAndQuizWebApi.Services.Interfaces;

namespace VoteAndQuizWebApi.Services
{
    public class VotesService : IVotesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IVoteRepository _voteRepository;

        public VotesService(
            IUnitOfWork unitOfWork,
            IVoteRepository voteRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _voteRepository = voteRepository;
            _mapper = mapper;
        }

        public List<VoteDTO> GetActiveVotes()
        {
            return _mapper.Map<List<VoteDTO>>(
                _unitOfWork.Vote.GetAll(v => v.IsActive).Include(v => v.Options));
        }

        public ServiceResult<VoteDTO> GetVoteDetails(int id)
        {
            var vote = _mapper.Map<VoteDTO>(_unitOfWork.Vote.Get(q => q.Id == id, "Options"));
            if (vote == null)
            {
                return ServiceResult<VoteDTO>.Fail(ServiceErrorType.NotFound, "Vote not found.");
            }

            if (vote.IsDeleted)
            {
                return ServiceResult<VoteDTO>.Fail(ServiceErrorType.Validation, "This vote has been deleted.");
            }

            return ServiceResult<VoteDTO>.Success(vote);
        }

        public ServiceResult CreateVote(VoteForCreateMethodDTO voteDto, string userId)
        {
            if (voteDto == null || voteDto.Options == null || voteDto.Options.Count < 2)
            {
                return ServiceResult.Fail(ServiceErrorType.Validation, "Vote must have at least 2 options.");
            }

            if (voteDto.VoteEndDate < DateTime.UtcNow.AddDays(1))
            {
                return ServiceResult.Fail(ServiceErrorType.Validation, "Vote end date must be at least one day from today.");
            }

            var user = _unitOfWork.User.Get(u => u.Id == userId, "UserVoteAnswers");
            if (user == null)
            {
                return ServiceResult.Fail(ServiceErrorType.Unauthorized, "Log in to create a vote");
            }

            var voteObj = _voteRepository.Get(v => v.Name.Trim().ToUpper() == voteDto.Name.TrimEnd().ToUpper());
            if (voteObj != null)
            {
                return ServiceResult.Fail(ServiceErrorType.Conflict, "Vote already exists");
            }

            var newVote = new Vote
            {
                Name = voteDto.Name,
                UpdatedAt = DateTime.UtcNow.AddHours(3),
                DeletedAt = null,
                CreatedAt = DateTime.UtcNow.AddHours(3),
                VoteEndDate = voteDto.VoteEndDate,
                Options = _mapper.Map<List<VoteOption>>(voteDto.Options),
                UserVoteAnswers = new List<UserVoteAnswer>(),
                voteVotes = 0,
                IsActive = true,
                IsDeleted = false,
                ShowVote = true,
                CreatorId = userId
            };

            if (!_voteRepository.CreateVote(newVote))
            {
                return ServiceResult.Fail(ServiceErrorType.Failure, "Something went wrong while saving");
            }

            return ServiceResult.Success();
        }

        public ServiceResult<List<VoteOptionDTO>> GetVoteResult(int id)
        {
            if (!_voteRepository.VoteExists(id))
            {
                return ServiceResult<List<VoteOptionDTO>>.Fail(ServiceErrorType.NotFound, "Vote not found.");
            }

            var result = _voteRepository.GetVoteResult(id);
            if (result == null || !result.Any())
            {
                return ServiceResult<List<VoteOptionDTO>>.Fail(ServiceErrorType.NotFound, "No results found for this vote.");
            }

            var vote = _unitOfWork.Vote.Get(v => v.Id == id);
            if (vote == null)
            {
                return ServiceResult<List<VoteOptionDTO>>.Fail(ServiceErrorType.NotFound, "Vote not found.");
            }

            if (vote.IsDeleted)
            {
                return ServiceResult<List<VoteOptionDTO>>.Fail(ServiceErrorType.Validation, "This vote has been deleted.");
            }

            var voteOptionDtos = result.Select(option => new VoteOptionDTO
            {
                voteCount = option.VoteCount,
                Option = option.Option,
            }).ToList();

            return ServiceResult<List<VoteOptionDTO>>.Success(voteOptionDtos);
        }

        public ServiceResult DeleteVote(int id, string userId)
        {
            if (!_voteRepository.VoteExists(id))
            {
                return ServiceResult.Fail(ServiceErrorType.NotFound, "Vote not found.");
            }

            var user = _unitOfWork.User.Get(u => u.Id == userId);
            if (user == null)
            {
                return ServiceResult.Fail(ServiceErrorType.Unauthorized, "Log in to delete a vote");
            }

            var voteFromDb = _unitOfWork.Vote.Get(u => u.Id == id);
            if (voteFromDb == null)
            {
                return ServiceResult.Fail(ServiceErrorType.NotFound, "Vote not found.");
            }

            if (voteFromDb.IsDeleted)
            {
                return ServiceResult.Fail(ServiceErrorType.Validation, "This vote is already deleted!");
            }

            if (voteFromDb.CreatorId != userId)
            {
                return ServiceResult.Fail(ServiceErrorType.Unauthorized, "You are not authorized to delete this vote.");
            }

            if (!_voteRepository.DeleteVote(voteFromDb))
            {
                return ServiceResult.Fail(ServiceErrorType.Failure, "Something went wrong deleting vote");
            }

            return ServiceResult.Success();
        }

        public ServiceResult FinishVote(int id, string userId)
        {
            var user = _unitOfWork.User.Get(u => u.Id == userId, "UserVoteAnswers");
            if (user == null)
            {
                return ServiceResult.Fail(ServiceErrorType.Unauthorized, "Log in to finish a vote");
            }

            var vote = _unitOfWork.Vote.Get(v => v.Id == id, "Options");
            if (vote == null)
            {
                return ServiceResult.Fail(ServiceErrorType.NotFound, "Vote not found.");
            }

            if (userId != vote.CreatorId)
            {
                return ServiceResult.Fail(ServiceErrorType.Validation, "You can't finish this vote because you are not the creator!");
            }

            var finished = _voteRepository.FinishVote(id);
            if (!finished)
            {
                return ServiceResult.Fail(ServiceErrorType.Failure, "Finishing the vote failed.");
            }

            if (vote.Options == null || !vote.Options.Any())
            {
                return ServiceResult.Fail(ServiceErrorType.Failure, "Finishing the vote failed.");
            }

            var maxVoteCount = vote.Options.Max(o => o.VoteCount);
            var winningOptions = vote.Options.Where(o => o.VoteCount == maxVoteCount);

            foreach (var currentUser in _unitOfWork.User.GetAll(null, "UserVoteAnswers").ToList())
            {
                var userVotedForWinningOption = currentUser.UserVoteAnswers != null &&
                    winningOptions.Any(wo =>
                        currentUser.UserVoteAnswers.Any(uva =>
                            wo.VoteId == uva.VoteId &&
                            wo.Option == uva.Option));

                if (userVotedForWinningOption)
                {
                    currentUser.Wins++;
                }
                else
                {
                    currentUser.Loses++;
                }

                _unitOfWork.User.Update(currentUser);
                _unitOfWork.Save();
            }

            return ServiceResult.Success();
        }

        public ServiceResult Vote(int id, int voteOptionId, string userId)
        {
            var user = _unitOfWork.User.Get(u => u.Id == userId, "UserVoteAnswers");
            if (user == null)
            {
                return ServiceResult.Fail(ServiceErrorType.Unauthorized, "Log in to vote");
            }

            var vote = _unitOfWork.Vote.Get(u => u.Id == id, "Options", true);
            if (vote == null)
            {
                return ServiceResult.Fail(ServiceErrorType.NotFound, "Vote not found.");
            }

            var voteOption = _unitOfWork.VoteOption.Get(vo => vo.Id == voteOptionId, null, true);
            if (voteOption == null)
            {
                return ServiceResult.Fail(ServiceErrorType.NotFound, "Vote option not found.");
            }

            if (voteOption.VoteId != vote.Id)
            {
                return ServiceResult.Fail(ServiceErrorType.Validation, "Vote option does not belong to this vote.");
            }

            var hasVoted = _unitOfWork.UserVoteAnswer.Get(uva => uva.VoteId == vote.Id && uva.UserId == userId);
            if (hasVoted != null)
            {
                return ServiceResult.Fail(ServiceErrorType.Validation, "You have already voted for this vote.");
            }

            vote.UpdatedAt = DateTime.UtcNow.AddHours(3);
            vote.voteVotes += 1;
            voteOption.VoteCount += 1;

            _unitOfWork.Vote.Modify(vote);
            _unitOfWork.VoteOption.Modify(voteOption);
            _unitOfWork.Vote.Save();
            _unitOfWork.VoteOption.Save();

            var userVoteAnswer = new UserVoteAnswer
            {
                Option = voteOption.Option,
                UserId = userId,
                VoteId = vote.Id
            };

            _unitOfWork.User.Save();
            _unitOfWork.UserVoteAnswer.Add(userVoteAnswer);
            _unitOfWork.UserVoteAnswer.Save();

            return ServiceResult.Success();
        }
    }
}
