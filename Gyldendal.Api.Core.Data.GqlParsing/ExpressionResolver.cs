using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.ShopAdmin.Infrastructure;
using Irony.Parsing.LINQ_Generator;

namespace Gyldendal.Api.CoreData.GqlValidator
{
    /// <summary>
    /// Parse GQL Expression and validate the syntax
    /// </summary>
    public class ExpressionResolver
    {
        /// <summary>
        /// Contains isbn query
        /// </summary>
        public string Expression { get; private set; }

        /// <summary>
        /// Property that tells whether the expression
        /// is valid or not
        /// </summary>
        private bool IsExpressionValid { get; set; }

        private SyntaxChecker _syntaxChecker;

        /// <summary>
        /// Parsed output of expression
        /// </summary>
        public string GeneratedExpression
        {
            get
            {
                _syntaxChecker = new SyntaxChecker();
                _syntaxChecker.Parse(Expression);
                return _syntaxChecker.GeneratedExpression;
            }
        }

        public List<GqlExpression> GqlExpressions
        {
            get
            {
                if (_syntaxChecker == null)
                {
                    _syntaxChecker = new SyntaxChecker();
                    _syntaxChecker.Parse(Expression);
                }
                return _syntaxChecker.GqlExpression;
            }
        }

        public List<GqlExpression> GqlExpressionWithParentheses
        {
            get
            {
                if (_syntaxChecker == null)
                {
                    _syntaxChecker = new SyntaxChecker();
                    _syntaxChecker.Parse(Expression);
                }
                return _syntaxChecker.GqlExpressionWithParentheses;
            }
        }

        public Node GqlExpressionTree
        {
            get
            {
                if (_syntaxChecker == null)
                {
                    _syntaxChecker = new SyntaxChecker();
                    _syntaxChecker.Parse(Expression);
                }
                return _syntaxChecker.GqlExpressionTree;
            }
        }

        /// <summary>
        /// category method is a special function, which can reference to other
        /// categories already defined. Therefore, it requires special handling.
        /// This method evaluates expressions containing category method into
        /// simple isbn expressions
        /// </summary>
        /// <param name="expression">isbn expression</param>
        /// <returns>true if expression is valid else false</returns>
        private bool Resolver(string expression)
        {
            try
            {
                // ReSharper disable once CollectionNeverQueried.Local
                var categoriesToCompare = new List<string>();
                Expression = expression.ToLower();
                var index = 0;
                var tempIndex = index;
                while (index >= 0)
                {
                    index = Expression.IndexOf(GqlKeywords.Category, tempIndex, StringComparison.CurrentCultureIgnoreCase);
                    if (index < 0)
                        break;
                    var endIndex = Expression.IndexOf(')', index);
                    var method = Expression.Substring(index, endIndex - index + 1);
                    var startIndex = method.IndexOf('(');
                    endIndex = method.IndexOf(')');
                    var arguments = method.Substring(startIndex + 1, endIndex - startIndex - 1);
                    //Get GQL Expression against provided category
                    FindSelfReferencingExpression(Expression, arguments, categoriesToCompare);

                    if (!IsExpressionValid)
                        break;
                    categoriesToCompare.Add(arguments);
                    tempIndex = index;
                }
            }
            catch
            {
                // ignored
            }
            return IsExpressionValid;
        }

        // TODO: Implement this in future.
        ///// <summary>
        ///// This method finds self-referencing (looping) conditions in expression.
        ///// </summary>
        ///// <param name="expression">isbn expression</param>
        ///// <param name="categoryName">name of current category</param>
        ///// <param name="categoriesToCompare">list of categories already processed</param>
        private void FindSelfReferencingExpression(string expression, string categoryName, IEnumerable<string> categoriesToCompare)
        {
            var result = string.Empty;
            if (result.Length == 0)
            {
                IsExpressionValid = false;
                return;
            }

            var match = categoriesToCompare.FirstOrDefault(c => c.Equals(categoryName));
            if (match != null)
            {
                IsExpressionValid = false;
            }
            else
            {
                var methodName = GqlKeywords.Category + GqlKeywords.BraceStart + categoryName + GqlKeywords.BraceEnd;
                Expression = expression.Replace(methodName, result) + " ";
            }
        }

        /// <summary>
        /// Main method which performs the given task of
        /// handling special method category of isbn expression.
        /// </summary>
        /// <param name="expression">isbn expression</param>
        /// <returns>output of expression resolver (valid/true or invalid/false)</returns>
        public bool Run(string expression)
        {
            IsExpressionValid = true;
            Expression = "";
            return Resolver(expression);
        }
    }
}