using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit;
using ChavLang;
using ChavLang.Extensions;
using ChavLang.Helpers;
using ChavLang.Tokens;

namespace GenericCompilerTests
{
    public class LexerTests
    {
#if DEBUG
        [Fact]
        public void LexerTokenMapHasEntriesForAllRegexes()
        {
            var lexer = new Lexer();

            // find all private fields inside the Lexer class that are type of System.Text.RegularExpressions.Regex, are readonly, and whose name ends with "TokenRegex".
            var lexerTokenRegexFields = typeof(Lexer).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.IsInitOnly && f.Name.EndsWith(@"TokenRegex") && f.FieldType == typeof(Regex));

            // get all the token patterns
            var tokenPatternMap = lexer.TokenPatternMapEnumerator.ToEnumerable().ToList();
            
            // check that there's a matching token pattern map entry for each lexer token regex field
            foreach (FieldInfo lexerField in lexerTokenRegexFields)
            {
                var regex = (Regex)lexerField.GetValue(lexer);
                Assert.True(tokenPatternMap.Any(kvp => kvp.Key == regex), $"Lexer regex field '{lexerField.Name}' is not used.");
            }
        }
#endif

        private void AssertResultMatchesExpectedTokens(string code, List<TokenBase> expectedTokens)
        {
            Lexer lexer = new Lexer();
            List<TokenBase> tokens = lexer.Lex(code);
            bool equal = tokens.SequenceEqual(expectedTokens, LambdaEqualityComparer<TokenBase>.Create(
                (a, b) => a.GetType() == b.GetType() && a.Contents == b.Contents
            ));

            Assert.True(equal, "Token sequence did not match.");
        }

        [Fact]
        public void ProgramLexes1()
        {
            string code = @"
int main() {
    return 2;
}
";
            var programTokens = new List<TokenBase>
            {
                new TypeKeywordToken("int"),
                new IdentifierToken("main"),
                new OpenParenToken("("),
                new CloseParenToken(")"),
                new OpenBraceToken("{"),

                new ReturnKeywordToken("return"),
                new IntegerLiteralToken("2"),
                new SemicolonToken(";"),

                new CloseBraceToken("}")
            };

            AssertResultMatchesExpectedTokens(code, programTokens);
        }

        [Fact]
        public void ProgramLexes2()
        {
            string code = @"
int main(int a, int b) {
    return 2;
}
";
            var programTokens = new List<TokenBase>
            {
                new TypeKeywordToken("int"),
                new IdentifierToken("main"),
                new OpenParenToken("("),
                new TypeKeywordToken("int"),
                new IdentifierToken("a"),
                new CommaToken(","),
                new TypeKeywordToken("int"),
                new IdentifierToken("b"),
                new CloseParenToken(")"),
                new OpenBraceToken("{"),

                new ReturnKeywordToken("return"),
                new IntegerLiteralToken("2"),
                new SemicolonToken(";"),

                new CloseBraceToken("}")
            };

            AssertResultMatchesExpectedTokens(code, programTokens);
        }

        [Fact]
        public void ProgramLexes3()
        {
            string code = @"
int main() {
    uint returnValue = 69;
    return (int)returnValue;
}
";
            var programTokens = new List<TokenBase>
            {
                new TypeKeywordToken("int"),
                new IdentifierToken("main"),
                new OpenParenToken("("),
                new CloseParenToken(")"),
                new OpenBraceToken("{"),

                new TypeKeywordToken("uint"),
                new IdentifierToken("returnValue"),
                new AssignmentToken("="),
                new IntegerLiteralToken("69"),
                new SemicolonToken(";"),

                new ReturnKeywordToken("return"),
                new OpenParenToken("("),
                new TypeKeywordToken("int"),
                new CloseParenToken(")"),
                new IdentifierToken("returnValue"),
                new SemicolonToken(";"),

                new CloseBraceToken("}")
            };

            AssertResultMatchesExpectedTokens(code, programTokens);
        }

        [Fact]
        public void ProgramLexes4()
        {
            string code = @"
int main() {
    int a = 2;
    if (a & 1 == 0)
        return 1;
    else
        return -1;
}
";
            var programTokens = new List<TokenBase>
            {
                new TypeKeywordToken("int"),
                new IdentifierToken("main"),
                new OpenParenToken("("),
                new CloseParenToken(")"),
                new OpenBraceToken("{"),

                new TypeKeywordToken("int"),
                new IdentifierToken("a"),
                new AssignmentToken("="),
                new IntegerLiteralToken("2"),
                new SemicolonToken(";"),

                new IfKeywordToken("if"),
                new OpenParenToken("("),
                new IdentifierToken("a"),
                new BitwiseAndToken("&"),
                new IntegerLiteralToken("1"),
                new EqualityToken("=="),
                new IntegerLiteralToken("0"),
                new CloseParenToken(")"),

                new ReturnKeywordToken("return"),
                new IntegerLiteralToken("1"),
                new SemicolonToken(";"),

                new ElseKeywordToken("else"),

                new ReturnKeywordToken("return"),
                new MinusToken("-"),
                new IntegerLiteralToken("1"),
                new SemicolonToken(";"),

                new CloseBraceToken("}")
            };

            AssertResultMatchesExpectedTokens(code, programTokens);
        }

    }
}
