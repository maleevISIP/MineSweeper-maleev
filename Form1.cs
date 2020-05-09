using System;
using System.Drawing;
using System.Windows.Forms;
using System.Media;

namespace MineSweeper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Height = 343;
            this.Width = 315;           // высота и длина прогр

            putButtons(9, 9, 29);
            resetGame();
        }

        static int rows = 9;
        static int colums = 9;
        string[,] gameArr = new string[rows, colums];
        static int totalBombs = 10;

        SoundPlayer bomb = new SoundPlayer("bomb.wav");             // звуки победы и проигрыша
        SoundPlayer victory = new SoundPlayer("victory.wav");       

        public void putButtons(int rows, int colums, int size)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                     this.Controls.Remove(Controls[$"b{i}-{j}"]);
                }
            }

            int x = 7;
            int y = 30;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < colums; j++)
                {
                    Button button = new Button();
                    button.Name = $"b{i}-{j}";
                    button.Size = new Size(size, size);
                    button.Location = new Point(x, y);
                    button.Click += this.b00_Click;
                    button.MouseDown += this.b00_MouseDown;
                    Controls.Add(button);
                    x += 32;
                }

                y += 30;
                x = 7;
            }
        }

        public void resetGame() // перезапуск игры
        {
            for (int j = 0; j < rows; j++)
            {
                for (int k = 0; k < colums; k++)
                {
                    gameArr[j, k] = "";
                    (this.Controls[$"b{j}-{k}"] as Button).BackColor = Color.White;
                    (this.Controls[$"b{j}-{k}"] as Button).Text = "";
                }
            }

            var random = new Random();

            for (int i = 0; i < totalBombs; i++)
            {
                int ranRow = random.Next(0, rows);
                int ranCol = random.Next(0, colums);
                if (gameArr[ranRow, ranCol] != "") i--;
                else gameArr[ranRow, ranCol] = "X";
            }

            countBombs();
        }

        public void countBombs()  // проверка бомбочек
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < colums; j++)
                {
                    int bombs = 0;

                    if (gameArr[i, j] != "X")
                    {
                        if (j > 0 && gameArr[i, j - 1] == "X") bombs++;                        
                        if (j < colums - 1 && gameArr[i, j + 1] == "X") bombs++;
                        if (i < rows - 1 &&  gameArr[i + 1, j] == "X") bombs++;
                        if (i > 0 && gameArr[i - 1, j] == "X") bombs++;
                        if (i > 0 && j > 0 && gameArr[i - 1, j - 1] == "X") bombs++;
                        if (i > 0 && j < colums - 1 && gameArr[i - 1, j + 1] == "X") bombs++;
                        if (i < rows - 1 && j > 0 && gameArr[i + 1, j - 1] == "X") bombs++;
                        if (i < rows - 1 && j < colums - 1 && gameArr[i + 1, j + 1] == "X") bombs++;

                        gameArr[i, j] = bombs.ToString();
                    }
                }
            }
        }

        public void putTextAndColor(int row, int colum)
        {
            string bombs = gameArr[row, colum];

            if (bombs != "0")
            {
                (this.Controls[$"b{row}-{colum}"] as Button).BackColor = Color.Silver;
                (this.Controls[$"b{row}-{colum}"] as Button).Text = bombs;
            }

            if (bombs == "1") (this.Controls[$"b{row}-{colum}"] as Button).ForeColor = Color.DarkBlue;
            else if (bombs == "2") (this.Controls[$"b{row}-{colum}"] as Button).ForeColor = Color.DarkGreen;
            else (this.Controls[$"b{row}-{colum}"] as Button).ForeColor = Color.Red;
        }

        public void checkWin() // проверка победы
        {
            int taggedCells = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < colums; j++)
                {
                    if ((this.Controls[$"b{i}-{j}"] as Button).BackColor != Color.White)
                        taggedCells++;
                }
            }

            if (taggedCells == colums * rows - totalBombs)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < colums; j++)
                    {
                        if (gameArr[i, j] == "X")
                        {
                            (this.Controls[$"b{i}-{j}"] as Button).BackColor = Color.Yellow;
                        }
                    }
                }

                victory.Play();
                MessageBox.Show("YOU WON!","Егор постарался.");
                resetGame();
            }
        }

        public bool checkCell(int row, int colum)
        {
            if ((this.Controls[$"b{row}-{colum}"] as Button).BackColor != Color.White)
                return true;
            return false;
        }

        public void openEmptyCells(int row, int colum)
        {
            if (checkCell(row, colum)) return;
            putTextAndColor(row, colum);

            if (gameArr[row, colum] == "0")
            {
                (this.Controls[$"b{row}-{colum}"] as Button).BackColor = Color.LightGray;
                
                if (row < rows - 1) openEmptyCells(row + 1, colum);                
                if (row > 0) openEmptyCells(row - 1, colum);
                if (colum > 0) openEmptyCells(row, colum - 1);
                if (colum < rows - 1) openEmptyCells(row, colum + 1);
                if (row < rows - 1 && colum < colums - 1) openEmptyCells(row + 1, colum + 1);
                if (row > 0 && colum > 0) openEmptyCells(row - 1, colum - 1);
                if (row > 0 && colum < colums - 1) openEmptyCells(row - 1, colum + 1);
                if (row < rows - 1 && colum > 0) openEmptyCells(row + 1, colum - 1);
            }
        }

        private void b00_Click(object sender, EventArgs e)
        {
            string buttnName = (sender as Button).Name.Replace("b", "");

            int row = int.Parse(buttnName.Substring(0, buttnName.IndexOf("-")));
            int colum = int.Parse(buttnName.Substring(buttnName.IndexOf("-") + 1));

            if (gameArr[row, colum] == "X" && (sender as Button).Text != "⛿")
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < colums; j++)
                    {
                        if (gameArr[i, j] == "X" && this.Controls[$"b{i}-{j}"].Text == "⛿")
                        {
                            (this.Controls[$"b{i}-{j}"] as Button).BackColor = Color.Yellow;
                        }
                        else if (gameArr[i, j] == "X")
                        {
                            (this.Controls[$"b{i}-{j}"] as Button).BackColor = Color.Red;
                        }
                    }
                }

                bomb.Play();
                MessageBox.Show("GAME OVER!", "Егор постарался");
                resetGame();
            }
            else if ((sender as Button).Text != "⛿" && gameArr[row, colum] != "0")
            {
                putTextAndColor(row, colum);
            }
            else if ((sender as Button).Text != "⛿" && (sender as Button).BackColor != Color.Silver)
            {
                openEmptyCells(row, colum);
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < colums; j++)
                    {
                        if ((this.Controls[$"b{i}-{j}"] as Button).BackColor != Color.White &&
                            (this.Controls[$"b{i}-{j}"] as Button).Text == "⛿")
                        {
                            (this.Controls[$"b{i}-{j}"] as Button).Text = "";
                        }                            
                    }
                }
            }

            checkWin();
        }

        private void b00_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && (sender as Button).Text == "⛿")
            {
                (sender as Button).Text = "";
            }
            else if (e.Button == MouseButtons.Right && (sender as Button).BackColor == Color.White)
            {
                (sender as Button).Text = "⛿";
                (sender as Button).ForeColor = Color.Red;
            }
        }

        private void среднийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Height = 553;
            this.Width = 538;
            rows = 16;
            colums = 16;
            totalBombs = 40;
            gameArr = new string[rows, colums];

            putButtons(rows, colums, 29);
            resetGame();
        }

        private void новичок99ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Height = 343;
            this.Width = 315;
            rows = 9;
            colums = 9;
            totalBombs = 10;
            gameArr = new string[rows, colums];

            putButtons(rows, colums, 29);
            resetGame();
        }

        private void профиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Height = 553;
            this.Width = 988;
            rows = 16;
            colums = 30;
            totalBombs = 99;
            gameArr = new string[rows, colums];

            putButtons(rows, colums, 29);
            resetGame();
        }

         private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
         {
            about about = new about();
            about.Show();
         
         }

        private void новаяИграToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            resetGame();
        }
    }
}