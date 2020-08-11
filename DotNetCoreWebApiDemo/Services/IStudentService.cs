using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreWebApiDemo.Models;

namespace DotNetCoreWebApiDemo.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAsync();

        Task<Student> GetAsync(string studentId);

        Task<Student> CreateAsync(Student student);

        Task<bool> UpdateAsync(Student updatedStudent);

        Task<bool> DeleteAsync(string id);
    }
}
