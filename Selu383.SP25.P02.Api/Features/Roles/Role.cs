﻿using Microsoft.AspNetCore.Identity;
using Selu383.SP25.P02.Api.Features.UserRoles;

namespace Selu383.SP25.P02.Api.Features.Theaters.Roles
{
    public class Role : IdentityRole<int>
    {
        public virtual ICollection<UserRole> Users { get; set; } = new List<UserRole>();
    }
}