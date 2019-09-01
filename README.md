# ChavLang

Compiler and build toolchain for a toy C-like language. Based on [Nora Sandler's tutorial series](https://norasandler.com/2017/11/29/Write-a-Compiler.html).

This toolchain is written in C# for .NET Core 3.0, so it should theoretically run on Windows, Linux, and Mac OS.

Work in progress - it can't actually build programs yet.

## Why?

I have never written a compiler before, so it's a learning exercise. I also often write CTF challenges and crackmes that utilise VMs/emulators for custom toy architectures, and having my own compile toolchain that I can use for those architectures makes it much easier to write code for them.

## So it's a C compiler written in C#?

Nope! The syntax is very similar and many trivial C programs are also valid ChavLang programs, but this is absolutely not a standards-compliant C compiler. Many modern C features will be missing and some syntax will be different.

## How is the code structured?

There are three main projects:

* **ChavLang**: A library that contains the implementation of the compiler.
* **ChavLangTest**: Contains tests for ChavLang.
* **ChavBuild**: Command line tool for compiling programs.

The ChavLang library is split into a number of parts:

* **Lexer**: Takes input program code as a string and converts it into a sequence of tokens. See `Lexer.cs` and the `Tokens` directory.
* **Parser**: Takes a sequence of tokens from the lexer and converts it into a syntax tree (AST). See `Parser.cs` and the `Nodes` directory.

### How do the lexer and parser work internally?

The general approach is to use a set of regular expressions (regexes) to do the bulk of the work.

The lexer loops through a set of regexes to find valid tokens at the current file pointer, then moves along and repeats the process until it hits the end of the file.

The parser turns the given sequence of tokens into a string representation in order to use regex in a similar manner to the lexer. The major difference is that the parser maintains state information about where it is in the syntax tree in order to select which syntax regexes are valid - for example, a function definition is only valid at the program scope (because ChavLang does not support nested functions).

### Adding a new token to the lexer

Adding a new lexer token manually is fairly easy:

1. Create a new class in the `Tokens` directory for your token type, using the `FooBarToken` naming convention. The class must derive from `BaseToken` and must implement the `base(contents)` constructor.
2. In `Lexer.cs`, add a new `Regex` instance to the token regexes at the top of the code for your new token, preferably keeping the declarations in precedence order.
3. In `Lexer.cs`, add a new entry to the `_tokenPatternMap` dictionary in the constructor in order to map the regex you created in step 2 to the new token class you created in step 1.

To help speed up the process of creating a lot of new lexer tokens, there's a LinqPad script called `TokenMapGenerator.linq` which automatically generates default token class files for a given list of names, and generates the token pattern map for placing in the `Lexer` class constructor. If you're adding a new token I generally suggest starting by updating the list in this script and then running it. You just need to change the path at the top to match the location of the repository on your disk. Make sure `replaceFiles` is set to false, as setting it to true destroys all changes in the existing `*Token.cs` files.

### Adding new syntax to the parser

Steps as follows:

1. Create a new node class inside of the `Nodes` directory, and inherit from `NodeBase`. Override the `ToString` method to provide proper output for the human-readable tree view.
2. In `Parser.cs`, add a new `Regex` instance to the syntax regexes at the top of the file.
3. In `Parser.cs`, inside the constructor, add your new syntax regex variable to the `_validSyntaxForLocation` dictionary in order to specify where the syntax can be used.
4. In `Parser.cs`, add a new function to parse your new syntax. Refer to the other parse functions for more information.
5. In `Parser.cs`, inside the constructor, add a new entry to the `_syntaxHandlers` dictionary that maps the syntax regex variable you created in step 2 to the function you created in step 4.

There is not currently a helper script for this because there's a lot of manual work involved in writing new parse functions.

# License

Code is MIT licensed. See the `LICENSE` file for full text.
