using Gyldendal.Api.CoreData.GqlValidator;
using Irony.Parsing.LINQ_Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.Tests.Gql.SyntaxCheckerTest
{
    [TestClass]
    public class ExpressionTreeParsingTests
    {
        [DataTestMethod]
        [DataRow("ISBN(9788714118167)", 0, 1, 1)]
        [DataRow("Area(abc)", 0, 1, 1)]
        public void SingleFunctionGql_VerifyParsedGql(string gql, int numberOfOperators, int numberOfOperands, int treeDepth)
        {
            VerifyGqlExpressionTree(gql, numberOfOperators, numberOfOperands, treeDepth);
        }

        [DataTestMethod]
        [DataRow("ISBN(9788714118167) OR Area(abc) AND Imprint(Stereo) OR Area(Kashmir) AND SubArea(Azaad Kashmir)", 4, 5, 4)]
        [DataRow("ISBN(9788714118167) OR Area(abc)", 1, 2, 2)]
        [DataRow("ISBN(9788714118167) OR Imprint(abc)", 1, 2, 2)]
        public void MultipleFunctionGql_VerifyParsedGqlWithOperatorPrecedence(string gql, int numberOfOperators, int numberOfOperands, int treeDepth)
        {
            VerifyGqlExpressionTree(gql, numberOfOperators, numberOfOperands, treeDepth);
        }

        [DataTestMethod]
        [DataRow("publication_date(1, 30) AND (isPhysical(true) OR Imprint(Stereo Imprint))", 2, 3, 3)]
        [DataRow("publication_date(1, 30) AND (isPhysical(true) OR Imprint(Stereo Imprint)) OR ISBN(9788702048018)", 3, 4, 4)]
        public void ComplexFunctionGql_VerifyParsedGqlNoOperatorPrecedence(string gql, int numberOfOperators, int numberOfOperands, int treeDepth)
        {
            VerifyGqlExpressionTree(gql, numberOfOperators, numberOfOperands, treeDepth);
        }

        private void VerifyGqlExpressionTree(string gql, int numberOfOperators, int numberOfOperands, int treeDepth)
        {
            var syntaxChecker = new SyntaxChecker();

            var result = syntaxChecker.Parse(gql);
            Assert.IsTrue(result);

            var depth = MaxDepth(syntaxChecker.GqlExpressionTree);

            Assert.AreEqual(depth, treeDepth);
            var operandsCount = CountLeafNodes(syntaxChecker.GqlExpressionTree);
            var operatorsCount = CountNonLeafNodes(syntaxChecker.GqlExpressionTree);

            Assert.AreEqual(operandsCount, numberOfOperands);
            Assert.AreEqual(operatorsCount, numberOfOperators);

            var postfixExpression = syntaxChecker.Linq.PostfixGqlExpression;
            VerifyTreeFromPostfixExpression(syntaxChecker.GqlExpressionTree, postfixExpression);
            Assert.AreEqual(0, postfixExpression.Count);
        }

        /// <summary>
        /// Resolve the Expression tree and match it with the postfix expression.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="postfixExpressions"></param>
        private static void VerifyTreeFromPostfixExpression(Node root, IList<GqlExpression> postfixExpressions)
        {
            if (root.Value.Type == GqlType.Function && root.Value.Equals(postfixExpressions.First()))
            {
                postfixExpressions.RemoveAt(0);
                return;
            }

            VerifyTreeFromPostfixExpression(root.Left, postfixExpressions);
            VerifyTreeFromPostfixExpression(root.Right, postfixExpressions);

            if (root.Value.Type == GqlType.Operator && root.Value.Equals(postfixExpressions.First()))
            {
                postfixExpressions.RemoveAt(0);
            }
        }

        private static int MaxDepth(Node root)
        {
            if (root == null)
                return 0;

            // Recursively find the depth of each subtree.
            var leftDepth = MaxDepth(root.Left);
            var rightDepth = MaxDepth(root.Right);

            // Get the larger depth and add 1 to it to
            // account for the root.
            return leftDepth > rightDepth ? leftDepth + 1 : rightDepth + 1;
        }

        public virtual int CountLeafNodes(Node node)
        {
            if (node == null)
            {
                return 0;
            }
            if (node.Left == null && node.Right == null)
            {
                return 1;
            }
            else
            {
                return CountLeafNodes(node.Left) + CountLeafNodes(node.Right);
            }
        }

        /* Computes the number of non-leaf nodes in a tree. */

        private static int CountNonLeafNodes(Node root)
        {
            // Base cases.
            if (root == null || (root.Left == null && root.Right == null))
                return 0;

            // If root is Not NULL and its one of its
            // child is also not NULL
            return 1 + CountNonLeafNodes(root.Left) + CountNonLeafNodes(root.Right);
        }
    }
}