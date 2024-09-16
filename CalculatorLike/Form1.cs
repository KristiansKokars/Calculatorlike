using CalculatorLike.Base;
using CalculatorLike.Game;

namespace CalculatorLike;

public partial class Form1 : Form
{
    private readonly CalculatorViewModel viewModel;
    private readonly List<Label> allLabelNumberUses;
    private readonly List<Label> allLabelOperationUses;
    private readonly List<Control> allGameElements;

    public Form1()
    {
        InitializeComponent();
        viewModel = new();

        viewModel.BasicCalculator.OnNumberUpdated += OnNumberUpdated;
        viewModel.RoguelikeCalculator.OnNumberUseUpdated += OnNumberUseUpdated;
        viewModel.RoguelikeCalculator.OnOperationUseUpdated += OnOperationUseUpdated;
        viewModel.RoguelikeCalculator.OnSpecialActionUseUpdated += OnSpecialActionUseUpdated;
        viewModel.RoguelikeCalculator.OnNewRound += OnNewRound;
        viewModel.RoguelikeCalculator.OnIsOlinsImpatient += OnIsOlinsImpatient;
        viewModel.OnGameFinished += OnGameFinished;

        allLabelNumberUses = [labelUses0, labelUses1, labelUses2, labelUses3, labelUses4, labelUses5, labelUses6, labelUses7, labelUses8, labelUses9];
        allLabelOperationUses = [labelUsesAdd, labelUsesDivide, labelUsesMultiply, labelUsesSubtract];
        allGameElements = [labelShopTitle, panelShop, buttonShopItem1, buttonShopItem2, buttonShopItem3, buttonShopItem4, buttonShopItem5, buttonShopItem6, labelShopItem1Cost, labelShopItem2Cost, labelShopItem3Cost, labelShopItem4Cost, labelShopItem5Cost, labelShopItem6Cost, buttonReroll, buttonContinueRound, groupGame, labelYouNeed, labelRound, labelCoins, btnRoguelikeCalculate, labelNumberToGet, labelRoundCount, labelCoinCount, labelUses0, labelUses1, labelUses2, labelUses3, labelUses4, labelUses5, labelUses6, labelUses7, labelUses8, labelUses9, labelUsesAdd, labelUsesDivide, labelUsesMultiply, labelUsesSubtract,];
    }

    #region Calculator
    private void OnNumberUpdated(int number)
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
        labelOlinsIsImpatient.Visible = isImpatient;
    }

    private void OnGameFinished(bool isWon)
    {
        if (isWon)
        {
            labelYouWon.Visible = true;
            btnRoguelike.Visible = true;
            foreach (var element in allGameElements)
            {
                element.Visible = false;
            }
        }
        else
        {
            foreach (var element in allGameElements)
            {
                element.Visible = false;
            }
            pictureGameLost.Visible = true;
            BackColor = Color.Black;
        }
    }

    private void OnNewRound()
    {
        labelNumberToGet.Text = viewModel.RoguelikeCalculator.NumberToGet.ToString();
        labelRoundCount.Text = viewModel.RoguelikeCalculator.Round.ToString();
        labelCoinCount.Text = viewModel.RoguelikeCalculator.Coins.ToString();
    }
    private void OnNumberUseUpdated(int number)
    {
        var uses = viewModel.RoguelikeCalculator.NumberUses[number];
        allLabelNumberUses[number].Text = uses.ToString();

        // TODO: custom colors
        if (uses > 0)
        {
            allLabelNumberUses[number].ForeColor = Color.Green;
        }
        else
        {
            allLabelNumberUses[number].ForeColor = Color.DarkRed;
        }
    }
    private void OnOperationUseUpdated(CalculatorOperation operation)
    {
        var uses = viewModel.RoguelikeCalculator.OperationUses[operation].ToString();
        switch (operation)
        {
            case CalculatorOperation.Add:
                labelUsesAdd.Text = uses;
                return;
            case CalculatorOperation.Subtract:
                labelUsesSubtract.Text = uses;
                return;
            case CalculatorOperation.Multiply:
                labelUsesMultiply.Text = uses;
                return;
            case CalculatorOperation.Divide:
                labelUsesDivide.Text = uses;
                return;
        }
    }

    private void OnSpecialActionUseUpdated(SpecialAction specialAction)
    {
        var uses = viewModel.RoguelikeCalculator.SpecialActionUses[specialAction];
        var usesText = uses.ToString();
        switch (specialAction)
        {
            case SpecialAction.Square:
                if (uses >= 1)
                {
                    labelSpecialSquare.Visible = true;
                    btnSpecialSquare.Visible = true;
                }
                labelSpecialSquare.Text = usesText;
                return;
            case SpecialAction.SquareRoot:
                if (uses >= 1)
                {
                    labelSpecialSquareRoot.Visible = true;
                    btnSpecialSqrt.Visible = true;
                }
                labelSpecialSquareRoot.Text = usesText;
                return;
            case SpecialAction.CashToNumber:
                if (uses >= 1)
                {
                    labelSpecialCashToMoney.Visible = true;
                    btnSpecialCashToNumber.Visible = true;
                }
                labelSpecialCashToMoney.Text = usesText;
                return;
            case SpecialAction.Modulus:
                if (uses >= 1)
                {
                    labelSpecialModulus.Visible = true;
                    btnSpecialModulus.Visible = true;
                }
                labelSpecialModulus.Text = usesText;
                return;
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
    #endregion

    private void label6_Click(object sender, EventArgs e)
    {

    }
}
