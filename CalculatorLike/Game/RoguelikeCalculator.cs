using CalculatorLike.Base;
using CalculatorLike.Game.Gambling;
using Timer = System.Windows.Forms.Timer;

namespace CalculatorLike.Game;

class RoguelikeCalculator
{
    private const int COINS_PER_ROUND = 16;
    private const int MAX_ROUND_COUNT = 20;
    private const int DEFAULT_STARTING_NUMBER_USE_COUNT = 2;
    private const int DEFAULT_STARTING_OPERATION_USE_COUNT = 3;
    private const int TIME_TO_SOLVE_IN_SECONDS = 60;
    private const int SECONDS_REMOVED_PER_ROUND = 4;
    private const int TIMER_SECONDS_PER_TICK = 5;
    private const int OLINS_WARNING_AT_SECONDS = 30;
    private const int SHOP_ITEM_COUNT = 6;

    private Random random = new();
    private BasicCalculator calculator;
    private GamblingMachine gamblingMachine = new();
    private CalculatorOperation? currentOperation;
    private readonly Timer solutionTimer = new();
    private int secondsLeftForSolution = TIME_TO_SOLVE_IN_SECONDS * 2;
    private bool isShopping;

    public int NumberToGet { get; private set; }
    public int Round { get; private set; }
    public int Coins { get; private set; }
    public int RerollCost { get; private set; }
    public int GamblingCost { get { return gamblingMachine.GamblingCost; } }
    public Dictionary<int, int> NumberUses { get; private set; } = [];
    public Dictionary<CalculatorOperation, int> OperationUses { get; private set; } = [];
    public Dictionary<SpecialAction, int> SpecialActionUses { get; private set; } = [];
    public Dictionary<int, ShopItem> AvailableShopItems { get; private set; } = [];

    public event Action? OnNewRound;
    public event Action<int>? OnNumberUseUpdated;
    public event Action<int>? OnNumberToGetUpdated;
    public event Action<CalculatorOperation>? OnOperationUseUpdated;
    public event Action<SpecialAction>? OnSpecialActionUseUpdated;
    public event Action<bool>? OnGameFinished;
    public event Action<bool>? OnIsOlinsImpatient;
    public event Action<bool>? OnIsShoppingUpdated;
    public event Action? OnAvailableShopItemsUpdated;
    public event Action? OnCoinsUpdated;
    public event Action<int>? OnRerollCostUpdated;

    public event Action<bool>? HasConsentedToGamblingTOSUpdated
    {
        add { gamblingMachine.HasConsentedToGamblingTOSUpdated += value; }
        remove { gamblingMachine.HasConsentedToGamblingTOSUpdated -= value; }
    }
    public event Action? OnRIP
    {
        add { gamblingMachine.OnRIP += value; }
        remove { gamblingMachine.OnRIP -= value; }
    }
    public event Action<bool>? OnShouldShowSpecialOlinsPic
    {
        add { gamblingMachine.OnShouldShowSpecialOlinsPic += value; }
        remove { gamblingMachine.OnShouldShowSpecialOlinsPic -= value; }
    }
    public event Action<string?>? OnEventMessage
    {
        add { gamblingMachine.OnEventMessage += value; }
        remove { gamblingMachine.OnEventMessage -= value; }
    }

    public RoguelikeCalculator(BasicCalculator calculator)
    {
        this.calculator = calculator;
        gamblingMachine.OnMoneyEarned += Gamble_OnMoneyChanged;
        gamblingMachine.OnVID += Gamble_OnVID;
    }

    public void Gamble()
    {
        if (!gamblingMachine.HasConsentedToGamblingTOS) return;
        if (Coins < gamblingMachine.GamblingCost) return;

        SetCoins(Coins - gamblingMachine.GamblingCost);

        gamblingMachine.Gamble();
    }
    public void AppendNumber(int number)
    {
        if (NumberUses[number] == 0) return;
        if (isShopping) return;

        calculator.AppendNumber(number);
        OnNumberUsed(number);
    }

    public void SetOperation(CalculatorOperation operation)
    {
        if (OperationUses[operation] == 0) return;
        if (isShopping) return;

        calculator.SetOperation(operation);
        OnOperationUsed(operation);
    }

    public void PerformSpecialAction(SpecialAction specialAction)
    {
        if (SpecialActionUses[specialAction] == 0) return;
        if (isShopping) return;

        switch (specialAction)
        {
            case SpecialAction.Square:
                calculator.SetCalculatorNumber(calculator.CurrentInput * calculator.CurrentInput);
                break;
            case SpecialAction.SquareRoot:
                calculator.SetCalculatorNumber((int)Math.Floor(Math.Sqrt(calculator.CurrentInput)));
                break;
            case SpecialAction.CashToNumber:
                calculator.SetCalculatorNumber(calculator.CurrentInput + Coins);
                break;
            case SpecialAction.Modulus:
                calculator.SetOperation(CalculatorOperation.Modulus);
                break;
            case SpecialAction.Reroll:
                UpdateRandomNumberToGet();
                break;
        }
        OnSpecialActionUsed(specialAction);
    }

    public void ClearNumber()
    {
        if (isShopping) return;

        calculator.ClearNumber();
    }

    public void Calculate()
    {
        if (isShopping) return;

        try
        {
            calculator.Calculate();
        }
        catch (DivideByZeroException)
        {
            solutionTimer.Stop();
            throw;
        }

        var result = calculator.CurrentInput;

        if (result == NumberToGet)
        {
            FinishCurrentRound();
        }
    }

    public void StartGame()
    {
        UpdateRandomNumberToGet();
        Round = 1;
        Coins = 0;

        var randomStartingNumber = random.Next(1, 100);
        while (randomStartingNumber == NumberToGet)
        {
            randomStartingNumber = random.Next(1, 100);
        }
        calculator.SetCalculatorNumber(randomStartingNumber);

        NumberUses.Clear();
        for (int i = 0; i < 10; i++)
        {
            NumberUses.Add(i, DEFAULT_STARTING_NUMBER_USE_COUNT);
            OnNumberUseUpdated?.Invoke(i);
        }

        OperationUses.Clear();
        foreach (CalculatorOperation operation in Enum.GetValues(typeof(CalculatorOperation)))
        {
            OperationUses.Add(operation, DEFAULT_STARTING_OPERATION_USE_COUNT);
            OnOperationUseUpdated?.Invoke(operation);
        }

        SpecialActionUses.Clear();
        foreach (SpecialAction specialAction in Enum.GetValues(typeof(SpecialAction)))
        {
            SpecialActionUses.Add(specialAction, 0);
            OnSpecialActionUseUpdated?.Invoke(specialAction);
        }

        OnNewRound?.Invoke();
        solutionTimer.Interval = TIMER_SECONDS_PER_TICK * 1000;
        solutionTimer.Tick += SolutionTimer_Tick;
        solutionTimer.Start();
    }

    public void StartNextRound()
    {
        if (!isShopping) return;

        SetIsShopping(false);
        OnNewRound?.Invoke();
        OnIsOlinsImpatient?.Invoke(false);
        secondsLeftForSolution = TIME_TO_SOLVE_IN_SECONDS * 2 - Round * 4;
    }

    public void BuyShopItem(int itemIndex)
    {
        var itemToBuy = AvailableShopItems[itemIndex];
        if (itemToBuy.Cost > Coins) return;

        SetCoins(Coins - itemToBuy.Cost);

        if (itemToBuy is ShopItem.NumberItem numberItem)
        {
            NumberUses[numberItem.Number] = NumberUses[numberItem.Number] + 1;
            OnNumberUseUpdated?.Invoke(numberItem.Number);
        }
        if (itemToBuy is ShopItem.OperationItem operationItem)
        {
            OperationUses[operationItem.Operation] = OperationUses[operationItem.Operation] + 1;
            OnOperationUseUpdated?.Invoke(operationItem.Operation);
        }
        if (itemToBuy is ShopItem.SpecialActionItem specialActionItem)
        {
            SpecialActionUses[specialActionItem.SpecialAction] = SpecialActionUses[specialActionItem.SpecialAction] + 1;
            OnSpecialActionUseUpdated?.Invoke(specialActionItem.SpecialAction);
        }

        AvailableShopItems.Remove(itemIndex);
        OnAvailableShopItemsUpdated?.Invoke();
    }

    public void RerollShopItems()
    {
        if (RerollCost > Coins) return;

        SetCoins(Coins - RerollCost);

        GenerateNewShopItems(RerollCost);
    }

    public void AcceptGamblingTOS()
    {
        gamblingMachine.ConsentToGamblingTOS();
    }

    private void FinishCurrentRound()
    {
        Coins += COINS_PER_ROUND;
        Round += 1;
        NumberToGet = GenerateRandomNumberToGet();
        OnIsOlinsImpatient?.Invoke(false);
        secondsLeftForSolution = TIME_TO_SOLVE_IN_SECONDS * 2;

        if (Round == MAX_ROUND_COUNT + 1)
        {
            GameWon();
            return;
        }

        GenerateNewShopItems();
        SetIsShopping(true);
    }

    private void GenerateNewShopItems(int currentRerollCostSum = 0)
    {
        AvailableShopItems.Clear();
        // TODO: make it generate more based on rounds too
        int shopItemCount = random.Next(3, SHOP_ITEM_COUNT + 1);
        if (currentRerollCostSum == 0)
        {
            RerollCost = random.Next(3, 12);
        }
        else
        {
            RerollCost = currentRerollCostSum + random.Next(2, 12);
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
                specialActionItem.Cost = random.Next(1, 12);
            }

            AvailableShopItems.Add(i, shopItem);
        }

        OnAvailableShopItemsUpdated?.Invoke();
    }

    private void GameWon()
    {
        solutionTimer.Stop();
        OnGameFinished?.Invoke(true);
    }

    private void OnNumberUsed(int number)
    {
        NumberUses[number] = NumberUses[number] - 1;
        OnNumberUseUpdated?.Invoke(number);
    }

    private void OnOperationUsed(CalculatorOperation operation)
    {
        OperationUses[operation] = OperationUses[operation] - 1;
        OnOperationUseUpdated?.Invoke(operation);
    }

    private void OnSpecialActionUsed(SpecialAction specialAction)
    {
        SpecialActionUses[specialAction] = SpecialActionUses[specialAction] - 1;
        OnSpecialActionUseUpdated?.Invoke(specialAction);
    }

    private void UpdateRandomNumberToGet()
    {
       NumberToGet = GenerateRandomNumberToGet();
       OnNumberToGetUpdated?.Invoke(NumberToGet);
    }

    private int GenerateRandomNumberToGet()
    {
        int generatedNumber = NumberToGet;
        // TODO: could be moved up somewhere
        List<int> lastRounds = [16, 17, 18, 19];

        while (generatedNumber == NumberToGet)
        {
            if (Round == 20)
            {
                generatedNumber = random.Next(1000, 9999);
            }
            else if (lastRounds.Contains(Round))
            {
                generatedNumber = random.Next(1, 999);
            }
            else if (Round >= 5 && Round % 5 == 0)
            {
                generatedNumber = random.Next(100, 999);
            }
            else 
            {
                generatedNumber = random.Next(1, 100);
            }
        }

        return generatedNumber;
    }

    private void SetIsShopping(bool isShopping)
    {
        this.isShopping = isShopping;
        OnIsShoppingUpdated?.Invoke(isShopping);
    }

    private void SetCoins(int coins)
    {
        Coins = coins;
        OnCoinsUpdated?.Invoke();
    }

    private void SolutionTimer_Tick(object? sender, EventArgs e)
    {
        Console.WriteLine($"{secondsLeftForSolution}");

        secondsLeftForSolution -= TIMER_SECONDS_PER_TICK;

        if (secondsLeftForSolution <= OLINS_WARNING_AT_SECONDS)
        {
            OnIsOlinsImpatient?.Invoke(true);
        }

        if (secondsLeftForSolution < 0)
        {
            OnGameFinished?.Invoke(false);
            solutionTimer.Stop();
        }
    }

    private void Gamble_OnMoneyChanged(int money)
    {
        SetCoins(Coins + money);
    }

    private void Gamble_OnVID()
    {
        SetCoins(0);
    }
}
