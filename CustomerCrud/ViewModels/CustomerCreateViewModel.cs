﻿using System.ComponentModel.DataAnnotations;

namespace CustomerCrud.ViewModels
{
    public class CustomerCreateViewModel
    {
        public int? CustomersId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerNo { get; set; }
        public string? CustomerAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public DateTime BusinessStart { get; set; } 

        public decimal CreditLimit { get; set; }

        public int CustomerTypeId { get; set; }
        public string? CustomerTypeName { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile CustomerPhoto { get; set; }
        public string? CustomerPhotoLink { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile CustomerSignature { get; set; }
        public byte[] CustomerSignatureShow { get; set; }
        public string? CustomerSignatureLink { get; set; }

        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
    }

    public class AddressViewModel
    {
        public int? Id { get; set; }
        public string AddressName { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
    }
}
