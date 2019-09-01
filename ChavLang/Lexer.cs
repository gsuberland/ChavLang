using ChavLang.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ChavLang
{
    public class Lexer
    {
        private const string WhitespaceRegex = @"^[\s\r\n]*";
        private const string NonIdentifierRegex = @"(?![a-zA-Z0-9_])";
        private const int RegexContentGroupIndex = 1;

        private readonly Regex _openBraceTokenRegex				    = new Regex(WhitespaceRegex + @"({)");
        private readonly Regex _closeBraceTokenRegex			    = new Regex(WhitespaceRegex + @"(})");
        private readonly Regex _openParenTokenRegex				    = new Regex(WhitespaceRegex + @"(\()");
        private readonly Regex _closeParenTokenRegex				= new Regex(WhitespaceRegex + @"(\))");
        private readonly Regex _commaTokenRegex				        = new Regex(WhitespaceRegex + @"(,)");
        private readonly Regex _semicolonTokenRegex				    = new Regex(WhitespaceRegex + @"(;)");
        private readonly Regex _minusTokenRegex                     = new Regex(WhitespaceRegex + @"(-)");
        private readonly Regex _complimentTokenRegex                = new Regex(WhitespaceRegex + @"(~)");
        private readonly Regex _negationTokenRegex                  = new Regex(WhitespaceRegex + @"(!)");
        private readonly Regex _additionTokenRegex                  = new Regex(WhitespaceRegex + @"(\+)");
        private readonly Regex _multiplyTokenRegex                  = new Regex(WhitespaceRegex + @"(\*)");
        private readonly Regex _divideTokenRegex                    = new Regex(WhitespaceRegex + @"(/)");
        private readonly Regex _logicalAndTokenRegex                = new Regex(WhitespaceRegex + @"(&&)");
        private readonly Regex _logicalOrTokenRegex                 = new Regex(WhitespaceRegex + @"(\|\|)");
        private readonly Regex _bitwiseAndTokenRegex                = new Regex(WhitespaceRegex + @"(&(?!&))");
        private readonly Regex _bitwiseOrTokenRegex                 = new Regex(WhitespaceRegex + @"(\|(?!\|))");
        private readonly Regex _equalityTokenRegex		            = new Regex(WhitespaceRegex + @"(==|!=|<=?|>=?)");
        private readonly Regex _assignmentTokenRegex		        = new Regex(WhitespaceRegex + @"(=)");
        private readonly Regex _typeKeywordTokenRegex			    = new Regex(WhitespaceRegex + @"(uint|int|byte)" + NonIdentifierRegex);
        private readonly Regex _returnKeywordTokenRegex			    = new Regex(WhitespaceRegex + @"(return)" + NonIdentifierRegex);
        private readonly Regex _ifKeywordTokenRegex                 = new Regex(WhitespaceRegex + @"(if)" + NonIdentifierRegex);
        private readonly Regex _elseKeywordTokenRegex               = new Regex(WhitespaceRegex + @"(else)" + NonIdentifierRegex);
        private readonly Regex _identifierTokenRegex				= new Regex(WhitespaceRegex + @"([a-zA-Z_][a-zA-Z0-9_]*)");
        private readonly Regex _unsignedIntegerLiteralTokenRegex	= new Regex(WhitespaceRegex + @"([0-9]+u)");
        private readonly Regex _integerLiteralTokenRegex			= new Regex(WhitespaceRegex + @"([0-9]+)");

        private readonly Dictionary<Regex, Type> _tokenPatternMap;

#if DEBUG
        /// <summary>
        /// Used for unit tests; not exposed in public builds.
        /// </summary>
        public Dictionary<Regex, Type>.Enumerator TokenPatternMapEnumerator
        {
            get
            {
                return _tokenPatternMap.GetEnumerator();
            }
        }
#endif

        public Lexer()
        {
            _tokenPatternMap = new Dictionary<Regex, Type>() {
                { _openBraceTokenRegex,                 typeof(OpenBraceToken) },
                { _closeBraceTokenRegex,                typeof(CloseBraceToken) },
                { _openParenTokenRegex,                 typeof(OpenParenToken) },
                { _closeParenTokenRegex,                typeof(CloseParenToken) },
                { _commaTokenRegex,                     typeof(CommaToken) },
                { _semicolonTokenRegex,                 typeof(SemicolonToken) },
                { _minusTokenRegex,                     typeof(MinusToken) },
                { _complimentTokenRegex,                typeof(ComplimentToken) },
                { _negationTokenRegex,                  typeof(NegationToken) },
                { _additionTokenRegex,                  typeof(AdditionToken) },
                { _multiplyTokenRegex,                  typeof(MultiplyToken) },
                { _divideTokenRegex,                    typeof(DivideToken) },
                { _logicalAndTokenRegex,                typeof(LogicalAndToken) },
                { _logicalOrTokenRegex,                 typeof(LogicalOrToken) },
                { _bitwiseAndTokenRegex,                typeof(BitwiseAndToken) },
                { _bitwiseOrTokenRegex,                 typeof(BitwiseOrToken) },
                { _equalityTokenRegex,                  typeof(EqualityToken) },
                { _assignmentTokenRegex,                typeof(AssignmentToken) },
                { _typeKeywordTokenRegex,               typeof(TypeKeywordToken) },
                { _returnKeywordTokenRegex,             typeof(ReturnKeywordToken) },
                { _ifKeywordTokenRegex,                 typeof(IfKeywordToken) },
                { _elseKeywordTokenRegex,               typeof(ElseKeywordToken) },
                { _identifierTokenRegex,                typeof(IdentifierToken) },
                { _unsignedIntegerLiteralTokenRegex,    typeof(UnsignedIntegerLiteralToken) },
                { _integerLiteralTokenRegex,            typeof(IntegerLiteralToken) },
            };
        }

        public List<TokenBase> Lex(string code)
        {
            var lines = code.Split(new char[] { '\r', '\n' }).Select(line => line.Trim());
            var tokens = new List<TokenBase>();
            int lineNumber = 0;
            foreach (string line in lines)
            {
                string remainingCode = line;
                while (remainingCode.Length > 0)
                {
                    bool foundMatch = false;
                    foreach ((Regex pattern, Type type) in _tokenPatternMap)
                    {
                        Match match = pattern.Match(remainingCode);
                        if (match.Success)
                        {
                            foundMatch = true;
                            var token = (TokenBase)Activator.CreateInstance(type, match.Groups[RegexContentGroupIndex].Value);
                            tokens.Add(token);
                            // snip code down
                            if (match.Length == remainingCode.Length)
                            {
                                remainingCode = "";
                            }
                            else
                            {
                                remainingCode = remainingCode.Substring(match.Length);
                            }
                            break;
                        }
                    }
                    if (!foundMatch)
                    {
                        throw new LexingException($"Invalid token on line {lineNumber} at {remainingCode}");
                    }
                }
                lineNumber++;
            }
            return tokens;
        }
    }
}
