using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetCoreWebApiDemo.Models;
using DotNetCoreWebApiDemo.Dtos;
using Microsoft.AspNetCore.Authorization;
using DotNetCoreWebApiDemo.Services;

namespace DotNetCoreWebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private IStudentService studentService;
        

        public StudentsController(IStudentService studentService)
        {
            this.studentService = studentService;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var students = await studentService.GetAsync();
            return students.Select(x => ToDto(x)).ToList();

            //_context.Students.Select(x => ToDto(x)).ToListAsync();
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(string id)
        {
            var student = await studentService.GetAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudentAsync(string id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            var status = await studentService.UpdateAsync(student);
            if(!status)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Students
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            await studentService.CreateAsync(student);
            
            return CreatedAtAction("GetStudent", new { id = student.Id }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> DeleteStudent(string id)
        {
            var status = await studentService.DeleteAsync(id);

            if(!status)
            {
                return NotFound();
            }

            return NoContent();
        }

        private static StudentDto ToDto(Student student) =>
            new StudentDto { Id = student.Id, Name = student.Name };
    }
}
