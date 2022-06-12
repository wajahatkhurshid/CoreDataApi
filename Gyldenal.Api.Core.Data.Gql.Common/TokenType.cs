namespace Gyldendal.Api.CoreData.Gql.Common
{
    /// <summary>
    /// defines enumerated types of different
    /// identifiers present in the expression
    /// </summary>
    public enum TokenType
    {
        Operator = 0,
        OpeningBracket,
        ClosingBracket,
        Method,
        Parameter,
        None,
        Any,
        Error,
        PreValidated,
        First,
        OnePerWork,
        OrderBy,
        Top
    }

}