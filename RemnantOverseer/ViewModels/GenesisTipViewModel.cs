using System;
using System.Threading;
using System.Threading.Tasks;

// Credit to the detectives on reddit and elsewhere that made this possible
// https://www.reddit.com/r/remnantgame/comments/1g4b01z/how_we_actually_solved_the_genesis_puzzle/

namespace RemnantOverseer.ViewModels;
public partial class GenesisTipViewModel: ViewModelBase, IDisposable
{
    private const int _cardID = 9358314;
    private const long _releaseDate = 638258940000000000; // new DateTime(2023, 07, 25, 15, 0, 0, DateTimeKind.Utc).Ticks
    private readonly int[] _offsets = [10, 0, 10, 0, 2, 8, 2, 8];

    private CancellationTokenSource _cancellationTokenSource { get; set; }

    public GenesisTipViewModel()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }

    private void SetSymbols()
    {
        var current = DateTime.UtcNow.Ticks;
        var next = DateTime.UtcNow.AddHours(1).Ticks;
        var currentSymbols = GetCode(current);
        var nextSymbols = GetCode(next);
        Console.WriteLine(currentSymbols);
        Console.WriteLine(nextSymbols);
    }

    private int[] GetCode(long time)
    {
        var hoursSinceRelease = (int)Math.Floor((decimal)(time - _releaseDate) / ((long)3600 * 1000 * 10000));
        var offsetIndex = hoursSinceRelease % 8;
        int code = hoursSinceRelease * _cardID + _offsets[offsetIndex]; // Overflow bros... it is our time
        var result = new int[4];
        for (int j = 3; j >= 0; j--)
        {
            var tmp = code % 10;
            result[j] = tmp < 0 ? tmp*-1 :tmp;
            code /= 10;
        }
        return result;
    }

    private async Task WaitForUpdate(CancellationToken cancellationToken)
    {
        var time = DateTime.Now.Minute;
        var span = TimeSpan.FromMinutes(60 - time);
        await Task.Delay(span, cancellationToken);
        SetSymbols();
    }
}
