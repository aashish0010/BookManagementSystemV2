﻿using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Domain.Entities;

namespace BookManagementSystem.Service.Interfaces
{
    public interface IUserManagementService
    {
        Task<Common> Register(RegisterRequest register);
        Task<LoginResponse> Login(LoginRequest login);
    }
}