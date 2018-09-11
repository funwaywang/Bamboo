using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bamboo
{
    public class ObjectTreeNode
    {
        [Key, Column(Order = 0), Required, StringLength(50)]
        public string ObjectType { get; set; }

        [Key, Column(Order = 1)]
        public int ObjectId { get; set; }

        public int L { get; set; }

        public int R { get; set; }

        public int Deepth { get; set; }

        public int Capacity
        {
            get
            {
                return Math.Max(0, (R - L - 1) / 2);
            }
        }

        public void CopyTo(ObjectTreeNode target, bool withId = true)
        {
            if (target == null)
                throw new ArgumentNullException();
            if (target == this)
                return;

            if (withId)
                target.ObjectId = ObjectId;

            target.ObjectType = ObjectType;
            target.L = L;
            target.R = R;
            target.Deepth = Deepth;
        }

        public override string ToString()
        {
            return $"L:{L} R:{R}";
        }
    }
}
