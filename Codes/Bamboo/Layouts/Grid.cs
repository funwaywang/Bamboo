using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bamboo.Layouts
{
    public class Grid : IEntity, IPropertyProvider, IGridContent
    {
        public int Id { get; set; }

        [StringLength(Defaults.StringLength)]
        public string Code { get; set; }

        [StringLength(Defaults.StringLength)]
        public string Caption { get; set; }

        public Dictionary<string, string> Properties => throw new NotImplementedException();

        [NotMapped]
        public GridCell Container { get; set; }

        public int ContainerId { get; set; }
    }
}
