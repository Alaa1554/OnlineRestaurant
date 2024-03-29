﻿using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IChefReviewService
    {
        IEnumerable<ChefReviewView> GetReviews(int id, PaginateDto dto);
        Task<ChefReview> GetReviewByIdAsync(int id);
        Task<ChefReviewView> CreateReview(string token,ChefReview comment);
        ChefReviewView UpdateReviewAsync(ChefReview comment, UpdateReviewDto dto);
        ChefReviewView DeleteReviewAsync(ChefReview comment);
    }
}
