using CalculatorLike.Base.Model;

namespace CalculatorLike.Base;

class BasicCalculator
{
    private Int64? previousInput;
    private bool shouldSetNewNumber;
    private CalculatorOperation? currentOperation;

    public long CurrentInput { get; private set; }
    public event Action<long>? OnNumberUpdated;

    public void AppendNumber(long number)
    {
        try
        {
            SetCalculatorNumber(long.Parse($"{CurrentInput}{number}"));
        }
        catch (OverflowException)
        {
            // do nothing and do not change number
        }
    }

    public void Calculate()
    {
        if (previousInput is null || currentOperation is null)
        {
            return;
        }

        var previousNumber = (long)previousInput;
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

        Calculate();

        currentOperation = operation;
        previousInput = CurrentInput;
        CurrentInput = 0;
    }

    public void ClearNumber()
    {
        CurrentInput = 0;
        OnNumberUpdated?.Invoke(CurrentInput);
    }

    public void SetCalculatorNumber(long number)
    {
        CurrentInput = number;
        OnNumberUpdated?.Invoke(CurrentInput);
    }
}
