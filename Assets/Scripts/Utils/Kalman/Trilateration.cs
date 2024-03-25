using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class Trilateration
{
    private static int size = TerrainManager.size;

    //Landmark agli angoli della mappa
    private static List<Landmark> landmarks = new List<Landmark>
    {
        new Landmark(new Vector2(0, 0)),
        new Landmark(new Vector2(0, size)),
        new Landmark(new Vector2(size, 0)),
        new Landmark(new Vector2(size, size))
    };

    //Metodo per calcolare la posizione del Robot basandosi sulla trilaterazione
    public static Vector2 CalculatePosition(Vector2 position)
    {
        List<float> distances = new List<float>();

        //Calcolo delle distanze dai landmark
        for (int i = 0; i < landmarks.Count; i++)
        {
            distances.Add(landmarks[i].Distance(position));
        }

        Vector2 estimatedPosition = CalculatePosition(landmarks, distances);

        return estimatedPosition;
    }

    //Metodo per calcolare la posizione del Robot basandosi sulla trilaterazione utilizzando 4 trilaterazioni con 3 landmark l'uno e calcolandone la media
    private static Vector2 CalculatePosition(List<Landmark> landmarks, List<float> distances)
    {
        Vector2 estimatedPosition = Vector2.zero;

        //Calcola la posizione stimata usando diverse triple di landmark e relative distanze
        for (int i = 0; i < landmarks.Count; i++)
        {
            estimatedPosition += CalculatePositionWith3Landmarks(new List<Landmark> { landmarks[i % landmarks.Count], landmarks[(i + 1) % landmarks.Count], landmarks[(i + 2) % landmarks.Count] }, new List<float> { distances[i % landmarks.Count], distances[(i + 1) % landmarks.Count], distances[(i + 2) % landmarks.Count] });
        }
        //Calcola la media delle posizioni stimate
        estimatedPosition /= landmarks.Count;

        return estimatedPosition;
    }

    //Metodo per calcolare la posizione del Robot basandosi sulla trilaterazione utilizzando 3 landmark
    private static Vector2 CalculatePositionWith3Landmarks(List<Landmark> landmarks, List<float> distances)
    {
        //Coefficienti per risolvere il sistema di equazioni lineari
        float A = 2 * (landmarks[1].position.x - landmarks[0].position.x);
        float B = 2 * (landmarks[1].position.y - landmarks[0].position.y);
        float C = 2 * (landmarks[2].position.x - landmarks[0].position.x);
        float D = 2 * (landmarks[2].position.y - landmarks[0].position.y);

        //Termini noti del sistema di equazioni
        float E = Mathf.Pow(distances[0], 2) - Mathf.Pow(distances[1], 2) +
                  Mathf.Pow(landmarks[1].position.x, 2) - Mathf.Pow(landmarks[0].position.x, 2) +
                  Mathf.Pow(landmarks[1].position.y, 2) - Mathf.Pow(landmarks[0].position.y, 2);

        float F = Mathf.Pow(distances[0], 2) - Mathf.Pow(distances[2], 2) +
                  Mathf.Pow(landmarks[2].position.x, 2) - Mathf.Pow(landmarks[0].position.x, 2) +
                  Mathf.Pow(landmarks[2].position.y, 2) - Mathf.Pow(landmarks[0].position.y, 2);

        //Calcola il denominatore per risolvere il sistema di equazioni
        float denominator = A * D - B * C;

        //Calcola le coordinate X e Y della posizione stimata
        float posX = (E * D - B * F) / denominator;
        float posY = (A * F - E * C) / denominator;

        //Crea il vettore della posizione stimata
        Vector2 estimatedPosition = new Vector2(posX, posY);

        return estimatedPosition;
    }
}
