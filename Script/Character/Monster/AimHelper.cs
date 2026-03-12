using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AimHelper
{
    private static List<Transform> mEnemyTransList;

    static AimHelper()
    {
        mEnemyTransList = new List<Transform>(32);
    }

    public static void AddEnemeyTrans(Transform trans)
    {
        Debug.Assert(!checkContain(trans), $"you just passed aready contained enemey to aim helper[{trans.gameObject.name}]");
        mEnemyTransList.Add(trans);
    }

    public static void RemoveEnemeyTrans(Transform trans)
    {
        Debug.Assert(checkContain(trans), $"you try to remove not in list enemey in aim helper[{trans.gameObject.name}]");
        mEnemyTransList.Remove(trans);
    }
    public static void TryRemoveEnemeyTrans(Transform trans)
    {
        if(!checkContain(trans))
        {
            return;
        }
        mEnemyTransList.Remove(trans);
    }

    public static bool TryToGetAimAssist(ref Vector3 attackAim, out Vector3 outTargetEnemey, Vector3 playerPos ,AimAssistanceConfig config)
    {
        const float TRHESHOLD = 10.0f;
        outTargetEnemey = Vector3.zero;
        float minAngleCos = float.MinValue;
        float minMagnitude = float.MinValue;
        bool isFounded = false;

        for (int i = 0; i < mEnemyTransList.Count; i++)
        {
            Vector3 enemyPos = mEnemyTransList[i].position;
            Vector3 playerToEnemy = enemyPos - playerPos;
            playerToEnemy.Set(playerToEnemy.x, 0.0f, playerToEnemy.z);
            float playerToEnemyDistance = playerToEnemy.magnitude;
            if (playerToEnemyDistance > config.MaxAssistanceDistance)
            {
                continue;
            }

            Vector3 playerToEnemyNorm = playerToEnemy / playerToEnemyDistance;

            float normalizedDistance = playerToEnemyDistance / config.MaxAssistanceDistance;
            float assistanceAngle = config.EvaluateAngle(normalizedDistance);
            float assitanceCos = Mathf.Cos(assistanceAngle * Mathf.Deg2Rad);
            float cos = Vector3.Dot(playerToEnemyNorm, attackAim);
            cos = samplingCos(cos, TRHESHOLD);

            if (cos < assitanceCos)
            {
                continue;
            }

            if(isFounded == false)
            {
                outTargetEnemey = enemyPos;
                minAngleCos = cos;
                minMagnitude = playerToEnemyDistance;
                attackAim = playerToEnemyNorm;
                isFounded = true;
                continue;
            }

            if(cos > minAngleCos)
            {
                outTargetEnemey = enemyPos;
                minAngleCos = cos;
                minMagnitude = playerToEnemyDistance;
                attackAim = playerToEnemyNorm;
            }
            else if(cos == minAngleCos)
            {
                if (playerToEnemyDistance < minMagnitude)
                {
                    outTargetEnemey = enemyPos;
                    minAngleCos = cos;
                    minMagnitude = playerToEnemyDistance;
                    attackAim = playerToEnemyNorm;
                }
            }
        }

        return isFounded;
    }

    private static bool checkContain(Transform trans)
    {
        for (int i = 0; i < mEnemyTransList.Count; i++)
        {
            if(mEnemyTransList[i].GetInstanceID() == trans.GetInstanceID())
            {
                return true;
            }
        }
        return false;
    }

    private static float samplingCos(float cos, float threshold)
    {
        int step = (int)Mathf.Round((180 / threshold));

        cos += 1; //Re Range To Cos from -1 ~ 1 to 0 ~ 2;
        cos *= step; //Re Range to cos from 0 ~ 2 to 0 ~ 2 * step;
        float ceiling = Mathf.Ceil(cos);
        cos = ceiling / step;
        cos -= 1;
        return cos;
    }
}
