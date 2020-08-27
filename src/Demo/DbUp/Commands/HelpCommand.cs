using System;
using System.Collections.Generic;
using System.Text;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;

namespace DbUp.Commands
{
    [Command]
    [Description("Display help text.")]
    class HelpCommand
    {
        [Positional(DefaultValue = null)]
        [Description("The topic on which help is required.")]
        public List<string> Topic { get; set; }
    }
}
