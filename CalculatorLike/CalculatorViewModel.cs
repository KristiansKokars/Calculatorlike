using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorLike;

class CalculatorViewModel
{
    private int? previousInput;
    private int currentInput;
    private bool shouldSetNewNumber;

    private bool isInRoguelikeMode;

    private CalculatorOperation? currentOperation;
    private Random random = new();

    public int NumberToGet { get; private set; }

    public void SetCurrentNumber(int value)
    {
        if (shouldSetNewNumber)
        {
            previousInput = currentInput;
        }

        currentInput = value;
    }

    public void SetOperation(CalculatorOperation? operation)
    {
        currentOperation = operation;
        shouldSetNewNumber = true;
    }

    public int Calculate()
    {
        if (previousInput is null || currentOperation is null)
        {
            return currentInput;
        }

        var previousNumber = (int)previousInput;
        previousInput = null;

        // TODO: handle divide by zero
        var result = currentOperation switch
        {
            CalculatorOperation.Add => previousNumber + currentInput,
            CalculatorOperation.Subtract => previousNumber - currentInput,
            CalculatorOperation.Multiply => previousNumber * currentInput,
            CalculatorOperation.Divide => previousNumber / currentInput,
            _ => 0,
        };
        currentOperation = null;
        return result;
    }

    public void StartGame()
    {
        isInRoguelikeMode = true;
    }

    public int GenerateRandomNumberToGet()
    {
        NumberToGet = random.Next(1, 100);
        return NumberToGet;
    }
}

/*
 * TODO list for game:
 * Implement button to toggle beginning
 * Track rounds between numbers
 * Track uses for each button
 * Give coins for each round
 * Show random shop to buy keys from
 * Randomly generate the number you need
 * Higher you go, the more difficult the number is
 * Show end screen or something and reset calculator back
 * Add reroll to shop
 * [Maybe] Custom powerup buttons
 */