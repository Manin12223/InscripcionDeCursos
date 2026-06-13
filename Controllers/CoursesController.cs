using CourseEnrollmentAPI.Data;
using CourseEnrollmentAPI.DTOs;
using CourseEnrollmentAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courses = await _context.Courses
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    TotalSeats = c.TotalSeats,
                    AvailableSeats = c.AvailableSeats
                })
                .ToListAsync();

            return Ok(courses);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound($"Course with id {id} was not found.");
            }

            var courseDto = new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                TotalSeats = course.TotalSeats,
                AvailableSeats = course.AvailableSeats
            };

            return Ok(courseDto);
        }

        // POST: api/Courses
        [HttpPost]
        public async Task<ActionResult<CourseDto>> PostCourse(CourseCreateDto courseCreateDto)
        {
            var course = new Course
            {
                Name = courseCreateDto.Name,
                Description = courseCreateDto.Description,
                TotalSeats = courseCreateDto.TotalSeats,
                AvailableSeats = courseCreateDto.TotalSeats // when created, available = total
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var courseDto = new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                TotalSeats = course.TotalSeats,
                AvailableSeats = course.AvailableSeats
            };

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, courseDto);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, CourseUpdateDto courseUpdateDto)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound($"Course with id {id} was not found.");
            }

            if (courseUpdateDto.AvailableSeats < 0)
            {
                return BadRequest("Available seats cannot be negative.");
            }

            course.Name = courseUpdateDto.Name;
            course.Description = courseUpdateDto.Description;
            course.TotalSeats = courseUpdateDto.TotalSeats;
            course.AvailableSeats = courseUpdateDto.AvailableSeats;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound($"Course with id {id} was not found.");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
