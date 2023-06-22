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
    void Start()
    {
		Cursor.visible = true;
		csvFilePath = Path.Combine(Application.dataPath, "accuracy.csv");

		// Initialisez le chronomètre
		stopwatch = new Stopwatch();
		stopwatch.Start();

		lycaon = GameObject.Find("Lycaon");
		lighting_end = GameObject.Find("LightningEnd");

    }

    // Update is called once per frame
    void Update()
    {
		if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)) && GameObject.Find("Nectar(Clone)") == null)
		{
			//Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 mousePosition = lighting_end.transform.position;
			
			if (lycaon != null)
			{
				Vector3 lycaonPosition = lycaon.transform.position;

				// Obtenir le temps écoulé depuis le début du jeu
				float elapsedTime = stopwatch.ElapsedMilliseconds / 1000f;

				// Écrire les positions et le temps dans le fichier CSV
				string[] data = { mousePosition.x.ToString(), mousePosition.y.ToString(), mousePosition.z.ToString(),
								  lycaonPosition.x.ToString(), lycaonPosition.y.ToString(), lycaonPosition.z.ToString(),
								  elapsedTime.ToString() };

				string csvLine = string.Join(",", data);

				bool fileExists = File.Exists(csvFilePath);

				using (StreamWriter sw = new StreamWriter(csvFilePath, true))
				{
					if (!fileExists)
					{
						string header = "MouseX,MouseY,MouseZ,LycaonX,LycaonY,LycaonZ,ElapsedTime";
						sw.WriteLine(header);
					}

					sw.WriteLine(csvLine);
				}
			}
		}
    }
}
