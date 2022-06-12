using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Irony;
using Irony.Parsing;
using Irony.Parsing.LINQ_Generator;
using Irony.Samples.Gql;

namespace Gyldendal.Api.CoreData.GqlValidator
{
    /// <summary>
    /// Validates the syntax of isbn expression
    /// </summary>
    public class SyntaxChecker
    {
        #region Private Members

        private readonly Parser _parser;

        private ParseTree _parseTree;

        #endregion Private Members

        #region Constructor

        /// <summary>
        /// Loads assembly file that contains grammar definition.
        /// It also initializes parser with loaded grammar.
        /// </summary>
        public SyntaxChecker()
        {
            Grammar grammar = new GqlGrammar();
            var language = new LanguageData(grammar);
            _parser = new Parser(language);
            Linq = null;
        }

        #endregion Constructor

        #region Public Memebers

        public LinqV2 Linq;

        public List<GqlExpression> GqlExpression
        {
            get
            {
                Linq = new LinqV2(_parseTree);
                Linq.GenerateQuery();
                return Linq.GqlExpression;
            }
        }

        public List<GqlExpression> GqlExpressionWithParentheses
        {
            get
            {
                Linq = new LinqV2(_parseTree);
                Linq.GenerateQuery();
                return Linq.GqlExpressionWithParentheses;
            }
        }

        public Node GqlExpressionTree
        {
            get
            {
                Linq = new LinqV2(_parseTree);
                return Linq.GenerateExpressionTree();
            }
        }

        public Node GenerateGqlExpressionTree(List<GqlExpression> gqlExpressions)
        {
            Linq = Linq ?? new LinqV2(_parseTree);
            return Linq.GenerateExpressionTree(gqlExpressions);
        }

        /// <summary>
        /// expression returned after syntax checking
        /// </summary>
        public string GeneratedExpression
        {
            get
            {
                if (_parseTree == null) return "";
                Linq = new LinqV2(_parseTree);
                return Linq.GenerateQuery();
            }
        }

        /// <summary>
        /// set of error messages
        /// </summary>
        public LogMessageList ErrorMessages => _parseTree?.ParserMessages;

        #endregion Public Memebers

        #region Public Methods

        /// <summary>
        /// It parses isbn expression and checks for syntax errors.
        /// If no error is found, expression is considered as valid expression.
        /// and if error is found expression is invalid.
        /// </summary>
        /// <param name="expression">expression to be checked</param>
        /// <returns>true if expression valid else false</returns>
        public bool Parse(string expression)
        {
            try
            {
                _parser.Parse(expression, "<source>");
                _parseTree = _parser.Context.CurrentParseTree;

                return _parseTree.Status == ParseTreeStatus.Parsed;
            }
            catch
            {
                return false;
            }
        }

        #endregion Public Methods
    }
}