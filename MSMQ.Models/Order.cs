//===============================================================================
// Microsoft Developer Advisory Services
// ASP.Net Microsoft Message Queueing Sample
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using System;
using System.ComponentModel.DataAnnotations;

namespace MSMQ.Models
{
    [Serializable]
    public class Order
    {
        public Guid OrderId { get; set; }

        [Display(Name = "Customer Name")]
        [Required]
        [MaxLength(50)]
        public string CustomerName { get; set; }

        [Display(Name = "Product Name")]
        [Required]
        [MaxLength(50)]
        public string ProductName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Display(Name = "Order Total")]
        [Range(0, int.MaxValue, ErrorMessage = "Order total must be greater than 0")]
        public decimal OrderTotal { get; set; }
    }
}
