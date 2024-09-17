namespace CalculatorLike.Base;

class BasicCalculator
{
    private int? previousInput;
    private bool shouldSetNewNumber;
    private CalculatorOperation? currentOperation;

    public int CurrentInput { get; private set; }
    public event Action<int>? OnNumberUpdated;

    public void AppendNumber(int number)
    {
        SetCalculatorNumber(int.Parse($"{CurrentInput}{number}"));
    }

    public void Calculate()
    {
        if (previousInput is null || currentOperation is null)
        {
            return;
        }

        var previousNumber = (int)previousInput;
        var result = currentOperation switch
        {
            CalculatorOperation.Add => previousNumber + CurrentInput,
            CalculatorOperation.Subtract => previousNumber - CurrentInput,
            CalculatorOperation.Multiply => previousNumber * CurrentInput,
            CalculatorOperation.Divide => previousNumber / CurrentInput,
            CalculatorOperation.Modulus => previousNumber % CurrentInput,
            _ => 0,
        };
        previousInput = null;
        currentOperation = null;
        SetCalculatorNumber(result);
    }

    public void SetOperation(CalculatorOperation operation)
    {
        if (currentOperation == operation && CurrentInput == 0) return;

        if (currentOperation == operation)
        {
            Calculate();
        }

        currentOperation = operation;
        previousInput = CurrentInput;
        CurrentInput = 0;
    }

    public void ClearNumber()
    {
        CurrentInput = 0;
        OnNumberUpdated?.Invoke(CurrentInput);
    }

    public void SetCalculatorNumber(int number)
    {
        CurrentInput = number;
        OnNumberUpdated?.Invoke(CurrentInput);
    }
}
