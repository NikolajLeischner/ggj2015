using UnityEngine;
using System.Collections;

public enum MoveDirection
{
	front,
	right,
	back,
	left
}

public enum Face
{
	front,
	right,
	back,
	left,
	top,
	bottom
}

public class CubeSide
{
	public int colorId = 0;
	public Face face;
	
	public CubeSide (Face face, int colorId)
	{
		this.colorId = colorId;
		this.face = face;
	}
	
	public CubeSide Copy ()
	{
		return new CubeSide (face, colorId);
	}
}

public class CubeState
{
	public CubeSide front;
	public CubeSide back;
	public CubeSide top;
	public CubeSide bottom;
	public CubeSide left;
	public CubeSide right;
	public CubeSide currentSide;
	public int colorCount = 0;
	
	CubeState ()
	{
	}
	
	public CubeState (Level level)
	{
		colorCount = level.materials.Length;
		front = new CubeSide (Face.front, level.colorFront);
		back = new CubeSide (Face.back, level.colorBack);
		top = new CubeSide (Face.top, level.colorTop);
		bottom = new CubeSide (Face.bottom, level.colorBottom);
		left = new CubeSide (Face.left, level.colorLeft);
		right = new CubeSide (Face.right, level.colorRight);
		currentSide = bottom;
	}
	
	public CubeState Copy ()
	{
		var copy = new CubeState ();
		copy.colorCount = colorCount;
		copy.front = front.Copy ();
		copy.back = back.Copy ();
		copy.top = top.Copy ();
		copy.bottom = bottom.Copy ();
		copy.right = right.Copy ();
		copy.left = left.Copy ();

		if (currentSide == front)
			copy.currentSide = copy.front;
		else if (currentSide == back)
			copy.currentSide = copy.back;
		else if (currentSide == left)
			copy.currentSide = copy.left;
		else if (currentSide == right)
			copy.currentSide = copy.right;
		else if (currentSide == top)
			copy.currentSide = copy.top;
		else if (currentSide == bottom)
			copy.currentSide = copy.bottom;

		return copy;
	}
	
	public void PerformMove (MoveDirection moveDirection)
	{
		var face = currentSide.face;
		if (face == Face.top) {
			if (moveDirection == MoveDirection.front)
				currentSide = back;
			else if (moveDirection == MoveDirection.back)
				currentSide = front;
			else if (moveDirection == MoveDirection.right)
				currentSide = right;
			else if (moveDirection == MoveDirection.left)
				currentSide = left;
		} else if (face == Face.bottom) {
			if (moveDirection == MoveDirection.front)
				currentSide = front;
			else if (moveDirection == MoveDirection.back)
				currentSide = back;
			else if (moveDirection == MoveDirection.right)
				currentSide = right;
			else if (moveDirection == MoveDirection.left)
				currentSide = left;
		} else if (face == Face.left) {
			if (moveDirection == MoveDirection.front)
				currentSide = front;
			else if (moveDirection == MoveDirection.back)
				currentSide = back;
			else if (moveDirection == MoveDirection.right)
				currentSide = bottom;
			else if (moveDirection == MoveDirection.left)
				currentSide = top;
		} else if (face == Face.right) {
			if (moveDirection == MoveDirection.front)
				currentSide = front;
			else if (moveDirection == MoveDirection.back)
				currentSide = back;
			else if (moveDirection == MoveDirection.right)
				currentSide = top;
			else if (moveDirection == MoveDirection.left)
				currentSide = bottom;
		} else if (face == Face.front) {
			if (moveDirection == MoveDirection.front)
				currentSide = top;
			else if (moveDirection == MoveDirection.back)
				currentSide = bottom;
			else if (moveDirection == MoveDirection.right)
				currentSide = right;
			else if (moveDirection == MoveDirection.left)
				currentSide = left;
		} else if (face == Face.back) {
			if (moveDirection == MoveDirection.front)
				currentSide = bottom;
			else if (moveDirection == MoveDirection.back)
				currentSide = top;
			else if (moveDirection == MoveDirection.right)
				currentSide = right;
			else if (moveDirection == MoveDirection.left)
				currentSide = left;
		}
		
		currentSide.colorId = (currentSide.colorId + 1) % colorCount;
	}
	
	public bool IsSolved ()
	{
		int color = front.colorId;
		return back.colorId == color && top.colorId == color && bottom.colorId == color && left.colorId == color && right.colorId == color;
	}
	
}

public class LevelSolverState
{
	
	public MoveDirection[] moves = new MoveDirection[0];
	CubeState cubeState;
	
	public LevelSolverState (CubeState state)
	{
		this.cubeState = state;
	}
	
	public bool IsSolved ()
	{
		return cubeState.IsSolved ();
	}
	
	private LevelSolverState FromMove (MoveDirection move)
	{
		var next = new LevelSolverState (cubeState.Copy ());
		next.moves = new MoveDirection[moves.Length + 1];
		for (int i = 0; i < moves.Length; ++i) {
			next.moves [i] = moves [i];		
		}
		next.moves [moves.Length] = move;
		next.cubeState.PerformMove (move);
		return next;
	}
	
	public LevelSolverState[] Children ()
	{
		LevelSolverState[] children = new LevelSolverState[4];
		int i = 0;
		foreach (MoveDirection direction in System.Enum.GetValues(typeof(MoveDirection))) {
			children [i] = FromMove (direction);
			++i;
		}
		return children;
	}
}

public class LevelSolver
{
	public static MoveDirection[] SolveLevel (Level level)
	{
		var initialState = new LevelSolverState (new CubeState (level));
		
		if (initialState.IsSolved ())
			return new MoveDirection [0];
		
		int stateCount = 1;
		LevelSolverState[] current = new LevelSolverState[stateCount];
		current [0] = initialState;
		
		stateCount = stateCount * 4;
		LevelSolverState[] next = new LevelSolverState[stateCount];
		
		for (int depth = 0; depth < 10; ++depth) {
			for (int i = 0; i < current.Length; ++i) {
				var children = current [i].Children ();
				for (int j = 0; j < 4; ++j) {
					if (children [j].IsSolved ())
						return children [j].moves;
					next [i * 4 + j] = children [j];
				}
			}
			current = next;
			stateCount = stateCount * 4;
			next = new LevelSolverState[stateCount];
		}
		
		return new MoveDirection [0];
	}
}

