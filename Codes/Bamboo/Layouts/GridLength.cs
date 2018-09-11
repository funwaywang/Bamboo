using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Bamboo.Layouts
{
    public enum GridLengthUnit
    {
        Auto,
        Pixel,
        Percentage,
        Centimeter
    }

    [Owned]
    public class GridLength
    {
        public double Value { get; set; }
        public GridLengthUnit Unit { get; set; }
    }
}
