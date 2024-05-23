using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    float MoveSpeed { get; }
    float SpeedRate { get { return 1 - (plusRate + minusRate); } }

    float plusRate { get; set; }
    float minusRate { get; set; }

    public void PlusMoveSpeed(float _spdRate, float _duration = 0);

    public void MinusMoveSpeed(float _spdRate, float _duration = 0);

    public void ResetMoveSpeed(SpeedRateEnum _spdEnum, float _spdRate);
}
public enum SpeedRateEnum
{
    plusRate,
    minusRate,
}
