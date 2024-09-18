using CalculatorLike.Base;

namespace CalculatorLike.Game.Shop;

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
        new(new OperationItem(CalculatorOperation.Add), 4),
        new(new OperationItem(CalculatorOperation.Subtract), 4),
        new(new OperationItem(CalculatorOperation.Multiply), 6),
        new(new OperationItem(CalculatorOperation.Divide), 6),
        new(new SpecialActionItem(SpecialAction.Modulus), 6),
        new(new SpecialActionItem(SpecialAction.Square), 5),
        new(new SpecialActionItem(SpecialAction.SquareRoot), 5),
        new(new SpecialActionItem(SpecialAction.CashToNumber), 3),
    ];

    public static List<WeightedItem<ShopItem>> ShopItems = [.. weightedNumberItems, .. weightedItems];

    public static int WeightSum = ShopItems.Sum(item => item.Weight);
}
