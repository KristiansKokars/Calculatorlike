namespace CalculatorLike.Game.Model.Shop;

class Shop(Wallet wallet, Inventory inventory)
{
    private const int SHOP_ITEM_COUNT = 6;

    private readonly Random random = new();

    public Dictionary<int, ShopItem> AvailableShopItems { get; private set; } = [];
    public int RerollCost { get; private set; }

    public event Action<int>? OnRerollCostUpdated;
    public event Action? OnAvailableShopItemsUpdated;

    public void BuyShopItem(int itemIndex)
    {
        var itemToBuy = AvailableShopItems[itemIndex];
        if (!wallet.CanPurchase(itemToBuy.Cost)) return;

        wallet.Purchase(itemToBuy.Cost);

        if (itemToBuy is ShopItem.NumberItem numberItem)
        {
            inventory.AddNumberUse(numberItem.Number, 1);
        }
        if (itemToBuy is ShopItem.OperationItem operationItem)
        {
            inventory.AddOperationUse(operationItem.Operation, 1);
        }
        if (itemToBuy is ShopItem.SpecialActionItem specialActionItem)
        {
            inventory.AddSpecialActionUse(specialActionItem.SpecialAction, 1);
        }

        AvailableShopItems.Remove(itemIndex);
        OnAvailableShopItemsUpdated?.Invoke();
    }

    public void RerollShopItems()
    {
        if (!wallet.CanPurchase(RerollCost)) return;

        wallet.Purchase(RerollCost);

        GenerateNewShopItems(RerollCost);
    }

    public void GenerateNewShopItems(int currentRerollCostSum = 0)
    {
        AvailableShopItems.Clear();
        int shopItemCount = random.Next(SHOP_ITEM_COUNT - 2, SHOP_ITEM_COUNT + 1);
        if (currentRerollCostSum == 0)
        {
            RerollCost = random.Next(3, 11);
        }
        else
        {
            RerollCost = currentRerollCostSum + random.Next(2, 9);
        }
        OnRerollCostUpdated?.Invoke(RerollCost);

        for (int i = 0; i < shopItemCount; i++)
        {
            // create a random shop item and put it in that position
            // generate a random cost for that item based on the rounds
            var randomWeight = random.Next(0, ShopItem.WeightSum);
            var currentWeightSum = 0;

            ShopItem? shopItem = null;
            foreach (var item in ShopItem.ShopItems)
            {
                currentWeightSum += item.Weight;
                if (randomWeight < currentWeightSum)
                {
                    shopItem = item.Value;
                    break;
                }
            }

            if (shopItem == null)
            {
                i--;
                continue;
            }

            // TODO: make it generate more cost based on the round possibly
            if (shopItem is ShopItem.NumberItem numberShopItem)
            {
                numberShopItem.Cost = random.Next(3, 10);
            }
            if (shopItem is ShopItem.OperationItem operationShopItem)
            {
                operationShopItem.Cost = random.Next(1, 8);
            }
            if (shopItem is ShopItem.SpecialActionItem specialActionItem)
            {
                specialActionItem.Cost = random.Next(3, 12);
            }

            AvailableShopItems.Add(i, shopItem);
        }

        OnAvailableShopItemsUpdated?.Invoke();
    }
}
