using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;

namespace WizardsChess.Movement
{
    public class MovePlanner : IMovePlanner
    {
        public MovePlanner (IChessBoard b)
        {
            board = b;
        }

        public List<IList<Point2D>> PlanMove(Point2D start, Point2D end, Point2D? captured=null)	//inputs (0-7, 0-7)
        {
            List<IList<Point2D>> paths = new List<IList<Point2D>>();
            
			if (captured.HasValue)
			{
				paths.Add(getCapturedPath(end));
			}
			if (captured != end && board.PieceAt(end) != null)
			{
				paths.Add(getCapturedPath(end));
			}
			paths.Add(getMovePath(start, end));

			return paths;
        }

		//castles, moving the king over two squares, and moving the rook around it
		public List<IList<Point2D>> PlanCastle(Point2D rookStart, int kingCol)	//rookPoint is the point in (0-7,0-7) form
		{
			List<IList<Point2D>> castlePaths = new List<IList<Point2D>>();

			int rookDir = Math.Sign(rookStart.X - kingCol);	//direction from King to Rook

			//get King Movement
			Point2D kingStartPoint = pointConversion(new Point2D(kingCol, rookStart.Y));
			Point2D kingEndPoint = kingStartPoint + new Vector2D(2 * spacing * rookDir, 0);	//the king moves 2 squares towards the rook
			castlePaths.Add(getStraightMovePath(kingStartPoint, kingEndPoint));

			castlePaths.Add(getCastlingRookPath(pointConversion(rookStart), rookDir, xConversion(kingCol)));  //get Rook Movement- must convert rookStart and kingCol, but rookDir is just a direction and constant

			return castlePaths;
		}

		//Point Conversions from (0-7,0-7) to (-11 to 11, -8 to 8) done here
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
				return getDiagonalMovePath(startPoint, endPoint);
			}
			else //piece moving is a non knight piece
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

		private List<Point2D> getDiagonalMovePath(Point2D startPoint, Point2D endPoint)
		{
			List<Point2D> path = new List<Point2D>();
			int Dist = Math.Abs(endPoint.X - startPoint.X);	//number of points moving in each direction
			int xDir = Math.Sign(endPoint.X - startPoint.X);	//x-direction moving in the x direction
			int yDir = Math.Sign(endPoint.Y - startPoint.Y);	//y-direction of movement

			path.Add(startPoint);
			path.Add(new Point2D(startPoint.X + xDir, startPoint.Y));
			if (Dist > spacing)	//if moving more than 1 square diagonally, 4 part moves required
			{
				path.Add(new Point2D(path[path.Count - 1].X, endPoint.Y - yDir));
				path.Add(new Point2D(endPoint.X, path[path.Count - 1].Y));
			}
			else // Dist == spacing, only 3 part moves required
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
                path = getCapturedPathWithTeam(startPoint, whiteRemovalDir, whiteEmptyCol, whiteCapturedCol, whiteCapturedStart + whiteCapturedAddDir * numCaptured);
			}
            else    //team == ChessTeam.Black
            {
                path = getCapturedPathWithTeam(startPoint, blackRemovalDir, blackEmptyCol, blackCapturedCol, blackCapturedStart + blackCapturedAddDir * numCaptured);
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

        private List<Point2D> getCastlingRookPath(Point2D rookStartPoint, int rookDir, int kingColIndex)	//expects converted (to large index) inputs 
		{
			List<Point2D> rookPath = new List<Point2D>();
			Point2D rookEndPoint = new Point2D(kingColIndex + spacing * rookDir, rookStartPoint.Y);	//the rook's final position is 1 square in its direction from the king's column
			rookPath.Add(rookStartPoint + new Vector2D(0, Math.Sign(rookStartPoint.Y) * castlingStepDir));  //assumes converted points use 0,0 as the centre of the board
			rookPath.Add(new Point2D(rookEndPoint.X, rookPath[rookPath.Count - 1].Y));
			rookPath.Add(rookEndPoint);
			return rookPath;
		}
		
		//converts from a (0-7,0-7) point to large coordinates (-11 to 11, -8 to 8) point
        private Point2D pointConversion(Point2D point)
        {
            return new Point2D(xConversion(point.X), yConversion(point.Y));
        }

		private int xConversion(int x)
		{
			return xOffset + spacing * x;
		}

		private int yConversion(int y)
		{
			return yOffset + spacing * y;
		}

        private IChessBoard board; 

		//constants getCapturedPath needs (in large coordinates)
		const int whiteEmptyCol = 9;     //index of the centre of the empty column in between the board and white captured pieces
        const int blackEmptyCol = -9;      //index of the centre of the empty column in between the board and black captured pieces
        const int whiteCapturedCol = 11;     //x-index of the centre of the column where white captured pieces are stored
        const int blackCapturedCol = -11;      //x-index of the centre of the column where black captured pieces are stored
        const int whiteCapturedStart = -7;    //y-index where the first white piece captured is placed
        const int blackCapturedStart = 7;   //y-index where the first black piece captured is placed
        const int whiteRemovalDir = -1;   //y-direction the white pieces are pulled a half square in before being captured off the board
        const int blackRemovalDir = 1;    //y-direction the black pieces are pulled a half square in before being captured off the board
        const int whiteCapturedAddDir = 1;   //y-direction white pieces are added to the white captured trough
        const int blackCapturedAddDir = -1;  //y-direction black pieces are added to the black captured trough

		//constants getCastlingRookPath needs
		const int castlingStepDir = 1;	//1 means away from the centre of the board, -1 means towards the centre of the board

		//constants pointConversion needs
		const int spacing = 2;    //number of points per square in one dimension
		const int xOffset = -7;    //x-index of the centre of the left most playing board column
		const int yOffset = -7;    //y-index of the centre of the bottom row
	}
}
