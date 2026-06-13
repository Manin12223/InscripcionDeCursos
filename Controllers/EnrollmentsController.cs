using CourseEnrollmentAPI.Data;
using CourseEnrollmentAPI.DTOs;
using CourseEnrollmentAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Enrollments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollments()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student!.FirstName + " " + e.Student!.LastName,
                    CourseId = e.CourseId,
                    CourseName = e.Course!.Name,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    AmountPaid = e.AmountPaid
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        // GET: api/Enrollments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentDto>> GetEnrollment(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
            {
                return NotFound($"Enrollment with id {id} was not found.");
            }

            var enrollmentDto = new EnrollmentDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentName = enrollment.Student!.FirstName + " " + enrollment.Student!.LastName,
                CourseId = enrollment.CourseId,
                CourseName = enrollment.Course!.Name,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                AmountPaid = enrollment.AmountPaid
            };

            return Ok(enrollmentDto);
        }

        // POST: api/Enrollments
        [HttpPost]
        public async Task<ActionResult<EnrollmentDto>> PostEnrollment(EnrollmentCreateDto enrollmentCreateDto)
        {
            var course = await _context.Courses.FindAsync(enrollmentCreateDto.CourseId);
            if (course == null)
            {
                return NotFound($"Course with id {enrollmentCreateDto.CourseId} was not found.");
            }

            var student = await _context.Students.FindAsync(enrollmentCreateDto.StudentId);
            if (student == null)
            {
                return NotFound($"Student with id {enrollmentCreateDto.StudentId} was not found.");
            }

            // Validate available seats
            if (course.AvailableSeats <= 0)
            {
                return BadRequest("There are no available seats for this course.");
            }

            var enrollment = new Enrollment
            {
                StudentId = enrollmentCreateDto.StudentId,
                CourseId = enrollmentCreateDto.CourseId,
                EnrollmentDate = DateTime.Now,
                Status = "Pending",
                AmountPaid = enrollmentCreateDto.AmountPaid
            };

            // Decrease the course's available seats
            course.AvailableSeats -= 1;

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var enrollmentDto = new EnrollmentDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentName = student.FirstName + " " + student.LastName,
                CourseId = enrollment.CourseId,
                CourseName = course.Name,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                AmountPaid = enrollment.AmountPaid
            };

            return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, enrollmentDto);
        }

        // PUT: api/Enrollments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnrollment(int id, EnrollmentUpdateDto enrollmentUpdateDto)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
            {
                return NotFound($"Enrollment with id {id} was not found.");
            }

            var validStatuses = new[] { "Pending", "Paid", "Cancelled" };
            if (!validStatuses.Contains(enrollmentUpdateDto.Status))
            {
                return BadRequest("Status must be 'Pending', 'Paid' or 'Cancelled'.");
            }

            // If the enrollment is being cancelled and it wasn't cancelled before, release the seat
            if (enrollmentUpdateDto.Status == "Cancelled" && enrollment.Status != "Cancelled")
            {
                enrollment.Course!.AvailableSeats += 1;
            }

            // If a previously cancelled enrollment is being reactivated, validate and take a seat again
            if (enrollment.Status == "Cancelled" && enrollmentUpdateDto.Status != "Cancelled")
            {
                if (enrollment.Course!.AvailableSeats <= 0)
                {
                    return BadRequest("There are no available seats to reactivate this enrollment.");
                }

                enrollment.Course.AvailableSeats -= 1;
            }

            enrollment.Status = enrollmentUpdateDto.Status;
            enrollment.AmountPaid = enrollmentUpdateDto.AmountPaid;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Enrollments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
            {
                return NotFound($"Enrollment with id {id} was not found.");
            }

            // If the enrollment wasn't cancelled, release the course's seat
            if (enrollment.Status != "Cancelled")
            {
                enrollment.Course!.AvailableSeats += 1;
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
