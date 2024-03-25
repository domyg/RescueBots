using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class KalmanFilter
{
    // Variabili per il filtro di Kalman
    private Matrix4x4 x; // Stato
    private Matrix4x4 A; // Matrice di transizione di stato
    private Matrix4x4 S; // Covarianza
    private Matrix4x4 C; // Matrice di osservazione
    private Matrix4x4 R; // Covarianza del rumore di processo
    private Matrix4x4 Q; // Covarianza del rumore di misura

    public KalmanFilter(Vector2 position)
    {
        x = Matrix4x4.zero;
        x.SetColumn(0, new Vector4(position.x, position.y, 0, 0));

        //Matrice di transizione di stato che tiene in considerazione posizione e velocità su assi x e z
        A = new Matrix4x4(
            new Vector4(1, 0, Time.deltaTime, 0),
            new Vector4(0, 1, 0, Time.deltaTime),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 0, 0, 1)
        );
        S = Matrix4x4.identity;
        C = Matrix4x4.identity;
        R = Matrix4x4.identity;
        Q = Matrix4x4.identity;
    }

    //Metodo per aggiornare la posizione del Robot
    public Vector2 UpdatePosition(Vector2 position)
    {
        Predict();
        Correct(position);
        return new Vector2(x.m00, x.m10);
    }

    private void Predict()
    {
        // Predizione dello stato
        x = A * x;

        // Predizione della covarianza
        S = SumMatrix4x4(A * S * A.transpose, R);
    }

    private void Correct(Vector2 position)
    {
        // Ottenere le misure della posizione (esempio: dalla triangolazione dei landmark)
        Vector2 measuredPosition = Trilateration.CalculatePosition(position);
        Matrix4x4 measuredPositionMatrix = Matrix4x4.zero;
        measuredPositionMatrix.SetColumn(0, new Vector4(measuredPosition.x, measuredPosition.y, 0, 0));

        // Calcolo della matrice di guadagno di Kalman
        Matrix4x4 K = S * C.transpose * Matrix4x4.Inverse(SumMatrix4x4(C * S * C.transpose, Q));

        // Aggiornamento dello stato e della covarianza
        x = SumMatrix4x4(x, K * SubMatrix4x4(measuredPositionMatrix, C * x));
        S = SubMatrix4x4(Matrix4x4.identity, K * C) * S;
    }

    //Metodo per le somme tra matrici 4x4 (non presente in C#)
    private Matrix4x4 SumMatrix4x4(Matrix4x4 m1, Matrix4x4 m2)
    {
        Matrix4x4 sum = Matrix4x4.zero;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                sum[i, j] = m1[i, j] + m2[i, j];
            }
        }
        return sum;
    }

    //Metodo per le sottrazioni tra matrici 4x4 (non presente in C#)
    private Matrix4x4 SubMatrix4x4(Matrix4x4 m1, Matrix4x4 m2)
    {
        Matrix4x4 sub = Matrix4x4.zero;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                sub[i, j] = m1[i, j] - m2[i, j];
            }
        }
        return sub;
    }
}
