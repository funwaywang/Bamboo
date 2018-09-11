using System;
using System.Collections.Generic;
using System.Text;

namespace Bamboo.Forms
{
    public enum DataUnicity
    {
        Normal,
        Unique,
        PrimaryKey,
        LogicalKey,
    }

    public static class DataUnicityExtensions
    {
        public static bool IsUnique(this DataUnicity unicity) => unicity == DataUnicity.Unique || unicity == DataUnicity.PrimaryKey || unicity == DataUnicity.LogicalKey;
    }
}
