using Rent.Application.DTOs;
using Rent.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rent.Application.Services
{


    
        public interface IAuthService
        {
            Task<bool> ValidateUserAsync(string username, string password);
            Task<bool> RegisterUserAsync(RegisterDto registerDto);
            Task<bool> RegisterAdminAsync(RegisterDto registerDto);
            Task<User> GetById(Guid id);
            Task<User> GetByUsername(string username);
            Task<Result<User>> LoginUser(LoginUserDto loginUserDto);
            Task<bool> RegisterAdminAsyncIData(RegisterDto registerDto); 
            Task<Result<User>> LoginIData(LoginUserDto loginUserDto); 
        }


    }









