using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCloudantWebstarter.Models
{
    public class ConfigSelector
    {
        public Selector selector { get; set; }
        public List<string> fields { get; set; }

        public class Selector
        {
        }

        public static ConfigSelector Build()
        {
            ConfigSelector configSelector = new ConfigSelector();
            configSelector.selector = new Selector();
            configSelector.fields = new List<string>();
            configSelector.fields.Add("environment");
            configSelector.fields.Add("superLotoTopic");
            return configSelector;
        }
    }
}
