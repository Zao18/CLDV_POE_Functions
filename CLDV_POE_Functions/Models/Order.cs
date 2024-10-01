using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDV_POE_Functions.Models
{
    using Azure;
    using Azure.Data.Tables;
    using System.ComponentModel.DataAnnotations;

    namespace CLDV_POE.Models
    {
        public class Order : ITableEntity
        {
            [Key]
            public string? Order_Id { get; set; }
            public string? PartitionKey { get; set; }
            public string? RowKey { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }
            [Required(ErrorMessage = "Please select a customer")]
            public string Customer_ID { get; set; }
            [Required(ErrorMessage = "Please select a product")]
            public string Product_ID { get; set; }
            [Required(ErrorMessage = "Please select the date")]
            public DateTime Order_Date { get; set; }
        }
    }
}
