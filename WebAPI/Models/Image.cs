using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class Image
    {
        public Image()
        {
            Questions = new HashSet<Question>();
        }

        public int ImageId { get; set; }
        public string Link { get; set; } = null!;

        public virtual ICollection<Question> Questions { get; set; }
    }
}
