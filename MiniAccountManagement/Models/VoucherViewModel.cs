using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniAccountManagement.Models
{
    public class VoucherViewModel
    {
        [Required]
        public DateTime VoucherDate { get; set; } = DateTime.Today;

        [Required]
        public string VoucherType { get; set; }

        public string ReferenceNo { get; set; }

        [StringLength(500)]
        public string Narration { get; set; }

        public List<VoucherDetailViewModel> Details { get; set; }

        public VoucherViewModel()
        {
            Details = new List<VoucherDetailViewModel>();
        }
    }
}