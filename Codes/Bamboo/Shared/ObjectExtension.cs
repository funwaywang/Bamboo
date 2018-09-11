using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bamboo
{
    public enum GeneralDataType
    {
        Text,
        Integer,
        Numerical,
        Boolean,
        DateTime
    }

    public class ObjectExtension : IEntity
    {
        public int Id { get; set; }
        [StringLength(50), Required]
        public string Code { get; set; }
        [StringLength(50), Required]
        public string Name { get; set; }
    }

    public class ObjectExtensionProperty : IEntity
    {
        public int Id { get; set; }
        [NotMapped]
        public ObjectExtension Owner { get; set; }
        public int OwnerId { get; set; }
        [StringLength(50), Required]
        public string Code { get; set; }
        [StringLength(50), Required]
        public string Name { get; set; }
        public GeneralDataType DataType { get; set; } = GeneralDataType.Text;
        [StringLength(50)]
        public string DefaultValue { get; set; }
        public int SortIndex { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class ObjectExtensionValue
    {
        public int EntityId { get; set; }
        public int PropertyId { get; set; }
        [StringLength(50), Required]
        public string PropertyValue { get; set; }
        [NotMapped]
        public string PropertyName { get; set; }
    }
}
