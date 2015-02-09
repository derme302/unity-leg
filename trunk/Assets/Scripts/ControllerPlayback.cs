using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ControllerPlayback : MonoBehaviour {

    // Bone Positions
    public Transform bonePrefab;                        // Prefab to use for the bone model
    Transform[] bones;                                  // Array to hold all handles of the bone transforms
    int boneCount;                                      // Ammount of bones in the animation (automatically calculted)

    // Joint Positions
    public Transform[] point;                           // Array to hold all handles of the joint transforms
    int pointLength;                                    // Ammount of joints in the animation (automatically calculated from inspector)

    // UI
    public Text frameCounter;                           // UI element to display <currrentFrame>/<totalFrames>
    public Slider frameSlider;                          // UI element to change the current frame

    public string shoulderDataFilename;                 // Name of the CSV where the shoulder data positions are
    public string elbowDataFilename;                    // Name of the CSV where the elbow data positions are
    
    public int fps;                                     // The frames per second the animation should run at
    public float interpolation;                         // The interpolation value for the animation

    int animationLength;                                // The ammount of frames in the animation

    Vector3 shoulderPosition = Vector3.zero;            // The position of the shoulder joint
    Vector3 elbowPosition = Vector3.zero;               // The position of the elbow joint

    List<Dictionary<string, object>> shoulderData;      // Data structure to hold all the shoulder positions
    List<Dictionary<string, object>> elbowData;         // Data structure to hold all the elbow positions

    float frameLength;                                  // Holds the ammount of seconds a frame goes for (automatically calculated)
    int frame;                                          // Holds the current frame value

	// Use this for initialization
    void Start() {
        // Calculte the length of a frame (in seconds) based off the fps
        frameLength = 1.0f / fps;

        frameSlider.onValueChanged.AddListener(delegate { UpdateFrame((int)frameSlider.value); });

        // Retrieve the length of the joint array from what has been set in the inspector
        pointLength = point.Length;

        // Run playback setup
        Setup();
    }
	
	// Update is called once per frame
	void Update() {
        // Move the joints from their current positions to the desired position (interpolated)
        point[0].position = Vector3.Lerp(point[0].position, Vector3.zero, interpolation * Time.deltaTime);
        point[1].position = Vector3.Lerp(point[1].position, elbowPosition, interpolation * Time.deltaTime); 
        point[2].position = Vector3.Lerp(point[2].position, shoulderPosition, interpolation * Time.deltaTime);


        /* Update positions of Bones */
        Transform cylinderRef;
        Transform spawn;
        Transform target;
                
        for (int i = 0; i < boneCount; i++) {

            if (bones[i] == null)
                Debug.Log("No bone at:" + i.ToString());

            cylinderRef = bones[i];

            int ni = i + 1; // Next i value

            spawn = point[i];
            target = point[ni];

            // Find the distance between 2 points
            Vector3 newScale = cylinderRef.localScale;
            newScale.z = Vector3.Distance(spawn.position, target.position) / 2;

            cylinderRef.localScale = newScale;
            cylinderRef.position = spawn.position;        // place bond here
            cylinderRef.LookAt(target);                   // aim bond at positiion
        }
        /* Bone positions have been updated */
	}

    public void Setup() {
        // Read data from the CSVs into the designated data structures
        shoulderData = CsvReader.Read(shoulderDataFilename);
        elbowData = CsvReader.Read(elbowDataFilename);

        /* Verify data
        Debug.Log("Shoulder Data");
        for(var i=0; i < shoulderData.Count; i++) {
            print("x " + shoulderData[i]["x"] + " " +
                   "y " + shoulderData[i]["y"] + " " +
                   "z " + shoulderData[i]["z"]); 
		}

        Debug.Log("Elbow Data");
        for (var i = 0; i < elbowData.Count; i++) {
            print("x " + elbowData[i]["x"] + " " +
                   "y " + elbowData[i]["y"] + " " +
                   "z " + elbowData[i]["z"]);
        } 
         * */

        // Calcuate and log animation length
        animationLength = shoulderData.Count;
        Debug.Log("Animation: " + animationLength.ToString());
    }

    public void Play() {
        // Reset frame to zero
        frame = 0;
        
        // Create bones
        boneCount = pointLength - 1;
        bones = new Transform[boneCount];

        for (int i = 0; i < boneCount; i++) {
            bones[i] = Instantiate(bonePrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as Transform;
        }

        // Set playback to play a the set fps
        InvokeRepeating("Playback", frameLength, frameLength);

        // Display current frame in UI
        frameCounter.text = frame.ToString() + "/" + animationLength.ToString();
        Debug.Log("Animation Started");
    }

    void Playback() {
        Debug.Log("Playing Frame: " + frame.ToString());

        // Check to make sure the animation isn't finished
        if (frame >= animationLength)
            frame = 0;

        // Read data from data structures
        shoulderPosition.x = Convert.ToSingle(shoulderData[frame]["x"]);
        shoulderPosition.y = Convert.ToSingle(shoulderData[frame]["y"]);
        shoulderPosition.z = Convert.ToSingle(shoulderData[frame]["z"]);

        elbowPosition.x = Convert.ToSingle(elbowData[frame]["x"]);
        elbowPosition.y = Convert.ToSingle(elbowData[frame]["y"]);
        elbowPosition.z = Convert.ToSingle(elbowData[frame]["z"]);

        // Display current frame in UI
        frameCounter.text = frame.ToString() + "/" + animationLength.ToString();
        frameSlider.value = frame;

        // Increment frame
        frame ++;
    }

    public void Stop() {
        // Stop the Playback() function from triggering
        CancelInvoke();
        Debug.Log("Animation Stopped");
    }

    public void UpdateFrame(int value) {
        // Set the new value of the frame
        frame = value;
        
        // Run Playback
        Playback();
    }
}
