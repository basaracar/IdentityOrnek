using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityOrnek.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityOrnek.Data
{
    public class AppdbContext : IdentityDbContext<AppUser>
    {
        public AppdbContext(DbContextOptions<AppdbContext> options) : base(options)
        {
        }

       
    }
}