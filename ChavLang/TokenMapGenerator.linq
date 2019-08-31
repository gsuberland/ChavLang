<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Drawing.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.IO.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Numerics.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Numerics.Vectors.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Security.dll</Reference>
  <Namespace>System.Collections</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Collections.Specialized</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.IO</Namespace>
  <Namespace>System.IO.MemoryMappedFiles</Namespace>
  <Namespace>System.IO.Pipes</Namespace>
  <Namespace>System.IO.Ports</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Sockets</Namespace>
  <Namespace>System.Numerics</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Runtime.Serialization</Namespace>
  <Namespace>System.Runtime.Serialization.Formatters.Binary</Namespace>
  <Namespace>System.Security</Namespace>
  <Namespace>System.Security.AccessControl</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
  <Namespace>System.Security.Principal</Namespace>
  <Namespace>System.Text</Namespace>
  <Namespace>System.Threading</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

string folder = @"C:\Users\Graham\Source\Repos\ChavLang\ChavLang\Tokens\";
string template = File.ReadAllText(folder + "_TemplateToken.cs.template");

// WARNING: If you enable this, all *Token.cs files in ChavLang\Tokens will be replaced with default versions.
// This will DESTROY ANY CHANGES in those files.
bool replaceFiles = false;

string[] tokens = {
	"OpenBraceToken",
	"CloseBraceToken",
	"OpenParenToken",
	"CloseParenToken",
	"CommaToken",
	"SemicolonToken", 
	"MinusToken", 
	"ComplimentToken", 
	"NegationToken", 
	"AdditionToken", 
	"MultiplyToken", 
	"DivideToken", 
	"LogicalAndToken", 
	"LogicalOrToken", 
	"BitwiseAndToken", 
	"BitwiseOrToken", 
	"EqualityToken", 
	"AssignmentToken", 
	"TypeKeywordToken", 
	"ReturnKeywordToken", 
	"IfKeywordToken", 
	"ElseKeywordToken", 
	"IdentifierToken", 
	"IntegerLiteralToken"
};

foreach (string token in tokens)
{
	string path = folder + token + ".cs";
	if (!replaceFiles && File.Exists(path))
	{
		continue;
	}
	string contents = template.Replace("$CLASSNAME$", token);
	File.WriteAllText(path, contents);
}

StringBuilder tokenMap = new StringBuilder();
tokenMap.AppendLine(@"_tokenPatternMap = new Dictionary<Regex, Type>() {");
foreach (string token in tokens)
{
	string regexName = @"{ _" + token.First().ToString().ToLower() + token.Substring(1) + "Regex,";
	regexName = regexName.PadRight(40);
	tokenMap.AppendLine("\t" + regexName + "typeof(" + token + ") },");
}
tokenMap.AppendLine("};");
Console.WriteLine(tokenMap.ToString());