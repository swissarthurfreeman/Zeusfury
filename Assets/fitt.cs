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
	private Stopwatch stopwatch;
	private Vector3 mousePosition;
    private GameObject lighting_end;
	private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
		csvFilePath = Path.Combine(Application.dataPath, "fitt.csv");
		spawned = false;
		lighting_end = GameObject.Find("LightningEnd");
    }

    // Update is called once per frame
    void Update()
    {
		nectar = GameObject.Find("Nectar(Clone)");
		if (spawned == false && nectar != null)
		{
			// Marquez le nectar comme apparant
			spawned = true;

			// Initialisez le chronomètre
			stopwatch = new Stopwatch();
			stopwatch.Start();

			// Get the spawn position
			position = Camera.main.WorldToScreenPoint(nectar.transform.position);
			width = get_size(nectar);
			mousePosition = Camera.main.WorldToScreenPoint(lighting_end.transform.position);
		}
		else if (spawned == true && nectar == null){
				// Marquez le nectar comme disparu
				spawned = false;

				// Obtenir le temps écoulé depuis le début du jeu
				elapsedTime = stopwatch.ElapsedMilliseconds / 1000f;
				stopwatch.Stop();
				save_data();
		}
    }

	private float get_size(GameObject obj){
		// prendre le renderer de l'objet
		Renderer targetRenderer = obj.GetComponent<Renderer>();

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
						  elapsedTime.ToString(), width.ToString() };

		string csvLine = string.Join(",", data);

		bool fileExists = File.Exists(csvFilePath);

		using (StreamWriter sw = new StreamWriter(csvFilePath, true))
		{
			if (!fileExists)
			{
				string header = "MouseX,MouseY,MouseZ,nectarX,nectarY,nectarZ,ElapsedTime,width";
				sw.WriteLine(header);
			}

			sw.WriteLine(csvLine);
		}
	}
}
