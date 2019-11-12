using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace BankAccounts.Models
{
    public class User
    {
        [Key]
        public int UserId{get;set;}   
        [Required]
        public string FirstName{get;set;}
        [Required]
        public string LastName{get;set;}
        [Required]
        public string Email{get;set;}
        [Required]
        [DataType(DataType.Password)]
        public string Password{get;set;}
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
        public List<Transaction> TransactionsMade {get;set;}
    }
    

}