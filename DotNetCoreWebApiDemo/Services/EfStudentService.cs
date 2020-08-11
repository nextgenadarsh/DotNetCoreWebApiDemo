using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreWebApiDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApiDemo.Services
{
    public class EfStudentService : IStudentService
    {
        private readonly StudentContext _context;

        public EfStudentService(StudentContext context)
        {
            _context = context;
        }

        public async Task<Student> CreateAsync(Student student)
        {
            student.Id = Guid.NewGuid().ToString();
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return false;
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<List<Student>> GetAsync()
        {
            return _context.Students.ToListAsync();
        }

        public async Task<Student> GetAsync(string studentId)
        {
            return await _context.Students.FindAsync(studentId);
        }

        public async Task<bool> UpdateAsync(Student updatedStudent)
        {
            _context.Entry(updatedStudent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(updatedStudent.Id))
                {
                    return false; // NotFound();
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        private bool StudentExists(string id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

    }
}
