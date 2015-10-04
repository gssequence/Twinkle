using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinkle.Models
{
    public class FilterScriptTemplate
    {
        public string Name { get; private set; }
        public string Script { get; private set; }

        public FilterScriptTemplate(string name, string script)
        {
            Name = name;
            Script = script;
        }
    }
}
