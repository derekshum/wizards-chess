using System;
using System.Collections.Generic;

// To control the motions
namespace WizardChess{
	
	class GameManager{
		// To test only
		public static void Main(){
			
			// Initiialize a game
			GameManager game = new GameManager();
			game.initialize();
			game.printNodes();


			while(true){
				Console.WriteLine("Enter Start Location");
				string Start = Console.ReadLine();
				Console.WriteLine("Enter End Location");
				string End = Console.ReadLine();

				int[] startCoordinates = game.getFormattedCoordinate(Start);
				int[] endCoordinates = game.getFormattedCoordinate(End);

				bool status = game.checkMoveValidity(startCoordinates,endCoordinates);

				Console.WriteLine("Move from "+Start+" to "+End+" is: ");

				if(status){
					Console.WriteLine("Valid!");
					game.movePiece(startCoordinates[0],startCoordinates[1],endCoordinates[0],endCoordinates[1]);
					game.printNodes();
				}else{
					Console.WriteLine("Invalid!");
				}

			}
			

			//string Start = "G1";
			//string End = "F3";

			

			// Get what piece is at the input location
			//string pieceName = (grid[startCoordinates[0],startCoordinates[1]]).getName();
			//Console.WriteLine(startCoordinates[0]+" "+startCoordinates[1]);
			//Console.WriteLine(endCoordinates[0]+" "+endCoordinates[1]);
			

			// We get an input such as "A3 --> A5". Ex. A8 Sequence is (8,A)--> 8,0

		}

		// To see if move valid
		public bool checkMoveValidity(int[] startCoordinates, int[] endCoordinates){
			bool isValidMove = false;

			// Get piece at input location

			ChessPiece startPiece = grid[startCoordinates[0],startCoordinates[1]];
			ChessPiece endPiece = grid[endCoordinates[0],endCoordinates[1]];
			

		
			// To hold object values
			string startPieceName;
			string startPieceTeam;
			string endPieceTeam;
			bool attemptMoveCheck = false;

			// To hold if possible to move there or not
			bool placementValid = false;

			// To hold onto movement vectors
			int vectorX;
			int vectorY;
			

			List<int[]> allowedMoveSets = new List<int[]>();
			List<int[]> specialMoveSets = new List<int[]>();

			// // check if valid move then check if anything blocking seperatly
			if (startPiece != null){
				startPieceName = startPiece.getName();
				startPieceTeam = startPiece.getTeamName();


				// If there is a piece there we need to check if friendly or not
				if(endPiece != null){
					// if is not a friently piece can move there
					endPieceTeam = endPiece.getTeamName();
					if(endPieceTeam!=startPieceName){
						attemptMoveCheck = true;
					}

				}else{
					attemptMoveCheck = true;
				}
				if(attemptMoveCheck){
					// This tile is empty, we can look into moving here
					List<int[]> allowedMoveVectors = startPiece.getAllowedMotionVector();

					// generate a list of allowed moves
					List<int[]>  allowedTiles = new List<int[]>();

					// Check to see if the move is possible (in constraints of piece movement allowment)
					foreach (int[] vector in allowedMoveVectors){
						placementValid = checkTransform(startCoordinates,endCoordinates,vector);
						if(placementValid){
							vectorX=vector[0];
							vectorY=vector[1];
							break;
						}
					}

					// Check to make sure no collissions if these apply
					if(placementValid){
						if(startPieceName=="Pawn" || startPieceName=="Rook" || startPieceName=="Bishop" || startPieceName=="Queen"){

						}else{
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
			grid[endX,endY].setMoved();

			grid[startX,startY] = null;

		}

		// Given a start coordinate pair, end coordinate pair, and modiifying vector, returns if the vector modifies start to end
		public bool checkTransform(int[] startCoordinates, int[] endCoordinates,int[] vector){
			bool success= false;

			if((startCoordinates[0]+vector[0]==endCoordinates[0]) && (startCoordinates[1]+vector[1]==endCoordinates[1])){
				success = true;			
			}
			return success;
		}



		// To take a coordinate input into something readable
		// Returns an array where val[0]=x, val[1]=y
		// Depreciated. Morgan will be parsing inputs
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
			grid[0,0]=new Rook("Rook","Team1");
			grid[0,7]=new Rook("Rook","Team1");
			grid[7,0]=new Rook("Rook","Team2");
			grid[7,7]=new Rook("Rook","Team2");

			// initialize the knights
			grid[0,1]=new Knight("Knight","Team1");
			grid[0,6]=new Knight("Knight","Team1");
			grid[7,1]=new Knight("Knight","Team2");
			grid[7,6]=new Knight("Knight","Team2");

			// initialize the Bishops
			grid[0,2]=new Bishop("Bishop","Team1");
			grid[0,5]=new Bishop("Bishop","Team1");
			grid[7,2]=new Bishop("Bishop","Team2");
			grid[7,5]=new Bishop("Bishop","Team2");

			// initialize the Kings
			grid[0,4]=new King("King","Team1");
			grid[7,4]=new King("King","Team2");

			// initialize the Kings
			grid[0,3]=new Queen("Queen","Team1");
			grid[7,3]=new Queen("Queen","Team2");

			// initialize pawns
			for (int i=1; i< 7; i+=5){
				for (int k = 0; k< 8; k++){
					if(i==1){
						grid[i,k] = new Pawn("Pawn","Team1");
					}else{
						grid[i,k] = new Pawn("Pawn","Team2");
					}
					
				}

			}
			
			
			

			


		}

		public void printNodes(){

			int ASCIIA = 64;

			for(int k=-1; k< grid.GetLength(0);k++){
				string output = "";
				for(int j=-1; j < grid.GetLength(1);j++){
					if (k==-1){
						if(j==-1){
							//output+= " |           | ";
							Console.Write(" |           | ");
						}else{	
							//output+=" |    "+(char)ASCIIA+"     | ";
							Console.Write(" |    "+(char)ASCIIA+"     | ");
						}
						ASCIIA++;

					}else{
						if(j ==-1 && k ==-1){
							Console.Write(" |          | ");
							
						}else if(j==-1){
							Console.Write(" |     "+(k+1)+"     | ");
						}else{
							if(grid[k,j] ==null){
								//output+=" |          | ";
								Console.Write(" |          | ");
							}else{
								string spacer = grid[k,j].getName();
								int spacerIndex = 0;
								spacerIndex = 10 - grid[k,j].getName().Length;
								for(int i=0; i< spacerIndex; i++){
									spacer+=" ";
								}
								//output+= " |"+spacer+"| ";
								Console.Write(" |");
								string team = grid[k,j].getTeamName();
								if(team=="Team1"){
									Console.ForegroundColor = ConsoleColor.Blue;
								}else{
									Console.ForegroundColor = ConsoleColor.Red;
								}
								Console.Write(spacer);
								Console.ResetColor();
								Console.Write("| ");
							}
						}

					}
				}
				//Console.WriteLine(output);
				Console.Write("\n");
			}
		}

		

		


	}





	

}

