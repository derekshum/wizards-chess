using System;
using System.Collections;
using System.Collections.Generic;

// Board arranged in A-H, 1-8. where A-H is replaced by 9-16

namespace WizardChess{
	class ChessPiece{
		protected string Name;
		protected int xCoordinate;
		protected int yCoordinate;
		protected bool Team1;
		protected bool moveForward;
		protected bool unmoved;

		
		public ChessPiece(string name, bool team1){
			Name = name;
			
			if(team1){
				Team1 = true;
				moveForward = true;
			}else{
				Team1 = false;
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

		public virtual List<int[]> getAllowedMotionVector(){
			List<int[]> test = new List<int[]>();
			return test; 
			
		}
		public virtual List<int[]> getAttackMotionVectors(){
			List<int[]> test = new List<int[]>();
			return test;
		}
		


	}

	class Pawn: ChessPiece{
		public Pawn(string name, bool team1): base(name,team1){
			// if the other team we flip these vectors
			if(team1){
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
		public Knight(string name, bool team1): base(name,team1){
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

		// contains attack motion vectors
		List<int[]> attackMotionVectors = new List<int[]>();

		public override List<int[]> getAllowedMotionVector(){
			return allowedMotionVectors;
		}
		public override List<int[]> getAttackMotionVectors(){
			return attackMotionVectors;
		}

	}

	class Rook: ChessPiece{
		public Rook(string name, bool team1): base(name,team1){
			
		}	

	}

	class Bishop: ChessPiece{
		public Bishop(string name, bool team1): base(name,team1){
			
		}

	}

	class Queen: ChessPiece{
		public Queen(string name, bool team1): base(name,team1){
			
		}
	}

	class King: ChessPiece{
		public King(string name, bool team1): base(name,team1){
			
		}

	}
}