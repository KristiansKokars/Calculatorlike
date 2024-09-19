using CalculatorLike.Base;
using CalculatorLike.Base.Model;
using CalculatorLike.Game.Model;
using CalculatorLike.Game.Model.Gambling;
using CalculatorLike.Game.Model.Shop;
using Timer = System.Windows.Forms.Timer;

namespace CalculatorLike.Game;

class RoguelikeCalculator
{
    private static readonly List<int> lastRounds = [16, 17, 18, 19];

    private const int COINS_PER_ROUND = 18;
    private const int MAX_ROUND_COUNT = 20;
    private const int DEFAULT_STARTING_NUMBER_USE_COUNT = 2;
    private const int DEFAULT_STARTING_OPERATION_USE_COUNT = 3;
    private const int CLEAR_USE_COUNT = 2;
    private const int TIME_TO_SOLVE_IN_SECONDS = 60;
    private const int SECONDS_REMOVED_PER_ROUND = 3;
    private const int TIMER_SECONDS_PER_TICK = 5;
    private const int OLINS_WARNING_AT_SECONDS = 30;

    private readonly Random random = new();
    private readonly BasicCalculator calculator;
    private readonly GamblingMachine gamblingMachine;
    private readonly Wallet wallet = new();
    private readonly Shop shop;
    private readonly Inventory inventory = new();
    private readonly Timer solutionTimer = new();

    private int secondsLeftForSolution = TIME_TO_SOLVE_IN_SECONDS * 2;
    private bool isShopping;

    public long NumberToGet { get; private set; }
    public int Round { get; private set; }

    public event Action? OnNewRound;
    public event Action<long>? OnNumberToGetUpdated;
    public event Action<bool>? OnGameFinished;
    public event Action<bool>? OnIsOlinsImpatient;
    public event Action<bool>? OnIsShoppingUpdated;

    #region Wallet
    public int Coins { get { return wallet.Coins; } }

    public event Action? OnCoinsUpdated
    {
        add { wallet.OnCoinsUpdated += value; }
        remove { wallet.OnCoinsUpdated -= value; }
    }
    #endregion

    #region Shop
    public Dictionary<int, ShopItem> AvailableShopItems { get { return shop.AvailableShopItems; } }
    public int RerollCost { get { return shop.RerollCost; } }

    public event Action<int>? OnRerollCostUpdated
    {
        add { shop.OnRerollCostUpdated += value; }
        remove { shop.OnRerollCostUpdated -= value; }
    }

    public event Action? OnAvailableShopItemsUpdated
    {
        add { shop.OnAvailableShopItemsUpdated += value; }
        remove { shop.OnAvailableShopItemsUpdated -= value; }
    }
    #endregion

    #region Inventory
    public Dictionary<int, int> NumberUses { get { return inventory.NumberUses; } }
    public Dictionary<CalculatorOperation, int> OperationUses { get { return inventory.OperationUses; } }
    public Dictionary<SpecialAction, int> SpecialActionUses { get { return inventory.SpecialActionUses; } }

    public event Action<int>? OnNumberUseUpdated
    {
        add { inventory.OnNumberUseUpdated += value; }
        remove { inventory.OnNumberUseUpdated -= value; }
    }
    public event Action<CalculatorOperation>? OnOperationUseUpdated
    {
        add { inventory.OnOperationUseUpdated += value; }
        remove { inventory.OnOperationUseUpdated -= value; }
    }
    public event Action<SpecialAction>? OnSpecialActionUseUpdated
    {
        add { inventory.OnSpecialActionUseUpdated += value; }
        remove { inventory.OnSpecialActionUseUpdated -= value; }
    }
    #endregion

    #region Gambling
    public int GamblingCost { get { return gamblingMachine.GamblingCost; } }

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
    #endregion

    public RoguelikeCalculator(BasicCalculator calculator)
    {
        this.calculator = calculator;
        gamblingMachine = new(wallet);
        shop = new(wallet, inventory);
        gamblingMachine.OnRerollActionsGained += Gamble_OnRerollActionsGained;
    }

    public void Gamble()
    {
        if (!gamblingMachine.HasConsentedToGamblingTOS) return;

        var wasPurchased = wallet.TryPurchase(gamblingMachine.GamblingCost);
        if (!wasPurchased) return;

        // we do not want Oliņš timer to go down while you are actively gambling and rush you, to encourage more gambling
        secondsLeftForSolution = TIME_TO_SOLVE_IN_SECONDS * 2;

        gamblingMachine.Gamble();
    }

    public void AppendNumber(int number)
    {
        if (NumberUses[number] == 0) return;
        if (isShopping) return;

        calculator.AppendNumber(number);
        inventory.OnNumberUsed(number);
    }

    public void SetOperation(CalculatorOperation operation)
    {
        if (OperationUses[operation] == 0) return;
        if (isShopping) return;

        calculator.SetOperation(operation);
        inventory.OnOperationUsed(operation);
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
                calculator.SetCalculatorNumber(calculator.CurrentInput + wallet.Coins);
                break;
            case SpecialAction.Modulus:
                calculator.SetOperation(CalculatorOperation.Modulus);
                break;
            case SpecialAction.Reroll:
                UpdateRandomNumberToGet();
                break;
            case SpecialAction.Random1To100:
                calculator.SetCalculatorNumber(random.Next(1, 101));
                break;
            case SpecialAction.AddOrRemoveClosestOr10:
                var numberDifference = NumberToGet - calculator.CurrentInput;
                if (Math.Abs(numberDifference) > 10)
                {
                    numberDifference = 10 * (numberDifference / Math.Abs(numberDifference));
                }
                calculator.SetCalculatorNumber(calculator.CurrentInput + numberDifference);
                break;
            case SpecialAction.IncrementByOne:
                calculator.SetCalculatorNumber(calculator.CurrentInput + 1);
                break;
            case SpecialAction.Reverse:
                try
                {
                    var reversedNumber = long.Parse(Reverse(calculator.CurrentInput.ToString()));
                    calculator.SetCalculatorNumber(reversedNumber);
                    break;
                }
                catch (OverflowException)
                {
                    return;
                }
            case SpecialAction.Clear:
                calculator.ClearNumber();
                break;
        }
        inventory.OnSpecialActionUsed(specialAction);
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
        wallet.SetCoins(0);

        var randomStartingNumber = random.Next(1, 100);
        while (randomStartingNumber == NumberToGet)
        {
            randomStartingNumber = random.Next(1, 100);
        }
        calculator.SetCalculatorNumber(randomStartingNumber);

        NumberUses.Clear();
        for (int i = 0; i < 10; i++)
        {
            inventory.AddNumberUse(i, DEFAULT_STARTING_NUMBER_USE_COUNT);
        }

        OperationUses.Clear();
        foreach (CalculatorOperation operation in Enum.GetValues(typeof(CalculatorOperation)))
        {
            inventory.AddOperationUse(operation, DEFAULT_STARTING_OPERATION_USE_COUNT);
        }

        SpecialActionUses.Clear();
        foreach (SpecialAction specialAction in Enum.GetValues(typeof(SpecialAction)))
        {
            if (specialAction == SpecialAction.Clear)
            {
                inventory.AddSpecialActionUse(specialAction, CLEAR_USE_COUNT);
            }
            else
            {
                inventory.AddSpecialActionUse(specialAction, 0);
            }
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
        secondsLeftForSolution = TIME_TO_SOLVE_IN_SECONDS * 2 - Round * SECONDS_REMOVED_PER_ROUND;
    }

    public void BuyShopItem(int itemIndex)
    {
        shop.BuyShopItem(itemIndex);
    }

    public void RerollShopItems()
    {
        shop.RerollShopItems();
    }

    public void AcceptGamblingTOS()
    {
        gamblingMachine.ConsentToGamblingTOS();
    }

    private void FinishCurrentRound()
    {
        wallet.Add(COINS_PER_ROUND);
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
        shop.GenerateNewShopItems(currentRerollCostSum);
    }

    private void GameWon()
    {
        solutionTimer.Stop();
        OnGameFinished?.Invoke(true);
    }

    private void UpdateRandomNumberToGet()
    {
       NumberToGet = GenerateRandomNumberToGet();
       OnNumberToGetUpdated?.Invoke(NumberToGet);
    }

    private long GenerateRandomNumberToGet()
    {
        var generatedNumber = NumberToGet;

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

    private void Gamble_OnRerollActionsGained(int rerollCount)
    {
        inventory.AddSpecialActionUse(SpecialAction.Reroll, rerollCount);
    }

    private static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}
