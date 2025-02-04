﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TagsCloudVisualization.Factories;
using TagsCloudVisualization.ResultOf;

namespace TagsCloudVisualization.Visualization.Configurator
{
    public class WordsVisualizingConfigurator : IVisualizingConfigurator
    {
        private readonly IVisualizingTokenFactory tokenFactory;
        private readonly int fontSize;
        private readonly Func<string,Color> colorizer;

        public WordsVisualizingConfigurator(IVisualizingTokenFactory tokenFactory,
            Func<string, Color> colorizer, int fontSize)
        {
            this.tokenFactory = tokenFactory;
            this.colorizer = colorizer;
            this.fontSize = fontSize;
        }   
        
        public Result<IEnumerable<IVisualizingToken>> Configure(IEnumerable<string> visualizingValues)
        {
            var frequencyDict = GetFrequencyDict(visualizingValues);
            var fontCoefficient = fontSize / 10 * 2;
            
            return Result.Of(() => frequencyDict.Keys.Select(
                    value => tokenFactory.NewToken(value,
                        new Font(FontFamily.GenericSansSerif, fontSize + fontCoefficient * (frequencyDict[value] - 1)),
                        colorizer(value)))
                .ToArray())
                .Then(x => x as IEnumerable<IVisualizingToken>)
                .RefineError("Something wrong with colorizer");
        }

        private Dictionary<string, int> GetFrequencyDict(IEnumerable<string> visualizingValues)
        {
            var frequencyDict = new Dictionary<string, int>();
            foreach (var value in visualizingValues)
            {
                if (!frequencyDict.ContainsKey(value))
                {
                    frequencyDict[value] = 0;
                }
                
                frequencyDict[value]++;
            }

            return frequencyDict;
        }
    }
}