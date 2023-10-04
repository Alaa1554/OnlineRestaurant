﻿using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IChefReviewService
    {
        Task<IEnumerable<ChefReviewView>> GetReviewsAsync(int id);
        Task<ChefReview> GetReviewByIdAsync(int id);
        Task<ChefReviewView> CreateReview(ChefReview comment);
        ChefReviewView UpdateReviewAsync(ChefReview comment, UpdateReviewDto dto);
        ChefReviewView DeleteReviewAsync(ChefReview comment);
    }
}
