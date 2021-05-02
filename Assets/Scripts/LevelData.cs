using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public string _LevelName;
    public Gradient _BackgroundGradient;
    [ColorUsageAttribute(true,true)]
    public Color _ScenaryColor;
    public Color _ScenaryBorderColor;
    [ColorUsageAttribute(true,true)]
    public Color _BarsMainColor;
    [ColorUsageAttribute(true,true)]
    public Color _BarsSecondColor;
}
