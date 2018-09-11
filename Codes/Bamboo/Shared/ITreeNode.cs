using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bamboo
{
    public interface ITreeNode
    {
        int Id { get; }

        int ParentId { get; }
    }

    public interface ITreeNode<T> : ITreeNode
    {
        T[] Items { get; set; }
    }
}
