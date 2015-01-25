using UnityEngine;
using System.Collections;

public class CubePusher : MonoBehaviour
{
	GameObject mainCamera;
	GameObject eye;
	Light pushLight;
		public Transform center;
		public float pushForce = 50;
		
	public float nod_timer_public = 1;
	float nod_timer;
	public float nod_highpoint = 325f;
	bool nod_timer_active = false;
	bool is_nodding = false;
	float current_nod_angle;
	bool nodActive = false;
	GameController controller;

	public float lightIntensity = 0.03f;

	void Start () {
		nod_timer = nod_timer_public;
	}

	public void Initialise(GameObject mainCamera, GameObject eye, bool nodActive, GameController controller) {
		this.nodActive = nodActive;
		this.controller = controller;
		this.mainCamera = mainCamera;
		this.eye = eye;
		this.pushLight = eye.transform.GetChild (0).GetComponent<Light>();
		this.pushLight.intensity = 0.0f;
	}

		void Update ()
		{
		mainCamera.transform.position = center.position;

		var backward = eye.transform.forward * -1.0f;
				backward = new Vector3 (backward.x, 0, backward.z);
				var pushPosition = center.position + (backward.normalized * 0.2f);
				pushPosition = new Vector3 (pushPosition.x, 0, pushPosition.z);

		// nod-mechanic
		current_nod_angle = eye.transform.rotation.eulerAngles.x;

		if (current_nod_angle <= 350 && current_nod_angle >= 340 && is_nodding == false && nod_timer_active == false) {
			nod_timer_active = true;
				}

		if (nod_timer_active == true) {
			nod_timer -= Time.deltaTime;
			is_nodding = true;
			if (current_nod_angle < nod_highpoint) {
					nod_timer_active = false;
					nod_timer = nod_timer_public;
				if (nodActive) {
					controller.startup = false;
					rigidbody.AddForceAtPosition (Vector3.up * pushForce*4, pushPosition);

				}
					is_nodding = false;
					}
				if (nod_timer <= 0f){
					nod_timer_active = false;
					nod_timer = nod_timer_public;
					is_nodding = false;
					}
				}

				if (Input.GetKeyDown ("space")) {
						rigidbody.AddForceAtPosition (Vector3.up * pushForce, pushPosition);
				}

		UpdateLightIntensity ();
		}

	void UpdateLightIntensity() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			pushLight.intensity = Mathf.Min (1.0f, pushLight.intensity + lightIntensity);
		} else {
			pushLight.intensity = Mathf.Max (0.0f, pushLight.intensity - lightIntensity * 0.2f);
				}
	}
}
