namespace Gyldendal.Api.CoreData.Gql.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// type of token i.e. First, OnePerWork etc
        /// </summary>
        public TokenType Type { get; set; }
        /// <summary>
        /// value of parsed token/expression
        /// </summary>
        public string Value { get; set; }

    }
}