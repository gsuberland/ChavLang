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
using ChavLang.Nodes;

namespace ChavLangTests
{
    public class ParserTests
    {
        private void AssertResultMatchesExpectedProgram(string program, ProgramNode expectedProgram)
        {
            Lexer lexer = new Lexer();
            List<TokenBase> tokens = lexer.Lex(program);
            Parser parser = new Parser(tokens);
            ProgramNode actualProgram = parser.Program;
            Assert.Equal(expectedProgram.GetProgramTreeString(), actualProgram.GetProgramTreeString());
        }

        [Fact]
        public void ProgramParsesEmptyMain()
        {
            string code = @"
int main()
{
}
";
            var program = new ProgramNode();
            program.AddChild(new FunctionNode(program, "main", new List<FunctionParameter>()));

            AssertResultMatchesExpectedProgram(code, program);
        }

        [Fact]
        public void ProgramParsesGlobalVariableDefinition()
        {
            string code = @"int foo;
int main()
{
}
";
            var program = new ProgramNode();
            program.AddChild(new VariableDeclarationNode(program, "int", "foo", null));
            program.AddChild(new FunctionNode(program, "main", new List<FunctionParameter>()));

            AssertResultMatchesExpectedProgram(code, program);
        }

        [Fact]
        public void ProgramParsesGlobalVariableDefinitionWithValue()
        {
            string code = @"int foo = 5;
int main()
{
}
";
            var program = new ProgramNode();
            program.AddChild(new VariableDeclarationNode(program, "int", "foo", "5"));
            program.AddChild(new FunctionNode(program, "main", new List<FunctionParameter>()));

            AssertResultMatchesExpectedProgram(code, program);
        }

        [Fact]
        public void ProgramParsesLocalVariableDefinition()
        {
            string code = @"
int main()
{
    int foo;
}
";
            var program = new ProgramNode();
            var function = new FunctionNode(program, "main", new List<FunctionParameter>());
            function.AddChild(new VariableDeclarationNode(function, "int", "foo", null));
            program.AddChild(function);

            AssertResultMatchesExpectedProgram(code, program);
        }

        [Fact]
        public void ProgramParsesLocalVariableDefinitionWithValue()
        {
            string code = @"
int main()
{
    int foo = 5;
}
";
            var program = new ProgramNode();
            var function = new FunctionNode(program, "main", new List<FunctionParameter>());
            function.AddChild(new VariableDeclarationNode(function, "int", "foo", "5"));
            program.AddChild(function);

            AssertResultMatchesExpectedProgram(code, program);
        }
    }
}
