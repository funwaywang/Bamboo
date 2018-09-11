using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bamboo.Forms
{
    [Owned]
    public class DataAssociation
    {
        [StringLength(Defaults.StringLength)]
        public string DataObject { get; set; }

        [NotMapped]
        public Form Form { get; set; }

        public int FormId { get; set; }

        [StringLength(Defaults.StringLength)]
        public string ReferrerKey { get; set; }

        [StringLength(Defaults.StringLength)]
        public string DisplayMember { get; set; }

        [StringLength(Defaults.StringLength)]
        public string DescriptionMember { get; set; }

        [StringLength(Defaults.StringLength)]
        public string AdditionQuery { get; set; }
    }
}
