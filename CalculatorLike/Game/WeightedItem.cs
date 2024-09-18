namespace CalculatorLike.Game;

class WeightedItem<T>
{
    public T Value { get; set; }
    public int Weight { get; set; }

    public WeightedItem(T value, int weight) {
        Value = value;
        Weight = weight;
    }
}
