using System;
using System.ComponentModel.DataAnnotations;

namespace Bamboo
{
    public enum EntityChangeOperation
    {
        Create,
        Update,
        Delete
    }

    public class EntityChangeLog<T> : IEntity
    {
        public int Id { get; set; }
        [StringLength(50), Required]
        public string DataType { get; set; }
        public T EntityId { get; set; }
        public EntityChangeOperation ChangeOperation { get; set; }
        [StringLength(50), Required]
        public string OperatorId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
