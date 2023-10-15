using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class fitt : MonoBehaviour
{
	private float width;
	private bool spawned;
	private Vector3 position;
	private GameObject nectar;
	private string csvFilePath;
	private Stopwatch game_time;
	private Vector3 mousePosition;
    private GameObject lighting_end;
	private float elapsedTime;
	private float time_marker;

    void Start()
    {
		csvFilePath = Path.Combine(Application.dataPath, "fitt.csv");
		spawned = false;
		lighting_end = GameObject.Find("LightningEnd");
		
			// Initialisez le chronomètre
			game_time = new Stopwatch();
			game_time.Start();
    }

	public void Process(float deltaT) {
		nectar = GameObject.Find("Nectar(Clone)");
		// Get the spawn position
		position = Camera.main.WorldToScreenPoint(nectar.transform.position);
		width = get_size(nectar);
		mousePosition = Camera.main.WorldToScreenPoint(lighting_end.transform.position);

		// Obtenir le temps écoulé depuis le début du jeu
		elapsedTime = deltaT;
		time_marker = game_time.ElapsedMilliseconds / 1000f;
		save_data();
	}

	private float get_size(GameObject obj){
		// prendre le renderer de l'objet
		MeshRenderer targetRenderer = obj.GetComponent<MeshRenderer>();

		// la taille de l'objet dans le jeu
		Vector3 boundSize = targetRenderer.bounds.size;

		// la taille de l'objet dans la caméra principale
		Vector3 bottomLeft = Camera.main.WorldToScreenPoint(targetRenderer.bounds.min);
		Vector3 topRight = Camera.main.WorldToScreenPoint(targetRenderer.bounds.max);
		Vector2 sizeOnScreen = new Vector2(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
		UnityEngine.Debug.Log("La taille de l'objet à l'écran : " + sizeOnScreen);
		return Mathf.Max(sizeOnScreen.x, sizeOnScreen.y);
	}

	private void save_data(){
		// Écrire les positions et le temps dans le fichier CSV
		string[] data = { mousePosition.x.ToString(), mousePosition.y.ToString(), mousePosition.z.ToString(),
		position.x.ToString(), position.y.ToString(), position.z.ToString(),
		elapsedTime.ToString(), width.ToString(), time_marker.ToString() };

		string csvLine = string.Join(",", data);

		bool fileExists = File.Exists(csvFilePath);

		using (StreamWriter sw = new StreamWriter(csvFilePath, true)) {
			if (!fileExists) {
				string header = "MouseX,MouseY,MouseZ,nectarX,nectarY,nectarZ,TaskTime,width,GameTime";
				sw.WriteLine(header);
			}

			sw.WriteLine(csvLine);
		}
	}
}
