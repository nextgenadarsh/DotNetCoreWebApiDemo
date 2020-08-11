using System;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApiDemo.Models
{
    public class StudentContext : DbContext
    {
        public StudentContext(DbContextOptions<StudentContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Student> Students { get; set; }

    }
}
