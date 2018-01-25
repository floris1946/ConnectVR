// Created by Carlos Arturo Rodriguez Silva (Legend)
// Video: https://www.youtube.com/watch?v=LXYWPNltY0s
// Contact: carlosarturors@gmail.com

// Rhythm Visualizator PRO //

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class RhythmVisualizatorPro : MonoBehaviour {

    [SerializeField]
    private GameObject snakeHead;

	#region Variables

	public GameObject soundBarPrefabCenter;
	public GameObject soundBarPrefabDownside;
	public Transform soundBarsTransform;

	[Header ("Audio Settings")] [Tooltip ("Do you want to listen all incoming audios from other AudioSources?")]
	public bool listenAllSounds;
	public AudioSource audioSource;

	[Space (5)]

	[Header ("Sound Bars Options [Requires Restart]")]

	public List<GameObject> soundBars;

	[Range (32, 256)]
	public int barsQuantity = 100;

	int usedSoundBars = 100;

	public enum ScaleFrom
	{
		Center,
		Downside}

	;

	public ScaleFrom scaleFrom = ScaleFrom.Downside;

	[Range (0.1f, 10f)] 
	public float soundBarsWidth = 2f;

	[Header ("Camera Control")] [Tooltip ("Deactivate to use your own camera")]
	public Transform center;

	public bool cameraControl = true;
	[Tooltip ("Rotating around camera")]
	public bool rotateCamera = true;

	public bool UseDefaultCameraOnChange = true;

	[Range (-35, 35)] [Tooltip ("Camera rotating velocity, positive = right, negative = left")]
	public float velocity = 15f;
	[Range (0, 200f)]
	public float height = 40f;

	[Range (0, 500)]
	public float orbitDistance = 300f;

	[Range (1, 179)]
	public int fieldOfView = 60;

	[Header ("Visualization Control")]

	public bool ScaleByRhythm = false;


	[Range (10, 200f)] [Tooltip ("Visualization Length")]
	public float length = 65f;

	public enum Visualizations
	{
		Line,
		Circle,
		ExpansibleCircle,
		Sphere}

	;

	[Tooltip ("Visualization Form")]
	public Visualizations visualization = Visualizations.Line;

	[Range (1f, 50f)]
	public float extraScaleVelocity = 50f;

	[Header ("Levels Control")]
	[Range (0.75f, 15f)] [Tooltip ("Sound Bars global scale")]
	public float globalScale = 3f;
	[Range (1, 15)] [Tooltip ("Sound Bars smooth velocity to return to 0")]
	public int smoothVelocity = 3;

	[Range (0f, 5f)]
	public float minHeight = 1.5f;

	public enum Channels
	{
		n512,
		n1024,
		n2048,
		n4096,
		n8192}

	;

	[Tooltip ("Large value of channels represents more spectrum values, you will need increase the SoundBars amount to represent all these values. Recommended: 4096, 2048")]
	public Channels channels = Channels.n2048;
	[Tooltip ("FFTWindow to use, it is a type of filter. Rectangular = Very Low filter, BlackmanHarris = Very High filter. Recommended = Blackman")]
	public FFTWindow method = FFTWindow.Blackman;
	int channelValue = 2048;

	[Header ("Auto Rhythm Particles [Experimental]")]
	public ParticleSystem rhythmParticleSystem;

	public bool autoRhythmParticles = true;

	[Tooltip ("Rhythm Sensibility, highter values is equal to more particles. Recommended: 5")]
	[Range (0f, 100f)]
	public float rhythmSensibility = 5;

	// Rhythm Minimum Sensibility. This don't need to change, use Rhythm Sensibility instead. Recommended: 1.5
	const float minRhythmSensibility = 1.5f;

	[Tooltip ("Amount of Particles to Emit")]
	[Range (1, 150)]
	public int amountToEmit = 100;

	[Tooltip ("Rhythm Particles Interval Time (Recommended: 0.05 Seconds).")]
	[Range (0.01f, 1f)]
	public float rhythmParticlesMaxInterval = 0.05f;

	float remainingRhythmParticlesTime;
	bool rhythmSurpassed = false;

	[Header ("Bass Control")] // Channel 0 (LEFT)
	[Range (1f, 300f)]
	public float bassSensibility = 40f;
	[Range (0.5f, 2f)]
	public float bassHeight = 1.5f;
	[Range (1, 5)]
	public int bassHorizontalScale = 1;
	[Range (0, 256)] [Tooltip ("Bass Horizontal Off-set")]
	public int bassOffset = 0;

	[Header ("Treble Control")] // Channel 1 (RIGHT)
	[Range (1f, 300f)]
	public float trebleSensibility = 80f;
	[Range (0.5f, 2f)]
	public float trebleHeight = 1.35f;
	[Range (1, 5)]
	public int trebleHorizontalScale = 3;
	[Range (0, 256)] [Tooltip ("Treble Horizontal Off-set, don't decrease or you will get bass values")]
	public int trebleOffset = 67;

	[Header ("Appearance Control")]
	public bool soundBarsParticles = true;

	[Tooltip ("Particles Interval Time (Recommended: 0.005 Seconds). 0 = No interval")]
	[Range (0f, 0.1f)]
	public float particlesMaxInterval = 0.005f;

	float remainingParticlesTime;
	bool surpassed = false;

	[Range (0.1f, 2f)]
	public float minParticleSensibility = 1.5f;
	public bool lerpColor = true;
	public Color [] colors = new Color[4];

	[Range (0.1f, 5f)]
	public float colorIntervalTime = 3f;

	[Range (0.1f, 5f)]
	public float colorLerpTime = 2f;

	public bool useGradient = false;
	public Gradient gradient;
	public Color rhythmParticleSystemColor = Color.white;



	[Header ("Rays [Requires Restart]")]

	[Range (0f, 2f)]
	public float raysLenght = 0.5f;

	[Range (0f, 1f)]
	public float raysAlpha = 0.3f;

	int posColor;

	[HideInInspector]
	public Color actualColor;

	Vector3 prevLeftScale;
	Vector3 prevRightScale;

	Vector3 rightScale;
	Vector3 leftScale;

	float timeChange;

	int halfBarsValue;

	int visualizationNumber = 1;

	float newLeftScale;

	float newRightScale;

	float rhythmAverage;

	Visualizations lastVisualizationForm = Visualizations.Line;
	int lastVisualization = 1;
    
	#endregion

	#region Extra

	/// <summary>
	/// Emits particles if there are rhythm.
	/// </summary>
	public void EmitIfThereAreRhythm () {
		float [] spectrumLeftData;
		float [] spectrumRightData;

		#pragma warning disable 618
		spectrumLeftData = audioSource.GetSpectrumData (channelValue, 0, method);
		spectrumRightData = audioSource.GetSpectrumData (channelValue, 1, method);
		#pragma warning restore 618
	
		int count = 0;
		float spectrumSum = 0;

		// Using bass data only
		for (int i = 0; i < 40; i++) {
			spectrumSum += Mathf.Max (spectrumLeftData [i], spectrumRightData [i]);
			count++;
		}

		rhythmAverage = (spectrumSum / count) * rhythmSensibility;

		// If the spectrum value exceeds the minimum 
		if (rhythmAverage >= minRhythmSensibility) {
			rhythmSurpassed = true;
		}

		// Auto Rhythm Particles
		if (autoRhythmParticles) {
			if (rhythmSurpassed) {
				// Emit particles
				rhythmParticleSystem.Emit (amountToEmit);
			}
		}
	}

	#endregion

	#region Start

	public void Restart () {
		colorUpdated = false;

		for (int i = 0; i < soundBars.Count; i++) {
			DestroyImmediate (soundBars [i]);
		}

		soundBars.Clear ();

		//	Application.targetFrameRate = -1;

		// Get actual visualization
		if (visualization == Visualizations.Line) {
			visualizationNumber = 1;
		} else if (visualization == Visualizations.Circle) {
			visualizationNumber = 2;
		} else if (visualization == Visualizations.ExpansibleCircle) {
			visualizationNumber = 3;
		} else if (visualization == Visualizations.Sphere) {
			visualizationNumber = 4;
		}

		lastVisualization = visualizationNumber;

		// Check the prefabs
		if ((soundBarPrefabCenter != null) && (soundBarPrefabDownside != null)) {

			usedSoundBars = barsQuantity;
			halfBarsValue = usedSoundBars / 2;

			CreateCubes ();

		} else {
			Debug.LogWarning ("Please assign Sound Bar Prefabs to the script");
			enabled = false;
		}

	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake () {
		Restart ();
	}

	/// <summary>
	/// Creates the cubes.
	/// </summary>
	void CreateCubes () {

		float newRayScale = (raysLenght * 5);
		float newWidth = soundBarsWidth - 1;
		GameObject soundBarToInstantiate;

		if (scaleFrom == ScaleFrom.Center) {
			soundBarToInstantiate = soundBarPrefabCenter;
			newWidth = (newWidth / 2f) - 0.5f;
		} else {
			soundBarToInstantiate = soundBarPrefabDownside;
		}

        GameObject prevClone = null;

        for (int i = 0; i < usedSoundBars; i++) { 

			
			var clone = Instantiate (soundBarToInstantiate, transform.position, Quaternion.identity) as GameObject;
			clone.transform.SetParent (soundBarsTransform.transform);
			clone.GetComponent<SoundBar> ().cube.transform.localScale = new Vector3 (soundBarsWidth, 1, 1);

			clone.name = string.Format ("SoundBar {0}", i + 1);

			var renderers = clone.GetComponentsInChildren<Renderer> ();

			Color newColor = colors [0];
			Color newColor2 = newColor;
			newColor2.a = raysAlpha;

			if (useGradient) {
				newColor = gradient.Evaluate (((float)(i + 1) / (float)usedSoundBars));
		

				var rhythmParticleS = rhythmParticleSystem.main;
				rhythmParticleS.startColor = rhythmParticleSystemColor;
			}

			foreach (Renderer rend in renderers) {
				rend.material.color = newColor;
			}

			var actualParticleSystem = clone.GetComponentInChildren<ParticleSystem> ().main;
			actualParticleSystem.startColor = newColor;

			clone.GetComponent<SoundBar> ().ray.material.SetColor ("_TintColor", newColor2);

			if (scaleFrom == ScaleFrom.Downside) {
				clone.GetComponent<SoundBar> ().ray.transform.localScale = new Vector3 (Mathf.Clamp (newWidth, 1, Mathf.Infinity), 1, raysLenght);
				clone.GetComponent<SoundBar> ().ray.transform.localPosition = new Vector3 (0, newRayScale, 0);
			} else {
				clone.GetComponent<SoundBar> ().ray.transform.localScale = new Vector3 (Mathf.Clamp (newWidth, 0.5f, Mathf.Infinity), 1, raysLenght);
			}

            if (prevClone != null)
            {
                Destroy(clone.GetComponent<ConfigurableJoint>());
                //clone.GetComponent<Rigidbody>().mass = 50;
                clone.AddComponent<SnakeBody>();
                snakeHead.GetComponent<SnakeMovement>().bodyParts.Add(clone.transform);
            }

            soundBars.Add (clone);

            prevClone = clone;
        }

		UpdateVisualizations ();
	}

	/// <summary>
	/// Change to the next form. TRUE = Next, FALSE = PREVIOUS
	/// </summary>
	/// <param name="next">If set to <c>true</c> next.</param>
	public void NextForm (bool next) {
		if (next) {
			visualizationNumber++;
		} else {
			visualizationNumber--;
		}

		if (visualizationNumber > 4) {
			visualizationNumber = 1;
		} else if (visualizationNumber <= 0) {
			visualizationNumber = 4;
		}

		if (visualizationNumber == 1) {
			visualization = Visualizations.Line;
		} else if (visualizationNumber == 2) {
			visualization = Visualizations.Circle;
		} else if (visualizationNumber == 3) {
			visualization = Visualizations.ExpansibleCircle;
		} else if (visualizationNumber == 4) {
			visualization = Visualizations.Sphere;
		}

		UpdateVisualizations ();
	}

	/// <summary>
	/// Updates the channels of audio.
	/// </summary>
	void UpdateChannels () {
		if (channels == Channels.n512) {
			channelValue = 512;
		} else if (channels == Channels.n1024) {
			channelValue = 1024;
		} else if (channels == Channels.n2048) {
			channelValue = 2048;
		} else if (channels == Channels.n4096) {
			channelValue = 4096;
		} else if (channels == Channels.n8192) {
			channelValue = 8192;
		}
	}

	#endregion

	#region Camera


	/// <summary>
	/// Change to Camera Predefined Positions
	/// </summary>
	void CameraPosition () {
		if (visualization == Visualizations.Line) {
			Camera.main.fieldOfView = fieldOfView;
			var cameraPos = transform.position;
			cameraPos.z -= 170f;
			Camera.main.transform.position = cameraPos;
			cameraPos.y += 5f + height;
			Camera.main.transform.position = cameraPos;
			Camera.main.transform.LookAt (center);


		} else if (visualization == Visualizations.Circle) {
			Camera.main.fieldOfView = fieldOfView;
			var cameraPos = transform.position;
			cameraPos.y += ((1f + height) / 20f);
			cameraPos.z += 5f; 
			Camera.main.transform.position = cameraPos;

			Camera.main.transform.LookAt (soundBarsTransform.position);

		} else if (visualization == Visualizations.ExpansibleCircle) {
			Camera.main.fieldOfView = fieldOfView;
			var cameraPos = transform.position;
			cameraPos.y += 55f;
			Camera.main.transform.position = cameraPos;
			Camera.main.transform.LookAt (soundBarsTransform.position);
		

		} else if (visualization == Visualizations.Sphere) {
			Camera.main.fieldOfView = fieldOfView;
			var cameraPos = transform.position;
			cameraPos.z -= 40f;
			cameraPos.y += 5f + height;

			Camera.main.transform.position = cameraPos;

			Camera.main.transform.LookAt (soundBarsTransform.position);
			Camera.main.transform.position = cameraPos;
		}

	       
	}

	void SetVisualizationPredefinedValues () {
		if (visualizationNumber == 1) {
			height = 40;
			orbitDistance = 300;


		} else if (visualizationNumber == 2) {
			height = 40;
			orbitDistance = 125;


		} else if (visualizationNumber == 3) {
			height = 40;
			orbitDistance = 175;



		} else if (visualizationNumber == 4) {
			height = 15;
			orbitDistance = 220;

		}
	}
		
	/// <summary>
	/// Camera Rotating Around Movement.
	/// </summary>
	void CameraMovement () {
		Camera.main.transform.position = center.position + (Camera.main.transform.position - center.position).normalized * orbitDistance;

		if (rotateCamera) {
			Camera.main.transform.RotateAround (center.position, Vector3.up, -velocity * Time.deltaTime);
		}
	}

	#endregion

	#region ColorLerp

	Color currentColor;

	/// <summary>
	/// Change SoundBars and Particles Color.
	/// </summary>
	void ChangeColor () {

		currentColor = soundBars [0].GetComponent<SoundBar> ().cube.material.color;

		actualColor = Color.Lerp (currentColor, colors [posColor], Time.deltaTime / colorLerpTime);

		foreach (GameObject cube in soundBars) {
			var newColor = actualColor;
			newColor.a = raysAlpha;
			cube.GetComponent<SoundBar> ().ray.material.SetColor ("_TintColor", newColor);
			cube.GetComponent<SoundBar> ().cube.material.color = actualColor;

			var ps = cube.GetComponent<SoundBar> ().particleSystem.main;
			ps.startColor = actualColor;

			var actualParticleSystem = rhythmParticleSystem.main;
			actualParticleSystem.startColor = actualColor;
		}
	}

	/// <summary>
	/// Change SoundBars and Particles Color Helper.
	/// </summary>
	void NextColor () {

		timeChange = colorIntervalTime;
		lerpColor = false;

		if (posColor < colors.Length - 1) {
			posColor++;
		} else {
			posColor = 0;
		}
		lerpColor = true;
	}

	#endregion

	#region Visualizations

	/// <summary>
	/// Updates the visualizations.
	/// </summary>
	public void UpdateVisualizations () {
		
		// Visualizations

		if (visualization == Visualizations.Circle) {
			for (int i = 0; i < usedSoundBars; i++) {
				float angle = i * Mathf.PI * 2f / usedSoundBars;
				Vector3 pos = soundBarsTransform.transform.localPosition;
				pos -= new Vector3 (Mathf.Cos (angle), 0, Mathf.Sin (angle)) * length;
				soundBars [i].transform.localPosition = pos;
				soundBars [i].transform.LookAt (soundBarsTransform.position);

				var rot = soundBars [i].transform.eulerAngles;
				rot.x = 0;
				soundBars [i].transform.localEulerAngles = rot;
			}

		} else if (visualization == Visualizations.Line) {
			for (int i = 0; i < usedSoundBars; i++) {
				Vector3 pos = soundBarsTransform.transform.localPosition;
				pos.x -= length * 5;
				pos.x += (length / usedSoundBars) * (i * 10);

				soundBars [i].transform.localPosition = pos;
				soundBars [i].transform.localEulerAngles = Vector3.zero;
			}
		} else if (visualization == Visualizations.ExpansibleCircle) {
			for (int i = 0; i < usedSoundBars; i++) {
				float angle = i * Mathf.PI * 2f / usedSoundBars;
				Vector3 pos = soundBarsTransform.transform.localPosition;
				pos -= new Vector3 (Mathf.Cos (angle), 0, Mathf.Sin (angle)) * length;
				soundBars [i].transform.localPosition = pos;
				soundBars [i].transform.LookAt (soundBarsTransform.position);

				var newRot = soundBars [i].transform.eulerAngles;
				newRot.x -= 90;

				soundBars [i].transform.eulerAngles = newRot;
			}

		} else if (visualization == Visualizations.Sphere) {

			var points = UniformPointsOnSphere (usedSoundBars, length);

			for (var i = 0; i < usedSoundBars; i++) {

				soundBars [i].transform.localPosition = points [i];

				soundBars [i].transform.LookAt (soundBarsTransform.position);

				var rot = soundBars [i].transform.eulerAngles;
				rot.x -= 90;

				soundBars [i].transform.eulerAngles = rot;
			}
		}
			
		UpdateChannels ();

		if (cameraControl) {
			
			if (lastVisualizationForm != visualization) {
				lastVisualization = visualizationNumber;

				if (visualization == Visualizations.Line) {
					visualizationNumber = 1;
				} else if (visualization == Visualizations.Circle) {
					visualizationNumber = 2;
				} else if (visualization == Visualizations.ExpansibleCircle) {
					visualizationNumber = 3;
				} else if (visualization == Visualizations.Sphere) {
					visualizationNumber = 4;
				}

				if (UseDefaultCameraOnChange) {
					SetVisualizationPredefinedValues ();
				}


			}

			CameraPosition ();

		}
	}

	/// <summary>
	/// Create a Sphere with the given verticles number.
	/// </summary>
	/// <returns>The points on sphere.</returns>
	/// <param name="verticlesNum">Verticles number.</param>
	/// <param name="scale">Scale.</param>
	Vector3[] UniformPointsOnSphere (float verticlesNum, float scale) {
		var points = new List<Vector3> ();
		var i = Mathf.PI * (3 - Mathf.Sqrt (5));
		var o = 2 / verticlesNum;
		for (var k = 0; k < verticlesNum; k++) {
			var y = k * o - 1 + (o / 2);
			var r = Mathf.Sqrt (1 - y * y);
			var phi = k * i;
			points.Add (new Vector3 (Mathf.Cos (phi) * r, y, Mathf.Sin (phi) * r) * scale);
		}
		return points.ToArray ();
	}

	#endregion

	bool colorUpdated = false;


    #region BaseScript

    /// <summary>
    /// Updates every frame this instance.
    /// </summary>
    void LateUpdate () {

		// Change Colors
		if (lerpColor) {
			timeChange -= Time.deltaTime;

			// When the counter are less than 0, change to the next Color
			if (timeChange < 0f) {
				NextColor ();
			}

			// Execute color lerping
			ChangeColor ();

			colorUpdated = false;
		} else {
			if (useGradient) {
				if (!colorUpdated) {


					for (int i = 0; i < soundBars.Count; i++) {
						var newColor = gradient.Evaluate (((float)(i + 1) / (float)usedSoundBars));

						soundBars[i].GetComponent<SoundBar> ().cube.material.color = newColor;

						var actualParticleSystem = soundBars[i].GetComponentInChildren<ParticleSystem> ().main;
						actualParticleSystem.startColor = newColor;

						newColor.a = raysAlpha;

						soundBars[i].GetComponent<SoundBar> ().ray.material.SetColor ("_TintColor", newColor);

					}

					var actualRhythmPS = rhythmParticleSystem.main;
					actualRhythmPS.startColor = rhythmParticleSystemColor;

					colorUpdated = true;
				}
			} else {
				colorUpdated = false;
			}
		}

		// Get Spectrum Data from Both Channels of audio
		float [] spectrumLeftData;
		float [] spectrumRightData;


		if (listenAllSounds) {
			// Get Spectrum Data from Both Channels of audio
			#pragma warning disable 618
			spectrumLeftData = AudioListener.GetSpectrumData (channelValue, 0, method);
			spectrumRightData = AudioListener.GetSpectrumData (channelValue, 1, method);
			#pragma warning restore 618
		} else {

			if (audioSource == null) {
				Debug.LogWarning ("No AudioSource detected 'Listen All Sounds' activated");
				listenAllSounds = true;

				// Get Spectrum Data from Both Channels of audio
				#pragma warning disable 618
				spectrumLeftData = AudioListener.GetSpectrumData (channelValue, 0, method);
				spectrumRightData = AudioListener.GetSpectrumData (channelValue, 1, method);
				#pragma warning restore 618
			} else {

				if (audioSource.clip == null) {
					Debug.LogWarning ("Assign an AudioClip to the AudioSource and Play it!");
				}

				// Get Spectrum Data from Both Channels of audio
				#pragma warning disable 618
				spectrumLeftData = audioSource.GetSpectrumData (channelValue, 0, method);
				spectrumRightData = audioSource.GetSpectrumData (channelValue, 1, method);
				#pragma warning restore 618
			}
		}

		// Wait for Rhythm Particles Interval (for performance)
		if (remainingRhythmParticlesTime <= 0) {
			
			int count = 0;
			float spectrumSum = 0;

			// Using bass data only
			for (int i = 0; i < 40; i++) {
				spectrumSum += Mathf.Max (spectrumLeftData [i], spectrumRightData [i]);
				count++;
			}

			rhythmAverage = (spectrumSum / count) * rhythmSensibility;


			// If the spectrum value exceeds the minimum 
			if (rhythmAverage >= minRhythmSensibility) {
				rhythmSurpassed = true;
			}

			// Auto Rhythm Particles
			if (autoRhythmParticles) {
				if (rhythmSurpassed) {
					// Emit particles
					rhythmParticleSystem.Emit (amountToEmit);
				}
			}
		}
			

		// Scale SoundBars Normally
		if (!ScaleByRhythm) {

			// SoundBars for Left Channel and Right Channel
			for (int i = 0; i < halfBarsValue; i++) {

				// Apply Off-Sets to get the AudioSpectrum
				int spectrumLeft = i * bassHorizontalScale + bassOffset;
				int spectrumRight = i * trebleHorizontalScale + trebleOffset;

				// Get Actual Scale from SoundBar in "i" position
				prevLeftScale = soundBars [i].transform.localScale;
				prevRightScale = soundBars [i + halfBarsValue].transform.localScale;

				var spectrumLeftValue = spectrumLeftData [spectrumLeft] * bassSensibility;
				var spectrumRightValue = spectrumRightData [spectrumRight] * trebleSensibility;

				// Left Channel //

				// Apply scale to that SoundBar using Lerp
				newLeftScale = Mathf.Lerp (prevLeftScale.y,
				                           spectrumLeftValue * bassHeight * globalScale,
				                           Time.deltaTime * extraScaleVelocity);

				EmitParticle (i, spectrumLeftValue);

				// If the New Scale is greater than Previous Scale, set the New Value
				if (newLeftScale >= prevLeftScale.y) {
					prevLeftScale.y = newLeftScale;
					leftScale = prevLeftScale;

//					leftScale.y = Mathf.Lerp (soundBars [i].transform.localScale.y, newLeftScale, Time.deltaTime * smoothVelocity * 10);
				} else { // Else, Lerp to MinYValue
					leftScale = prevLeftScale;
					leftScale.y = Mathf.Lerp (prevLeftScale.y, minHeight, Time.deltaTime * smoothVelocity);
				}
					
				// Set new scale
				soundBars [i].transform.localScale = leftScale;

				// Right Channel //

				// Apply scale to that SoundBar using Lerp
				newRightScale = Mathf.Lerp (prevRightScale.y,
				                            spectrumRightValue * trebleHeight * globalScale,
				                            Time.deltaTime * extraScaleVelocity);

				EmitParticle (i + halfBarsValue, spectrumRightValue);

				// If the New Scale is greater than Previous Scale, set the New Value
				if (newRightScale >= prevRightScale.y) {
					prevRightScale.y = newRightScale;
					rightScale = prevRightScale;
//					rightScale.y = Mathf.Lerp (soundBars [i].transform.localScale.y, newRightScale.y, Time.deltaTime * smoothVelocity * 10);

				} else { // Else, Lerp to MinY
					rightScale = prevRightScale;
					rightScale.y = Mathf.Lerp (prevRightScale.y, minHeight, Time.deltaTime * smoothVelocity);
				}

				// Set new scale
				soundBars [i + halfBarsValue].transform.localScale = rightScale;

			}

		} else { // Scale All SoundBars by Rhythm

			for (int i = 0; i < usedSoundBars; i++) {
				
				prevLeftScale = soundBars [i].transform.localScale;

				// If Minimum Particle Sensibility is exceeded (volume is clamped beetween 0.01 and 1 to avoid 0)
				if (rhythmSurpassed) {

					// Apply extra scale to that SoundBar using Lerp
					newLeftScale = Mathf.Lerp (prevLeftScale.y,
					                           rhythmAverage * bassHeight * globalScale,
					                           Time.deltaTime * smoothVelocity);

					// If the Particles are activated, emit a particle too
					if (soundBarsParticles) {
						if (remainingParticlesTime <= 0f) {
							soundBars [i].GetComponentInChildren<ParticleSystem> ().Play ();

							surpassed = true;
						}
					}

				} else { 	// Else, Lerp to the previous scale
					newLeftScale = Mathf.Lerp (prevLeftScale.y,
					                           rhythmAverage * globalScale,
					                           Time.deltaTime * extraScaleVelocity);
				}

				// If the New Scale is greater than Previous Scale, set the New Value
				if (newLeftScale >= prevLeftScale.y) {
					prevLeftScale.y = newLeftScale;
					rightScale = prevLeftScale;
				} else { // Else, Lerp to 0.1
					rightScale = prevLeftScale;
					rightScale.y = Mathf.Lerp (prevLeftScale.y, minHeight, Time.deltaTime * smoothVelocity);
				}

				// Set new scale
				soundBars [i].transform.localScale = rightScale;
			
			}
			

		}

		// Particles Interval Reset
		if (soundBarsParticles) {
			if (surpassed) {
				surpassed = false;
				remainingParticlesTime = particlesMaxInterval;
			} else {
				remainingParticlesTime -= Time.deltaTime;
			}
		}
			
		// Rhythm Interval Reset
		if (rhythmSurpassed) {
			rhythmSurpassed = false;
			remainingRhythmParticlesTime = rhythmParticlesMaxInterval;
		} else {
			remainingRhythmParticlesTime -= Time.deltaTime;
		}

		// Execute Camera Control
		if (cameraControl) {
			CameraMovement ();
		}
	}

	void EmitParticle (int index, float spectrumValue) {
		// If the Particles are activated, emit a particle too
		if (soundBarsParticles) {

			if (spectrumValue >= minParticleSensibility) {
				if (remainingParticlesTime <= 0f) {

					soundBars [index].GetComponentInChildren<ParticleSystem> ().Play ();

					surpassed = true;
				}
			}
		}
	}

	#endregion

}