using System;
using System.Diagnostics.CodeAnalysis;

namespace MarvelAPI.ConsoleTester
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Console Tester");
            var marvel = new Marvel("67d146c4c462f0b55bf12bb7d60948af", "54fd1a8ac788767cc91938bcb96755186074970b");
            var character = marvel.GetCharacter(1009268);
            Console.WriteLine(character.Name);
            var comic = marvel.GetComic(82325);
            Console.WriteLine(comic.Title);
            var creator = marvel.GetCreator(30);
            Console.WriteLine(creator.FullName);
        }
    }
}
