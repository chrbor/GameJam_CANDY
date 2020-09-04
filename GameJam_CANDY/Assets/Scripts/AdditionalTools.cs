using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Eine Klasse mit verschiedenen Funktionen
/// </summary>
public static class AdditionalTools
{
    /// <summary>
    /// Wandelt die angegebene Gradzahl in einen Einheitsvektor, der in die Gradrichtung zeigt, um
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static Vector2 RotToVec(float rotation)
    {
        return new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad), Mathf.Sin(rotation * Mathf.Deg2Rad));
    }
}
