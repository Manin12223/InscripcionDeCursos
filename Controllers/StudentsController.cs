using CourseEnrollmentAPI.Data;
using CourseEnrollmentAPI.DTOs;
using CourseEnrollmentAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var students = await _context.Students
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    IdentificationNumber = s.IdentificationNumber,
                    Email = s.Email,
                    Phone = s.Phone
                })
                .ToListAsync();

            return Ok(students);
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound($"Student with id {id} was not found.");
            }

            var studentDto = new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                IdentificationNumber = student.IdentificationNumber,
                Email = student.Email,
                Phone = student.Phone
            };

            return Ok(studentDto);
        }

        // POST: api/Students
        [HttpPost]
        public async Task<ActionResult<StudentDto>> PostStudent(StudentCreateDto studentCreateDto)
        {
            // Check that no student with the same identification number exists
            var exists = await _context.Students
                .AnyAsync(s => s.IdentificationNumber == studentCreateDto.IdentificationNumber);

            if (exists)
            {
                return BadRequest("A student with this identification number is already registered.");
            }

            var student = new Student
            {
                FirstName = studentCreateDto.FirstName,
                LastName = studentCreateDto.LastName,
                IdentificationNumber = studentCreateDto.IdentificationNumber,
                Email = studentCreateDto.Email,
                Phone = studentCreateDto.Phone
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var studentDto = new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                IdentificationNumber = student.IdentificationNumber,
                Email = student.Email,
                Phone = student.Phone
            };

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, studentDto);
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, StudentUpdateDto studentUpdateDto)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound($"Student with id {id} was not found.");
            }

            // Check for a duplicate identification number on another student
            var exists = await _context.Students
                .AnyAsync(s => s.IdentificationNumber == studentUpdateDto.IdentificationNumber && s.Id != id);

            if (exists)
            {
                return BadRequest("Another student with this identification number is already registered.");
            }

            student.FirstName = studentUpdateDto.FirstName;
            student.LastName = studentUpdateDto.LastName;
            student.IdentificationNumber = studentUpdateDto.IdentificationNumber;
            student.Email = studentUpdateDto.Email;
            student.Phone = studentUpdateDto.Phone;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound($"Student with id {id} was not found.");
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
