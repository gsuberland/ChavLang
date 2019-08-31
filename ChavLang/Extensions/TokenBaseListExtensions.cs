using System;
using System.Collections.Generic;
using System.Text;
using ChavLang.Tokens;

namespace ChavLang.Extensions
{
    // todo: change this to work with the AST, as the tokens don't have children and this makes no sense
    /*
    public static class TokenBaseListExtensions
    {
        public static string ToTreeString(this IEnumerable<TokenBase> tokens)
        {
            var output = new StringBuilder();

            var tokensToPrint = new Stack<Tuple<int, TokenBase>>();
            var tokensReversed = new List<TokenBase>(tokens);
            tokensReversed.Reverse();
            foreach (TokenBase token in tokensReversed)
            {
                tokensToPrint.Push(new Tuple<int, TokenBase>(0, token));
            }

            while (tokensToPrint.Count > 0)
            {
                (int depth, TokenBase currentToken) = tokensToPrint.Pop();

                // append the right number of tabs
                output.Append(new StringBuilder("\t".Length * depth).Insert(0, "\t", depth).ToString());
                // append the current token as a string
                output.AppendLine(currentToken.ToString());

                // now manage any child tokens
                var childTokensReversed = new List<TokenBase>(currentToken.Children);
                childTokensReversed.Reverse();
                foreach (TokenBase childToken in childTokensReversed)
                {
                    tokensToPrint.Push(new Tuple<int, TokenBase>(depth + 1, childToken));
                }
            }

            return output.ToString();
        }
    }*/
}
