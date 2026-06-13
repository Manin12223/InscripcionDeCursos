namespace CourseEnrollmentAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }

        // Relationship: a course can have many enrollments
        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
