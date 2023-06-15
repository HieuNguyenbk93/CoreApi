﻿using CoreApi.Dto.AuthDto;
using CoreApi.Model;
using Microsoft.AspNetCore.Identity;

namespace CoreApi.Repositories.AuthRepo
{
    public interface IAuthRepository
    {
        Task<AuthResult<TokenDto>> Register(RegisterDto registerDto);
    }
}