using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bamboo.Layouts
{
    public class GridRow : IEntity, IPropertyProvider
    {
        public int Id { get; set; }

        [NotMapped]
        public Grid Grid { get; set; }

        public int GridId { get; set; }

        public GridLength Height { get; set; }

        public Dictionary<string, string> Properties => throw new NotImplementedException();
    }
}
