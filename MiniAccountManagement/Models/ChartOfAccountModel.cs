using System.ComponentModel.DataAnnotations;
namespace MiniAccountManagement.Models
{
    public class ChartOfAccountModel
    {
        public int AccountID { get; set; }
        [Required(ErrorMessage = "Account name is required.")]
        [StringLength(255)]
        public string AccountName { get; set; }
        [StringLength(50)]
        public string? AccountCode { get; set; }
        public int? ParentAccountID { get; set; }
    }
}