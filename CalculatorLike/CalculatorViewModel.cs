using CalculatorLike.Base;
using CalculatorLike.Base.Model;
using CalculatorLike.Game;
using CalculatorLike.Game.Model;

namespace CalculatorLike;

class CalculatorViewModel
{
    private bool isInRoguelikeMode;

    public BasicCalculator BasicCalculator { get; private set; } = new();
    public RoguelikeCalculator RoguelikeCalculator { get; private set; }

    public event Action<bool>? OnGameFinished;

    public event Action? OnDivideByZeroInGame;
    public event Action? OnDivideByZero;

    public CalculatorViewModel()
    {
        RoguelikeCalculator = new(BasicCalculator);
        RoguelikeCalculator.OnGameFinished += OnGameWon;
    }
    
    public void AppendNumber(int number)
    {
        if (isInRoguelikeMode)
        {
            RoguelikeCalculator.AppendNumber(number);
        } 
        else
        {
            BasicCalculator.AppendNumber(number);
        }
    }

    public void SetOperation(CalculatorOperation operation)
    {
        if (isInRoguelikeMode)
        {
            try
            {
                RoguelikeCalculator.SetOperation(operation);
            }
            catch (DivideByZeroException)
            {
                OnDivideByZeroInGame?.Invoke();
            }
        } 
        else
        {
            try
            {
                BasicCalculator.SetOperation(operation);
            }
            catch (DivideByZeroException)
            {
                OnDivideByZero?.Invoke();
            }
        }
    }

    public void Calculate()
    {
        if (isInRoguelikeMode)
        {
            try
            {
                RoguelikeCalculator.Calculate();
            }
            catch (DivideByZeroException)
            {
                OnDivideByZeroInGame?.Invoke();
            }
        }
        else
        {
            try
            {
                BasicCalculator.Calculate();
            }
            catch (DivideByZeroException)
            {
                OnDivideByZero?.Invoke();
            }
        }
    }

    public void StartGame()
    {
        if (isInRoguelikeMode) return;

        isInRoguelikeMode = true;
        RoguelikeCalculator.StartGame();
    }

    public void ClearNumber()
    {
        if (isInRoguelikeMode)
        {
            RoguelikeCalculator.PerformSpecialAction(SpecialAction.Clear);
        }
        else
        {
            BasicCalculator.ClearNumber();
        }
    }

    private void OnGameWon(bool isWon)
    {
        if (isWon)
        {
            isInRoguelikeMode = false;
        }
        OnGameFinished?.Invoke(isWon);
    }
}
