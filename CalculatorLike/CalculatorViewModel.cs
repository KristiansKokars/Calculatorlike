using CalculatorLike.Base;
using CalculatorLike.Game;

namespace CalculatorLike;

class CalculatorViewModel
{
    private bool isInRoguelikeMode;

    public BasicCalculator BasicCalculator { get; private set; } = new();
    public RoguelikeCalculator RoguelikeCalculator { get; private set; }

    public event Action<bool>? OnGameFinished;

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
            RoguelikeCalculator.SetOperation(operation);
        } 
        else
        {
            BasicCalculator.SetOperation(operation);
        }
    }

    public void Calculate()
    {
        if (isInRoguelikeMode)
        {
            RoguelikeCalculator.Calculate();
        }
        else
        {
            BasicCalculator.Calculate();
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
            RoguelikeCalculator.ClearNumber();
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
