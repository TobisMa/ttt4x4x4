using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace ttt4x4x4
{
    public partial class Window : Form
    {
        public static readonly Size ButtonSize = new Size(90, 30);
        public static double MoveInterval = 5d;
        public static int fieldWidth = 26;

        private Button BtnPlayerBlue, BtnPlayerRed, BtnSimulate, BtnUndo, BtnNext, BtnReset;
        private CheckBox CbAlgoToggle;
        private Label LblGameInfo;

        private TTTGame Game;
        private bool SimulateGame = false, ActiveAlgo = true;
        private Thread SimulateThread;

        public Window()
        {
            InitializeComponent();

            Game = new TTTGame();
            AutoScaleMode = AutoScaleMode.Dpi;

            #region IniitalizeWindowElements

            # region BtnPlayerBlue
            this.BtnPlayerBlue = new Button();
            this.BtnPlayerBlue.Size = Window.ButtonSize;
            this.BtnPlayerBlue.Text = "Blue";
            this.BtnPlayerBlue.Top = 10;
            this.BtnPlayerBlue.Left = 10;
            this.BtnPlayerBlue.Click += new System.EventHandler(this.PlayerToggleEvent);
            # endregion

            # region BtnPlayerRed
            this.BtnPlayerRed = new Button();
            this.BtnPlayerRed.Size = Window.ButtonSize;
            this.BtnPlayerRed.Text = "Red";
            this.BtnPlayerRed.Top = 10;
            this.BtnPlayerRed.Left = 20 + Window.ButtonSize.Width;
            this.BtnPlayerRed.Enabled = false;
            this.BtnPlayerRed.Click += new System.EventHandler(this.PlayerToggleEvent);
            # endregion

            # region BtnSimulate
            this.BtnSimulate = new Button();
            this.BtnSimulate.Text = "Simulate";
            this.BtnSimulate.Size = Window.ButtonSize;
            this.BtnSimulate.Top = 100;
            this.BtnSimulate.Left = 10;
            this.BtnSimulate.Click += new System.EventHandler(this.BtnSimulate_Click);
            # endregion

            # region BtnUndo
            this.BtnUndo = new Button();
            this.BtnUndo.Text = "Undo";
            this.BtnUndo.Size = Window.ButtonSize;
            this.BtnUndo.Top = 100 + 10 + Window.ButtonSize.Height;
            this.BtnUndo.Left = 10;
            this.BtnUndo.Enabled = false;
            this.BtnUndo.Click += new System.EventHandler(this.BtnUndo_Click);
            # endregion

            # region BtnNext
            this.BtnNext = new Button();
            this.BtnNext.Text = "Next";
            this.BtnNext.Size = Window.ButtonSize;
            this.BtnNext.Top = 100 + 10 + 2 * Window.ButtonSize.Height;
            this.BtnNext.Left = 10;
            this.BtnNext.Enabled = false;
            this.BtnNext.Click += new System.EventHandler(this.BtnNext_Click);
            #endregion

            # region BtnReset
            this.BtnReset = new Button();
            this.BtnReset.Text = "Reset";
            this.BtnReset.Size = Window.ButtonSize;
            this.BtnReset.Top = 100 + 2 * 10 + 3 * Window.ButtonSize.Height;
            this.BtnReset.Left = 10;
            this.BtnReset.Click += new System.EventHandler(this.BtnReset_Click);
            # endregion

            # region CbAlgoToggle
            CbAlgoToggle = new CheckBox();
            CbAlgoToggle.Text = "Enable Algo?";
            CbAlgoToggle.Top = 70;
            CbAlgoToggle.Left = 10;
            CbAlgoToggle.Size = new Size(Window.ButtonSize.Width * 2, Window.ButtonSize.Height);
            CbAlgoToggle.Checked = true;
            CbAlgoToggle.CheckedChanged += new System.EventHandler(this.AlgoToggle_Changed);
            # endregion 

            #region LblPlayerInfo
            this.LblGameInfo = new Label();
            this.LblGameInfo.Text = "Waiting for Blue";
            this.LblGameInfo.Size = new Size(200, 60);
            this.LblGameInfo.Font = new Font(LblGameInfo.Font.FontFamily, 13f);
            this.LblGameInfo.TextAlign = ContentAlignment.TopCenter;
            this.LblGameInfo.Top = 10;
            this.LblGameInfo.Left = Width / 2 - 200 / 2;

            # endregion
            # endregion // IniitalizeWindowElements

            #region AddElementsToWindow
            // Buttons
            Controls.Add(this.BtnPlayerBlue);
            Controls.Add(this.BtnPlayerRed);
            Controls.Add(this.BtnSimulate);
            Controls.Add(this.BtnNext);
            Controls.Add(this.BtnUndo);
            Controls.Add(this.BtnReset);
            // Checkboxes
            Controls.Add(CbAlgoToggle);

            // Labels
            Controls.Add(this.LblGameInfo);
            #endregion
            CenterToScreen();
        }

        # region event handlers
        /// <summary>
        ///     Makes the move for the player according to the button and switches disabled mode
        /// </summary>
        /// <param name="sender">obejct</param>
        /// <param name="e">e</param>
        private void PlayerToggleEvent(object sender, EventArgs e) 
        {
            if (Game.GetWinner() != TTTGame.NoWinner) 
            {
                return;
            }
            else if (Game.GetPlayerTurn() == TTTGame.PlayerBlue)
            {
                BtnPlayerBlue.Enabled = false;

                Game.MakeMove();
                
                BtnPlayerRed.Enabled = true;
            } 
            else 
            {
                BtnPlayerRed.Enabled = false;

                Game.MakeMove();

                BtnPlayerBlue.Enabled = true;
            }
            Invalidate();
        }

        /// <summary>
        ///     Changes button text and starts/interrupts simulation of game
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">e</param>
        private void BtnSimulate_Click(object sender, EventArgs e)
        {

            SimulateGame = !SimulateGame;  // otherwise text of button is wrong
            BtnSimulate.Text = SimulateGame ? "Pause" : "Simulate";

            if (Game.GetWinner() == TTTGame.NoWinner && SimulateGame) {
                // TODO disable all buttons during simulation

                SimulateThread = new Thread(ThreadSimulate);
                SimulateThread.Start();
            }
            else if (!SimulateGame) 
            {
                if (SimulateThread != null && SimulateThread.IsAlive) {
                    SimulateThread.Join();
                }
                CleanUpSimulation();
            }
        }

        private void CleanUpSimulation() {
            SimulateGame = false;
            if (SimulateThread != null && SimulateThread.IsAlive) {
                SimulateThread.Join();
            }
            BtnSimulate.Text = "Simulate";

            BtnPlayerBlue.Enabled = (Game.GetPlayerTurn() == TTTGame.PlayerBlue);
            BtnPlayerRed.Enabled = !BtnPlayerBlue.Enabled;
        }

        /// <summary>
        /// Called trough BtnSimulate_Click as thread function
        /// </summary>
        private void ThreadSimulate() {
            while (Game.GetWinner() == TTTGame.NoWinner && SimulateGame) 
            {
                Game.MakeMove();
                Invalidate();
                System.Threading.Thread.Sleep(300);
            }
        }

        /// <summary>
        ///     Shows next move or calculates next move when history is exhausted
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">e</param>
        private void BtnNext_Click(object sender, EventArgs e) {
            if (Game.GetHistory().Count > Game.GetHistoryIndex()) 
            {
                Game.MoveForward();
                Game.SwitchPlayer();
                Invalidate();
            }
        }

        private void BtnUndo_Click(object sender, EventArgs e) {
            if (SimulateGame) {
                CleanUpSimulation();
            }
            Game.MoveBackward();
            Game.SwitchPlayer();
            
            Invalidate();
        }
        # endregion // button events

        /// <summary>
        ///     Deactivates or activates the one player mode
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">e</param>
        private void AlgoToggle_Changed(object sender, EventArgs e) 
        {
            ActiveAlgo = !ActiveAlgo;
            Logger.Debug("Toggled Algo mode to " + ActiveAlgo);
        }
        
        # region drawField
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            DrawEmptyField(g);
            DrawFieldBooks(g);

            // TODO Configure button enabled/disabled state

            if (Game.GetWinner() != TTTGame.NoWinner) {
                LblGameInfo.Text = Game.GetWinnerMessage();

                BtnPlayerBlue.Enabled = false;
                BtnPlayerRed.Enabled = false;
                BtnSimulate.Enabled = false;

                if (SimulateGame) {
                    SimulateGame = false;
                    CleanUpSimulation();
                }
            } else {
                int player = Game.GetPlayerTurn();

                LblGameInfo.Text = (player == TTTGame.PlayerBlue) ? "Waiting for Blue" : "Waiting for Red";

                BtnPlayerBlue.Enabled = (player == TTTGame.PlayerBlue);
                BtnPlayerRed.Enabled = (player == TTTGame.PlayerRed);
            }



            BtnUndo.Enabled = (Game.GetHistoryIndex() > 1);
            BtnNext.Enabled = (Game.GetHistoryIndex() < Game.GetHistory().Count);
        }

        private void DrawEmptyField(Graphics g) {
            Brush brush = Brushes.Black;
            Pen pen = new Pen(brush);

            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    for (int k = 0; k < 4; k++) {
                        g.DrawRectangle(
                            pen,
                            660 + k*fieldWidth, 
                            fieldWidth * j + 20 + i * (4*fieldWidth + fieldWidth/2), 
                            fieldWidth, fieldWidth
                        );
                    }
                }
            }
        }

        private void DrawFieldBooks(Graphics g) {

            int[,,] f = Game.GetCurrentField();

            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    for (int k = 0; k < 4; k++)
                    {
                        int current = f[k, j, i];

                        if (current == TTTGame.PlayerBlue) {
                            Brush brush = Brushes.Blue;

                            g.FillEllipse(brush, new Rectangle(
                                660 + k * fieldWidth,
                                fieldWidth * j + 20 + i * (4 * fieldWidth + fieldWidth / 2),
                                fieldWidth, fieldWidth
                            ));
                        } else if (current == TTTGame.PlayerRed) {
                            Brush brush = Brushes.Red;

                            g.FillEllipse(brush, new Rectangle(
                                660 + k * fieldWidth,
                                fieldWidth * j + 20 + i * (4 * fieldWidth + fieldWidth / 2),
                                fieldWidth, fieldWidth
                            ));
                        }
                    }
                }
            }
        }
        # endregion // drawing

        private void BtnReset_Click(object sender, EventArgs e) {
            SimulateGame = false;
            CleanUpSimulation();

            // reset variables
            Game = new TTTGame();

            BtnSimulate.Enabled = true;
            BtnPlayerBlue.Enabled = true;

            BtnPlayerRed.Enabled = false;
            BtnNext.Enabled = false;
            BtnUndo.Enabled = false;
            Invalidate();
        }

        private void MainWindow_Click(object sender, MouseEventArgs e) {
            if (Game.GetHistoryIndex() < Game.GetHistory().Count) {
                Game.RestartAt(Game.GetHistoryIndex());
                Invalidate();
            }
            SetMarker(sender, e);
        }

        private void SetMarker(object sender, MouseEventArgs e) {
            int x = e.X;
            int y = e.Y - 20;  // normalize y-offset instant to zero
            int z = 0;

            x = (x - 660) / fieldWidth; // x-axis on the n layer

            z = (int)(y / (4 * fieldWidth + 0.5 * fieldWidth));

            y = (int)((y - (z * (fieldWidth / 2))) / fieldWidth) % 4;

            if (x < 0 || x > 3 || y < 0 || y > 3 || z < 0 || z > 3) {
                return;
            }

            if (Game.Set(x, y, z)) {
                Invalidate();

                if (ActiveAlgo) {
                    PlayerToggleEvent(null, null);
                    Invalidate();
                }
            }   
        }
    }

    
}
