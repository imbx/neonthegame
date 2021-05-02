using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorsExtension
{
    public static Color32 RedA(byte alpha = 255) {
        return new Color32(235, 64, 52, alpha);
    }
    public static Color32 CyanA(byte alpha = 255) {
        return new Color32(52, 76, 235, alpha);
    }
    public static Color32 GreenA(byte alpha = 255) {
        return new Color32(82, 235, 52, alpha);
    }
    public static Color32 MagentaA(byte alpha = 255) {
        return new Color32(235, 219, 52, alpha);
    }
    public static Color32 Red{
        get{ return new Color32(235, 64, 52, 255); }
    }
    public static Color32 Blue{
        get{ return new Color32(0, 255, 255, 255); }
    }
    public static Color32 Green {
        get{ return new Color32(82, 235, 52, 255); }
    }
    public static Color32 Magenta{
        get{ return new Color32(255, 0, 255, 255); }
    }
    public static Color32 Yellow{
        get{ return new Color32(255, 255, 25, 255); }
    }
}

public enum LevelState
{
    DEFAULT,
    FIRSTLOAD,
    SEEDING,
    LOADINGDATA,
    UPDATINGBACKGROUND,
    LOADINGLEVEL,
    LOADINGPLAYERS,
    PLAYING,
    REMOVINGLEVEL,
    RESETINGLEVEL,
    ENDGAME,
    WAITING
};


public enum Facing{
    Right,
    Left,
    Up,
    Down,
    None
}

public enum WeaponType{
    Gun,
    ShotGun,
    Sniper,
    Grenade,
    Melee,
    Ak,
    Wasd,
    Axis,
    None
}

public static class IListExtensions {
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
 

 
public static class Rigidbody2DExtension
{
    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        body.AddForce(dir.normalized * (wearoff <= 0f ? 0f : explosionForce) * wearoff);
    }
 
    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector3 baseForce = dir.normalized * (wearoff <= 0f ? 0f : explosionForce) * wearoff;
        body.AddForce(baseForce);
 
        float upliftWearoff = 1 - upliftModifier / explosionRadius;
        Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
        body.AddForce(upliftForce);
    }
}