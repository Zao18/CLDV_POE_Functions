using Azure.Data.Tables;
using Azure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDV_POE_Functions.Models
{
    public class Customer : ITableEntity
    {
        [Key]

        public string? CustomerId { get; set; }
        public string? Customer_Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        // ITableEntity implementation
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
