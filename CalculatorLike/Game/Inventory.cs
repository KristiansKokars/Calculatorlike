using CalculatorLike.Base;

namespace CalculatorLike.Game;

class Inventory
{
    public Dictionary<int, int> NumberUses { get; private set; } = [];
    public Dictionary<CalculatorOperation, int> OperationUses { get; private set; } = [];
    public Dictionary<SpecialAction, int> SpecialActionUses { get; private set; } = [];

    public event Action<int>? OnNumberUseUpdated;
    public event Action<CalculatorOperation>? OnOperationUseUpdated;
    public event Action<SpecialAction>? OnSpecialActionUseUpdated;

    public void AddNumberUse(int number, int count)
    {
        if (NumberUses.ContainsKey(number))
        {
            NumberUses[number] = NumberUses[number] + count;
        }
        else
        {
            NumberUses.Add(number, count);
        }

        OnNumberUseUpdated?.Invoke(number);
    }

    public void AddOperationUse(CalculatorOperation operation, int count)
    {
        if (OperationUses.ContainsKey(operation))
        {
            OperationUses[operation] = OperationUses[operation] + count;
        }
        else
        {
            OperationUses.Add(operation, count);
        }

        OnOperationUseUpdated?.Invoke(operation);
    }

    public void AddSpecialActionUse(SpecialAction specialAction, int count)
    {
        if (SpecialActionUses.ContainsKey(specialAction))
        {
            SpecialActionUses[specialAction] = SpecialActionUses[specialAction] + count;
        } 
        else
        {
            SpecialActionUses.Add(specialAction, count);
        }

        OnSpecialActionUseUpdated?.Invoke(specialAction);
    }

    public void OnNumberUsed(int number)
    {
        NumberUses[number] = NumberUses[number] - 1;
        OnNumberUseUpdated?.Invoke(number);
    }

    public void OnOperationUsed(CalculatorOperation operation)
    {
        OperationUses[operation] = OperationUses[operation] - 1;
        OnOperationUseUpdated?.Invoke(operation);
    }

    public void OnSpecialActionUsed(SpecialAction specialAction)
    {
        SpecialActionUses[specialAction] = SpecialActionUses[specialAction] - 1;
        OnSpecialActionUseUpdated?.Invoke(specialAction);
    }
}
