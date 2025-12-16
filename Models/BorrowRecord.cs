using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryBookTracking.Models
{
    public class BorrowRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string BorrowerName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? BorrowerEmail { get; set; }

        [StringLength(20)]
        public string? BorrowerPhone { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        public DateTime? ReturnDate { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsReturned { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}

