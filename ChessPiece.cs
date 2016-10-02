using System;
using System.Collections;
using System.Collections.Generic;

// Board arranged in A-H, 1-8. where A-H is replaced by 9-16

namespace WizardChess{
	class ChessPiece{
		protected string Name;
		protected int xCoordinate;
		protected int yCoordinate;
		protected string teamName;
		protected bool moveForward;
		protected bool unmoved;

		
		public ChessPiece(string name, string tn){
			Name = name;
				
			if(tn =="Team1"){
				teamName = "Team1";
				moveForward = true;
			}else{
				teamName = "Team2";
				moveForward = false;
			}
			
			unmoved = true;
		}
		public void setCoordinates(int xcoor, int ycoor){
			xCoordinate = xcoor;
			yCoordinate = ycoor;
		}

		public string getName(){
			return Name;
		}

		public void setMoved(){
			unmoved = false;
		}
		public bool getIsItUnMoved(){
			return unmoved;
		}

		public string getTeamName(){
			return teamName;
		}

		public virtual List<int[]> getAllowedMotionVector(){
	
			return new List<int[]>();
		}
			
		public virtual List<int[]> getAttackMotionVectors(){
			return new List<int[]>();
		}
		


	}

	class Pawn: ChessPiece{
		public Pawn(string name, String tn): base(name,tn){
			// if the other team we flip these vectors

			if(tn=="Team1"){
				allowedMotionVectors.Add(new int[]{1,0});
				specialMotionVectors.Add(new int[]{2,0});
				attackMotionVectors.Add(new int[]{1,1});
				attackMotionVectors.Add(new int[]{-1,1});
			}else{
				allowedMotionVectors.Add(new int[]{-1,0});
				specialMotionVectors.Add(new int[]{-2,0});
				attackMotionVectors.Add(new int[]{-1,-1});
				attackMotionVectors.Add(new int[]{1,-1});
			}
			
		}

		// contains motions of allowed chess piece
		List<int[]> allowedMotionVectors = new List<int[]>();

		// contains special motion vectors where additional check required
		List<int[]> specialMotionVectors = new List<int[]>(); 

		// contains attack motion vectors
		List<int[]> attackMotionVectors = new List<int[]>();

		public override List<int[]> getAllowedMotionVector(){
			// if first move can move 2 spaces so, 
			List<int[]> returnable = new List<int[]>();
			if(getIsItUnMoved()){
				returnable.AddRange(allowedMotionVectors);
				returnable.AddRange(specialMotionVectors);
			}else{
				returnable.AddRange(allowedMotionVectors);
			}
			return returnable;
		}
		public override List<int[]> getAttackMotionVectors(){
			return attackMotionVectors;
		}

	}

	class Knight: ChessPiece{
		public Knight(string name, string tn): base(name,tn){
			// if the other team we flip these vectors
			
			allowedMotionVectors.Add(new int[]{-2,-1});
			allowedMotionVectors.Add(new int[]{-2,1});
			allowedMotionVectors.Add(new int[]{2,-1});
			allowedMotionVectors.Add(new int[]{2,1});
			allowedMotionVectors.Add(new int[]{-1,-2});
			allowedMotionVectors.Add(new int[]{1,-2});
			allowedMotionVectors.Add(new int[]{-1,2});
			allowedMotionVectors.Add(new int[]{1,2});
		
			
			
		}

		// contains motions of allowed chess piece
		List<int[]> allowedMotionVectors = new List<int[]>();

		

		public override List<int[]> getAllowedMotionVector(){
			return allowedMotionVectors;
		}
		

	}

	class Rook: ChessPiece{
		public Rook(string name, string tn): base(name,tn){
			allowedMotionVectors.Add(new int[]{-1,0});
			allowedMotionVectors.Add(new int[]{0,-1});
			allowedMotionVectors.Add(new int[]{1,0});
			allowedMotionVectors.Add(new int[]{0,1});
		}	
		// contains motions of allowed chess piece
		List<int[]> allowedMotionVectors = new List<int[]>();

		public override List<int[]> getAllowedMotionVector(){
			return allowedMotionVectors;
		}

	}

	class Bishop: ChessPiece{
		public Bishop(string name, string tn): base(name,tn){
			allowedMotionVectors.Add(new int[]{-1,-1});
			allowedMotionVectors.Add(new int[]{1,1});
			allowedMotionVectors.Add(new int[]{1,-1});
			allowedMotionVectors.Add(new int[]{-1,1});
		}
		// contains motions of allowed chess piece
		List<int[]> allowedMotionVectors = new List<int[]>();

		public override List<int[]> getAllowedMotionVector(){
			return allowedMotionVectors;
		}

	}

	class Queen: ChessPiece{
		public Queen(string name, string tn): base(name,tn){
			allowedMotionVectors.Add(new int[]{-1,-1});
			allowedMotionVectors.Add(new int[]{1,1});
			allowedMotionVectors.Add(new int[]{1,-1});
			allowedMotionVectors.Add(new int[]{-1,1});
			allowedMotionVectors.Add(new int[]{-1,0});
			allowedMotionVectors.Add(new int[]{0,-1});
			allowedMotionVectors.Add(new int[]{1,0});
			allowedMotionVectors.Add(new int[]{0,1});
		}
		// contains motions of allowed chess piece
		List<int[]> allowedMotionVectors = new List<int[]>();

		public override List<int[]> getAllowedMotionVector(){
			return allowedMotionVectors;
		}
	}

	class King: ChessPiece{
		public King(string name, string tn): base(name,tn){
			allowedMotionVectors.Add(new int[]{-1,-1});
			allowedMotionVectors.Add(new int[]{1,1});
			allowedMotionVectors.Add(new int[]{1,-1});
			allowedMotionVectors.Add(new int[]{-1,1});
			allowedMotionVectors.Add(new int[]{-1,0});
			allowedMotionVectors.Add(new int[]{0,-1});
			allowedMotionVectors.Add(new int[]{1,0});
			allowedMotionVectors.Add(new int[]{0,1});
		}
		// contains motions of allowed chess piece
		List<int[]> allowedMotionVectors = new List<int[]>();

		public override List<int[]> getAllowedMotionVector(){
			return allowedMotionVectors;
		}

	}
}