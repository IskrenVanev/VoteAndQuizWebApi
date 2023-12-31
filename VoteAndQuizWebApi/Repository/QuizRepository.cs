﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoteAndQuizWebApi.Data;
using VoteAndQuizWebApi.Dto;
using VoteAndQuizWebApi.Models;
using VoteAndQuizWebApi.Repository.IRepository;

namespace VoteAndQuizWebApi.Repository
{
    public class QuizRepository : Repository<Quiz>, IQuizRepository
    {
        private ApplicationDbContext _db;
        public QuizRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Quiz obj)
        {
            _db.Quizzes.Update(obj);
        }
        public bool CreateQuiz( Quiz quiz)
        {
            _db.Add(quiz);
            return Save();
        }
        public bool Save()
        {
            var saved = _db.SaveChanges();
            return saved > 0 ? true : false;
        }
        public bool DeleteQuiz(Quiz quiz)
        {
            quiz.IsDeleted = true;
            quiz.DeletedAt = DateTime.UtcNow.AddHours(3);
            quiz.ShowQuiz = false;
            quiz.IsActive = false;
            
            Update(quiz);
            return Save();
        }
        public bool QuizExists(int quizId)
        {
            return _db.Quizzes.Any(q => q.Id == quizId);
        }
    }
}
