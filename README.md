# ChavLang

Compiler and build toolchain for a toy C-like language. Based on [Nora Sandler's tutorial series](https://norasandler.com/2017/11/29/Write-a-Compiler.html).

This toolchain is written in C# for .NET Core 3.0, so it should theoretically run on Windows, Linux, and Mac OS.

Work in progress - it can't actually build programs yet.

## Why?

I have never written a compiler before, so it's a learning exercise. I also often write CTF challenges and crackmes that utilise VMs/emulators for custom toy architectures, and having my own compile toolchain that I can use for those architectures makes it much easier to write code for them.

## So it's a C compiler written in C#?

Nope! The syntax is very similar and many trivial C programs are also valid ChavLang programs, but this is absolutely not a standards-compliant C compiler. Many modern C features will be missing and some syntax will be different.
