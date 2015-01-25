using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SideColorFactory
{
		Material[] materials = new Material[3];

		public SideColorFactory (Material[] materials)
		{
				if (materials != null) {
						this.materials = materials;
				}
		}

		public SideColor Create (int id)
		{
				return new SideColor (id, materials [id]);
		}
}

[System.Serializable]
public class Level
{
		public Material[] materials;
		public int colorFront = 0;
		public int colorBack = 1;
		public int colorLeft = 0;
		public int colorRight = 1;
		public int colorTop = 0;
		public int colorBottom = 1;
}

public class GameController : MonoBehaviour
{
	public GameObject effectPrefab;
		public AudioSource audioTryAgain;
		public AudioSource audioWin;
	    public AudioSource audioImpact;
		public Text guiLevelCount;
		public Text guiMovesCount;
		public GameObject cubePrefab;
		GameObject cube = null;
		public GameObject mainCamera;
	public GameObject eye;
		public Transform ground;
		public Level[] levels;
		int currentLevel = 0;
		public CubeColorController cubeColorController;
		public int allowedMoveFactor = 5;
		int allowedMoves = 0;
		int moves = 0;
	bool switchingLevel = false;

		void Start ()
		{
				NextLevel ();
		}

		void NextLevel ()
		{
				if (currentLevel < levels.Length) {
			
						if (cube != null) {
								Destroy (cube);
						}

						var level = levels [currentLevel];
						MoveDirection[] solution = LevelSolver.SolveLevel (level);
						Debug.Log (solution.Length);
						allowedMoves = solution.Length * allowedMoveFactor;

						var sideColorFactory = new SideColorFactory (level.materials);

						cube = (GameObject)Instantiate (cubePrefab);
						cubeColorController = cube.GetComponent<CubeColorController> ();
						cubeColorController.Initialize (sideColorFactory, level, ground);
						var pusher = cube.GetComponent<CubePusher> ();
						pusher.Initialise (mainCamera, eye);
						guiLevelCount.text = "Level: " + currentLevel;

						++currentLevel;
		} else {
			Application.Quit ();
				}
		}

		void RestartLevel ()
		{
				--currentLevel;
				NextLevel ();
		}

		void UpdateMoveText (int currentMoves)
		{
				var alpha = Mathf.Max (0, guiMovesCount.color.a - 0.02f);
				var color = guiMovesCount.color;
				if (moves < currentMoves && moves >= 0) {

			var effectPos = cubeColorController.transform.position;
				effectPos = new Vector3(effectPos.x, 0, effectPos.z);

			Instantiate (effectPrefab, effectPos, Quaternion.identity);

			int sidesWithOneColor = cubeColorController.SidesWithOneColor();
			float pitch = 0.6f + (sidesWithOneColor * 0.15f);
			audioImpact.pitch = pitch;
			            audioImpact.Play ();
						guiMovesCount.text = "" + (moves + 1);
						alpha = 1.0f;
				}
				guiMovesCount.color = new Color (color.r, color.g, color.b, alpha);
	}

	IEnumerator WaitAndNextLevel(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		audioWin.Play ();
		NextLevel ();
		switchingLevel = false;
	}
	
	IEnumerator WaitAndRestartLevel(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		audioTryAgain.Play ();
		RestartLevel ();
		switchingLevel = false;
	}

		void Update ()
		{
				if (cubeColorController != null) {
						int currentMoves = cubeColorController.stats.moves;
						UpdateMoveText (currentMoves);
						moves = currentMoves;
			if (cubeColorController.AllSidesHaveOneColor () && !switchingLevel) {
				switchingLevel = true;
				StartCoroutine(WaitAndNextLevel(2.0F));
			} else if (moves > allowedMoves && !switchingLevel) {
				switchingLevel = true;
				StartCoroutine(WaitAndRestartLevel(2.0F));
						}
				}
		}
}
