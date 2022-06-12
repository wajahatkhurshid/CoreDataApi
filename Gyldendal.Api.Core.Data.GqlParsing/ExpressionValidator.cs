using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Gql.Common;
using Irony.Parsing.LINQ_Generator;
using NewRelic.Api.Agent;

namespace Gyldendal.Api.CoreData.GqlValidator
{
    public class ExpressionValidator
    {
        #region Data Members

        /// <summary>
        /// Html formatted output of error checker
        /// </summary>
        private string _formattedOutput;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public List<string> Errors { get; private set; }

        /// <summary>
        /// Operators allowed in the expression
        /// </summary>
        private readonly string[] _operators = { "and", "or", "not" };

        #endregion Data Members

        /// <summary>
        /// Returns if given expression is valid or not
        /// It searches for special methods called PreValidation.
        /// After PreValidation stage, it parses expression
        /// and checks for syntax errors.
        /// </summary>
        /// <param name="expression">isbn expression</param>
        /// <returns>Information about expression
        /// whether it is a valid expression or not
        /// </returns>
        [Trace]
        public SyntaxInfo Validate(string expression)
        {
            expression = new GqlGrammarTraslator(expression).TranslatedExpression;
            var validationOutput = ValidateExpression(expression);
            var syntaxInfo = new SyntaxInfo
            {
                Result =
                {
                    IsValidated = validationOutput.IsValidated,
                    Message = validationOutput.Message,
                    GqlExpressions = validationOutput.GqlExpressions,
                    GqlExpressionTree = validationOutput.GqlExpressionTree,
                    GqlExpressionWithParentheses = validationOutput.GqlExpressionWithParentheses
                },
                PostProcessingTokensExist = false
            };

            // Return if expression is not valid
            if (!(validationOutput.IsValidated))
            {
                return syntaxInfo;
            }

            // Returnn syntaxInfo if expr. is valid, but no post processing tokens exist.
            var postProcessSyntaxInfo = ExtractPostProcessingTokens(validationOutput.GqlExpressionWithParentheses);
            if (!postProcessSyntaxInfo.PostProcessingTokensExist)
            {
                return syntaxInfo;
            }

            // Gather the post processing tokens info.
            syntaxInfo.PostProcessingTokensExist = true;
            syntaxInfo.PostProcessTokens = postProcessSyntaxInfo.PostProcessTokens;

            syntaxInfo.Result.Message = postProcessSyntaxInfo.MainExpression;
            syntaxInfo.Result.GqlExpressions = postProcessSyntaxInfo.MainExpressionGql.Where(x => x.Type != GqlType.Parentheses).ToList();
            syntaxInfo.Result.GqlExpressionTree = new SyntaxChecker().GenerateGqlExpressionTree(postProcessSyntaxInfo.MainExpressionGql);

            syntaxInfo.PostProcessingExpression = postProcessSyntaxInfo.PostProcessingExpression;
            syntaxInfo.PostProcessingExpressionGql = postProcessSyntaxInfo.PostProcessingExpressionGql;

            return syntaxInfo;
        }

        /// <summary>
        /// It performs syntax checking and returns error
        /// in case expression is not valid.
        /// </summary>
        /// <param name="expression">isbn expression</param>
        /// <returns>Returns whether expression is valid or not</returns>
        [Trace]
        private ExpressionValidationResult ValidateExpression(string expression)
        {
            var expressionChecker = new SyntaxChecker();
            var exprParsed = expressionChecker.Parse(expression);
            var validationResult = new ExpressionValidationResult();

            if (!(exprParsed))
            {
                GenerateFormattedErrorInfo(expression);
                validationResult.IsValidated = false;
                validationResult.Message = _formattedOutput;
                return validationResult;
            }

            var resolver = new ExpressionResolver();
            var isValid = resolver.Run(expression);
            if (isValid)
            {
                validationResult.IsValidated = true;
                validationResult.Message = resolver.GeneratedExpression;
                validationResult.GqlExpressions = resolver.GqlExpressions;
                validationResult.GqlExpressionWithParentheses = resolver.GqlExpressionWithParentheses;
                validationResult.GqlExpressionTree = resolver.GqlExpressionTree;
                validationResult.Expression = resolver.Expression;
            }
            else
            {
                validationResult.IsValidated = false;
                validationResult.Message = "<span style='color:red'>self referencing expresion found</span>";
            }

            return validationResult;
        }

        /// <summary>
        /// It pre validates expression and searches for special methods such
        /// as first and oneperwork. These methods are evaluated in a special way.
        /// </summary>
        /// <param name="expression">isbn expression</param>
        /// <returns>returns whether expression contains special methods or not
        /// </returns>
        private static SyntaxInfo ExtractPostProcessingTokens(IEnumerable<GqlExpression> expression)
        {
            var searchKeywords = GqlKeywords.PostProcessingKeywords;

            var syntaxInfo = new SyntaxInfo();
            var mainExpressionGql = new List<GqlExpression>();
            var postProcessingExpressionGql = new List<GqlExpression>();
            var mainExpression = "";
            GqlExpression prevToken = null;

            foreach (var token in expression)
            {
                if (token.Type == GqlType.Operator)
                {
                    prevToken = token;
                    continue;
                }
                var result = searchKeywords.FirstOrDefault(s => token.Name.ToLower().Contains(s.ToLower()));
                if (result != null)
                {
                    var parsedToken = new ParsedUnits
                    {
                        Type = Util.ParseToken(token.Name),
                        Parameters = token.Parameters.ToList(),
                        GqlExpression = token
                    };
                    if (postProcessingExpressionGql.Any())
                        postProcessingExpressionGql.Add(new GqlExpression(GqlType.Operator, "+", null));
                    postProcessingExpressionGql.Add(token);
                    syntaxInfo.PostProcessTokens.Add(parsedToken);
                    prevToken = null;
                }
                else
                {
                    if (prevToken != null && mainExpressionGql.Any())
                    {
                        mainExpressionGql.Add(prevToken);
                        mainExpression += prevToken.ToString();
                    }
                    mainExpression += token.ToString();
                    mainExpressionGql.Add(token);
                    prevToken = null;
                }
            }

            syntaxInfo.MainExpressionGql = mainExpressionGql;
            syntaxInfo.PostProcessingExpressionGql = postProcessingExpressionGql;
            syntaxInfo.MainExpression = mainExpression;

            if (syntaxInfo.PostProcessTokens.Count != 0)
            {
                syntaxInfo.PostProcessingTokensExist = true;
                syntaxInfo.PostProcessingExpression = string.Join(" or ",
                    syntaxInfo.PostProcessTokens.Select(s => s.ToMethod()));
            }

            return syntaxInfo;
        }

        /// <summary>
        /// Main method which runs parser to identify any syntax errors
        /// </summary>
        /// <param name="expression">isbn expression</param>
        /// <returns>true if expression is valid else false</returns>
        [Trace]
        private void GenerateFormattedErrorInfo(string expression)
        {
            var tokens = expression.Split(' ');
            var nextToken = TokenType.Method;
            var errorList = new List<string>();
            var formattedOutput = "";
            foreach (var key in tokens)
            {
                TokenInfo tokenInfo;

                formattedOutput = GetTokenErrorInfo(key, formattedOutput, out tokenInfo);

                if (tokenInfo.Type == TokenType.None && (nextToken == TokenType.Operator || nextToken == TokenType.Any))
                {
                    var result = _operators.FirstOrDefault(o => o.Equals(key, StringComparison.CurrentCultureIgnoreCase));
                    if (result == null)
                    {
                        nextToken = TokenType.Any;
                        errorList.Add("operator not found:" + key);
                        formattedOutput += " <span style='color:red'>" + key + "</span>";
                    }
                    else if (result.ToLower() == "and")
                    {
                        nextToken = TokenType.Any;
                        formattedOutput += " " + key;
                    }
                    else
                    {
                        formattedOutput += " " + key;
                        nextToken = TokenType.Method;
                    }
                }
                else if (tokenInfo.Type == TokenType.Error)
                {
                    errorList.Add(tokenInfo.Value);
                    nextToken = TokenType.Any;
                }
                else if (tokenInfo.Type == TokenType.Method &&
                         (nextToken == TokenType.Method || nextToken == TokenType.Any))
                {
                    nextToken = TokenType.Operator;
                }
                else
                {
                    errorList.Add("missing operator");
                }
            }
            _formattedOutput = formattedOutput;
            Errors = errorList;
        }

        /// <summary>
        /// Parses given expression and checks for syntax erros
        /// </summary>
        /// <param name="expression">given expression</param>
        /// <param name="formattedString">error formatted form of expression
        /// </param>
        /// <param name="info">result of current token</param>
        /// <returns>error formatted form of expression</returns>
        private string GetTokenErrorInfo(string expression, string formattedString, out TokenInfo info)
        {
            var keywords = Enum.GetNames(typeof(GqlOperation))
                .Select(x => ((GqlOperation)Enum.Parse(typeof(GqlOperation), x)).ToTokenString())
                .ToArray();

            var startIndex = expression.IndexOf('(');
            info = new TokenInfo { Type = TokenType.None };
            var myFormattedString = formattedString;
            if (startIndex >= 0)
            {
                var methodName = expression.Substring(0, startIndex);
                var endIndex = expression.IndexOf(')');
                var match = keywords.FirstOrDefault(k => k.Equals(methodName, StringComparison.CurrentCultureIgnoreCase));
                if (match != null)
                {
                    var arguments = "";
                    var argumentLength = endIndex - startIndex - 1;
                    if (endIndex > startIndex)
                    {
                        arguments = expression.Substring(startIndex + 1, endIndex - startIndex - 1);
                    }
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        info.Type = TokenType.Method;
                        info.Value = match;
                        myFormattedString += " " + expression;
                    }
                    else
                    {
                        if (match == "oneperwork" && argumentLength == 0)
                        {
                            info.Type = TokenType.Method;
                            info.Value = match;
                            myFormattedString += " " + expression;
                        }
                        else
                        {
                            info.Type = TokenType.Error;
                            info.Value = "missing arguments";
                            arguments = expression.Remove(0, match.Length);
                            myFormattedString += " " + methodName + "<span style='color:red'>" + arguments + "</span>";
                        }
                    }
                }
                else
                {
                    info.Type = TokenType.Error;
                    info.Value = "method not found : " + methodName;
                    myFormattedString += " <span style='color:red'>" + methodName + "</span>" +
                                         expression.Substring(methodName.Length);
                }
            }
            else
            {
                var match = keywords.FirstOrDefault(k => k.Equals(expression, StringComparison.CurrentCultureIgnoreCase));
                if (match == null)
                    return myFormattedString;
                info.Type = TokenType.Error;
                info.Value = "missing (";
                myFormattedString += " <span style='color:red'>" + expression + "</span>";
            }

            return myFormattedString;
        }
    }
}