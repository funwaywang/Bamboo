using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bamboo
{
    public class Sequence : IEntity<string>
    {
        [Key, StringLength(50), Required]
        public string Id { get; set; }
        [ConcurrencyCheck]
        public int NextValue { get; set; }
    }
}
