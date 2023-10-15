using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class collector : MonoBehaviour
{
	public Stopwatch stopwatch;
	public GameObject lycaon;
	public string csvFilePath;
    public GameObject lighting_end;
    // Start is called before the first frame update
    void Start() {
			gameTime = 0;
			Cursor.visible = true;
			csvFilePath = Path.Combine(Application.dataPath, "accuracy.csv");
    }

	public void Process(Vector3 raycastPoint, Vector3 lycaonPosition) {
		string[] data = { raycastPoint.x.ToString(), raycastPoint.y.ToString(), raycastPoint.z.ToString(),
		lycaonPosition.x.ToString(), lycaonPosition.y.ToString(), lycaonPosition.z.ToString(),
		gameTime.ToString() };

		string csvLine = string.Join(",", data);
		bool fileExists = File.Exists(csvFilePath);

		using (StreamWriter sw = new StreamWriter(csvFilePath, true)) {
			if (!fileExists) {
				string header = "StrikeX,StrikeY,StrikeZ,LycaonX,LycaonY,LycaonZ,ElapsedTime";
				sw.WriteLine(header);
			}
			sw.WriteLine(csvLine);
		}
	}

	float gameTime;
    // Update is called once per frame
    void Update() {
		gameTime += Time.deltaTime;
    }
}
