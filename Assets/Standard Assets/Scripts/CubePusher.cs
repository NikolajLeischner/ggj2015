using UnityEngine;
using System.Collections;

public class CubePusher : MonoBehaviour
{
		GameObject mainCamera;
		GameObject eye;
		Light pushLight;
		public Transform center;
		public float pushForce = 50;
		public float lightIntensity = 0.03f;

		public void Initialise (GameObject mainCamera, GameObject eye)
		{
				this.eye = eye;
				this.mainCamera = mainCamera;
				this.pushLight = eye.transform.GetChild (0).GetComponent<Light> ();
				this.pushLight.intensity = 0.0f;
		}

		void Update ()
		{
				mainCamera.transform.position = center.position;

				var backward = eye.transform.forward * -1.0f;
				backward = new Vector3 (backward.x, 0, backward.z);
				var pushPosition = center.position + (backward.normalized * 0.2f);
				pushPosition = new Vector3 (pushPosition.x, 0, pushPosition.z);
		
				if (Input.GetKeyDown ("space")) {
						rigidbody.AddForceAtPosition (Vector3.up * pushForce, pushPosition);
				}

				UpdateLightIntensity ();
		}

		void UpdateLightIntensity ()
		{
				if (Input.GetKeyDown (KeyCode.Space)) {
						pushLight.intensity = Mathf.Min (1.0f, pushLight.intensity + lightIntensity);
				} else {
						pushLight.intensity = Mathf.Max (0.0f, pushLight.intensity - lightIntensity * 0.2f);
				}
		}
}
