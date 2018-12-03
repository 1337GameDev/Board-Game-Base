using System;
using UnityEngine;

static class DIRECTION_Exensions {
    public static Vector3 ToVector3(this DIRECTION dir) {
        Vector3 directionVector = Vector3.zero;

        switch (dir) {
            case DIRECTION.LEFT:
                directionVector = Vector3.left;
                break;
            case DIRECTION.RIGHT:
                directionVector = Vector3.right;
                break;
            case DIRECTION.FORWARD:
                directionVector = Vector3.forward;
                break;
            case DIRECTION.FORWARD_LEFT:
                directionVector = Vector3.forward + Vector3.left;
                break;
            case DIRECTION.FORWARD_RIGHT:
                directionVector = Vector3.back + Vector3.right;
                break;
            case DIRECTION.BACKWARD:
                directionVector = Vector3.back;
                break;
            case DIRECTION.BACKWARD_LEFT:
                directionVector = Vector3.back + Vector3.left;
                break;
            case DIRECTION.BACKWARD_RIGHT:
                directionVector = Vector3.back + Vector3.right;
                break;

        }

        return directionVector;
    }

    public static Vector3 ToVector2(this DIRECTION dir) {
        Vector3 directionVector = dir.ToVector3();
        return new Vector2(directionVector.x, directionVector.z);
    }
}

public enum DIRECTION { LEFT, RIGHT, FORWARD, BACKWARD, FORWARD_LEFT, FORWARD_RIGHT, BACKWARD_LEFT, BACKWARD_RIGHT };
