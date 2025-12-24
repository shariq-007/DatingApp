using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Extensions;

public static class AppUserExtensions
{
    public static UserDto ToDto(this AppUser user, ITokenServices tokenServices)
    {
        return new UserDto
        {
            Id = user.Id,
            DisplayName=user.DisplayName,
            Email=user.Email,
            Token=tokenServices.CreateToken(user)
        };
    }

}
