﻿using CalculatorLike.Base.Model;
using CalculatorLike.Game.Model;
using CalculatorLike.Game.Model.Shop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CalculatorLike;

public partial class Form1 : Form
{
    private readonly CalculatorViewModel viewModel;
    private readonly List<Label> allLabelNumberUses;
    private readonly List<Label> allLabelOperationUses;
    private readonly List<Control> allGameElements;
    private readonly List<Control> allShopItemElements;
    private readonly List<Control> allShopItemCostElements;
    private readonly List<Control> allSpecialActionElements;

    public Form1()
    {
        InitializeComponent();
        viewModel = new();

        viewModel.BasicCalculator.OnNumberUpdated += OnNumberUpdated;
        viewModel.OnDivideByZero += OnDivideByZero;
        viewModel.RoguelikeCalculator.OnNumberUseUpdated += OnNumberUseUpdated;
        viewModel.RoguelikeCalculator.OnOperationUseUpdated += OnOperationUseUpdated;
        viewModel.RoguelikeCalculator.OnSpecialActionUseUpdated += OnSpecialActionUseUpdated;
        viewModel.RoguelikeCalculator.OnNewRound += OnNewRound;
        viewModel.RoguelikeCalculator.OnIsOlinsImpatient += OnIsOlinsImpatient;
        viewModel.RoguelikeCalculator.OnNumberToGetUpdated += OnNumberToGetUpdated;
        viewModel.RoguelikeCalculator.OnAvailableShopItemsUpdated += OnAvailableShopItemsUpdated;
        viewModel.RoguelikeCalculator.OnIsShoppingUpdated += IsShoppingUpdated;
        viewModel.RoguelikeCalculator.OnCoinsUpdated += OnCoinsUpdated;
        viewModel.RoguelikeCalculator.OnRerollCostUpdated += OnRerollCostUpdated;
        viewModel.RoguelikeCalculator.HasConsentedToGamblingTOSUpdated += HasConsentedToGamblingTOSUpdated;
        viewModel.RoguelikeCalculator.OnRIP += OnRIP;
        viewModel.RoguelikeCalculator.OnShouldShowSpecialOlinsPic += OnShouldShowSpecialOlinsPic;
        viewModel.RoguelikeCalculator.OnEventMessage += OnEventMessage;
        viewModel.OnDivideByZeroInGame += OnDivideByZeroInGame;
        viewModel.OnGameFinished += OnGameFinished;

        labelGamblingCost.Text = $"${viewModel.RoguelikeCalculator.GamblingCost}";

        allLabelNumberUses = [labelUses0, labelUses1, labelUses2, labelUses3, labelUses4, labelUses5, labelUses6, labelUses7, labelUses8, labelUses9];
        allLabelOperationUses = [labelUsesAdd, labelUsesDivide, labelUsesMultiply, labelUsesSubtract];
        allGameElements = [panelTermsAndConditions, labelShopTitle, panelShop, buttonShopItem1, buttonShopItem2, buttonShopItem3, buttonShopItem4, buttonShopItem5, buttonShopItem6, labelShopItem1Cost, labelShopItem2Cost, labelShopItem3Cost, labelShopItem4Cost, labelShopItem5Cost, labelShopItem6Cost, buttonReroll, groupGame, labelYouNeed, labelRound, labelCoins, btnRoguelikeCalculate, labelNumberToGet, labelRoundCount, labelCoinCount, labelUses0, labelUses1, labelUses2, labelUses3, labelUses4, labelUses5, labelUses6, labelUses7, labelUses8, labelUses9, labelUsesAdd, labelUsesDivide, labelUsesMultiply, labelUsesSubtract, labelUsesClear];
        allShopItemElements = [buttonShopItem1, buttonShopItem2, buttonShopItem3, buttonShopItem4, buttonShopItem5, buttonShopItem6];
        allShopItemCostElements = [labelShopItem1Cost, labelShopItem2Cost, labelShopItem3Cost, labelShopItem4Cost, labelShopItem5Cost, labelShopItem6Cost];
        allSpecialActionElements = [labelSpecialModulus, btnSpecialModulus, labelSpecialSquareRoot, btnSpecialSqrt, labelSpecialSquare, btnSpecialSquare, labelSpecialCashToMoney, btnSpecialCashToNumber, labelUsesRandom1To100, btnSpecialRandom1To100, labelUsesAddOrRemoveClosestOr10, btnSpecialAddOrRemoveClosestOr10, labelUsesIncrementByOne, btnIncrementByOne, labelUsesReverse, btnReverse, labelSpecialReroll, btnSpecialReroll,];
    }

    #region Calculator
    private void OnNumberUpdated(long number)
    {
        tbNumbers.Text = number.ToString();
    }

    private void btn1_Click(object sender, EventArgs e)
    {
        viewModel.AppendNumber(1);
    }

    private void btn2_Click(object sender, EventArgs e)
    {
        viewModel.AppendNumber(2);
    }

    private void btn3_Click(object sender, EventArgs e)
    {
        viewModel.AppendNumber(3);
    }

    private void btn4_Click(object sender, EventArgs e)
    {
        viewModel.AppendNumber(4);
    }

    private void btn5_Click(object sender, EventArgs e)
    {
        viewModel.AppendNumber(5);
    }

    private void btn6_Click(object sender, EventArgs e)
    {
        viewModel.AppendNumber(6);
    }

    private void btn7_Click(object sender, EventArgs e)
    {
        viewModel.AppendNumber(7);
    }

    private void btn8_Click(object sender, EventArgs e)
    {
        viewModel.AppendNumber(8);
    }

    private void btn9_Click(object sender, EventArgs e)
    {
        viewModel.AppendNumber(9);
    }

    private void btn0_Click(object sender, EventArgs e)
    {
        viewModel.AppendNumber(0);
    }

    private void btnplus_Click(object sender, EventArgs e)
    {
        setOperation(CalculatorOperation.Add);
    }

    private void btnSubtract_Click(object sender, EventArgs e)
    {
        setOperation(CalculatorOperation.Subtract);
    }

    private void btnMultiply_Click(object sender, EventArgs e)
    {
        setOperation(CalculatorOperation.Multiply);
    }

    private void btnDivide_Click(object sender, EventArgs e)
    {
        setOperation(CalculatorOperation.Divide);
    }

    private void btnCalculate_Click(object sender, EventArgs e)
    {
        viewModel.Calculate();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
        viewModel.ClearNumber();
    }

    private void setOperation(CalculatorOperation operation)
    {
        viewModel.SetOperation(operation);
    }

    private void OnDivideByZero()
    {
        MessageBox.Show("Tried to divide by zero, undoing that action", "Divide by Zero");
    }
    #endregion

    #region Game
    private void btnRoguelike_Click(object sender, EventArgs e)
    {
        viewModel.StartGame();

        foreach (var element in allGameElements)
        {
            element.Visible = true;
        }

        labelYouWon.Visible = false;
        btnRoguelike.Visible = false;
        tbNumbers.Text = viewModel.BasicCalculator.CurrentInput.ToString();

        foreach (var label in allLabelNumberUses)
        {
            label.Visible = true;
        }
        foreach (var label in allLabelOperationUses)
        {
            label.Visible = true;
        }
    }

    private void OnIsOlinsImpatient(bool isImpatient)
    {
        labelOlinsMessage.Text = "Oliņš grows impatient...";
        labelOlinsMessage.ForeColor = Color.DarkRed;
        labelOlinsMessage.Visible = isImpatient;
    }

    private void OnNumberToGetUpdated(long numberToGet)
    {
        labelNumberToGet.Text = numberToGet.ToString();
    }

    private void OnGameFinished(bool isWon)
    {
        foreach (var element in allGameElements)
        {
            element.Visible = false;
        }
        foreach (var element in allSpecialActionElements)
        {
            element.Visible = false;
        }
        labelOlinsMessage.Visible = false;

        if (isWon)
        {
            labelYouWon.Visible = true;
            btnRoguelike.Visible = true;
        }
        else
        {
            pictureGameLost.Visible = true;
            BackColor = Color.Black;
        }
    }

    private void OnNewRound()
    {
        labelNumberToGet.Text = viewModel.RoguelikeCalculator.NumberToGet.ToString();
        labelRoundCount.Text = viewModel.RoguelikeCalculator.Round.ToString();
        SyncCoinCount();
        buttonContinueRound.Visible = false;
    }

    private void OnNumberUseUpdated(int number)
    {
        var uses = viewModel.RoguelikeCalculator.NumberUses[number];

        allLabelNumberUses[number].Text = uses.ToString();
        allLabelNumberUses[number].ForeColor = ColorForUseCount(uses);
    }

    private void OnOperationUseUpdated(CalculatorOperation operation)
    {
        var uses = viewModel.RoguelikeCalculator.OperationUses[operation];
        var usesText = uses.ToString();
        var color = ColorForUseCount(uses);

        switch (operation)
        {
            case CalculatorOperation.Add:
                labelUsesAdd.Text = usesText;
                labelUsesAdd.ForeColor = color;
                return;
            case CalculatorOperation.Subtract:
                labelUsesSubtract.Text = usesText;
                labelUsesSubtract.ForeColor = color;
                return;
            case CalculatorOperation.Multiply:
                labelUsesMultiply.Text = usesText;
                labelUsesMultiply.ForeColor = color;
                return;
            case CalculatorOperation.Divide:
                labelUsesDivide.Text = usesText;
                labelUsesDivide.ForeColor = color;
                return;
        }
    }

    private Color ColorForUseCount(int useCount)
    {
        if (useCount > 0)
        {
            return Color.Green;
        }
        else
        {
            return Color.DarkRed;
        }
    }

    private void OnSpecialActionUseUpdated(SpecialAction specialAction)
    {
        var uses = viewModel.RoguelikeCalculator.SpecialActionUses[specialAction];
        var usesText = uses.ToString();
        var color = ColorForUseCount(uses);

        switch (specialAction)
        {
            case SpecialAction.Square:
                if (uses >= 1)
                {
                    labelSpecialSquare.Visible = true;
                    btnSpecialSquare.Visible = true;
                }
                labelSpecialSquare.Text = usesText;
                labelSpecialSquare.ForeColor = color;
                return;
            case SpecialAction.SquareRoot:
                if (uses >= 1)
                {
                    labelSpecialSquareRoot.Visible = true;
                    btnSpecialSqrt.Visible = true;
                }
                labelSpecialSquareRoot.Text = usesText;
                labelSpecialSquareRoot.ForeColor = color;
                return;
            case SpecialAction.CashToNumber:
                if (uses >= 1)
                {
                    labelSpecialCashToMoney.Visible = true;
                    btnSpecialCashToNumber.Visible = true;
                }
                labelSpecialCashToMoney.Text = usesText;
                labelSpecialCashToMoney.ForeColor = color;
                return;
            case SpecialAction.Modulus:
                if (uses >= 1)
                {
                    labelSpecialModulus.Visible = true;
                    btnSpecialModulus.Visible = true;
                }
                labelSpecialModulus.Text = usesText;
                labelSpecialModulus.ForeColor = color;
                return;
            case SpecialAction.Reroll:
                if (uses >= 1)
                {
                    labelSpecialReroll.Visible = true;
                    btnSpecialReroll.Visible = true;
                }
                labelSpecialReroll.Text = usesText;
                labelSpecialReroll.ForeColor = color;
                return;
            case SpecialAction.Random1To100:
                if (uses >= 1)
                {
                    labelUsesRandom1To100.Visible = true;
                    btnSpecialRandom1To100.Visible = true;
                }
                labelUsesRandom1To100.Text = usesText;
                labelUsesRandom1To100.ForeColor = color;
                break;
            case SpecialAction.AddOrRemoveClosestOr10:
                if (uses >= 1)
                {
                    labelUsesAddOrRemoveClosestOr10.Visible = true;
                    btnSpecialAddOrRemoveClosestOr10.Visible = true;
                }
                labelUsesAddOrRemoveClosestOr10.Text = usesText;
                labelUsesAddOrRemoveClosestOr10.ForeColor = color;
                break;
            case SpecialAction.IncrementByOne:
                if (uses >= 1)
                {
                    labelUsesIncrementByOne.Visible = true;
                    btnIncrementByOne.Visible = true;
                }
                labelUsesIncrementByOne.Text = usesText;
                labelUsesIncrementByOne.ForeColor = color;
                break;
            case SpecialAction.Reverse:
                if (uses >= 1)
                {
                    labelUsesReverse.Visible = true;
                    btnReverse.Visible = true;
                }
                labelUsesReverse.Text = usesText;
                labelUsesReverse.ForeColor = color;
                break;
            case SpecialAction.Clear:
                if (uses >= 1)
                {
                    labelUsesClear.Visible = true;
                    btnClear.Visible = true;
                }
                labelUsesClear.Text = usesText;
                labelUsesClear.ForeColor = color;
                break;
        }
    }

    private void btnSpecialSqrt_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.PerformSpecialAction(SpecialAction.SquareRoot);
    }

    private void btnSpecialSquare_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.PerformSpecialAction(SpecialAction.Square);
    }

    private void btnSpecialCashToNumber_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.PerformSpecialAction(SpecialAction.CashToNumber);
    }

    private void btnSpecialModulus_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.PerformSpecialAction(SpecialAction.Modulus);
    }

    private void btnSpecialReroll_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.PerformSpecialAction(SpecialAction.Reroll);
    }

    private void buttonContinueRound_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.StartNextRound();
    }

    private void SyncCoinCount()
    {
        labelCoinCount.Text = $"${viewModel.RoguelikeCalculator.Coins}";
    }

    private void buttonShopItem1_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.BuyShopItem(0);
    }

    private void buttonShopItem2_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.BuyShopItem(1);
    }

    private void buttonShopItem3_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.BuyShopItem(2);
    }

    private void buttonShopItem4_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.BuyShopItem(3);
    }

    private void buttonShopItem5_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.BuyShopItem(4);
    }

    private void buttonShopItem6_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.BuyShopItem(5);
    }

    private void buttonReroll_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.RerollShopItems();
    }

    private void buttonAcceptTOS_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.AcceptGamblingTOS();
    }

    private void buttonGamble_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.Gamble();
    }

    private void OnDivideByZeroInGame()
    {
        foreach (var element in allGameElements)
        {
            element.Visible = false;
        }
        foreach (var element in allSpecialActionElements)
        {
            element.Visible = false;
        }

        pictureDivisionBy0.Visible = true;
        BackColor = Color.Black;
    }

    private void HasConsentedToGamblingTOSUpdated(bool hasConsented)
    {
        panelTermsAndConditions.Visible = !hasConsented;
    }

    private void OnRerollCostUpdated(int rerollCost)
    {
        labelRerollCost.Text = $"${rerollCost}";
    }

    private void OnCoinsUpdated()
    {
        SyncCoinCount();
    }

    private void IsShoppingUpdated(bool isShopping)
    {
        if (isShopping)
        {
            SyncCoinCount();
            labelOlinsMessage.Text = "Go shop to the right";
            labelOlinsMessage.ForeColor = Color.Green;
            labelOlinsMessage.Visible = true;
            panelShopContainer.Visible = true;
            buttonContinueRound.Visible = true;
            labelRerollCost.Visible = true;
        }
        else
        {
            labelOlinsMessage.Visible = false;
            panelShopContainer.Visible = false;
            buttonContinueRound.Visible = false;
        }
    }

    private void OnAvailableShopItemsUpdated()
    {
        foreach (var element in allShopItemElements)
        {
            element.Visible = false;
        }
        foreach (var element in allShopItemCostElements)
        {
            element.Visible = false;
        }

        var availableShopItems = viewModel.RoguelikeCalculator.AvailableShopItems;
        if (availableShopItems == null) return;

        foreach (var (key, value) in availableShopItems)
        {
            var shopItemElement = allShopItemElements[key];
            var shopItemCostElement = allShopItemCostElements[key];

            if (value is ShopItem.NumberItem numberItem)
            {
                shopItemElement.Text = numberItem.Number.ToString();
                shopItemElement.BackColor = SystemColors.Control;
            }
            if (value is ShopItem.OperationItem operationItem)
            {
                var text = operationItem.Operation switch
                {
                    CalculatorOperation.Add => "+",
                    CalculatorOperation.Subtract => "-",
                    CalculatorOperation.Multiply => "*",
                    CalculatorOperation.Divide => "/",
                    CalculatorOperation.Modulus => "%",
                    _ => throw new NotImplementedException(),
                };
                shopItemElement.Text = text;
                shopItemElement.BackColor = SystemColors.ActiveCaption;
            }
            if (value is ShopItem.SpecialActionItem specialActionItem)
            {
                var text = specialActionItem.SpecialAction switch
                {
                    SpecialAction.Square => "x²",
                    SpecialAction.SquareRoot => "√x",
                    SpecialAction.Modulus => "%",
                    SpecialAction.Reroll => "-",
                    SpecialAction.CashToNumber => "$",
                    SpecialAction.Random1To100 => "R",
                    SpecialAction.AddOrRemoveClosestOr10 => "~10",
                    SpecialAction.IncrementByOne => "X++",
                    SpecialAction.Reverse => "<->",
                    _ => throw new NotImplementedException(),
                };
                shopItemElement.Text = text;
                shopItemElement.BackColor = Color.CornflowerBlue;
            }

            shopItemCostElement.Text = $"${value.Cost}";

            shopItemElement.Visible = true;
            shopItemCostElement.Visible = true;
        }
    }

    private void OnRIP()
    {
        foreach (var element in allGameElements)
        {
            element.Visible = false;
        }
        foreach (var element in allSpecialActionElements)
        {
            element.Visible = false;
        }

        labelOlinsMessage.Visible = false;
        panelShopContainer.Visible = false;
        pictureRIP.Visible = true;
        BackColor = Color.Black;
    }

    private void OnEventMessage(string? message)
    {
        if (message == null)
        {
            labelGambleMessage.Visible = false;
            labelGambleMessage.Text = "";
        }
        else
        {
            labelGambleMessage.Visible = true;
            labelGambleMessage.Text = message;
        }
    }

    private void OnShouldShowSpecialOlinsPic(bool shouldShow)
    {
        pictureSpecialOlins.Visible = shouldShow;
    }

    private void btnSpecialRandom1To100_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.PerformSpecialAction(SpecialAction.Random1To100);
    }

    private void btnSpecialAddOrRemoveClosestOr10_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.PerformSpecialAction(SpecialAction.AddOrRemoveClosestOr10);
    }

    private void btnIncrementByOne_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.PerformSpecialAction(SpecialAction.IncrementByOne);
    }

    private void btnReverse_Click(object sender, EventArgs e)
    {
        viewModel.RoguelikeCalculator.PerformSpecialAction(SpecialAction.Reverse);
    }
    #endregion
}
