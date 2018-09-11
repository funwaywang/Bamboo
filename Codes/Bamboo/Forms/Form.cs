using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bamboo.Forms
{
    public class Form : IEntity, IPropertyProvider, ILocalizable
    {
        public int Id { get; set; }

        [StringLength(Defaults.StringLength)]
        public string Code { get; set; }

        [StringLength(Defaults.StringLength)]
        public string Name { get; set; }

        [StringLength(Defaults.StringLength)]
        public string LanguageId { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsReadOnly { get; set; }

        [StringLength(Defaults.StringLength)]
        public string Description { get; set; }

        public Dictionary<string, string> Properties { get; private set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
