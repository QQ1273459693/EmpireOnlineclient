using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    /// <summary>
    /// 设置特效
    /// </summary>
    /// <param name="logicPosition"></param>
    public void SetFeectPos(VInt3 logicPosition)
    {
        transform.position = logicPosition.vec3;
        Destroy(gameObject,2);
    }
}
