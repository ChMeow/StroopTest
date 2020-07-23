using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Data;
using System.Drawing.Drawing2D;
using System.IO;

namespace StroopEffect
{
    public partial class Form1 : Form
    {
        Stopwatch sw = new Stopwatch();
        Stopwatch sw2 = new Stopwatch();
        Random random = new Random();
        int answerIndex = -1;
        string wrongAnswer = "x";
        string correctAnswer = "x";
        int previousColor = 1;
        int previousWord = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkBox_codeSwitch.Checked = false;
            checkBoxEasy.Checked = false;
            button7.ForeColor = Color.Snow;
            button7.Text = "EASY";
            button1.Text = "NORMAL (SELECTED)";
            button1.ForeColor = Color.LightCyan;
            buttonCode.Text = "CODE SWITCHING";
            buttonCode.ForeColor = Color.Snow;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelHS.Text = "HIGH SCORE: " + labelSCOREFILE.Text;
            labelHolder.Text = "RECORD HOLDER: " + labelSCOREHOLDER.Text;
            if (tabControl1.SelectedTab == tabStroop)
            {
                //Reset Result
                richTextBox_Result.Clear();
                richTextBox_Result.Text = "RESULTS:" + '\r' + '\n';
                richTextBox_Result.AppendText("=============================================================================" + '\r' + '\n');
                numericUpDownCorrect.Value = 0;

                //stopwatch start
                sw.Start();
                //question 1
                int textWord = random.Next(1, 11);
                int textColor = random.Next(1, 11);
                int textX = random.Next(1, 11);
                while (textX == textColor)
                {
                    textX = random.Next(1, 11);
                }
                answerIndex = random.Next(1, 3);
                label_AnswerHolder.Text = answerIndex.ToString();

                if (checkBox_codeSwitch.Checked) { codeSwitchQuestion(textWord, textColor, answerIndex, textX); }
                else { stroopTestQuestion(textWord, textColor, answerIndex, textX); }
            }
        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            //dont use
        }

        
        private void button_A1_Click(object sender, EventArgs e)
        {
            //check answer
            if (label_AnswerHolder.Text == "1")
            {
                richTextBox_Result.AppendText("QUESTION : CORRECT," + '\t' + "TIME TAKEN: ");
                numericUpDownCorrect.Value = numericUpDownCorrect.Value + 1;
            }
            else
            {
                richTextBox_Result.AppendText("QUESTION : INCORRECT," + '\t' + "TIME TAKEN: ");
            }
            double timeTaken = 1;
            timeTaken = Convert.ToDouble(sw.ElapsedMilliseconds.ToString());
            timeTaken = Math.Round(timeTaken / 1000 , 3);
            richTextBox_Result.AppendText(timeTaken.ToString("0.00") + " s" + '\r' + '\n');
            numericUpDownMaxQuestion.Value = numericUpDownMaxQuestion.Value - 1; 
            
            //Conclude or new question
            if(numericUpDownMaxQuestion.Value > 0)
            {
                int textWord = random.Next(1, 11);
                int textColor = random.Next(1, 11);
                int textX = random.Next(1, 11);
                while (previousColor == textColor || previousWord == textWord)
                {
                    textColor = random.Next(1, 11);
                    textWord = random.Next(1, 11);
                }
                while (textX == textColor)
                {
                    textX = random.Next(1, 11);
                }
                previousColor = textColor;
                previousWord = textWord;
                int answerIndex = random.Next(1, 3);
                if (checkBox_codeSwitch.Checked) { codeSwitchQuestion(textWord, textColor, answerIndex, textX); }
                else { stroopTestQuestion(textWord, textColor, answerIndex, textX); }
            }
            else
            {
                tabControl1.SelectTab(2);
                numericUpDownMaxQuestion.Value = 20;
                sw.Stop();
                sw.Reset();
                richTextBox_Result.AppendText("=============================================================================" + '\r' + '\n');
                if (checkBoxEasy.Checked) { labelMode.Text = "Difficulty: EASY"; }
                else if(checkBox_codeSwitch.Checked) { labelMode.Text = "Difficulty: CODE SWITCHING"; }
                else { labelMode.Text = "Difficulty: NORMAL"; }

                // Calculate Average Time
                richTextBoxCumulative.Text=timeTaken.ToString("0.00") + "/20";
                DataTable dt = new DataTable();
                var result = dt.Compute(richTextBoxCumulative.Text, "");
                labelEva.Text = result.ToString();

                double incorrectAns = Convert.ToDouble(numericUpDownMaxQuestion.Value - numericUpDownCorrect.Value);
                double correctAns = Convert.ToDouble(numericUpDownCorrect.Value);
                labelIncorrect.Text = incorrectAns.ToString();
                if (incorrectAns >= 6 && timeTaken < 6) { incorrectAns = incorrectAns * 10; }
                else { incorrectAns = incorrectAns * 0.5; }
                if (incorrectAns <= 0) { incorrectAns = 0.3; }
                richTextBoxScoreCount.Text = numericUpDownCorrect.Value + "/" + incorrectAns + "/" + labelEva.Text;
                var resultScore = dt.Compute(richTextBoxScoreCount.Text, "");
                labelScore.Text = resultScore.ToString();
                labelAccuracy.Text = "Accuracy: " + correctAns / 20 * 100 + " %";
                labelSpeed.Text = "Average Speed: " + labelEva.Text + " seconds";
                double tempScore = Convert.ToDouble(labelScore.Text);
                labelResultScore.Text = "Score: " + tempScore.ToString("0.00");
                if (tempScore < 1) { pictureBoxScore.Image = Image.FromFile("Pics\\E.png"); }
                else if (6 > tempScore && tempScore >= 1) { pictureBoxScore.Image = Image.FromFile("Pics\\D.png"); }
                else if (15 > tempScore && tempScore >= 6) { pictureBoxScore.Image = Image.FromFile("Pics\\C.png"); }
                else if (35 > tempScore && tempScore >= 15) { pictureBoxScore.Image = Image.FromFile("Pics\\B.png"); }
                else if (50 > tempScore && tempScore >= 35) { pictureBoxScore.Image = Image.FromFile("Pics\\A.png"); }
                else { pictureBoxScore.Image = Image.FromFile("Pics\\S.png"); }
                double previousHS = Convert.ToDouble(labelSCOREFILE.Text);
                double currentHS = tempScore;
                string sTextFromUser = "NONAME";
                if (currentHS > previousHS && !checkBoxEasy.Checked && !checkBox_codeSwitch.Checked)
                {
                    bool boolTryAgain = false;
                    do
                    {
                        sTextFromUser = PopUpBox.GetUserInput("Enter Your Name Below:", "Congratulation");

                        if (sTextFromUser == "")
                        {
                            sTextFromUser = "NONAME";
                        }
                        else
                        {
                            if (sTextFromUser == "cancel")
                            {
                                sTextFromUser = "NONAME";
                            }
                        }
                    } while (boolTryAgain == true);
                    StreamWriter sw = new StreamWriter("Score\\2Name.txt");
                    sw.WriteLine(sTextFromUser);
                    sw.Close();
                    labelSCOREHOLDER.Text = sTextFromUser;
                    labelHolder.Text = "RECORD HOLDER: " + sTextFromUser;
                    StreamWriter sw2 = new StreamWriter("Score\\2Score.txt");
                    sw2.WriteLine(tempScore.ToString("0.00"));
                    sw2.Close();
                    labelSCOREFILE.Text = tempScore.ToString("0.00");
                    labelHS.Text = "HIGH SCORE: " + tempScore.ToString("0.00");
                }  
            }            
        }

        private void button_A2_Click(object sender, EventArgs e)
        {
            //check answer
            if (label_AnswerHolder.Text == "2")
            {
                richTextBox_Result.AppendText("QUESTION : CORRECT," + '\t' + "TIME TAKEN: ");
                numericUpDownCorrect.Value = numericUpDownCorrect.Value + 1;
            }
            else
            {
                richTextBox_Result.AppendText("QUESTION : INCORRECT," + '\t' + "TIME TAKEN: ");
            }
            double timeTaken = 1;
            timeTaken = Convert.ToDouble(sw.ElapsedMilliseconds.ToString());
            timeTaken = Math.Round(timeTaken / 1000, 3);
            richTextBox_Result.AppendText(timeTaken.ToString("0.00") + " s" + '\r' + '\n');
            numericUpDownMaxQuestion.Value = numericUpDownMaxQuestion.Value - 1;

            //Conclude or new question
            if (numericUpDownMaxQuestion.Value > 0)
            {
                int textWord = random.Next(1, 11);
                int textColor = random.Next(1, 11);
                int textX = random.Next(1, 11);
                while (previousColor == textColor || previousWord == textWord)
                {
                    textColor = random.Next(1, 11);
                    textWord = random.Next(1, 11);
                }
                while (textX == textColor)
                {
                    textX = random.Next(1, 11);
                }
                previousColor = textColor;
                previousWord = textWord;
                int answerIndex = random.Next(1, 3);
                if (checkBox_codeSwitch.Checked) { codeSwitchQuestion(textWord, textColor, answerIndex, textX); }
                else { stroopTestQuestion(textWord, textColor, answerIndex, textX); }
            }
            else
            {
                tabControl1.SelectTab(2);
                numericUpDownMaxQuestion.Value = 20;
                sw.Stop();
                sw.Reset();
                richTextBox_Result.AppendText("=============================================================================" + '\r' + '\n');
                if (checkBoxEasy.Checked) { labelMode.Text = "Difficulty: EASY"; }
                else if (checkBox_codeSwitch.Checked) { labelMode.Text = "Difficulty: CODE SWITCHING"; }
                else { labelMode.Text = "Difficulty: NORMAL"; }

                // Calculate Average Time
                richTextBoxCumulative.Text = timeTaken.ToString("0.00") + "/20";
                DataTable dt = new DataTable();
                var result = dt.Compute(richTextBoxCumulative.Text, "");
                labelEva.Text = result.ToString();

                double incorrectAns = Convert.ToDouble(numericUpDownMaxQuestion.Value - numericUpDownCorrect.Value);
                double correctAns = Convert.ToDouble(numericUpDownCorrect.Value);
                labelIncorrect.Text = incorrectAns.ToString();
                if (incorrectAns >= 6 && timeTaken < 6) { incorrectAns = incorrectAns * 10; }
                else { incorrectAns = incorrectAns * 0.5; }
                if (incorrectAns <= 0) { incorrectAns = 0.3; }
                richTextBoxScoreCount.Text = numericUpDownCorrect.Value + "/" + incorrectAns + "/" + labelEva.Text;
                var resultScore = dt.Compute(richTextBoxScoreCount.Text, "");
                labelScore.Text = resultScore.ToString();
                labelAccuracy.Text = "Accuracy: " + correctAns / 20 * 100 + " %";
                labelSpeed.Text = "Average Speed: " + labelEva.Text + " seconds";
                double tempScore = Convert.ToDouble(labelScore.Text);
                labelResultScore.Text = "Score: " + tempScore.ToString("0.00");
                if (tempScore < 1) { pictureBoxScore.Image = Image.FromFile("Pics\\E.png"); }
                else if (6 > tempScore && tempScore >= 1) { pictureBoxScore.Image = Image.FromFile("Pics\\D.png"); }
                else if (15 > tempScore && tempScore >= 6) { pictureBoxScore.Image = Image.FromFile("Pics\\C.png"); }
                else if (35 > tempScore && tempScore >= 15) { pictureBoxScore.Image = Image.FromFile("Pics\\B.png"); }
                else if (50 > tempScore && tempScore >= 35) { pictureBoxScore.Image = Image.FromFile("Pics\\A.png"); }
                else { pictureBoxScore.Image = Image.FromFile("Pics\\S.png"); }
                double previousHS = Convert.ToDouble(labelSCOREFILE.Text);
                double currentHS = tempScore;
                string sTextFromUser = "NONAME";
                if (currentHS > previousHS && !checkBoxEasy.Checked && !checkBox_codeSwitch.Checked)
                {
                    bool boolTryAgain = false;
                    do
                    {
                        sTextFromUser = PopUpBox.GetUserInput("Enter Your Name Below:", "Congratulation");

                        if (sTextFromUser == "")
                        {
                            sTextFromUser = "NONAME";
                        }
                        else
                        {
                            if (sTextFromUser == "cancel")
                            {
                                sTextFromUser = "NONAME";
                            }
                        }
                    } while (boolTryAgain == true);
                    StreamWriter sw = new StreamWriter("Score\\2Name.txt");
                    sw.WriteLine(sTextFromUser);
                    sw.Close();
                    labelSCOREHOLDER.Text = sTextFromUser;
                    labelHolder.Text = "RECORD HOLDER: " + sTextFromUser;
                    StreamWriter sw2 = new StreamWriter("Score\\2Score.txt");
                    sw2.WriteLine(tempScore.ToString("0.00"));
                    sw2.Close();
                    labelSCOREFILE.Text = tempScore.ToString("0.00");
                    labelHS.Text = "HIGH SCORE: " + tempScore.ToString("0.00");
                }
            }
        }

        //STROOP QUESTIONS
        public void stroopTestQuestion(int textW, int textC, int ansInt, int textWrong)
        {
            int HardMode = 17;
            if(checkBoxEasy.Checked) { HardMode = 0; }
            int i = 1;
            i = Convert.ToInt32(numericUpDownMaxQuestion.Value);
            if (i > HardMode)
            {
                switch (textWrong)
                {
                    case 1:
                        wrongAnswer = "RED";
                        break;
                    case 2:
                        wrongAnswer = "ORANGE";
                        break;
                    case 3:
                        wrongAnswer = "YELLOW";
                        break;
                    case 4:
                        wrongAnswer = "GREEN";
                        break;
                    case 5:
                        wrongAnswer = "BLUE";
                        break;
                    case 6:
                        wrongAnswer = "PURPLE";
                        break;
                    case 7:
                        wrongAnswer = "PINK";
                        break;
                    case 8:
                        wrongAnswer = "BROWN";
                        break;
                    case 9:
                        wrongAnswer = "BLACK";
                        break;
                    case 10:
                        wrongAnswer = "WHITE";
                        break;
                    default:
                        break;
                }

                switch (textC)
                {
                    case 1:
                        label_Question.ForeColor = System.Drawing.Color.Red;
                        label_Question.Text = "RED";
                        correctAnswer = "RED";
                        break;
                    case 2:
                        label_Question.ForeColor = System.Drawing.Color.Orange;
                        label_Question.Text = "ORANGE";
                        correctAnswer = "ORANGE";
                        break;
                    case 3:
                        label_Question.ForeColor = System.Drawing.Color.Yellow;
                        label_Question.Text = "YELLOW";
                        correctAnswer = "YELLOW";
                        break;
                    case 4:
                        label_Question.ForeColor = System.Drawing.Color.Green;
                        label_Question.Text = "GREEN";
                        correctAnswer = "GREEN";
                        break;
                    case 5:
                        label_Question.ForeColor = System.Drawing.Color.Blue;
                        label_Question.Text = "BLUE";
                        correctAnswer = "BLUE";
                        break;
                    case 6:
                        label_Question.ForeColor = System.Drawing.Color.Purple;
                        label_Question.Text = "PURPLE";
                        correctAnswer = "PURPLE";
                        break;
                    case 7:
                        label_Question.ForeColor = System.Drawing.Color.Pink;
                        label_Question.Text = "PINK";
                        correctAnswer = "PINK";
                        break;
                    case 8:
                        label_Question.ForeColor = System.Drawing.Color.Sienna;
                        label_Question.Text = "BROWN";
                        correctAnswer = "BROWN";
                        break;
                    case 9:
                        label_Question.ForeColor = System.Drawing.Color.Black;
                        label_Question.Text = "BLACK";
                        correctAnswer = "BLACK";
                        break;
                    case 10:
                        label_Question.ForeColor = System.Drawing.Color.White;
                        label_Question.Text = "WHITE";
                        correctAnswer = "WHITE";
                        break;
                    default:
                        break;
                }
                if (ansInt == 1) { button_A1.Text = correctAnswer; button_A2.Text = wrongAnswer; label_AnswerHolder.Text = "1"; }
                else { button_A2.Text = correctAnswer; button_A1.Text = wrongAnswer; label_AnswerHolder.Text = "2"; }
            }

            else
            {
                switch (textWrong)
                {
                    case 1:
                        wrongAnswer = "RED";
                        break;
                    case 2:
                        wrongAnswer = "ORANGE";
                        break;
                    case 3:
                        wrongAnswer = "YELLOW";
                        break;
                    case 4:
                        wrongAnswer = "GREEN";
                        break;
                    case 5:
                        wrongAnswer = "BLUE";
                        break;
                    case 6:
                        wrongAnswer = "PURPLE";
                        break;
                    case 7:
                        wrongAnswer = "PINK";
                        break;
                    case 8:
                        wrongAnswer = "BROWN";
                        break;
                    case 9:
                        wrongAnswer = "BLACK";
                        break;
                    case 10:
                        wrongAnswer = "WHITE";
                        break;
                    default:
                        break;
                }

                switch (textW)
                {
                    case 1:
                        label_Question.Text = "RED";
                        break;
                    case 2:
                        label_Question.Text = "ORANGE";
                        break;
                    case 3:
                        label_Question.Text = "YELLOW";
                        break;
                    case 4:
                        label_Question.Text = "GREEN";
                        break;
                    case 5:
                        label_Question.Text = "BLUE";
                        break;
                    case 6:
                        label_Question.Text = "PURPLE";
                        break;
                    case 7:
                        label_Question.Text = "PINK";
                        break;
                    case 8:
                        label_Question.Text = "BROWN";
                        break;
                    case 9:
                        label_Question.Text = "BLACK";
                        break;
                    case 10:
                        label_Question.Text = "WHITE";
                        break;
                    default:
                        break;
                }

                switch (textC)
                {
                    case 1:
                        label_Question.ForeColor = System.Drawing.Color.Red;
                        correctAnswer = "RED";
                        break;
                    case 2:
                        label_Question.ForeColor = System.Drawing.Color.Orange;
                        correctAnswer = "ORANGE";
                        break;
                    case 3:
                        label_Question.ForeColor = System.Drawing.Color.Yellow;
                        correctAnswer = "YELLOW";
                        break;
                    case 4:
                        label_Question.ForeColor = System.Drawing.Color.Green;
                        correctAnswer = "GREEN";
                        break;
                    case 5:
                        label_Question.ForeColor = System.Drawing.Color.Blue;
                        correctAnswer = "BLUE";
                        break;
                    case 6:
                        label_Question.ForeColor = System.Drawing.Color.Purple;
                        correctAnswer = "PURPLE";
                        break;
                    case 7:
                        label_Question.ForeColor = System.Drawing.Color.Pink;
                        correctAnswer = "PINK";
                        break;
                    case 8:
                        label_Question.ForeColor = System.Drawing.Color.SaddleBrown;
                        correctAnswer = "BROWN";
                        break;
                    case 9:
                        label_Question.ForeColor = System.Drawing.Color.Black;
                        correctAnswer = "BLACK";
                        break;
                    case 10:
                        label_Question.ForeColor = System.Drawing.Color.White;
                        correctAnswer = "WHITE";
                        break;
                    default:
                        break;
                }

                if(textC == textW)
                {
                    wrongAnswer = wrongAnswer;
                }
                else
                {
                    wrongAnswer = label_Question.Text;
                }
                if (ansInt == 1) { button_A1.Text = correctAnswer; button_A2.Text = wrongAnswer; label_AnswerHolder.Text = "1"; }
                else { button_A2.Text = correctAnswer; button_A1.Text = wrongAnswer; label_AnswerHolder.Text = "2"; }
                
            }

        }

        public void codeSwitchQuestion(int textW, int textC, int ansInt, int textWrong)
        {
            int lang1 = random.Next(1, 4);
            int lang2 = random.Next(1, 4);
            int lang3 = random.Next(1, 4);

            switch (textWrong)
            {
                case 1:
                    if(lang3 == 3) { wrongAnswer = "红色"; }
                    else if(lang3 == 2) { wrongAnswer = "MERAH"; }
                    else { wrongAnswer = "RED"; }
                    break;
                case 2:
                    if (lang3 == 3) { wrongAnswer = "橙色"; }
                    else if (lang3 == 2) { wrongAnswer = "JINGGA"; }
                    else { wrongAnswer = "ORANGE"; }
                    break;
                case 3:
                    if (lang3 == 3) { wrongAnswer = "黄色"; }
                    else if (lang3 == 2) { wrongAnswer = "KUNING"; }
                    else { wrongAnswer = "YELLOW"; }
                    break;
                case 4:
                    if (lang3 == 3) { wrongAnswer = "青色"; }
                    else if (lang3 == 2) { wrongAnswer = "HIJAU"; }
                    else { wrongAnswer = "GREEN"; }
                    break;
                case 5:
                    if (lang3 == 3) { wrongAnswer = "蓝色"; }
                    else if (lang3 == 2) { wrongAnswer = "BIRU"; }
                    else{ wrongAnswer = "BLUE"; }
                        
                    break;
                case 6:
                    if (lang3 == 3) { wrongAnswer = "紫色"; }
                    else if (lang3 == 2) { wrongAnswer = "UNGU"; }
                    else
                    { wrongAnswer = "PURPLE"; }
                    break;
                case 7:
                    if (lang3 == 3) { wrongAnswer = "粉红色"; }
                    else if (lang3 == 2) { wrongAnswer = "MERAH JAMBU"; }
                    else { wrongAnswer = "PINK"; }
                    break;
                case 8:
                    if (lang3 == 3) { wrongAnswer = "褐色"; }
                    else if (lang3 == 2) { wrongAnswer = "COKLAT / PERANG"; }
                    else { wrongAnswer = "BROWN"; }
                    break;
                case 9:
                    if (lang3 == 3) { wrongAnswer = "黑色"; }
                    else if (lang3 == 2) { wrongAnswer = "HITAM"; }
                    else { wrongAnswer = "BLACK"; }
                    break;
                case 10:
                    if (lang3 == 3) { wrongAnswer = "白色"; }
                    else if (lang3 == 2) { wrongAnswer = "PUTIH"; }
                    else { wrongAnswer = "WHITE"; }
                    break;
                default:
                    break;
            }

            switch (textW)
            {
                case 1:
                    if (lang2 == 3) { label_Question.Text = "红色"; }
                    else if (lang2 == 2) { label_Question.Text = "MERAH"; }
                    else { label_Question.Text = "RED"; }
                    break;
                case 2:
                    if (lang2 == 3) { label_Question.Text = "橙色"; }
                    else if (lang2 == 2) { label_Question.Text = "JINGGA"; }
                    else { label_Question.Text = "ORANGE"; }
                    break;
                case 3:
                    if (lang2 == 3) { label_Question.Text = "黄色"; }
                    else if (lang2 == 2) { label_Question.Text = "KUNING"; }
                    else { label_Question.Text = "YELLOW"; }
                    break;
                case 4:
                    if (lang2 == 3) { label_Question.Text = "青色"; }
                    else if (lang2 == 2) { label_Question.Text = "HIJAU"; }
                    else { label_Question.Text = "GREEN"; }
                    break;
                case 5:
                    if (lang2 == 3) { label_Question.Text = "蓝色"; }
                    else if (lang2 == 2) { label_Question.Text = "BIRU"; }
                    else { label_Question.Text = "BLUE"; }
                    break;
                case 6:
                    if (lang2 == 3) { label_Question.Text = "紫色"; }
                    else if (lang2 == 2) { label_Question.Text = "UNGU"; }
                    else
                    { label_Question.Text = "PURPLE"; }
                    break;
                case 7:
                    if (lang2 == 3) { label_Question.Text = "粉红色"; }
                    else if (lang2 == 2) { label_Question.Text = "MERAH JAMBU"; }
                    else { label_Question.Text = "PINK"; }
                    break;
                case 8:
                    if (lang2 == 3) { label_Question.Text = "褐色"; }
                    else if (lang2 == 2) { label_Question.Text = "COKLAT / PERANG"; }
                    else { label_Question.Text = "BROWN"; }
                    break;
                case 9:
                    if (lang2 == 3) { label_Question.Text = "黑色"; }
                    else if (lang2 == 2) { label_Question.Text = "HITAM"; }
                    else { label_Question.Text = "BLACK"; }
                    break;
                case 10:
                    if (lang2 == 3) { label_Question.Text = "白色"; }
                    else if (lang2 == 2) { label_Question.Text = "PUTIH"; }
                    else { label_Question.Text = "WHITE"; }
                    break;
                default:
                    break;
            }

            switch (textC)
            {
                case 1:
                    label_Question.ForeColor = System.Drawing.Color.Red;
                    if (lang1 == 3) { correctAnswer = "红色"; }
                    else if (lang1 == 2) { correctAnswer = "MERAH"; }
                    else { correctAnswer = "RED"; }
                    break;
                case 2:
                    label_Question.ForeColor = System.Drawing.Color.Orange;
                    if (lang1 == 3) { correctAnswer = "橙色"; }
                    else if (lang1 == 2) { correctAnswer = "JINGGA"; }
                    else { correctAnswer = "ORANGE"; }
                    break;
                case 3:
                    label_Question.ForeColor = System.Drawing.Color.Yellow;
                    if (lang1 == 3) { correctAnswer = "黄色"; }
                    else if (lang1 == 2) { correctAnswer = "KUNING"; }
                    else { correctAnswer = "YELLOW"; }
                    break;
                case 4:
                    label_Question.ForeColor = System.Drawing.Color.Green;
                    if (lang1 == 3) { correctAnswer = "青色"; }
                    else if (lang1 == 2) { correctAnswer = "HIJAU"; }
                    else { correctAnswer = "GREEN"; }
                    break;
                case 5:
                    label_Question.ForeColor = System.Drawing.Color.Blue;
                    if (lang1 == 3) { correctAnswer = "蓝色"; }
                    else if (lang1 == 2) { correctAnswer = "BIRU"; }
                    else { correctAnswer = "BLUE"; }
                    break;
                case 6:
                    label_Question.ForeColor = System.Drawing.Color.Purple;
                    if (lang1 == 3) { correctAnswer = "紫色"; }
                    else if (lang1 == 2) { correctAnswer = "UNGU"; }
                    else { correctAnswer = "PURPLE"; }
                    break;
                case 7:
                    label_Question.ForeColor = System.Drawing.Color.Pink;
                    if (lang1 == 3) { correctAnswer = "粉红色"; }
                    else if (lang1 == 2) { correctAnswer = "MERAH JAMBU"; }
                    else { correctAnswer = "PINK"; }
                    break;
                case 8:
                    label_Question.ForeColor = System.Drawing.Color.SaddleBrown;
                    if (lang1 == 3) { correctAnswer = "褐色"; }
                    else if (lang1 == 2) { correctAnswer = "COKLAT / PERANG"; }
                    else { correctAnswer = "BROWN"; }
                    break;
                case 9:
                    label_Question.ForeColor = System.Drawing.Color.Black;
                    if (lang1 == 3) { correctAnswer = "黑色"; }
                    else if (lang1 == 2) { correctAnswer = "HITAM"; }
                    else { correctAnswer = "BLACK"; }
                    break;
                case 10:
                    label_Question.ForeColor = System.Drawing.Color.White;
                    if (lang1 == 3) { correctAnswer = "白色"; }
                    else if (lang1 == 2) { correctAnswer = "PUTIH"; }
                    else { correctAnswer = "WHITE"; }
                    break;
                default:
                    break;
            }

            //if (textC == textW) {wrongAnswer = wrongAnswer;}
            //else{wrongAnswer = label_Question.Text;}

            if (ansInt == 1) { button_A1.Text = correctAnswer; button_A2.Text = wrongAnswer; label_AnswerHolder.Text = "1"; }
            else { button_A2.Text = correctAnswer; button_A1.Text = wrongAnswer; label_AnswerHolder.Text = "2"; }
        }


        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {     
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.A))
            {
                button_A1.PerformClick();
                return true;
            }
            if (keyData == (Keys.D))
            {
                button_A2.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void buttonEva_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            var result = dt.Compute(richTextBoxCumulative.Text, "");
            labelEva.Text = result.ToString();
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            checkBox_codeSwitch.Checked = true;
            checkBoxEasy.Checked = false;
            button7.ForeColor = Color.Snow;
            button7.Text = "EASY";
            button1.Text = "NORMAL";
            button1.ForeColor = Color.Snow;
            buttonCode.Text = "CODE SWITCHING (SELECTED)";
            buttonCode.ForeColor = Color.LightCyan;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectTab(1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(4);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(5);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(0);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBoxHow.LoadFile("Docs\\how.rtf");
            richTextBoxInfo.LoadFile("Docs\\info.rtf");
            String line;
            try
            {
                StreamReader sr = new StreamReader("Score\\2Score.txt");
                line = sr.ReadLine();
                labelSCOREFILE.Text = line;
                labelHS.Text = "HIGH SCORE: " + labelSCOREFILE.Text;
                sr.Close();
            }
            catch (InvalidCastException)
            {
            }
            
            try
            {
                StreamReader sr = new StreamReader("Score\\2Name.txt");
                line = sr.ReadLine();
                labelSCOREHOLDER.Text = line;
                labelHolder.Text = "RECORD HOLDER: " + labelSCOREHOLDER.Text;
                sr.Close();
            }
            catch (InvalidCastException)
            {
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            checkBoxEasy.Checked = true;
            checkBox_codeSwitch.Checked = false;
            button7.ForeColor = Color.LightCyan;
            button7.Text = "EASY (SELECTED)";
            button1.Text = "NORMAL";
            button1.ForeColor = Color.Snow;
            buttonCode.Text = "CODE SWITCHING";
            buttonCode.ForeColor = Color.Snow;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            bool boolTryAgain = false;
            do
            {
                string sTextFromUser = PopUpBox.GetUserInput("Enter Your Name Below:", "Congratulation");
                if (sTextFromUser == "")
                {
                    DialogResult dialogResult = MessageBox.Show("You did not enter anything. Try again?", "Error", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        boolTryAgain = true; //will reopen the dialog for user to input text again
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        //exit/cancel
                        MessageBox.Show("operation cancelled");
                        boolTryAgain = false;
                    }//end if
                }
                else
                {
                    if (sTextFromUser == "cancel")
                    {
                        MessageBox.Show("operation cancelled");
                    }
                    else
                    {
                        MessageBox.Show("Here is the text you entered: '" + sTextFromUser + "'");
                        //do something here with the user input
                    }

                }
            } while (boolTryAgain == true);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/ChMeow");
        }
    }
}
