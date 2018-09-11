using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bamboo
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CodeRuleItemType
    {
        Static,
        DateTime,
        Argument,
        Sequence,
        DaySequence,
        Radom,
        RandomDigit,
        RandomAlphabet,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NumberPadSide
    {
        Left,
        Right
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TextCase
    {
        Default,
        UpperCase,
        LowerCase
    }

    public class CodeRule : IEntity
    {
        public int Id { get; set; }
        [StringLength(50), Required]
        public string Code { get; set; }
        [StringLength(50), Required]
        public string Name { get; set; }
        public TextCase TextCase { get; set; } = TextCase.Default;
        [StringLength(50)]
        public string Description { get; set; }
        [NotMapped]
        public CodeRuleItem[] Items { get; set; }
    }

    public class CodeRuleItem : IEntity
    {
        public int Id { get; set; }
        [NotMapped]
        public CodeRule Rule { get; set; }
        public int RuleId { get; set; }
        public CodeRuleItemType Type { get; set; }
        public string TypeName => StringHelper.ParseWords(Type.ToString());
        [StringLength(50)]
        public string Value { get; set; }
        [StringLength(50)]
        public string Format { get; set; }
        public int Length { get; set; }
        public NumberPadSide PadSide { get; set; } = NumberPadSide.Left;
        [StringLength(1)]
        public string PadCharacter { get; set; } = "0";
        public TextCase TextCase { get; set; } = TextCase.Default;
        public int SortIndex { get; set; }
    }
}
