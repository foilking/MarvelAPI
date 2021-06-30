using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;


namespace MarvelAPI.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MarvelAPIVersions>();
            Console.WriteLine(summary);
        }
    }


    [Config(typeof(Config))]
    public class MarvelAPIVersions
    {
        private readonly MarvelAPI.Marvel marvelObject = new MarvelAPI.Marvel("67d146c4c462f0b55bf12bb7d60948af", "54fd1a8ac788767cc91938bcb96755186074970b");

        private class Config : ManualConfig
        {
            public Config()
            {
                var baseJob = Job.MediumRun;

                AddJob(baseJob.WithNuGet("MarvelAPI", "0.1.2.2").WithId("0.1.2.2"));
            }
        }

        [Benchmark]
        public void GetCharacter()
            => marvelObject.GetCharacter(1009268);

    }
}
