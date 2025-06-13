using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BorrowViewModel
    {
        [Required]
        public int BookId { get; set; }

        [BindNever]
        public string? BookTitle { get; set; }

        [Required(ErrorMessage = "Your Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? BorrowerName { get; set; }

        [Required(ErrorMessage = "Your Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email address.")]
        public string? BorrowerEmail { get; set; }

        [Required(ErrorMessage = "Your Phone Number is required.")]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        public string? Phone {  get; set; }
    }
}
