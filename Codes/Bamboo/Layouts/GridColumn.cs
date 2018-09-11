using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bamboo.Layouts
{
    public class GridColumn : IEntity, IPropertyProvider
    {
        public int Id { get; set; }

        [NotMapped]
        public Grid Grid { get; set; }

        public int GridId { get; set; }

        public GridLength Width { get; set; }

        public Dictionary<string, string> Properties => throw new NotImplementedException();
    }
}
