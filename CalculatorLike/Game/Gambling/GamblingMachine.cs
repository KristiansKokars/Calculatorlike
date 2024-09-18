using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;

namespace CalculatorLike.Game.Gambling;

class GamblingMachine
{
    private readonly Random random = new();
    private readonly Timer timer = new();
    private const bool IS_RIP_ENABLED = true;
    private int timeRemaining = 5000;

    public bool HasConsentedToGamblingTOS { get; private set; }
    public int GamblingCost { get; private set; }

    public event Action<bool>? HasConsentedToGamblingTOSUpdated;
    public event Action<string?>? OnEventMessage;
    public event Action? OnRIP;
    public event Action<int>? OnMoneyEarned;
    public event Action? OnVID;
    public event Action<bool>? OnShouldShowSpecialOlinsPic;

    public GamblingMachine()
    {
        GamblingCost = random.Next(1, 4);
        timer.Interval = 5000;
    }

    public void ConsentToGamblingTOS()
    {
        HasConsentedToGamblingTOS = true;
        HasConsentedToGamblingTOSUpdated?.Invoke(true);
    }

    public void Gamble()
    {
        OnShouldShowSpecialOlinsPic?.Invoke(false);

        var randomWeight = random.Next(0, WeightSum);
        var currentWeightSum = 0;

        GamblingEvent? eventThatHappened = null;
        foreach (var gamblingEvent in EventChances)
        {
            currentWeightSum += gamblingEvent.Weight;
            if (randomWeight < currentWeightSum)
            {
                eventThatHappened = gamblingEvent.Value;
                break;
            }
        }

        // Should never happen, but I will account for this case... sometime in the future.
        if (eventThatHappened == null) return;

        switch (eventThatHappened.Value)
        {
            case GamblingEvent.RIP:
                OnEventMessage?.Invoke("---");
                OnRIP?.Invoke();
                timer.Tick += Timer_TickRIP;
                timeRemaining = 5000;
                timer.Start();
                break;
            case GamblingEvent.SpecialSurprise:
                OnEventMessage?.Invoke("Opening up pictures of cute cats...");
                timer.Tick += Timer_TickSpecialSurprise;
                timeRemaining = 2000;
                timer.Start();
                break;
            case GamblingEvent.TaxEvasion:
                OnVID?.Invoke();
                OnEventMessage?.Invoke("VID caught you not filing taxes on your earnings.\nLose all money.");
                break;
            case GamblingEvent.Nothing:
                OnEventMessage?.Invoke("Nothing :(");
                break;
            case GamblingEvent.Jackpot:
                var jackpotMoney = random.Next(20, 60);
                OnMoneyEarned?.Invoke(jackpotMoney);
                OnEventMessage?.Invoke($"BIG JACKPOT\n BIG WINNING OF ${jackpotMoney}");
                break;
            case GamblingEvent.SmallGift:
                var smallGift = random.Next(3, 12);
                OnMoneyEarned?.Invoke(smallGift);
                OnEventMessage?.Invoke($"You won!\nGain ${smallGift}");
                break;
            case GamblingEvent.KurtsAssists:
                var kurtsWinnings = random.Next(5, 15);
                OnMoneyEarned?.Invoke(kurtsWinnings);
                OnEventMessage?.Invoke($"Kurts has been doing well in VentaBet and shares some winnings.\nGain ${kurtsWinnings}");
                break;
            case GamblingEvent.JekabsWentAllIn:
                var moneySpentInAllIn = 15;
                OnMoneyEarned?.Invoke(-moneySpentInAllIn);
                OnEventMessage?.Invoke($"Jēkabs went all in!!\nLose ${moneySpentInAllIn}.");
                break;
            case GamblingEvent.PicOfOlins:
                OnShouldShowSpecialOlinsPic?.Invoke(true);
                OnEventMessage?.Invoke(null);
                break;
            case GamblingEvent.OlinsWantsSnacks:
                var snackCost = random.Next(1, 5);
                OnMoneyEarned?.Invoke(-snackCost);
                OnEventMessage?.Invoke($"Oliņš wanted some snacks.\nSpent ${snackCost} on snacks.");
                break;
        }
    }

    private void Timer_TickSpecialSurprise(object? sender, EventArgs e)
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= 5000;
            return;
        }

        timer.Stop();
        timeRemaining = 5000;
        timer.Tick -= Timer_TickSpecialSurprise;
        Process.Start(new ProcessStartInfo { FileName = "https://www.youtube.com/watch?v=dQw4w9WgXcQ", UseShellExecute = true });
    }

    private void Timer_TickRIP(object? sender, EventArgs e)
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= 5000;
            return;
        }

        timer.Stop();
        timeRemaining = 5000;
        timer.Tick -= Timer_TickRIP;
        if (IS_RIP_ENABLED)
        {
            Process.Start("shutdown.exe", "-s -t 00");
        }
    }

    private static readonly List<WeightedItem<GamblingEvent>> EventChances =
    [
        new(GamblingEvent.RIP, 1),
        new(GamblingEvent.SpecialSurprise, 5),
        new(GamblingEvent.TaxEvasion, 3),
        new(GamblingEvent.Nothing, 30),
        new(GamblingEvent.Jackpot, 5),
        new(GamblingEvent.SmallGift, 15),
        new(GamblingEvent.KurtsAssists, 10),
        new(GamblingEvent.JekabsWentAllIn, 6),
        new(GamblingEvent.PicOfOlins, 5),
        new(GamblingEvent.OlinsWantsSnacks, 20),
    ];

    private static readonly int WeightSum = EventChances.Sum(item => item.Weight);
}
