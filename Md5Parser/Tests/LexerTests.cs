using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Parser;

namespace Tests
{
    [TestFixture]
    public class LexerTests
    {
          const string SampleMd5FileContent = @"
MD5Version 10
commandline ""mesh models/ monsters / hellknight""

numJoints 3
numMeshes 1

joints {
    ""origin"" -1 ( 0 0 0 ) ( -0.7071067095 0 0 )		// 
    ""body""  0 ( 0 0.0000047749 80.1090087891 ) ( -0.7071067095 0 0 )		// origin
    ""body2"" 1 ( 0 0.0138006303 80.162979126 ) ( -0.7071067095 0 0 )		// body
}


mesh {
	// meshes: hellknightmesh
	shader ""models / hellknight / hellknight""

    numverts 6
    vert 0 ( 0.0306090005 0.6130819917 ) 0 2
    vert 1 ( 0.012623 0.6387490034 ) 2 3
    vert 2 ( 0.0384639986 0.6419249773 ) 5 2
    vert 3 ( 0.1679390073 0.5384820104 ) 7 4
    vert 4 ( 0.1329669952 0.5576809645 ) 11 1
    vert 5 ( 0.1573729962 0.5706579685 ) 12 4


    numtris 5
	tri 0 2 1 0
	tri 1 5 4 3
	tri 2 8 7 6
	tri 3 11 10 9
	tri 4 7 9 12

    numweights 3
	weight 0 5 0.7900747061 ( 1.9485528469 7.4760251045 23.7117900848 )
	weight 1 64 0.209925279 ( -2.1919908524 4.9578447342 1.4142047167 )
	weight 2 5 0.6999999881 ( 0.0828403607 5.4251484871 23.0223522186 )
}";

        private string _pathMesh;
        private string _pathAnim;

        [SetUp]
        public void SetUp()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            _pathMesh = Path.Combine(dir, "hand_LowPoly.md5mesh");
            _pathAnim = Path.Combine(dir, "hand_LowPoly.md5anim");
        }

        [Test]
        public void TestLexerSampleString()
        {
            var lexer = new Md5Lexer();

            lexer.Tokenize(SampleMd5FileContent)
                .ToList()
                .ForEach(Console.WriteLine);
        }

        [Test]
        public void TestParserSampleString()
        {
            var lexer = new Md5Lexer();
            var tokens = lexer.Tokenize(SampleMd5FileContent);

            var parser = new Md5MeshParser();
            var result = parser.Parse(tokens);

            Console.WriteLine(result);
        }

        [Test]
        public void TestLexerMeshFile()
        {
            var fileContent = File.ReadAllText(_pathMesh);

            var lexer = new Md5Lexer();

            lexer.Tokenize(fileContent)
                .ToList()
                .ForEach(Console.WriteLine);
        }

        [Test]
        public void TestParserMeshFile()
        {
            var fileContent = File.ReadAllText(_pathMesh);

            var tokens = new Md5Lexer().Tokenize(fileContent);

            var result = new Md5MeshParser().Parse(tokens);

            Console.WriteLine(result);
        }

        [Test]
        public void TestLexerAnimFile()
        {
            var fileContent = File.ReadAllText(_pathAnim);

            var lexer = new Md5Lexer();

            lexer.Tokenize(fileContent)
                .ToList()
                .ForEach(Console.WriteLine);
        }

        [Test]
        public void TestParserAnimFile()
        {
            var fileContent = File.ReadAllText(_pathAnim);

            var tokens = new Md5Lexer().Tokenize(fileContent);

            var result = new Md5AnimParser().Parse(tokens);

            Console.WriteLine(result);
        }

        [Test]
        public void TestSimpleMeshParser()
        {
            var parser = new ParserMd5();
            var mesh = parser.ParseMesh(_pathMesh);

            Console.WriteLine(mesh);
        }

        [Test]
        public void TestSimpleAnimParser()
        {
            var parser = new ParserMd5();
            var anim = parser.ParseAnim(_pathAnim);

            Console.WriteLine(anim);
        }
    }
}
