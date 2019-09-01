using ChavLang.Tokens;
using ChavLang.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ChavLang
{
    public class Parser
    {
        private readonly Regex _variableDefinitionSyntaxRegex = new Regex(@"^TypeKeyword\sIdentifier(\sAssignment\sIntegerLiteral)?\sSemicolon");
        private readonly Regex _functionDefintionSyntaxRegex = new Regex(@"^TypeKeyword\sIdentifier\sOpenParen\s((TypeKeyword\sIdentifier)(\sComma\sTypeKeyword\sIdentifier)*\s)?CloseParen\sOpenBrace");
        private readonly Regex _closeBraceSyntaxRegex = new Regex("^CloseBrace");

        /// <summary>Dictionary that specifies which syntax regexes are valid for a particular location.</summary>
        private readonly Dictionary<Location, List<Regex>> _validSyntaxForLocation;
        /// <summary>Dictionary that maps syntax regexes to the methods that handle the translation of input tokens to output nodes</summary>
        private readonly Dictionary<Regex, Action<int>> _syntaxHandlers;

        private enum Location
        {
            Program = 0,
            Function = 1,
        }

        private readonly List<TokenBase> _tokens;
        private readonly List<TokenBase> _remainingTokens;
        private NodeBase _currentNode;
        private readonly Stack<Location> _locationStack;

        /// <summary>
        /// The program that was generated from the given tokens.
        /// </summary>
        public ProgramNode Program
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new Parser instance and translates the given tokens.
        /// </summary>
        /// <param name="tokens"></param>
        public Parser(IEnumerable<TokenBase> tokens)
        {
            // build the list of valid syntax
            _validSyntaxForLocation = new Dictionary<Location, List<Regex>>()
            {
                {
                    // PROGRAM
                    // a program can contain function definitions and variable definitions
                    Location.Program,
                    new List<Regex>
                    {
                        _functionDefintionSyntaxRegex,
                        _variableDefinitionSyntaxRegex,
                    }
                },
                {
                    // FUNCTION
                    // a function can contain a close brace (to end the function), or a variable definition
                    Location.Function,
                    new List<Regex>
                    {
                        _closeBraceSyntaxRegex,
                        _variableDefinitionSyntaxRegex,
                    }
                },
            };

            // map syntax regexes to function definitions that parse the tokens on a match
            _syntaxHandlers = new Dictionary<Regex, Action<int>>
            {
                { _functionDefintionSyntaxRegex,            ParseFunctionDefinition },
                { _variableDefinitionSyntaxRegex,           ParseVariableDefinition },
                { _closeBraceSyntaxRegex,                   ParseCloseBrace },
            };

            _tokens = new List<TokenBase>(tokens);
            _remainingTokens = new List<TokenBase>(_tokens);
            Program = new ProgramNode();
            _currentNode = Program;
            _locationStack = new Stack<Location>();

            Parse();
        }

        /// <summary>
        /// Parses the tokens in _tokens and puts the resulting program in _program.
        /// </summary>
        private void Parse()
        {
            // first turn the whole token list into a space-separated string representation of the token names, so we can regex it
            var tokenStringBuilder = new StringBuilder();
            foreach (TokenBase token in _tokens)
            {
                string tokenName = token.GetType().Name;
                if (tokenName.EndsWith("Token"))
                    tokenName = tokenName.Substring(0, tokenName.Length - "Token".Length);
                tokenStringBuilder.Append(tokenName + " ");
            }
            string tokenString = tokenStringBuilder.ToString().Trim();

            // store a copy that stores the remaining tokens to parse
            string remainingTokenString = tokenString;

            // empty the location stack, and set the current location as being in the program
            _locationStack.Clear();
            _locationStack.Push(Location.Program);

            // loop through the remaining tokens looking for valid syntax, until we either encounter bad syntax or run out of things to parse
            while (remainingTokenString.Length > 0)
            {
                // get our current location
                Location currentLocation = _locationStack.Peek();

                // work out what syntax is valid for that location
                List<Regex> validSyntaxAtLocation = _validSyntaxForLocation[currentLocation];

                // try matching on each syntax in sequence, until we find something
                bool success = false;
                foreach (Regex syntaxRegex in validSyntaxAtLocation)
                {
                    var match = syntaxRegex.Match(remainingTokenString);
                    if (match.Success)
                    {
                        // work out how many tokens were in the matched string by counting the spaces and adding one
                        int matchedTokenCount = match.Value.Count(c => c == ' ') + 1;

                        // we're parsing this syntax, so remove the tokens we processed from from the remaining tokens (and trim spaces off)
                        remainingTokenString = remainingTokenString.Remove(0, match.Length).Trim();

                        // call the parsing function for this particular syntax
                        var parsingFunction = _syntaxHandlers[syntaxRegex];
                        parsingFunction(matchedTokenCount);

                        success = true;
                        break;
                    }
                }

                if (!success)
                {
                    // todo: make this throw a more useful exception
                    throw new ParsingException("Invalid syntax, could not parse program.");
                }
            }
        }

        /// <summary>
        /// Parses a function definition (_functionDefintionSyntaxRegex)
        /// </summary>
        /// <param name="tokenCount"></param>
        private void ParseFunctionDefinition(int tokenCount)
        {
            if (_locationStack.Peek() != Location.Program || _currentNode != Program)
            {
                throw new ParsingException("[BUG] Function definition parsing should not be reachable outside of a program location.");
            }

            if (tokenCount < 5)
            {
                throw new ParsingException("[BUG] Function definition parsing requires a minimum of five tokens.");
            }

            var tokens = _remainingTokens.GetRange(0, tokenCount);
            _remainingTokens.RemoveRange(0, tokenCount);

            int index = 0;
            var returnType = (TypeKeywordToken)tokens[index++];
            var functionName = (IdentifierToken)tokens[index++];
            var openParen = (OpenParenToken)tokens[index++];
            var parameters = new List<FunctionParameter>();

            // handle the parameters
            if (tokenCount == 5)
            {
                // no parameters => TypeKeyword Identifier OpenParen CloseParen OpenBrace
                // nothing to do here
            }
            else if (tokenCount == 7)
            {
                // one parameter => TypeKeyword Identifier OpenParen TypeKeyword Identifier CloseParen OpenBrace
                var paramType = (TypeKeywordToken)tokens[index++];
                var paramName = (IdentifierToken)tokens[index++];
                parameters.Add(new FunctionParameter
                {
                    Type = paramType.Contents,
                    Name = paramName.Contents
                });
            }
            else
            {
                // many parameters => TypeKeyword Identifier OpenParen TypeKeyword Identifier (Comma TypeKeyword Identifier)+ CloseParen OpenBrace
                if ((tokenCount - 7) % 3 != 0)
                {
                    throw new ParsingException("[BUG] Invalid function parameters in function definition.");
                }
                int paramCount = (tokenCount - 7) / 3;

                var firstParamType = (TypeKeywordToken)tokens[index++];
                var firstParamName = (IdentifierToken)tokens[index++];
                parameters.Add(new FunctionParameter
                {
                    Type = firstParamType.Contents,
                    Name = firstParamName.Contents
                });

                for (int p = 0; p < paramCount - 1; p++)
                {
                    var comma = (CommaToken)tokens[index++];
                    var paramType = (TypeKeywordToken)tokens[index++];
                    var paramName = (IdentifierToken)tokens[index++];
                    parameters.Add(new FunctionParameter
                    {
                        Type = paramType.Contents,
                        Name = paramName.Contents
                    });
                }
            }

            var closeParen = (CloseParenToken)tokens[index++];
            var openBrace = (OpenBraceToken)tokens[index++];

            var functionNode = new FunctionNode(Program, functionName.Contents, parameters);
            _currentNode.AddChild(functionNode);
            _currentNode = functionNode;
            _locationStack.Push(Location.Function);
        }

        /// <summary>
        /// Parses a variable definition (_variableDefinitionSyntaxRegex)
        /// </summary>
        /// <param name="tokenCount"></param>
        private void ParseVariableDefinition(int tokenCount)
        {
            if (tokenCount != 3 && tokenCount != 5)
            {
                throw new ParsingException("[BUG] Variable definition parsing requires either three or five tokens.");
            }

            var tokens = _remainingTokens.GetRange(0, tokenCount);
            _remainingTokens.RemoveRange(0, tokenCount);

            int index = 0;
            var variableType = (TypeKeywordToken)tokens[index++];
            var variableName = (IdentifierToken)tokens[index++];
            string variableDefaultValue = null;
            if (tokenCount == 5)
            {
                var assign = (AssignmentToken)tokens[index++];
                var variableValue = (IntegerLiteralToken)tokens[index++];
                variableDefaultValue = variableValue.Contents;
            }

            var variableNode = new VariableDeclarationNode(_currentNode, variableType.Contents, variableName.Contents, variableDefaultValue);
            _currentNode.AddChild(variableNode);
        }

        /// <summary>
        /// Parses a close brace (_closeBraceSyntaxRegex)
        /// </summary>
        /// <param name="tokenCount"></param>
        private void ParseCloseBrace(int tokenCount)
        {
            if (tokenCount != 1)
            {
                throw new ParsingException("[BUG] Close brace parsing requires exactly one token.");
            }

            var tokens = _remainingTokens.GetRange(0, tokenCount);
            _remainingTokens.RemoveRange(0, tokenCount);

            if (_locationStack.Peek() == Location.Program || _currentNode == Program)
            {
                throw new ParsingException("[BUG] Close brace should not be reachable in a program location.");
            }

            if (_currentNode.Parent == null)
            {
                throw new ParsingException("[BUG] Parent node must not be null.");
            }

            _locationStack.Pop();
            _currentNode = _currentNode.Parent;
        }
    }
}
