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
        private readonly Regex _variableDefinitionSyntaxRegex = new Regex(@"^TypeKeyword Identifier (Assignment IntegerLiteral)? Semicolon");
        private readonly Regex _functionDefintionSyntaxRegex = new Regex(@"^TypeKeyword Identifier OpenParen ((TypeKeyword Identifier) (Comma TypeKeyword Identifier)*)? OpenBrace");

        private readonly Dictionary<Location, List<Regex>> _validSyntaxForLocation;
        private readonly Dictionary<Regex, Action> _syntaxHandlers;

        private enum Location
        {
            Program = 0,
            Function = 1,
        }

        private readonly List<TokenBase> _tokens;
        private readonly List<TokenBase> _remainingTokens;
        private readonly ProgramNode _program;
        private readonly NodeBase _currentNode;
        private readonly Stack<Location> _locationStack;

        public Parser(IEnumerable<TokenBase> tokens)
        {
            _validSyntaxForLocation = new Dictionary<Location, List<Regex>>()
            {
                {
                    // a program can contain function definitions and variable definitions
                    Location.Program,
                    new List<Regex>
                    {
                        _functionDefintionSyntaxRegex,
                        _variableDefinitionSyntaxRegex,
                    }
                },
            };

            _syntaxHandlers = new Dictionary<Regex, Action>
            {
                { _variableDefinitionSyntaxRegex, ParseVariableDefinition }
            };

            _tokens = new List<TokenBase>(tokens);
            _remainingTokens = new List<TokenBase>(_tokens);
            _program = new ProgramNode();
            _currentNode = _program;
            _locationStack = new Stack<Location>();

            Parse();
        }

        public void Parse()
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
                        parsingFunction();

                        success = true;
                        break;
                    }
                }

                if (!success)
                {
                    // todo: make this throw a more useful exception
                    throw new ParsingException("Invalid syntax.");
                }
            }
        }

        private void ParseFunctionDefinition()
        {

        }

        private void ParseVariableDefinition()
        {

        }
    }
}
