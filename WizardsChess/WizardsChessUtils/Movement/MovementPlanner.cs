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

		//Provides movement paths for standard move (with and without capturing)
        public List<List<Point2D>> Move(Point2D start, Point2D end)
        {
            List<List<Point2D>> paths = new List<List<Point2D>>();
            
            if (board.PieceAt(end) != null)
            {
                paths.Add(getCapturedPath(end));
            }
            paths.Add(getMovePath(start, end));

			return paths;
        }

        //Provides movement paths for preforming en passant
        public List<List<Point2D>> Move(Point2D start, Point2D end, Point2D captured)
        {
            List<List<Point2D>> paths = new List<List<Point2D>>();

            paths.Add(getCapturedPath(captured));
            paths.Add(getMovePath(start, end));

			return paths;
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
			else if (start.X != end.X && start.Y != end.Y)	//diagonal movement, could be removed if diagonal movement of the motors is achived
			{
				return getDiagonalMove(startPoint, endPoint);
			}
			else // piece moving is a non knight piece
			{
				return getStraightMovePath(startPoint, endPoint);
			}
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

		private List<Point2D> getDiagonalMove(Point2D startPoint, Point2D endPoint)
		{
			List<Point2D> path = new List<Point2D>();
			int Dist = Math.Abs(endPoint.X - startPoint.X);	//number of points moving in each direction
			int xDir = (endPoint.X - startPoint.X) / Dist;	//x-direction moving in the x direction
			int yDir = (endPoint.Y - startPoint.Y) / Dist;	//y-direction of movement

			path.Add(startPoint);
			path.Add(new Point2D(startPoint.X + xDir, startPoint.Y));
			if (Dist > spacing)
			{
				path.Add(new Point2D(path[path.Count - 1].X, endPoint.Y - yDir));
				path.Add(new Point2D(endPoint.X, path[path.Count - 1].Y));
			}
			else // Dist == spacing
			{
				path.Add(new Point2D(path[path.Count - 1].X, endPoint.Y));
			}

			path.Add(endPoint);

			return path;
		}


		//gets the direct path of movement for non-knight piece moves (but not that of captured pieces)
		private List<Point2D> getStraightMovePath(Point2D startPoint, Point2D endPoint)
		{
			List<Point2D> path = new List<Point2D>();

			path.Add(startPoint);
			path.Add(endPoint);

			return path;
		}

		//gets the path of movement for a captured piece from the board to the trough
		//expects points to be in 23x17 form
		private List<Point2D> getCapturedPath(Point2D start) //change capturedTeam type
        {
            Point2D startPoint = pointConversion(start);
            ChessTeam team = board.PieceAt(start).Team; 
            int numCaptured = board.NumCapturedPieces(team); 

            List<Point2D> path = new List<Point2D>();

            if (team == ChessTeam.White)
            {
                path = getCapturedPathWithTeam(startPoint, whiteRemovalDir, whiteEmptyCol, whiteCapturedCol, whiteCapturedStart - whiteCapturedAddDir * numCaptured);
            }
            else    //team == ChessTeam.Black
            {
                path = getCapturedPathWithTeam(startPoint, blackRemovalDir, blackEmptyCol, blackCapturedCol, blackCapturedStart - blackCapturedAddDir * numCaptured);
            }
            return path;
        }

        //path[path.Count - 1].X or Y is just a way of saying the previous value for X or Y in the path
        private List<Point2D> getCapturedPathWithTeam(Point2D startPoint, int dir, int emptyCol, int capturedCol, int troughIndex)
        {
            //the number of half squares from the edge of the board to put the first captured piece at
            List<Point2D> path = new List<Point2D>();

            path.Add(startPoint);
            path.Add(new Point2D(startPoint.X, startPoint.Y + dir));    //only needs to move half a square, so no multiplier needed on Dir
            if (path[path.Count - 1].Y != troughIndex) //only need to reposition in the empty column if it cannot go straight across
            {
                path.Add(new Point2D(emptyCol, path[path.Count - 1].Y));
                path.Add(new Point2D(path[path.Count - 1].X, troughIndex));
            }
            path.Add(new Point2D(capturedCol, troughIndex));
            return path;
        }

        //converts from an (0-7,0-7) point to a (0-22, 0-16) point
        private Point2D pointConversion(Point2D point)
        {
            return new Point2D(xOffset + spacing * point.X, yOffset + spacing * point.Y);
        }

        private ChessBoard board; 

		//constants pointConversion needs
		const int spacing = 2;    //number of points per square in one dimension
        const int xOffset = 4;    //x-index of the centre of the left most playing board column
        const int yOffset = 1;    //y-index of the centre of the bottom row
        //constants getCapturedPath needs
        const int whiteEmptyCol = 20;     //index of the centre of the empty column in between the board and white captured pieces
        const int blackEmptyCol = 2;      //index of the centre of the empty column in between the board and black captured pieces
        const int whiteCapturedCol = 22;     //x-index of the centre of the column where white captured pieces are stored
        const int blackCapturedCol = 0;      //x-index of the centre of the column where black captured pieces are stored
        const int whiteCapturedStart = 1;    //y-index where the first white piece captured is placed
        const int blackCapturedStart = 15;   //y-index where the first black piece captured is placed
        const int whiteRemovalDir = -1;   //y-direction the white pieces are pulled a half square in before being captured off the board
        const int blackRemovalDir = 1;    //y-direction the black pieces are pulled a half square in before being captured off the board
        const int whiteCapturedAddDir = 1;   //y-direction white pieces are added to the white captured trough
        const int blackCapturedAddDir = -1;  //y-direction black pieces are added to the black captured trough

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
				toPrint += "end move\n";
			});
			return toPrint;
        }
	}
}
