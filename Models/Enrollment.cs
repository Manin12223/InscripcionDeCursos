namespace CourseEnrollmentAPI.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public DateTime EnrollmentDate { get; set; }

        // Possible values: Pending, Paid, Cancelled
        public string Status { get; set; } = "Pending";

        public decimal AmountPaid { get; set; }
    }
}
