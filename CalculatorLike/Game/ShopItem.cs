using CalculatorLike.Base;

namespace CalculatorLike.Game;

abstract class ShopItem
{
    public abstract int Cost { get; set; }

    class NumberItem : ShopItem
    {
        public override int Cost { get; set; }
        public int Number { get; set; }
    }

    class OperationItem : ShopItem
    {
        public override int Cost { get; set; }
        public CalculatorOperation Operation { get; set; }
    }
}
