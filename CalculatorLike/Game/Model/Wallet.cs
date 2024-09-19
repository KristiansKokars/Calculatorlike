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

    public bool TryPurchase(int cost)
    {
        if (!CanPurchase(cost)) return false;

        Coins -= cost;
        OnCoinsUpdated?.Invoke();

        return true;
    }

    public void Add(int amount)
    {
        Coins += amount;
        OnCoinsUpdated?.Invoke();
    }

    private bool CanPurchase(int amount)
    {
        return Coins >= amount;
    }
}
