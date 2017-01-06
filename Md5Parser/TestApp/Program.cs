using System;
using System.IO;
using Parser;

namespace TestApp
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var pathMesh = Path.Combine(dir, "hand_LowPoly.md5mesh");

            var fileContent = File.ReadAllText(pathMesh);

            var tokens = new Md5Lexer().Tokenize(fileContent);

            var result = new Md5MeshParser().Parse(tokens);

            Console.ReadKey();
        }
    }
}