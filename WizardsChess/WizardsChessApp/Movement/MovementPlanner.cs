using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.Movement; //right?
using WizardsChessApp.Chess;

namespace WizardsChessApp.Movement
{
    class MovementPlanner
    {
        public MovementPlanner (ChessBoard b)
        {
            board = b;
        }

        public void Move(Point2D start, Point2D end)
        {
            List<List<Point2D>> paths = new List<List<Point2D>>();
            
            if (board.getPieceAt(end) != null)
            {
                paths.Add(getTakenPath(end));
            }
            paths.Add(getMovePath(start, end));

            //! call motor control (print for debug)
            Print(paths);
        }

        //Only used fo en passant
        public void Move(Point2D start, Point2D end, Point2D taken) //change takenTeam type
        {
            List<List<Point2D>> paths = new List<List<Point2D>>();

            paths.Add(getTakenPath(taken));
            paths.Add(getMovePath(start, end));

            //! call motor control (print for debug)
            Print(paths);
        }

        private List<Point2D> getMovePath(Point2D start, Point2D end)
        {
            List<Point2D> path = new List<Point2D>();
            
            Point2D startPoint = pointConversion(start);
            Point2D endPoint = pointConversion(end);

            //could check for null here, if unsure if that check isn't done elsewhere
            if (board.getPieceAt(start).CanJump)
            {
                path = getKnightMovePath(startPoint, endPoint);
            }
            else // piece moving is a non knight piece
            {
                path = getDirectMovePath(startPoint, endPoint);
            }
            return path;
        }

        //gets the direct path of movement for non-knight piece moves (but not that of taken pieces)
        private List<Point2D> getDirectMovePath(Point2D startPoint, Point2D endPoint)
        {
            List<Point2D> path = new List<Point2D>();

            path.Add(new Point2D(startPoint.X, startPoint.Y));
            path.Add(new Point2D(endPoint.X, endPoint.Y));

            return path;
        }

        //gets the path of movement for knight piece moves
        //path[path.Count - 1].X or Y is just a way of saying the previous value for X or Y in the path
        private List<Point2D> getKnightMovePath(Point2D startPoint, Point2D endPoint)
        {
            List<Point2D> path = new List<Point2D>();

            int xMove = endPoint.X - startPoint.X; //number of squares moving should both be -4, -2, 2, or 4
            int yMove = endPoint.Y - startPoint.Y;
            int xVsY = Math.Abs(xMove) - Math.Abs(yMove);   //should be -2 or 2

            path.Add(new Point2D(startPoint.X, startPoint.Y));
            if (xVsY == 2)  //moving further in the x direction
            {
                path.Add(new Point2D(path[path.Count - 1].X, path[path.Count - 1].Y + 1));
                path.Add(new Point2D(endPoint.X, path[path.Count - 1].Y));
            }
            else if(xVsY == -2 )    //moving further in the y direction
            {
                path.Add(new Point2D(path[path.Count - 1].X + 1, path[path.Count - 1].Y));
                path.Add(new Point2D(path[path.Count - 1].X, endPoint.Y));
            }
            else
            {
                //! error
            }
            path.Add(new Point2D(endPoint.X, endPoint.Y));

            return path;
        }

        //gets the path of movement for a taken piece from the board to the trough
        //expects points to be in 23x17 form
        private List<Point2D> getTakenPath(Point2D start) //change takenTeam type
        {
            Point2D startPoint = pointConversion(start);
            ChessTeam team = board.getPieceAt(start).Team; 
            int numTaken = board.getNumDeadPieces(team); 

            List<Point2D> path = new List<Point2D>();

            if (team == ChessTeam.White)
            {
                path = getTakenPathWithTeam(startPoint, whiteRemovalDir, whiteEmptyCol, whiteTakenCol, whiteTakenStart - whiteTakenAddDir * numTaken);
            }
            else if(team == ChessTeam.Black)
            {
                path = getTakenPathWithTeam(startPoint, blackRemovalDir, blackEmptyCol, blackTakenCol, whiteTakenStart - whiteTakenAddDir * numTaken);
            }
            else
            {
                 //! error
            }

            return path;
        }

        //path[path.Count - 1].X or Y is just a way of saying the previous value for X or Y in the path
        private List<Point2D> getTakenPathWithTeam(Point2D startPoint, int dir, int emptyCol, int takenCol, int troughIndex)
        {
            //the number of half squares from the edge of the board to put the first taken piece at
            List<Point2D> path = new List<Point2D>();

            path.Add(new Point2D(startPoint.X, startPoint.Y));
            path.Add(new Point2D(path[path.Count - 1].X, path[path.Count - 1].Y + dir));    //only needs to move half a square, so no multiplier needed on Dir
            if (path[path.Count - 1].Y != troughIndex) //only need to reposition in the empty column if it cannot go straight across
            {
                path.Add(new Point2D(emptyCol, path[path.Count - 1].Y));
                path.Add(new Point2D(path[path.Count - 1].X, troughIndex));
            }
            path.Add(new Point2D(takenCol, path[path.Count - 1].Y));
            return path;
        }

        //converts from an (0-7,0-7) point to a (0-22, 0-16) point
        private Point2D pointConversion(Point2D point)
        {
            int spacing = 2;    //the number of points per square in one dimension
            int xOffset = 5;    //the x index of the centre of the left most playing board column
            int yOffset = 1;    //the y index of the centre of the bottom row
            return new Point2D(xOffset + spacing * point.X, yOffset + spacing + point.Y);
        }

        private ChessBoard board;
        
        //constants getTakenPath needs
        const int whiteEmptyCol = 20;     //index of the centre of the empty column in between the board and white taken pieces
        const int blackEmptyCol = 2;      //index of the centre of the empty column in between the board and black taken pieces
        const int whiteTakenCol = 22;     //index of the centre of the column where white taken pieces are stored
        const int blackTakenCol = 0;      //index of the centre of the column where black taken pieces are stored
        const int whiteRemovalDir = -1;   //direction the white pieces are pulled a half square in before being taken off the board
        const int blackRemovalDir = 1;    //direction the black pieces are pulled a half square in before being taken off the board
        const int whiteTakenStart = 1;    //index where the first white piece taken is placed
        const int blackTakenStart = 15;   //index where the first black piece taken is placed
        const int whiteTakenAddDir = 1;   //direction white pieces are added to the white taken trough
        const int blackTakenAddDir = -1;  //direction black pieces are added to the black taken trough

        //! debugging printer
        public void Print(List<List<Point2D>> paths)
        {
            paths.ForEach(delegate (List<Point2D> path) {
                path.ForEach(delegate (Point2D point) {
                    System.Diagnostics.Debug.WriteLine(point.ToString());
                });
            });
        }

    }
}
