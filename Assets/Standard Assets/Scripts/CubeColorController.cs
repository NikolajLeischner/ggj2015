using UnityEngine;
using System.Collections;

public class Side
{
		SideColorFactory sideColorFactory;
		Transform ground;
		public Transform mesh;
		public SideColor sideColor;
		Level level;
		private static float MAX_DISTANCE = 0.2f;

		public Side (Transform ground, SideColorFactory sideColorFactory, Level level)
		{
				this.level = level;
				this.sideColorFactory = sideColorFactory;
				this.ground = ground;
		}

		public void ChangeColor ()
		{
				int nextColor = (sideColor.id + 1) % level.materials.Length;
				SetColor (nextColor);
		}

		public void SetColor (int colorId)
		{
				sideColor = sideColorFactory.Create (colorId);
				mesh.renderer.material = sideColor.material;
				MovieTexture movieTexture = mesh.renderer.material.mainTexture as MovieTexture;
				if (movieTexture != null) {
						movieTexture.Play ();
						movieTexture.loop = true;
				}
		}

		public bool IsOnGround ()
		{
				bool isOnGround = (Mathf.Abs (ground.position.y - mesh.GetChild (0).position.y) <= MAX_DISTANCE);
				return isOnGround;
		}

		public bool Equals (Side other)
		{
				return other != null && other.sideColor.id == sideColor.id;
		}
}

public class Stats
{
		public int moves = -1;
		public float elapsedTime = 0.0f;
}

public class CubeColorController : MonoBehaviour
{
		Transform ground;
		public Transform front;
		public Transform back;
		public Transform left;
		public Transform right;
		public Transform top;
		public Transform bottom;
		Side[] sides;
		Side currentSide = null;
		public static int SIDE_COUNT = 6;
		SideColorFactory factory;
		public Stats stats = new Stats ();
		Level level;

		public bool AllSidesHaveOneColor ()
		{
				var firstSide = sides [0];
				for (int i = 1; i < sides.Length; ++i) {
						if (!sides [i].Equals (firstSide)) {
								return false;
						}
				}
				return true;
		}

	public int SidesWithOneColor() {
		int[] counters = new int[10];
		foreach (Side side in sides)
						counters [side.sideColor.id]++;

		int max = 0;
		foreach (int count in counters)
						max = Mathf.Max (max, count);
		return max;
	}

		private Side CreateSide (Transform mesh, int colorId)
		{
				var side = new Side (ground, factory, level);
				side.mesh = mesh;
				side.SetColor (colorId);
				return side;
		}

		public void Initialize (SideColorFactory factory, Level level, Transform ground)
		{
				this.ground = ground;
				this.level = level;
				stats = new Stats ();
				this.factory = factory;
				sides = new Side[SIDE_COUNT];
				sides [0] = CreateSide (front, level.colorFront);
				sides [1] = CreateSide (back, level.colorBack);
				sides [2] = CreateSide (left, level.colorLeft);
				sides [3] = CreateSide (right, level.colorRight);
				sides [4] = CreateSide (top, level.colorTop);
				sides [5] = CreateSide (bottom, level.colorBottom);
		}

		void Update ()
		{ 
				stats.elapsedTime += Time.deltaTime;
				foreach (Side side in sides) {
						if (side != currentSide && side.IsOnGround ()) {
								if (currentSide != null) {
										side.ChangeColor ();
								}
								currentSide = side;
								++stats.moves;
						}
				}
		}
}
