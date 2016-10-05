using System;
using System.Collections.Generic;
using WizardsChessApp.Game.Pieces;

// To control the motions
namespace WizardsChessApp.Game {
	
	class GameManager
	{
		// Global variables
		private static ChessTeam Turn = ChessTeam.White;

		// To play the game
		public void playGame(){
			System.Diagnostics.Debug.WriteLine("Blue Goes first");

			bool execute = false;
			while(true){
				System.Diagnostics.Debug.WriteLine("Enter Start Location");
				string Start = System.Diagnostics.Debug.ReadLine();
				System.Diagnostics.Debug.WriteLine("Enter End Location");
				string End = System.Diagnostics.Debug.ReadLine();

				int[] startCoordinates = getFormattedCoordinate(Start);
				int[] endCoordinates = getFormattedCoordinate(End);

				// Get team of piece to check if it's their turn
				String teamName = grid[startCoordinates[0],startCoordinates[1]].getTeamName();

				if(teamName=="Team1" && team1Turn){
					execute=true;
				}else if(teamName=="Team2" && !team1Turn){
					execute=true;
				}
				
				if(execute){
					bool status = checkMoveValidity(startCoordinates,endCoordinates);

					System.Diagnostics.Debug.WriteLine("Move from "+Start+" to "+End+" is: ");

					if(status){
						System.Diagnostics.Debug.WriteLine("Valid!");
						movePiece(startCoordinates[0],startCoordinates[1],endCoordinates[0],endCoordinates[1]);
						printNodes();

						if(team1Turn){
							team1Turn=false;
							}else{
							team1Turn=true;
						}


					}else{
						System.Diagnostics.Debug.WriteLine("Invalid!");
						printNodes();
					}
					// reset variables 
					execute=false;
			
				}else{
					System.Diagnostics.Debug.Write("You cannot move. It is ");
					if(team1Turn){
						System.Diagnostics.Debug.Write(" Blues Turn\n");
					}else{
						System.Diagnostics.Debug.Write(" Reds Turn\n");
					}
					printNodes();
				}

			}
		}


		// To see if move valid
		public bool CheckMoveValidity(Position startCoordinates, int[] endCoordinates){
			bool isValidMove = false;

			// Get piece at input location
			ChessPiece startPiece = grid[startCoordinates[0],startCoordinates[1]];
			ChessPiece endPiece = grid[endCoordinates[0],endCoordinates[1]];
			
			// To hold object values
			string startPieceName;
			string startPieceTeam;
			string endPieceTeam;
			bool attemptMoveCheck = false;

			// To break if objects in the way
			bool objectsInWay = false;

			// // check if valid move then check if anything blocking seperatly
			if (startPiece != null){
				startPieceName = startPiece.getName();
				System.Diagnostics.Debug.WriteLine(startPieceName);
				startPieceTeam = startPiece.getTeamName();


				// If there is a piece there we need to check if friendly or not
				if(endPiece != null){
					// if is not a friently piece can move there
					endPieceTeam = endPiece.getTeamName();
					if(endPieceTeam!=startPieceTeam){
						attemptMoveCheck = true;
					}

				}else{
					attemptMoveCheck = true;
				}
				if(attemptMoveCheck){
					// This tile is empty, we can look into moving here
					List<int[]> allowedMoveVectors = startPiece.getAllowedMotionVector();

					// given start and end coordinates, let us subtract to get vector
					int xVector = endCoordinates[0]-startCoordinates[0];
					int yVector = endCoordinates[1]-startCoordinates[1];

					// Check to see if move is possible in constraints of piece movement allowment
					foreach (int[] allocatedMoveSet in allowedMoveVectors){
						// if pawn, knight, king, need exact
						if(startPieceName=="Pawn" || startPieceName=="Knight" || startPieceName=="King"){
							foreach (int[] vector in allowedMoveVectors){
								if(vector[0]==xVector && vector[1]==yVector){
									isValidMove = true;
								}
							}
						}else{
							// All of these can have variants of allowed movement vectors
							foreach (int[] vector in allowedMoveVectors){

								if(startPieceName=="Bishop"){
									if(xVector!=0 && yVector !=0){
										if(xVector%vector[0]==yVector%vector[1]){
											// Check for collisions here
											if(checkCollisions(startCoordinates,endCoordinates,vector)){
												isValidMove=true;
												break;
											}
											
										}
									}
								}


								if(startPieceName=="Queen"){
									System.Diagnostics.Debug.WriteLine(xVector);
									System.Diagnostics.Debug.WriteLine(yVector);
									if(xVector!= 0 && yVector!=0){
										//System.Diagnostics.Debug.WriteLine("Diag");
										if(Math.Abs(xVector) == Math.Abs(yVector)){
											// Check for collisions here
											if(checkCollisions(startCoordinates,endCoordinates,vector)){

												isValidMove=true;
												break;
											}
										}
									}else if((xVector==0 && yVector!=0)||(yVector==0 && xVector!=0)){
										//System.Diagnostics.Debug.WriteLine("X Dir");
										
										if(checkCollisions(startCoordinates,endCoordinates,vector)){


											isValidMove = true;
											break;
										}
										
									}
								}

								if(startPieceName=="Rook"){
									if((xVector==0 && yVector!=0)||(yVector==0 && xVector!=0)){
										
										// Only run if relevant to this particular search
										
											if(checkCollisions(startCoordinates,endCoordinates,vector)){
												isValidMove = true;
												break;
											}
											/*
											else{
												objectsInWay=true;
												break;
											}
											*/
										
										
										
									}
								}	
								
							}
						}
						if(isValidMove){
							break;
						}else if(objectsInWay){
							break;
						}
					}

				}

			}else{
				System.Diagnostics.Debug.WriteLine("Piece 1 doesn't exist. Valid input needed to proceed");
			}

			return isValidMove;
		}

		// To check whether there will be collisions
		// Input Start coordinate, end coordinates, vector
		// output if any collisions. True if okay to move
		public bool checkCollisions(int[] startCoordinates,int[] endCoordinates,int[] vector){
			bool ableToMove = true;



			int xIncrementer=0;
			int yIncrementer=0;
			int incrementsNeededToCheck = 0;

			if(vector[0]>0){
				xIncrementer=1;
			}else if(vector[0]<0){
				xIncrementer=-1;
			}else{
				xIncrementer=0;
			}
			if(vector[1]>0){
				yIncrementer = -1;
			}else if(vector[1]<0){
				yIncrementer=1;
			}else{
				yIncrementer=0;
			}	

			if(Math.Abs(endCoordinates[0]-startCoordinates[0])>Math.Abs(endCoordinates[1]-startCoordinates[1])){
				incrementsNeededToCheck=Math.Abs(endCoordinates[0]-startCoordinates[0]);
			}else{
				incrementsNeededToCheck=Math.Abs(endCoordinates[1]-startCoordinates[1]);
			}

			
			int X = startCoordinates[0];
			int Y = startCoordinates[1];
			for(int i=0; i< incrementsNeededToCheck;i++){
				X += xIncrementer;
				Y += yIncrementer;
				
				// ensure values in grid()
				if(X<0 || X>7 || Y<0 || Y>7){
					ableToMove=false;
					break;
				}
				
				if(grid[X,Y]!=null){
					System.Diagnostics.Debug.WriteLine("Stuff in WAY!");
					ableToMove=false;
					break;
				}
			}

			return ableToMove;
		}

		// to move the piece once verified
		// input: start, and end coordinates
		// output: void
		public void movePiece(int startX, int startY, int endX, int endY){
			grid[endX,endY] = grid[startX,startY];
			grid[endX,endY].setMoved();

			grid[startX,startY] = null;
		}


		// To take a coordinate input and turn into something readable
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
					System.Diagnostics.Debug.WriteLine("Invalid move was given");
					break;

			}
			XFinal = Int32.Parse(XRaw);

			// We need to subtract by 1 for the matrix locations
			returnable[0] = XFinal-1;
			returnable[1] = YFinal-1;

			return returnable;

		}

		public void printNodes(){

			int ASCIIA = 64;

			for(int k=-1; k< grid.GetLength(0);k++){
				for(int j=-1; j < grid.GetLength(1);j++){
					if (k==-1){
						if(j==-1){
							//output+= " |           | ";
							System.Diagnostics.Debug.Write(" |           | ");
						}else{	
							//output+=" |    "+(char)ASCIIA+"     | ";
							System.Diagnostics.Debug.Write(" |    "+(char)ASCIIA+"     | ");
						}
						ASCIIA++;

					}else{
						if(j ==-1 && k ==-1){
							System.Diagnostics.Debug.Write(" |          | ");
							
						}else if(j==-1){
							System.Diagnostics.Debug.Write(" |     "+(k+1)+"     | ");
						}else{
							if(grid[k,j] ==null){
								//output+=" |          | ";
								System.Diagnostics.Debug.Write(" |          | ");
							}else{
								string spacer = grid[k,j].getName();
								int spacerIndex = 0;
								spacerIndex = 10 - grid[k,j].getName().Length;
								for(int i=0; i< spacerIndex; i++){
									spacer+=" ";
								}
								//output+= " |"+spacer+"| ";
								System.Diagnostics.Debug.Write(" |");
								string team = grid[k,j].getTeamName();
								if(team=="Team1"){
									System.Diagnostics.Debug.ForegroundColor = ConsoleColor.Blue;
								}else{
									System.Diagnostics.Debug.ForegroundColor = ConsoleColor.Red;
								}
								System.Diagnostics.Debug.Write(spacer);
								System.Diagnostics.Debug.ResetColor();
								System.Diagnostics.Debug.Write("| ");
							}
						}

					}
				}
				//System.Diagnostics.Debug.WriteLine(output);
				System.Diagnostics.Debug.Write("\n");
			}
		}

		

		


	}





	

}

