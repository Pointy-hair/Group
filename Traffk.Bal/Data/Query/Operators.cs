using System.ComponentModel;

namespace Traffk.Bal.Data.Query
{
    public enum Operators
    {
        [DisplayName("any of")]
        AnyOf,
        [DisplayName("none of")]
        NoneOf,
        [DisplayName("==")]
        Equals,
        [DisplayName("<>")]
        NotEquals,
        [DisplayName(">")]
        GreaterThan,
        [DisplayName(">=")]
        GreaterThanOrEqual,
        [DisplayName("<")]
        LessThan,
        [DisplayName("<=")]
        LessThanOrEqual,
        [DisplayName("is true")]
        IsTrue,
        [DisplayName("is false")]
        IsFalse,
        [DisplayName("is defined")]
        IsDefined,
        [DisplayName("is not defined")]
        IsNotDefined,
        [DisplayName("is null")]
        IsNotNull,
    }
}
