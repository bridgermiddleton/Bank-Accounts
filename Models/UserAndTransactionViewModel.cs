using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccounts.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAccounts.Models
{
    public class UserAndTransactionViewModel
    {
        public User TheUser {get;set;}
        public Transaction TheTransaction{get;set;}
        [Range(1, int.MaxValue, ErrorMessage="Not enough money!")]
        public double Balance{get;set;}
    }
    
}