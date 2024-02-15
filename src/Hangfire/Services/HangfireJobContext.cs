using AutoHelper.Application.Common.Interfaces.Queue;
using Hangfire.Console;
using Hangfire.Console.Progress;
using Hangfire.Server;

namespace AutoHelper.Hangfire.Services;

internal class HangfireJobContext : IQueueContext
{
    private PerformContext? _context;
    private IProgressBar? _progress = null;

    public HangfireJobContext()
    {
    }

    public void Initialize(PerformContext context)
    {
        _context = context;
    }

    public void LogInformation(string value, bool asProgress = false)
    {
        if (_context == null) throw new InvalidOperationException("PerformContext not initialized.");
        if (asProgress)
        {
            _context.WriteProgressBar(value);
        }
        else
        {
            _context.WriteLine(value);
        }
    }

    public void LogWarning(string value)
    {
        if (_context == null) throw new InvalidOperationException("PerformContext not initialized.");

        _context.SetTextColor(ConsoleTextColor.Yellow);
        _context.WriteLine(value);
        _context.ResetTextColor();
    }

    public void LogError(string value)
    {
        if (_context == null) throw new InvalidOperationException("PerformContext not initialized.");

        _context.SetTextColor(ConsoleTextColor.Red);
        _context.WriteLine(value);
        _context.ResetTextColor();
    }

    public void LogProgress(int procentage, bool useNewInstance = false)
    {
        if (_context == null) throw new InvalidOperationException("PerformContext not initialized.");

        if (useNewInstance || _progress == null)
        {
            _progress = _context.WriteProgressBar();
        }

        _progress.SetValue(procentage);
    }

}
