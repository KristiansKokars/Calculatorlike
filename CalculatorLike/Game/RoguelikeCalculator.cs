using CalculatorLike.Base;
using Timer = System.Windows.Forms.Timer;

namespace CalculatorLike.Game;

/*
 * TODO list for game:
 * Show random shop to buy keys from
 * Add reroll to shop
 * Higher you go, the more difficult the number is
 */
class RoguelikeCalculator
{
    private const int COINS_PER_ROUND = 16;
    private const int MAX_ROUND_COUNT = 20;
    private const int DEFAULT_STARTING_USE_COUNT = 2;
    private const int TIME_TO_SOLVE_IN_SECONDS = 60;
    private const int SECONDS_REMOVED_PER_ROUND = 4;
    private const int TIMER_SECONDS_PER_TICK = 5;
    private const int OLINS_WARNING_AT_SECONDS = 30;
    private const int SHOP_ITEM_COUNT = 6;

    private Random random = new();
    private BasicCalculator calculator;
    private CalculatorOperation? currentOperation;
    private readonly Timer solutionTimer = new();
    private int secondsLeftForSolution = TIME_TO_SOLVE_IN_SECONDS * 2;

    public int NumberToGet { get; private set; }
    public int Round { get; private set; }
    public int Coins { get; private set; }
    public Dictionary<int, int> NumberUses { get; private set; } = [];
    public Dictionary<CalculatorOperation, int> OperationUses { get; private set; } = [];
    public Dictionary<int, ShopItem> AvailableShopItems { get; private set; } = [];

    public event Action? OnNewRound;
    public event Action<int>? OnNumberUseUpdated;
    public event Action<CalculatorOperation>? OnOperationUseUpdated;
    public event Action<bool>? OnGameFinished;
    public event Action<bool>? OnIsOlinsImpatient;

    public RoguelikeCalculator(BasicCalculator calculator)
    {
        this.calculator = calculator;
    }

    public void AppendNumber(int number)
    {
        if (NumberUses[number] == 0) return;

        calculator.AppendNumber(number);
        OnNumberUsed(number);
    }

    public void SetOperation(CalculatorOperation operation)
    {
        if (OperationUses[operation] == 0) return;

        calculator.SetOperation(operation);
        OnOperationUsed(operation);
    }

    public void ClearNumber()
    {
        calculator.ClearNumber();
    }

    public void Calculate()
    {
        calculator.Calculate();
        var result = calculator.CurrentInput;

        if (result == NumberToGet)
        {
            AdvanceRound();
        }

        calculator.SetCalculatorNumber(result);
    }

    public void StartGame()
    {
        NumberToGet = GenerateRandomNumberToGet();
        Round = 0;
        Coins = 0;

        var randomStartingNumber = GenerateRandomNumberToGet();
        while (randomStartingNumber == NumberToGet)
        {
            randomStartingNumber = GenerateRandomNumberToGet();
        }
        calculator.SetCalculatorNumber(randomStartingNumber);

        NumberUses.Clear();
        for (int i = 0; i < 10; i++)
        {
            NumberUses.Add(i, DEFAULT_STARTING_USE_COUNT);
            OnNumberUseUpdated?.Invoke(i);
        }

        OperationUses.Clear();
        foreach (CalculatorOperation operation in Enum.GetValues(typeof(CalculatorOperation)))
        {
            OperationUses.Add(operation, DEFAULT_STARTING_USE_COUNT);
            OnOperationUseUpdated?.Invoke(operation);
        }

        OnNewRound?.Invoke();
        solutionTimer.Interval = TIMER_SECONDS_PER_TICK * 1000;
        solutionTimer.Tick += SolutionTimer_Tick;
        solutionTimer.Start();
    }

    private void AdvanceRound()
    {
        Coins += COINS_PER_ROUND;
        NumberToGet = random.Next(1, 100);
        Round += 1;

        if (Round == MAX_ROUND_COUNT)
        {
            GameWon();
        }

        OnNewRound?.Invoke();
        OnIsOlinsImpatient?.Invoke(false);
        secondsLeftForSolution = TIME_TO_SOLVE_IN_SECONDS * 2 - Round * 4;
    }

    private void GenerateNewShopItems()
    {
        // TODO: make it generate more based on rounds too
        int shopItemCount = random.Next(1, SHOP_ITEM_COUNT + 1);

        for (int i = 0; i < shopItemCount; i++)
        {
            // create a random shop item and put it in that position
            // generate a random cost for that item based on the rounds
        }
    }

    private void GameWon()
    {
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

    private int GenerateRandomNumberToGet()
    {
        var generatedNumber = random.Next(1, 100);
        return generatedNumber;
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
}
