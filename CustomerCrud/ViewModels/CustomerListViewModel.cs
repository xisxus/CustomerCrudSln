﻿using System.ComponentModel.DataAnnotations;

namespace CustomerCrud.ViewModels
{
    public class CustomerListViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerTypeName { get; set; }
        public DateTime? BusinessStart { get; set; }
        public string CustomerAddress { get; set; }
        public decimal? CreditLimit { get; set; }
        public int AdditionalAddressesCount { get; set; }
        public bool IsSelected { get; set; }

        [DataType(DataType.Upload)]
        public string CustomerPhotoLink { get; set; }

        [DataType(DataType.Upload)]
        public byte[] CustomerSignature { get; set; }

        public string CustomerSignatureUrl { get; set; }



        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
    }

    public class CustomerIndexViewModel
    {
        public IEnumerable<CustomerListViewModel> Customers { get; set; }
        public int TotalCustomers { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public string SelectedIds { get; set; }

    }


}
