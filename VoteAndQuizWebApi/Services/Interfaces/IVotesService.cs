using VoteAndQuizWebApi.Dto;
using VoteAndQuizWebApi.Dto.VoteDtos;

namespace VoteAndQuizWebApi.Services.Interfaces
{
    public interface IVotesService
    {
        List<VoteDTO> GetActiveVotes();
        ServiceResult<VoteDTO> GetVoteDetails(int id);
        ServiceResult CreateVote(VoteForCreateMethodDTO voteDto, string userId);
        ServiceResult<List<VoteOptionDTO>> GetVoteResult(int id);
        ServiceResult DeleteVote(int id, string userId);
        ServiceResult FinishVote(int id, string userId);
        ServiceResult Vote(int id, int voteOptionId, string userId);
    }
}
