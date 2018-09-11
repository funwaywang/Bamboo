using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bamboo.Forms
{
    public class FormItem : IEntity, IPropertyProvider, ILocalizable
    {
        public int Id { get; set; }

        [NotMapped]
        public Form Form { get; set; }

        public int FomdId { get; set; }

        [StringLength(Defaults.StringLength)]
        public string Code { get; set; }

        [StringLength(Defaults.StringLength)]
        public string Name { get; set; }

        [StringLength(Defaults.StringLength)]
        public string LanguageId { get; set; }

        [StringLength(Defaults.StringLength)]
        public string DataFieldName { get; set; }

        public DataType DataType { get; set; } = DataType.Text;

        [StringLength(Defaults.StringLength)]
        public string DataTypeDescription { get; set; }

        public DataAssociation DataSource { get; set; }

        public DataUnicity Unicity { get; set; } = DataUnicity.Normal;

        public bool AllowNull { get; set; }

        public int MaxLength { get; set; }

        public int MinLength { get; set; }

        [StringLength(Defaults.StringLength)]
        public string MaxValue { get; set; }

        [StringLength(Defaults.StringLength)]
        public string MinValue { get; set; }

        [StringLength(Defaults.StringLength)]
        public string DefaultValue { get; set; }

        public DataGenerateMethod InsertValueGenerateMethod { get; set; } = DataGenerateMethod.None;

        public string InsertValueGenerateToken { get; set; }

        public DataGenerateMethod UpdateValueGenerateMethod { get; set; } = DataGenerateMethod.None;

        public string UpdateValueGenerateToken { get; set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsReadOnly { get; set; }

        public bool IsVisible { get; set; } = true;

        public int SortIndex { get; set; }

        [StringLength(Defaults.StringLength)]
        public string Description { get; set; }

        public Dictionary<string, string> Properties => throw new NotImplementedException();
    }
}
