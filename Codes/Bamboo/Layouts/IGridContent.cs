using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bamboo.Layouts
{
    public interface IGridContent
    {
        [NotMapped]
        GridCell Container { get; set; }
        int ContainerId { get; set; }
    }
}
