using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bamboo.Layouts
{
    public class GridCell : IEntity, IPropertyProvider
    {
        public int Id { get; set; }

        [NotMapped]
        public GridRow Row { get; set; }

        [NotMapped]
        public GridColumn Column { get; set; }

        public int RowIndex { get; set; } = -1;

        public int ColumnIndex { get; set; } = -1;

        public IGridContent Content { get; set; }

        public Dictionary<string, string> Properties => throw new NotImplementedException();
    }
}
