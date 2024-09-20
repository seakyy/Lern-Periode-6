using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace missPacMan
{
    public partial class Form1 : Form
    {
        bool goup;
        bool godown;
        bool goleft;
        bool goright;
        int speed = 4;
        bool isInvincible = true;  // Pacman starts as invincible
        Timer invincibilityTimer;

        // Ghosts' speed variables
        Dictionary<PictureBox, (int xSpeed, int ySpeed)> ghostSpeeds = new Dictionary<PictureBox, (int xSpeed, int ySpeed)>();

        PictureBox greenGhost = new PictureBox();


        int score = 0;

        public Form1()
        {
            InitializeComponent();
            label2.Visible = false;
            this.KeyDown += new KeyEventHandler(keyisdown);
            this.KeyUp += new KeyEventHandler(keyisup);
            this.KeyPreview = true;

            // Main game timer
            timer1 = new Timer();
            timer1.Interval = 20;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();

            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Invincibility timer for Pacman
            invincibilityTimer = new Timer();
            invincibilityTimer.Interval = 2000;  // 2 seconds invincibility after game start
            invincibilityTimer.Tick += new EventHandler(EndInvincibility);
            invincibilityTimer.Start();

            // Initialize ghost speeds
            ghostSpeeds[redGhost] = (6, 6);
            ghostSpeeds[yellowGhost] = (6, 6);
            ghostSpeeds[pinkGhost] = (6, 6);

            this.Load += new EventHandler(Form1_Load);
        }

        private void keyisdown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    goleft = true;
                    pacman.Image = Properties.Resources.left;
                    break;
                case Keys.Right:
                    goright = true;
                    pacman.Image = Properties.Resources.right;
                    break;
                case Keys.Up:
                    goup = true;
                    pacman.Image = Properties.Resources.Up;
                    break;
                case Keys.Down:
                    godown = true;
                    pacman.Image = Properties.Resources.down;
                    break;
            }
        }

        private void keyisup(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    goleft = false;
                    break;
                case Keys.Right:
                    goright = false;
                    break;
                case Keys.Up:
                    goup = false;
                    break;
                case Keys.Down:
                    godown = false;
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = "Score: " + score;
            MovePacman();
            MoveGhosts();
            CheckCollisions();
        }

        private void MovePacman()
        {
            if (goleft && pacman.Left > 0) pacman.Left -= speed;
            if (goright && pacman.Left + pacman.Width < this.ClientSize.Width) pacman.Left += speed;
            if (goup && pacman.Top > 0) pacman.Top -= speed;
            if (godown && pacman.Top + pacman.Height < this.ClientSize.Height) pacman.Top += speed;
        }

        private void MoveGhosts()
        {
            foreach (var ghost in ghostSpeeds.Keys.ToList())
            {
                var (xSpeed, ySpeed) = ghostSpeeds[ghost];
                ghost.Left += xSpeed;
                ghost.Top += ySpeed;

                if (ghost.Left < 0 || ghost.Left + ghost.Width > this.ClientSize.Width)
                {
                    xSpeed = -xSpeed;
                }

                if (ghost.Top < 0 || ghost.Top + ghost.Height > this.ClientSize.Height)
                {
                    ySpeed = -ySpeed;
                }

                // Check for wall collisions
                foreach (Control control in this.Controls)
                {
                    if (control is PictureBox && control.Tag == "wall")
                    {
                        if (ghost.Bounds.IntersectsWith(control.Bounds))
                        {
                            xSpeed = -xSpeed;
                            ySpeed = -ySpeed;
                        }
                    }
                }

                ghostSpeeds[ghost] = (xSpeed, ySpeed);
            }
        }

        private void CheckCollisions()
        {
            if (!isInvincible)
            {
                foreach (Control control in this.Controls)
                {
                    if (control is PictureBox pictureBox)
                    {
                        if (pictureBox.Tag == "wall" || pictureBox.Tag == "ghost")
                        {
                            if (pictureBox.Bounds.IntersectsWith(pacman.Bounds))
                            {
                                EndGame();
                                return;
                            }
                        }
                        else if (pictureBox.Tag == "coin" && pictureBox.Bounds.IntersectsWith(pacman.Bounds))
                        {
                            this.Controls.Remove(pictureBox);
                            score++;
                        }
                    }
                }
            }
        }

        private void EndGame()
        {
            timer1.Stop();
            label2.Text = "GAME OVER";
            label2.Visible = true;

            DialogResult result = MessageBox.Show(
                "Game Over! Do you want to start a new game or exit?",
                "Game Over",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                RestartGame();
            }
            else if (result == DialogResult.No)
            {
                this.Close();
            }
        }

        private void RestartGame()
        {
            pacman.Left = 0;
            pacman.Top = 25;

            Random die = new Random();
            int maxX = die.Next(0, this.Size.Width);
            redGhost.Left = 100;
            yellowGhost.Left = 200;
            pinkGhost.Left = 300;

            score = 0;
            label1.Text = "Score: " + score;
            label2.Visible = false;

            isInvincible = true;
            invincibilityTimer.Start();

            ResetCoins();
            timer1.Start();
        }

        private void EndInvincibility(object sender, EventArgs e)
        {
            isInvincible = false;
            invincibilityTimer.Stop();
        }

        private void ResetCoins()
        {
            foreach (Control control in this.Controls)
            {
                if (control is PictureBox && control.Tag == "coin")
                {
                    this.Controls.Remove(control);
                }
            }

            AddCoin(50, 50);
            AddCoin(100, 100);
            AddCoin(150, 150);
        }

        private void AddCoin(int x, int y)
        {
            PictureBox coin = new PictureBox
            {
                Tag = "coin",
                Image = Properties.Resources.coin,
                Size = new Size(15, 15),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Left = x,
                Top = y
            };
            this.Controls.Add(coin);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ResetCoins();
        }
    }
}
