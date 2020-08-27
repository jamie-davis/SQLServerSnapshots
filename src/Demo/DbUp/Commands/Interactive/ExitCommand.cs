using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.InteractiveSession;

namespace DbUp.Commands.Interactive
{
    [InteractiveCommand]
    [Description("End the interactive command line session.")]
    class ExitCommand
    {
        [CommandHandler]
        public void Handle(IConsoleAdapter console, IInteractiveSessionService interactiveSessionService)
        {
            interactiveSessionService.EndSession();
        }
    }
}