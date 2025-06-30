using System;
namespace MiniAccountManagement.Models
{
    public class VoucherListViewModel
    {
        public long VoucherMasterID { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherType { get; set; }
        public string ReferenceNo { get; set; }
        public string Narration { get; set; }
        public decimal TotalAmount { get; set; }
    }
}