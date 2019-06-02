﻿using UnityEngine;

public enum Direction
{
    North, East, South, West
}

public enum DirectionChange //Drehrichtungs Enums
{
    None, TurnRight, TurnLeft, TurnAround
}

public static class DirectionExtensions
{

    static Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0f, 270f, 0f)
    };

    public static Quaternion GetRotation(this Direction direction)
    {
        return rotations[(int)direction];
    }

    public static DirectionChange GetDirectionChangeTo(this Direction current, Direction next)  //wenn zwischen current und next ein unterschied ist, wird die Drehrichtung erkannt, bei keinem Unterschied gibt es keine Drehung
    {
        if (current == next)
        {
            return DirectionChange.None;
        }
        else if (current + 1 == next || current - 3 == next)
        {
            return DirectionChange.TurnRight;
        }
        else if (current - 1 == next || current + 3 == next)
        {
            return DirectionChange.TurnLeft;
        }
        return DirectionChange.TurnAround;
    }

    public static float GetAngle(this Direction direction)      //Wir drehen uns nur um eine Achse, deshalb ist ein einfacher Winkel ausreichend
    {
        return (float)direction * 90f;
    }

    static Vector3[] halfVectors = {
        Vector3.forward * 0.5f,
        Vector3.right * 0.5f,
        Vector3.back * 0.5f,
        Vector3.left * 0.5f
    };

    public static Vector3 GetHalfVector(this Direction direction)
    {
        return halfVectors[(int)direction];
    }
}