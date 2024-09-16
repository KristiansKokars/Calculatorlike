namespace CalculatorLike
{
    public partial class Form1 : Form
    {
        private readonly CalculatorViewModel viewModel;

        public Form1()
        {
            InitializeComponent();
            viewModel = new();
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            tbNumbers.Text += "1";
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            tbNumbers.Text += "2";
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            tbNumbers.Text += "3";
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            tbNumbers.Text += "4";
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            tbNumbers.Text += "5";
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            tbNumbers.Text += "6";
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            tbNumbers.Text += "7";
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            tbNumbers.Text += "8";
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            tbNumbers.Text += "9";
        }

        private void btn0_Click(object sender, EventArgs e)
        {
            tbNumbers.Text += "0";
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
            bool isParsed = int.TryParse(tbNumbers.Text, out int input);
            if (!isParsed)
            {
                return;
            }

            viewModel.SetCurrentNumber(input);
            var result = viewModel.Calculate();
            tbNumbers.Text = result.ToString();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbNumbers.Text = "";
        }

        private void setOperation(CalculatorOperation operation)
        {
            bool isParsed = int.TryParse(tbNumbers.Text, out int input);
            if (!isParsed)
            {
                return;
            }

            viewModel.SetCurrentNumber(input);
            viewModel.SetOperation(operation);
            // TODO: sync this up nicer
            tbNumbers.Text = "";
        }

        private void tbNumbers_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnRoguelike_Click(object sender, EventArgs e)
        {
            // TODO: check if in Roguelike, in which case do nothing
            viewModel.StartGame();
            groupGame.Visible = true;
            labelYouNeed.Visible = true;
            labelRound.Visible = true;
            labelCoins.Visible = true;
            label3Uses.Visible = true;

            btnRoguelikeCalculate.Visible = true;
            btnRoguelike.Visible = false;

            labelNumberToGet.Text = viewModel.GenerateRandomNumberToGet().ToString();
            labelNumberToGet.Visible = true;

            labelRoundCount.Visible = true;
            labelCoinCount.Visible = true;
        }

        private void labelRound_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
