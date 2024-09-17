namespace CalculatorLike.Game;

class GamblingMachine
{
    private readonly Random random = new();

    public bool HasConsentedToGamblingTOS { get; private set; }
    public int GamblingCost { get; private set; }

    public event Action<bool>? HasConsentedToGamblingTOSUpdated;
    
    public GamblingMachine()
    {
        GamblingCost = random.Next(1, 4);
    }

    public void ConsentToGamblingTOS()
    {
        HasConsentedToGamblingTOS = true;
        HasConsentedToGamblingTOSUpdated?.Invoke(true);
    }

    public void Gamble()
    {
        // TODO: figure out all the gambling possibilities
        // TODO: set pity modifier to something fun
        /*
         * RIP
         * Rick Roll
         * Nothing :( (give it a lot of weight)
         * JACKPOT (lots of moni, 40-60)
         * Small gift (1-8 coins)
         * Kurts came to help you (0-20 coins)
         * Jekabs went all in (-10 coins)
         * Pic of Oliņš (no effect)
         * Oliņš wanted snacks (minus 1-3 coins)
         * VID caught you evading taxes on earnings (coins set to 0) (can only happen if won a total of 10+ coins)
         */
    }
}
