using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class Account
    {
        public Account()
        {
            AccountExams = new HashSet<AccountExam>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<AccountExam> AccountExams { get; set; }
    }
}
