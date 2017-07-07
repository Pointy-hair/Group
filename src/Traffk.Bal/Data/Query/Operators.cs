using System.ComponentModel.DataAnnotations;

namespace Traffk.Bal.Data.Query
{
    public enum Operators
    {
        [Display(Name = "any of")]
        AnyOf,
        [Display(Name = "none of")]
        NoneOf,
        [Display(Name = "==")]
        Equals,
        [Display(Name = "<>")]
        NotEquals,
        [Display(Name = ">")]
        GreaterThan,
        [Display(Name = ">=")]
        GreaterThanOrEqual,
        [Display(Name = "<")]
        LessThan,
        [Display(Name = "<=")]
        LessThanOrEqual,
        [Display(Name = "is true")]
        IsTrue,
        [Display(Name = "is false")]
        IsFalse,
        [Display(Name = "is defined")]
        IsDefined,
        [Display(Name = "is not defined")]
        IsNotDefined,
        [Display(Name = "is null")]
        IsNotNull,
    }
}
