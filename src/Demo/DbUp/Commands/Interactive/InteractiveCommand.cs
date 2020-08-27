using System;
using System.Collections.Generic;
using System.Text;
using ConsoleToolkit;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.InteractiveSession;

namespace DbUp.Commands.Interactive
{
    [NonInteractiveCommand]
    [Description("Open an interactive command line session.")]
    class InteractiveCommand
    {
        [CommandHandler]
        public void Handle(IConsoleAdapter console, IInteractiveSessionService interactiveSessionService)
        {
            interactiveSessionService.SetPrompt("-->");
            interactiveSessionService.BeginSession();
        }
    }
}
