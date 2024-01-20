using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Linq.Expressions;
using AutoHelper.Application.Common.Interfaces;
using Hangfire.Server;
using Hangfire.Console;
using Hangfire.Console.Progress;
using System;

namespace AutoHelper.Hangfire.Services;

internal class HangfireJobService : IQueueService
{
    private PerformContext? _context;
    private IProgressBar? _progress = null;

    public HangfireJobService()
    {
    }

    public void Initialize(PerformContext context)
    {
        _context = context;
    }

    public void LogInformation(string value, bool asProgress = false)
    {
        if (_context == null) throw new InvalidOperationException("PerformContext not initialized.");
        if(asProgress)
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
