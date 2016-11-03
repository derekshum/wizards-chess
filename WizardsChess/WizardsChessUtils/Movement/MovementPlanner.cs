using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;

namespace WizardsChess.Movement
{
    public class MovementPlanner
    {
        public MovementPlanner (ChessBoard b)
        {
            board = b;
        }

        public void Move(Point2D start, Point2D end)
        {
            List<List<Point2D>> paths = new List<List<Point2D>>();
            
            if (board.PieceAt(end) != null)
            {
                paths.Add(getTakenPath(end));
            }
            paths.Add(getMovePath(start, end));
			
			//TODO: call motor control
        }

        //Only used fo en passant
        public void Move(Point2D start, Point2D end, Point2D taken)
        {
            List<List<Point2D>> paths = new List<List<Point2D>>();

            paths.Add(getTakenPath(taken));
            paths.Add(getMovePath(start, end));

            //TODO: call motor control
        }

        private List<Point2D> getMovePath(Point2D start, Point2D end)
        {
            Point2D startPoint = pointConversion(start);
            Point2D endPoint = pointConversion(end);

            //could check for null here, if unsure if that check isn't done elsewhere
            if (board.PieceAt(start).CanJump)
            {
                return getKnightMovePath(startPoint, endPoint);
            }
            else // piece moving is a non knight piece
            {
                return getDirectMovePath(startPoint, endPoint);
            }
        }

        //gets the direct path of movement for non-knight piece moves (but not that of taken pieces)
        private List<Point2D> getDirectMovePath(Point2D startPoint, Point2D endPoint)
        {
            List<Point2D> path = new List<Point2D>();

            path.Add(startPoint);
            path.Add(endPoint);

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

            path.Add(startPoint);
            if (xVsY > 0)  //moving further in the x direction, xVsY should equal 2
            {
                path.Add(new Point2D(startPoint.X, startPoint.Y + yMove/2));
                path.Add(new Point2D(endPoint.X, path[path.Count - 1].Y));
            }
            else   //moving further in the y direction, xVsY should equal -2
            {
                path.Add(new Point2D(startPoint.X + xMove/2, startPoint.Y));
                path.Add(new Point2D(path[path.Count - 1].X, endPoint.Y));
            }
            path.Add(endPoint);

            return path;
        }

        //gets the path of movement for a taken piece from the board to the trough
        //expects points to be in 23x17 form
        private List<Point2D> getTakenPath(Point2D start) //change takenTeam type
        {
            Point2D startPoint = pointConversion(start);
            ChessTeam team = board.PieceAt(start).Team; 
            int numTaken = board.NumDeadPieces(team); 

            List<Point2D> path = new List<Point2D>();

            if (team == ChessTeam.White)
            {
                path = getTakenPathWithTeam(startPoint, whiteRemovalDir, whiteEmptyCol, whiteTakenCol, whiteTakenStart - whiteTakenAddDir * numTaken);
            }
            else    //team == ChessTeam.Black
            {
                path = getTakenPathWithTeam(startPoint, blackRemovalDir, blackEmptyCol, blackTakenCol, whiteTakenStart - whiteTakenAddDir * numTaken);
            }
            return path;
        }

        //path[path.Count - 1].X or Y is just a way of saying the previous value for X or Y in the path
        private List<Point2D> getTakenPathWithTeam(Point2D startPoint, int dir, int emptyCol, int takenCol, int troughIndex)
        {
            //the number of half squares from the edge of the board to put the first taken piece at
            List<Point2D> path = new List<Point2D>();

            path.Add(startPoint);
            path.Add(new Point2D(startPoint.X, startPoint.Y + dir));    //only needs to move half a square, so no multiplier needed on Dir
            if (path[path.Count - 1].Y != troughIndex) //only need to reposition in the empty column if it cannot go straight across
            {
                path.Add(new Point2D(emptyCol, path[path.Count - 1].Y));
                path.Add(new Point2D(path[path.Count - 1].X, troughIndex));
            }
            path.Add(new Point2D(takenCol, troughIndex));
            return path;
        }

        //converts from an (0-7,0-7) point to a (0-22, 0-16) point
        private Point2D pointConversion(Point2D point)
        {
            return new Point2D(xOffset + spacing * point.X, yOffset + spacing + point.Y);
        }

        public ChessBoard board; //TODO: make private when debugging done

		//constants pointConversion needs
		const int spacing = 2;    //number of points per square in one dimension
        const int xOffset = 4;    //x-index of the centre of the left most playing board column
        const int yOffset = 1;    //y-index of the centre of the bottom row
        //constants getTakenPath needs
        const int whiteEmptyCol = 20;     //index of the centre of the empty column in between the board and white taken pieces
        const int blackEmptyCol = 2;      //index of the centre of the empty column in between the board and black taken pieces
        const int whiteTakenCol = 22;     //x-index of the centre of the column where white taken pieces are stored
        const int blackTakenCol = 0;      //x-index of the centre of the column where black taken pieces are stored
        const int whiteTakenStart = 1;    //y-index where the first white piece taken is placed
        const int blackTakenStart = 15;   //y-index where the first black piece taken is placed
        const int whiteRemovalDir = -1;   //y-direction the white pieces are pulled a half square in before being taken off the board
        const int blackRemovalDir = 1;    //y-direction the black pieces are pulled a half square in before being taken off the board
        const int whiteTakenAddDir = 1;   //y-direction white pieces are added to the white taken trough
        const int blackTakenAddDir = -1;  //y-direction black pieces are added to the black taken trough

		//TODO: remove debugging methods below when they're no longer needed
        public String PrintDebug(List<List<Point2D>> paths)
        {
			String toPrint = "";
            paths.ForEach((path) => 
			{
				toPrint += "start move\n";
                path.ForEach((point) => 
				{
					toPrint += point.ToString() + "\n";
                });
				toPrint += "endMove\n";
			});
			return toPrint;
        }

		public List<List<Point2D>> MoveDebug(Point2D start, Point2D end)
		{
			List<List<Point2D>> paths = new List<List<Point2D>>();

			if (board.PieceAt(end) != null)
			{
				paths.Add(getTakenPath(end));
			}
			paths.Add(getMovePath(start, end));

			return paths;
		}

	}
}
