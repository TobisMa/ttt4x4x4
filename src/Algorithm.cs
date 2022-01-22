using System;
using System.Collections.Generic;


namespace ttt4x4x4
{


    partial class TTTGame
    {
        private List<Point> OptimalPoints = new List<Point>() {
            new Point(0, 0, 0),
            new Point(3, 0, 0),
            new Point(3, 3, 0),
            new Point(0, 3, 0),
            new Point(1, 1, 1),
            new Point(2, 1, 1),
            new Point(2, 2, 1),
            new Point(1, 2, 1),
            new Point(1, 1, 2),
            new Point(2, 1, 2),
            new Point(2, 2, 2),
            new Point(1, 2, 2),
            new Point(0, 0, 3),
            new Point(3, 0, 3),
            new Point(0, 3, 3),
            new Point(3, 3, 3)
        };

        private List<List<Point>> rows = new List<List<Point>>();
        private Dictionary<List<Point>, Dictionary<int, int>> ImportanceFieldDict = new Dictionary<List<Point>, Dictionary<int, int>>();  

        private void InitAlgorithm()
        {
            List<Point> points = new List<Point>();

            # region RowsToCheck

            for (int i = 0; i < 4; i++) {

                for (int j = 0; j < 4; j++) {
                    points = new List<Point>();

                    points.Add(new Point(0, j, i));
                    points.Add(new Point(1, j, i));
                    points.Add(new Point(2, j, i));
                    points.Add(new Point(3, j, i));

                    rows.Add(points);

                    points = new List<Point>();

                    points.Add(new Point(j, 0, i));
                    points.Add(new Point(j, 1, i));
                    points.Add(new Point(j, 2, i));
                    points.Add(new Point(j, 3, i));

                    rows.Add(points);

                    points = new List<Point>();

                    points.Add(new Point(i, j, 0));
                    points.Add(new Point(i, j, 1));
                    points.Add(new Point(i, j, 2));
                    points.Add(new Point(i, j, 3));

                    rows.Add(points);
                }

                points = new List<Point>();

                points.Add(new Point(i, 0, 0));
                points.Add(new Point(i, 1, 1));
                points.Add(new Point(i, 2, 2));
                points.Add(new Point(i, 3, 3));

                rows.Add(points);

                points = new List<Point>();

                points.Add(new Point(0, i, 0));
                points.Add(new Point(1, i, 1));
                points.Add(new Point(2, i, 2));
                points.Add(new Point(3, i, 3));

                rows.Add(points);

                points = new List<Point>();

                points.Add(new Point(i, 3, 0));
                points.Add(new Point(i, 2, 1));
                points.Add(new Point(i, 1, 2));
                points.Add(new Point(i, 0, 3));

                rows.Add(points);

                points = new List<Point>();

                points.Add(new Point(3, i, 0));
                points.Add(new Point(2, i, 1));
                points.Add(new Point(1, i, 2));
                points.Add(new Point(0, i, 3));

                rows.Add(points);

                points = new List<Point>();

                points.Add(new Point(0, 0, i));
                points.Add(new Point(1, 1, i));
                points.Add(new Point(2, 2, i));
                points.Add(new Point(3, 3, i));

                rows.Add(points);

                points = new List<Point>();

                points.Add(new Point(0, 3, i));
                points.Add(new Point(1, 2, i));
                points.Add(new Point(2, 1, i));
                points.Add(new Point(3, 0, i));

                rows.Add(points);
            }

            points = new List<Point>();
            for (int i = 0; i < 4; i++) {
                points.Add(new Point(i, i, i));
            }
            rows.Add(points);

            points = new List<Point>();
            for (int i = 0; i < 4; i++) {
                points.Add(new Point(-i + 3, -i + 3, i));
            }
            rows.Add(points);

            points = new List<Point>();
            for (int i = 0; i < 4; i++)
            {
                points.Add(new Point(-i + 3, i, i));
            }
            rows.Add(points);

            points = new List<Point>();
            for (int i = 0; i < 4; i++)
            {
                points.Add(new Point(i, -i + 3, i));
            }
            rows.Add(points);

            # endregion // rowsToCheck
        
            foreach (List<Point> row in rows) {
                ImportanceFieldDict[row] = new Dictionary<int, int>();
            }
        }

        private Point CalculateMove()
        {
            int player = GetPlayerTurn();
            int otherPlayer = (GetPlayerTurn() == PlayerBlue) ? PlayerRed : PlayerBlue;
            int i = 0;
            Point p = new Point(0, 0, 0);
            p = CheckPossibleWin();
            if (!p.Empty)
            {
                return p;
            }

            List<Point> currPlayerPinches = new List<Point>();
            for (i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        if (Get(k, j, i) == EmptyField)
                        {
                            Dictionary<int, bool> pinch = CheckForPinch(new Point(k, j, i));

                            if (pinch[otherPlayer]) {
                                return new Point(k, j, i);
                            }
                            else if (pinch[player]) {
                                currPlayerPinches.Add(new Point(i, j, k));
                            }
                        }
                    }
                }
            }
            if (currPlayerPinches.Count != 0) {
                return GetRandomPoint(currPlayerPinches);
            }

            Dictionary<int, int> required = new Dictionary<int, int>();
            required[PlayerBlue] = 0;
            required[PlayerRed] = 0;

            List<List<Point>> possibleRowsBlue = new List<List<Point>>();
            List<List<Point>> possibleRowsRed = new List<List<Point>>();

            i = 0;
            while (i < rows.Count) {
                Dictionary<int, int> rowCount = new Dictionary<int, int>();
                i = 0;

                for (i = 0; i < rows.Count; i++)
                {
                    List<Point> row = rows[i];
                    rowCount = Count(row);

                    if (rowCount[PlayerBlue] > required[PlayerBlue] || rowCount[PlayerRed] > required[PlayerRed])
                    {
                        // clear beacuse of better statistics
                        possibleRowsBlue = new List<List<Point>>();
                        possibleRowsRed = new List<List<Point>>();

                        if (required[PlayerBlue] < rowCount[PlayerBlue])
                        {
                            required[PlayerBlue] = rowCount[PlayerBlue];
                        }
                        if (required[PlayerRed] < rowCount[PlayerRed])
                        {
                            required[PlayerRed] = rowCount[PlayerRed];
                        }

                        break;
                    }
                    if (rowCount[PlayerBlue] <= required[PlayerBlue] && rowCount[PlayerRed] == required[PlayerRed])
                    {
                        if (CanWinWithRow(row)) {
                            possibleRowsBlue.Add(row);
                        }
                    }
                    if (rowCount[PlayerBlue] == required[PlayerBlue] && rowCount[PlayerRed] <= required[PlayerRed])
                    {
                        if (CanWinWithRow(row)) {
                            possibleRowsRed.Add(row);
                        }
                    }
                }
            }

            Console.WriteLine();  // hold-point

            List<Point> possiblePointsBlue = Program.flatten(possibleRowsBlue);
            List<Point> possiblePointsRed = Program.flatten(possibleRowsRed);

            possiblePointsBlue = RemoveFilledPoints(possiblePointsBlue);
            possiblePointsRed = RemoveFilledPoints(possiblePointsRed);

            List<Point> psB = LookForBestPoint(possiblePointsBlue);
            List<Point> psR = LookForBestPoint(possiblePointsRed);

            List<Point> otherPoints = (GetPlayerTurn() == PlayerBlue) ? psB : psR;
            List<Point> preferredPoints = (GetPlayerTurn() == PlayerBlue) ? psR : psB;

            if (preferredPoints.Count != 0) {
                return GetRandomPoint(preferredPoints);
            }
            else if (otherPoints.Count != 0) {
                return GetRandomPoint(otherPoints);
            }
            else
            {
                possiblePointsBlue = Program.flatten(possibleRowsBlue);
                possiblePointsRed = Program.flatten(possibleRowsRed);

                List<Point> res = new List<Point>();
                res.AddRange(possiblePointsBlue);
                res.AddRange(possiblePointsRed);

                res = RemoveFilledPoints(res);
                List<Point> doubled = Program.RemoveSingle(res);
                
                if (doubled.Count != 0) {
                    return GetRandomPoint(res);
                } else if (res.Count != 0) {
                    return GetRandomPoint(Program.RemoveDouble(res));
                }
            }

            // TODO  remove Used Points from list
            // TODO  look for another point
            // TODO  if no found use random point
            Console.WriteLine("Didn't find a possible move. Looking for a pinch");

            List<Point> otherOptions = new List<Point>();

            for (i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        if (Get(k, j, i) == EmptyField)
                        {
                            otherOptions.Add(new Point(k, j, i));
                        }
                    }
                }
            }
            Console.WriteLine("Using random point...");
            return GetRandomPoint(RemoveFilledPoints(otherOptions));
        }

        private Point CheckPossibleWin() {

            Point empty = new Point(true);
            int currPlayer = GetPlayerTurn();
            int otherPlayer = (GetPlayerTurn() == PlayerBlue) ? PlayerRed : PlayerBlue;
            int anz = 0;
            List<List<Point>> opponentWin = new List<List<Point>>();

            foreach (List<Point> row in rows)
            {
                anz++;
                ImportanceFieldDict[row][PlayerBlue] = 0;
                ImportanceFieldDict[row][PlayerRed] = 0;

                foreach (Point p in row)
                {
                    int f = Get(p);

                    if (f != EmptyField)
                    {
                        ImportanceFieldDict[row][f] += 1;
                    }
                }
                if (ImportanceFieldDict[row][currPlayer] == 3) {
                    Point np =  SetFromRow(row);
                    if (!np.Empty) {
                        return np;
                    }
                }
                else if (ImportanceFieldDict[row][otherPlayer] == 3) {
                    // TODO add opponent win
                    opponentWin.Add(row);
                }
            }

            foreach (List<Point> row in opponentWin) {
                Point p = SetFromRow(row);
                if (!p.Empty) {
                    return p;
                }
            }

            return empty;
        }

        private Point SetFromRow(List<Point> row) {
            foreach (Point p in row) {
                if (Get(p) == EmptyField) {
                    return p;
                }
            }
            return new Point(true);
        }

        private Dictionary<int, int> Count(List<Point> row) {
            Dictionary<int, int> result = new Dictionary<int, int>();

            result[PlayerBlue] = 0;
            result[PlayerRed] = 0;

            foreach (Point p in row) {
                int f = Get(p);
                if (f != EmptyField) {
                    result[f] += 1;
                }
            }

            return result;
        }

        private List<Point> LookForBestPoint(List<Point> ps) 
        {
            List<Point> goodPoints = new List<Point>();

            foreach (Point p in ps) {
                if (Program.IsPointInList(OptimalPoints, p) != -1) {
                    goodPoints.Add(p);
                }
            }

            return goodPoints;
        }

        private static Point GetRandomPoint(List<Point> ps) {
            if (ps.Count == 0) {
                throw new ArgumentException("Randomizer expects list of more than zero items");
            }
            Random r = new Random();

            return ps[r.Next(ps.Count)];
        }

        public List<Point> RemoveFilledPoints(List<Point> ps) {
            List<Point> points = new List<Point>();
            foreach (Point p in ps) {
                if (Get(p) == EmptyField) {
                    points.Add(p);
                }
            }
            return points;
        }

        private bool CanWinWithRow(List<Point> rows) {
            int blue = 0, red = 0, currPlayer = GetPlayerTurn();

            foreach (Point p in rows) {
                int f = Get(p);
                switch (f) {
                    case PlayerBlue:
                        blue++;
                        break;
                    case PlayerRed:
                        red++;
                        break;
                }
                if (blue != 0 && red != 0) {
                    return false;
                }
            }
            return true;
        }

        private Dictionary<int, bool> CheckForPinch(Point p) {
            int found_rows_opponent = 0;
            int found_rows = 0;
            Dictionary<int, bool> res = new Dictionary<int, bool>();
            int player = GetPlayerTurn();
            int otherPlayer = (GetPlayerTurn() == PlayerBlue) ? PlayerRed : PlayerBlue;

            res[player] = false;
            res[otherPlayer] = false;

            foreach (List<Point> row in rows) {
                if (Program.IsPointInList(row, p) != -1) {                    
                    Dictionary<int, int> rowCount = Count(row);
                    if (rowCount[otherPlayer] == 2) {
                        found_rows_opponent++;

                        if (found_rows_opponent >= 2) {
                            res[otherPlayer] = true;
                            return res;  // to speed program up
                        }
                    }
                    if (rowCount[player] == 2) {
                        found_rows++;

                        if (found_rows >= 2) {
                            res[player] = true;
                            // no return (maybe the opponent still has one)
                        }
                    }
                }
            }
            return res;
        }
    }
}