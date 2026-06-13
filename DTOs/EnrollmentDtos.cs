namespace CourseEnrollmentAPI.DTOs
{
    // DTO used to return enrollment information
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
    }

    // DTO used to create an enrollment
    public class EnrollmentCreateDto
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public decimal AmountPaid { get; set; }
    }

    // DTO used to update an enrollment (status and amount paid)
    public class EnrollmentUpdateDto
    {
        public string Status { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
    }
}
