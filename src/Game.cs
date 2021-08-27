using System.Collections.Generic;
using System.Threading;
using System;


namespace ttt4x4x4 {

    partial class TTTGame
    {
        public const int EmptyField = -1;
        public const int NoWinner = -2;
        public const int Tie = -1;
        public const int PlayerBlue = 0;
        public const int PlayerRed = 1;
        private int[,,] GameCube = new int[,,]{
            {
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField},
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField},
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField},
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField}
            },
            {
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField},
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField},
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField},
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField}
            },
            {
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField},
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField},
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField},
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField}
            },
            {
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField},
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField},
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField},
                {TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField, TTTGame.EmptyField}
            }
        };
        private int PlayerTurn = TTTGame.PlayerBlue, Winner = NoWinner;
        private List<int[,,]> History;
        private int HistoryIndex = 0;

        private Point lastBlue = new Point(true);
        private Point lastRed = new Point(true);

        public TTTGame() {
            History = new List<int[,,]>();
            AddCurrentToHistory();
            InitAlgorithm();
        }

        public int Get(int x, int y, int z) {
            return GameCube[x, y, z];
        }

        public int Get(Point p) {
            return Get(p.x, p.y, p.z);
        }

        public bool Set(Point p) {
            if (Get(p.x, p.y, p.z) == TTTGame.EmptyField && Winner == NoWinner) {
                GameCube[p.x, p.y, p.z] = PlayerTurn;
                AddCurrentToHistory();

                if (GetPlayerTurn() == PlayerBlue)
                {
                    lastBlue = p.Copy();
                }
                else 
                {
                    lastRed = p.Copy();
                }

                int w = CheckWinner();
                if (w != NoWinner) 
                {
                    Winner = w; 
                } 
                else 
                {
                    // turn does not change by winner
                    PlayerTurn = (PlayerTurn == PlayerBlue) ? PlayerRed : PlayerBlue;
                }
                return true;
            }
            return false;
        }

        public bool Set(int x, int y, int z) {
            return Set(new Point(x, y, z));
        }

        private void AddCurrentToHistory() {
            History.Add(Program.copy3dArray(GameCube));
            MoveForward();
        }

        public int GetWinner() {
            return Winner;
        }

        /// <summary>
        ///  Needs to be called for changing player turn
        /// </summary>
        /// <returns>Winner value</returns>
        private int CheckWinner(bool force = false) {
            if (Winner != NoWinner && !force) {
                return Winner;
            }

            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    if (
                        (  // horizontal
                            Get(0, j, i) == Get(1, j, i) && Get(2, j, i) == Get(3, j, i)
                         && Get(0, j, i) == Get(2, j, i) && Get(0, j, i) != EmptyField
                        ) || (  // vertical (y-axis)
                            Get(j, 0, i) == Get(j, 1, i) && Get(j, 2, i) == Get(j, 3, i)
                         && Get(j, 0, i) == Get(j, 2, i) && Get(j, 0, i) != EmptyField
                        ) || ( // z-axis
                            Get(i, j, 0) == Get(i, j, 1) && Get(i, j, 2) == Get(i, j, 3)
                         && Get(i, j, 0) == Get(i, j, 2) && Get(i, j, 0) != EmptyField
                        )
                    ) return GetPlayerTurn();
                }
            }
            // diagonals
            for (int i = 0; i < 4; i++) {
                if (
                    ( // bot -> top
                        Get(i, 0, 0) == Get(i, 1, 1) && Get(i, 2, 2) == Get(i, 3, 3)
                     && Get(i, 0, 0) == Get(i, 2, 2) && Get(i, 0, 0) != EmptyField
                    ) || ( // left -> right
                        Get(0, i, 0) == Get(1, i, 1) && Get(2, i, 2) == Get(3, i, 3)
                     && Get(0, i, 0) == Get(2, i, 2) && Get(0, i, 0) != EmptyField
                    ) || (  // bot -> top
                        Get(i, 3, 0) == Get(i, 2, 1) && Get(i, 1, 2) == Get(i, 0, 3)
                     && Get(i, 3, 0) == Get(i, 1, 2) && Get(i, 3, 0) != EmptyField
                    ) || (  // right -> left
                        Get(3, i, 0) == Get(2, i, 1) && Get(1, i, 2) == Get(0, i, 3)
                     && Get(3, i, 0) == Get(1, i, 2) && Get(3, i, 0) != EmptyField
                    ) || ( // topLeft -> botRight on layer Z=i
                        Get(0, 0, i) == Get(1, 1, i) && Get(2, 2, i) == Get(3, 3, i)
                     && Get(0, 0, i) == Get(2, 2, i) && Get(0, 0, i) != EmptyField
                    ) || ( // botLeft -> topRight on layer Z=i
                        Get(0, 3, i) == Get(1, 2, i) && Get(2, 1, i) == Get(3, 0, i)
                     && Get(0, 3, i) == Get(2, 1, i) && Get(0, 3, i) != EmptyField
                    )
                ) return GetPlayerTurn();
            }
            if (
                ( // topLeft -> botRight
                    Get(0, 0, 0) == Get(1, 1, 1) && Get(2, 2, 2) == Get(3, 3, 3)
                 && Get(0, 0, 0) == Get(2, 2, 2) && Get(0, 0, 0) != EmptyField
                ) || ( // topRight -> botLeft
                    Get(3, 0, 0) == Get(2, 1, 1) && Get(1, 2, 2) == Get(0, 3, 3)
                 && Get(3, 0, 0) == Get(1, 2, 2) && Get(3, 0, 0) != EmptyField
                ) || ( // no idea, but it is correct
                    Get(3, 3, 0) == Get(2, 2, 1) && Get(1, 1, 2) == Get(0, 0, 3)
                 && Get(3, 3, 0) == Get(1, 1, 2) && Get(3, 3, 0) != EmptyField
                ) || (
                    Get(0, 3, 0) == Get(1, 2, 1) && Get(2, 1, 2) == Get(3, 0, 3)
                 && Get(0, 3, 0) == Get(2, 1, 2) && Get(0, 3, 0) != EmptyField
                )
            )return GetPlayerTurn();

            // if no winnner was found
            return NoWinner;
        }

        public List<int[,,]> GetHistory() {
            return History;
        }

        public int GetHistoryIndex() {
            return HistoryIndex;
        }

        public void MoveForward() {
            if (HistoryIndex < History.Count) {
                HistoryIndex++;
            }
        }

        public void MoveBackward() {
            if (HistoryIndex > 0) {
                HistoryIndex--;
            }
        }

        public int GetPlayerTurn() {
            return PlayerTurn;
        }

        public string GetWinnerMessage() {
            switch (Winner) {
                case PlayerBlue: return "Winner: Blue";
                
                case PlayerRed: return "Winner: Red";

                case Tie: return "Winner: Tie";
                
                default: return null;
            }
        }

        public int[,,] GetCurrentField() {
            if (History.Count > 0) {
                return History[HistoryIndex - 1];
            }
            return GameCube;
        }

        public TTTGame RestartAt(int where) {
            if (where == 0) {
                return new TTTGame();
            }
            else
            {
                History = History.GetRange(0, where);
                HistoryIndex = where;
                GameCube = History[HistoryIndex - 1];
                Winner = CheckWinner(true);
                return this;
            }
        }

        public Point GetLastBlue() {
            return lastBlue;
        }

        public Point GetLastRed() {
            return lastRed;
        }

        public void SwitchPlayer() {
            PlayerTurn = (PlayerTurn == PlayerBlue) ? PlayerRed : PlayerBlue;
        }

        public void MakeMove() {
            if (Winner != NoWinner) {
                return;
            }
            Point p = CalculateMove();
            Thread.Sleep(300);
            Set(p);
        }
    }
}