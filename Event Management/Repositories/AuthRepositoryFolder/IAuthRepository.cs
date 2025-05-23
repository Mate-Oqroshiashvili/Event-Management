﻿using Event_Management.Models.Dtos.LoginDtos;
using Event_Management.Models.Dtos.UserDtos;

namespace Event_Management.Repositories.AuthRepositoryFolder
{
    public interface IAuthRepository
    {
        Task<UserDto> Registration(UserCreateDto registerUserDto); // Registers a new user
        Task<string> Authorization(LoginDto logInDto); // Authorizes a user
    }
}
