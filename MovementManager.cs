using System;
using System.Collections.Generic;

// To control the motions
namespace WizardChess{
	
	class MovementManager{
		// To test only
		public static void Main(){
			
			// Initiialize a game
			MovementManager game = new MovementManager();
			game.initialize();
			game.printNodes();

			string Start = "G1";
			string End = "F3";

			int[] startCoordinates = game.getFormattedCoordinate(Start);
			int[] endCoordinates = game.getFormattedCoordinate(End);

			// Get what piece is at the input location
			//string pieceName = (grid[startCoordinates[0],startCoordinates[1]]).getName();
			//Console.WriteLine(startCoordinates[0]+" "+startCoordinates[1]);
			//Console.WriteLine(endCoordinates[0]+" "+endCoordinates[1]);
			bool status = game.checkMoveValidity(startCoordinates,endCoordinates);

			Console.WriteLine("Move from "+Start+" to "+End+" is: ");

			if(status){
				Console.WriteLine("Valid!");
				game.movePiece(startCoordinates[0],startCoordinates[1],endCoordinates[0],endCoordinates[1]);
				game.printNodes();
			}else{
				Console.WriteLine("Invalid!");
			}

			// We get an input such as "A3 --> A5". Ex. A8 Sequence is (8,A)--> 8,0

		}

		// To see if move valid
		public bool checkMoveValidity(int[] startCoordinates, int[] endCoordinates){
			bool isValidMove = false;

			// Get piece at input location

			ChessPiece startPiece = grid[startCoordinates[0],startCoordinates[1]];
			ChessPiece endPiece = grid[endCoordinates[0],endCoordinates[1]];
			

		

			string startPieceName;
			List<int[]> allowedMoveSets = new List<int[]>();
			List<int[]> specialMoveSets = new List<int[]>();

			// // check if valid move then check if anything blocking seperatly
			if (startPiece != null){
				startPieceName = startPiece.getName();

				// If there is a piece there we need to check if friendly or not
				if(endPiece != null){

				}else{
					// This tile is empty, we can look into moving here
					List<int[]> allowedMoveVectors = startPiece.getAllowedMotionVector();

					// generate a list of allowed moves
					List<int[]>  allowedTiles = new List<int[]>();
					foreach (int[] vector in allowedMoveVectors){
						allowedTiles.Add(new int[]{startCoordinates[0]+vector[0],startCoordinates[1]+vector[1]});
						if((startCoordinates[0]+vector[0]==endCoordinates[0]) && (startCoordinates[1]+vector[1]==endCoordinates[1])){
							isValidMove = true;
						}
					}

					

					

				}

			}else{
				Console.WriteLine("Piece 1 doesn't exist. Valid input needed to proceed");
			}

			

			//string startPiece = grid[startCoordinates[0],startCoordinates[1]].getName();
			//string endPiece = grid[endCoordinates[0],endCoordinates[1]].getName();
			return isValidMove;
			
			
		}
		// to move the piece once verified
		public void movePiece(int startX, int startY, int endX, int endY){
			grid[endX,endY] = grid[startX,startY];
			grid[startX,startY] = null;

		}



		// To take a coordinate input into something readable
		// Returns an array where val[0]=x, val[1]=y
		public int[] getFormattedCoordinate(string coordinate){
			int[] returnable = new int[2];

			string XRaw;
			string YRaw;

			XRaw = coordinate[1].ToString();
			YRaw = coordinate[0].ToString();

			int XFinal=0;
			int YFinal=0;


			switch(YRaw){
				case "A":
					YFinal = 1;
					break;
				case "B":
					YFinal = 2;
					break;
				case "C":
					YFinal = 3;
					break;
				case "D":
					YFinal = 4;
					break;
				case "E":
					YFinal = 5;
					break;
				case "F":
					YFinal = 6;
					break;
				case "G":
					YFinal = 7;
					break;
				case "H":
					YFinal = 8;
					break;
				default:
					Console.WriteLine("Invalid move was given");
					break;

				



			}
			XFinal = Int32.Parse(XRaw);

			// We need to subtract by 1 for the matrix locations
			returnable[0] = XFinal-1;
			returnable[1] = YFinal-1;

			return returnable;

		}

		// To hold the game of chess ( our matrix)
		protected ChessPiece[,] grid = new ChessPiece[8,8];

		// to initialize a game of chess
		public void initialize(){
			// initialize the rooks
			grid[0,0]=new Rook("Rook",true);
			grid[0,7]=new Rook("Rook",true);
			grid[7,0]=new Rook("Rook",false);
			grid[7,7]=new Rook("Rook",false);

			// initialize the knights
			grid[0,1]=new Knight("Knight",true);
			grid[0,6]=new Knight("Knight",true);
			grid[7,1]=new Knight("Knight",false);
			grid[7,6]=new Knight("Knight",false);

			// initialize the Bishops
			grid[0,2]=new Bishop("Bishop",true);
			grid[0,5]=new Bishop("Bishop",true);
			grid[7,2]=new Bishop("Bishop",false);
			grid[7,5]=new Bishop("Bishop",false);

			// initialize the Kings
			grid[0,4]=new King("King",true);
			grid[7,4]=new King("King",false);

			// initialize the Kings
			grid[0,3]=new Queen("Queen",true);
			grid[7,3]=new Queen("Queen",false);

			// initialize pawns
			grid[1,0]=new Pawn("Pawn",true);
			grid[1,1]=new Pawn("Pawn",true);
			grid[1,2]=new Pawn("Pawn",true);
			grid[1,3]=new Pawn("Pawn",true);
			grid[1,4]=new Pawn("Pawn",true);
			grid[1,5]=new Pawn("Pawn",true);
			grid[1,6]=new Pawn("Pawn",true);
			grid[1,7]=new Pawn("Pawn",true);
			grid[6,0]=new Pawn("Pawn",false);
			grid[6,1]=new Pawn("Pawn",false);
			grid[6,2]=new Pawn("Pawn",false);
			grid[6,3]=new Pawn("Pawn",false);
			grid[6,4]=new Pawn("Pawn",false);
			grid[6,5]=new Pawn("Pawn",false);
			grid[6,6]=new Pawn("Pawn",false);
			grid[6,7]=new Pawn("Pawn",false);
			
			

			


		}

		// To print out the chess set
		public void printNodes(){

			for(int k=0; k< grid.GetLength(0);k++){
				string output = "";
				for(int j=0; j < grid.GetLength(1);j++){
					if(grid[k,j] ==null){
						output+=" |          | ";
					}else{
						string spacer = grid[k,j].getName();
						int spacerIndex = 0;
						spacerIndex = 10 - grid[k,j].getName().Length;
						for(int i=0; i< spacerIndex; i++){
							spacer+=" ";
						}
						output+= " |"+spacer+"| ";
					}
				}
				Console.WriteLine(output);
			}
		}

		// To generate where each piece can go

		// for pawn
		// return possible move vectors for pawn
		


	}





	

}

