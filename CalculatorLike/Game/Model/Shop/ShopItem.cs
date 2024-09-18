using CalculatorLike.Base.Model;

namespace CalculatorLike.Game.Model.Shop;

abstract class ShopItem
{
    public abstract int Cost { get; set; }

    public class NumberItem : ShopItem
    {
        public override int Cost { get; set; }
        public int Number { get; set; }

        public NumberItem(int number)
        {
            Number = number;
        }
    }

    public class OperationItem : ShopItem
    {
        public override int Cost { get; set; }
        public CalculatorOperation Operation { get; set; }

        public OperationItem(CalculatorOperation operation)
        {
            Operation = operation;
        }
    }

    public class SpecialActionItem : ShopItem
    {
        public override int Cost { get; set; }
        public SpecialAction SpecialAction { get; set; }

        public SpecialActionItem(SpecialAction specialAction)
        {
            SpecialAction = specialAction;
        }
    }

    private static List<WeightedItem<ShopItem>> weightedNumberItems = Enumerable
        .Range(0, 10)
        .Select(number => new WeightedItem<ShopItem>(new NumberItem(number), 6))
        .ToList();

    private static List<WeightedItem<ShopItem>> weightedItems = [
        new(new OperationItem(CalculatorOperation.Add), 6),
        new(new OperationItem(CalculatorOperation.Subtract), 6),
        new(new OperationItem(CalculatorOperation.Multiply), 8),
        new(new OperationItem(CalculatorOperation.Divide), 8),
        new(new SpecialActionItem(SpecialAction.Modulus), 8),
        new(new SpecialActionItem(SpecialAction.Square), 5),
        new(new SpecialActionItem(SpecialAction.SquareRoot), 5),
        new(new SpecialActionItem(SpecialAction.CashToNumber), 3),
        new(new SpecialActionItem(SpecialAction.Random1To100), 5),
        new(new SpecialActionItem(SpecialAction.AddOrRemoveClosestOr10), 3),
        new(new SpecialActionItem(SpecialAction.IncrementByOne), 5),
        new(new SpecialActionItem(SpecialAction.Reverse), 4),
    ];

    public static List<WeightedItem<ShopItem>> ShopItems = [.. weightedNumberItems, .. weightedItems];

    public static int WeightSum = ShopItems.Sum(item => item.Weight);
}
