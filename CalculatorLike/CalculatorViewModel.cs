namespace CalculatorLike;

class CalculatorViewModel
{
    private int? previousInput;
    private bool shouldSetNewNumber;

    public int CurrentInput { get; private set; }
    public event Action<int> OnNumberUpdated;

    private const int COINS_PER_ROUND = 16;
    private const int MAX_ROUND_COUNT = 20;
    private const int DEFAULT_STARTING_USE_COUNT = 2;

    // Game
    private bool isInRoguelikeMode;
    private CalculatorOperation? currentOperation;
    private Random random = new();

    public int NumberToGet { get; private set; }
    public int Round { get; private set; }
    public int Coins { get; private set; }

    public event Action OnNewRound;

    public Dictionary<int, int> NumberUses { get; private set; } = [];
    public Action<int> OnNumberUseUpdated;
    public Dictionary<CalculatorOperation, int> OperationUses { get; private set; } = [];
    public Action<CalculatorOperation> OnOperationUseUpdated;

    public Action<bool> OnGameFinished;

    public void AppendNumber(int number)
    {
        if (isInRoguelikeMode && NumberUses[number] == 0) return;

        UpdateCalculatorNumber(int.Parse($"{CurrentInput}{number}"));

        if (isInRoguelikeMode)
        {
            OnNumberUsed(number);
        }
    }

    public void SetOperation(CalculatorOperation operation)
    {
        if (isInRoguelikeMode && OperationUses[operation] == 0) return;
        if (currentOperation == operation && CurrentInput == 0) return;

        if (currentOperation == operation)
        {
            Calculate();
        }

        currentOperation = operation;
        previousInput = CurrentInput;
        CurrentInput = 0;

        if (isInRoguelikeMode)
        {
            OnOperationUsed(operation);
        }
    }

    public void Calculate()
    {
        if (previousInput is null || currentOperation is null)
        {
            return;
        }

        var previousNumber = (int)previousInput;
        previousInput = null;

        // TODO: handle divide by zero
        var result = currentOperation switch
        {
            CalculatorOperation.Add => previousNumber + CurrentInput,
            CalculatorOperation.Subtract => previousNumber - CurrentInput,
            CalculatorOperation.Multiply => previousNumber * CurrentInput,
            CalculatorOperation.Divide => previousNumber / CurrentInput,
            _ => 0,
        };
        currentOperation = null;

        if (isInRoguelikeMode && result == NumberToGet)
        {
            AdvanceRound();
        }

        UpdateCalculatorNumber(result);
    }

    public void StartGame()
    {
        if (isInRoguelikeMode) return;

        isInRoguelikeMode = true;
        NumberToGet = GenerateRandomNumberToGet();
        Round = 0;
        Coins = 0;

        var randomStartingNumber = GenerateRandomNumberToGet();
        while (randomStartingNumber == NumberToGet)
        {
            randomStartingNumber = GenerateRandomNumberToGet();
        }
        CurrentInput = randomStartingNumber;

        NumberUses.Clear();
        for (int i = 0; i < 10; i++)
        {
            NumberUses.Add(i, DEFAULT_STARTING_USE_COUNT);
            OnNumberUseUpdated(i);
        }

        OperationUses.Clear();
        foreach (CalculatorOperation operation in Enum.GetValues(typeof(CalculatorOperation)))
        {
            OperationUses.Add(operation, DEFAULT_STARTING_USE_COUNT);
            OnOperationUseUpdated(operation);
        }

        OnNewRound?.Invoke();
    }

    public void ClearNumber()
    {
        CurrentInput = 0;
        OnNumberUpdated?.Invoke(CurrentInput);
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
    }

    private void GameWon()
    {
        OnGameFinished?.Invoke(true);
        isInRoguelikeMode = false;
    }

    private void UpdateCalculatorNumber(int number)
    {
        CurrentInput = number;
        OnNumberUpdated?.Invoke(CurrentInput);
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
}

/*
 * TODO list for game:
 * Show random shop to buy keys from
 * Add reroll to shop
 * Higher you go, the more difficult the number is
 * [Maybe] Custom powerup buttons
 */