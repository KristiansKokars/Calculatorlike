namespace CalculatorLike.Game.Model;

class Wallet
{
    public int Coins { get; private set; }

    public event Action? OnCoinsUpdated;

    public void SetCoins(int coins)
    {
        Coins = coins;
        OnCoinsUpdated?.Invoke();
    }

    public bool CanPurchase(int amount)
    {
        return Coins >= amount;
    }

    // TODO: maybe make this a bool method
    public void Purchase(int amount)
    {
        if (amount > Coins) return;

        Coins -= amount;
        OnCoinsUpdated?.Invoke();
    }

    public void Add(int amount)
    {
        Coins += amount;
        OnCoinsUpdated?.Invoke();
    }
}
