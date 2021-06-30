using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
            var comics = marvel.GetComicsForCharacter(character.Id);
            var comic = comics.FirstOrDefault();
            Console.WriteLine(comic.Title);
            var creators = marvel.GetCreatorsForComic(comic.Id);
            var creator = creators.FirstOrDefault();
            Console.WriteLine(creator.FullName);
            var events = marvel.GetEventsForCharacter(character.Id);
            var marvelEvent = events.FirstOrDefault();
            Console.WriteLine(marvelEvent.Title);
            var series = marvel.GetSeriesForCharacter(character.Id);
            var firstSeries = series.FirstOrDefault();
            Console.WriteLine(firstSeries.Title);
        }
    }
}
