using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;

namespace CalculatorLike.Game.Model.Gambling;

class GamblingMachine
{
    private const bool IS_RIP_ENABLED = true;

    private readonly Random random = new();
    private readonly Timer timer = new();
    private readonly Wallet wallet;
    private int timeRemaining = 5000;
    private int moneyWon = 0;

    public bool HasConsentedToGamblingTOS { get; private set; }
    public int GamblingCost { get; private set; }

    public event Action<bool>? HasConsentedToGamblingTOSUpdated;
    public event Action<string?>? OnEventMessage;
    public event Action? OnRIP;
    public event Action<bool>? OnShouldShowSpecialOlinsPic;
    public event Action<int>? OnRerollActionsGained;

    public GamblingMachine(Wallet wallet)
    {
        this.wallet = wallet;
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
                OnEventMessage?.Invoke($"VID caught you not filing taxes on your earnings.\nFined all gambling profit of ${moneyWon}.");
                AddMoney(-moneyWon);
                break;
            case GamblingEvent.Nothing:
                OnEventMessage?.Invoke("Nothing :(");
                break;
            case GamblingEvent.Jackpot:
                var jackpotMoney = random.Next(20, 60);
                AddMoney(jackpotMoney);
                OnEventMessage?.Invoke($"BIG JACKPOT\n BIG WINNING OF ${jackpotMoney}");
                break;
            case GamblingEvent.SmallGift:
                var smallGift = random.Next(3, 12);
                AddMoney(smallGift);
                OnEventMessage?.Invoke($"You won!\nGain ${smallGift}");
                break;
            case GamblingEvent.KurtsAssists:
                var kurtsWinnings = random.Next(5, 15);
                AddMoney(kurtsWinnings);
                OnEventMessage?.Invoke($"Kurts has been doing well in VentaBet and shares some winnings.\nGain ${kurtsWinnings}");
                break;
            case GamblingEvent.JekabsWentAllIn:
                var jekabsRandomRoll = random.Next(1, 11);
                var didJekabsWin = jekabsRandomRoll > 8; // 20% chance of winning
                var wonOrLostText = didJekabsWin ? "Won" : "Lost";

                OnEventMessage?.Invoke($"Jēkabs went all in!!\n{wonOrLostText} ${wallet.Coins}.");
                AddMoney(didJekabsWin ? wallet.Coins : -wallet.Coins);
                break;
            case GamblingEvent.PicOfOlins:
                OnShouldShowSpecialOlinsPic?.Invoke(true);
                OnEventMessage?.Invoke(null);
                break;
            case GamblingEvent.OlinsWantsSnacks:
                var snackCost = random.Next(1, 5);
                AddMoney(-snackCost);
                OnEventMessage?.Invoke($"Oliņš wanted some snacks.\nSpent ${snackCost} on snacks.");
                break;
            case GamblingEvent.Reroll:
                var rerollCount = 5;
                OnRerollActionsGained?.Invoke(rerollCount);
                OnEventMessage?.Invoke($"Cukuriņš has descended from the trees to grant you valuable supplies in this fight.\nGain {rerollCount} reroll needed number actions.");
                break;
            case GamblingEvent.HopOnDeadlock:
                OnEventMessage?.Invoke("Hop on and play Deadlock");
                break;
            case GamblingEvent.CPUCrawlersAttack:
                var lostMoney = 15;
                AddMoney(-lostMoney);
                OnEventMessage?.Invoke($"Kurts CPU crawlers have breached containment!\nLose ${lostMoney} to these invaders.");
                break;
        }
    }

    private void AddMoney(int money)
    {
        moneyWon += money;
        wallet.Add(money);
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
        new(GamblingEvent.SpecialSurprise, 3),
        new(GamblingEvent.TaxEvasion, 2),
        new(GamblingEvent.Nothing, 25),
        new(GamblingEvent.Jackpot, 5),
        new(GamblingEvent.SmallGift, 15),
        new(GamblingEvent.KurtsAssists, 10),
        new(GamblingEvent.JekabsWentAllIn, 3),
        new(GamblingEvent.PicOfOlins, 6),
        new(GamblingEvent.OlinsWantsSnacks, 20),
        new(GamblingEvent.Reroll, 2),
        new(GamblingEvent.CPUCrawlersAttack, 3),
        new(GamblingEvent.HopOnDeadlock, 5)
    ];

    private static readonly int WeightSum = EventChances.Sum(item => item.Weight);
}
