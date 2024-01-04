using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float g = 12f;

    public GameObject target;
    public GameObject target_1;
    public float speed = 10;
    private float verticalSpeed;
    private Vector3 moveDirection;

    public float angleSpeed;
    private float angle;

    public bool Jump;
    bool Jumping;
    bool DobleJump;
    Vector3 OridPos;
    private float time;
    public List<Vector3> TargetList;
    public List<Transform> TargetTsLs;
    public int JumpCount;
    public int mCurJumpIndex;
    public TextMesh textMesh;
    void Start()
    {
        Debug.Log($"{0%2}--{1%2}--{2%2}--{3%2}--{4%2}");
        for (int i = 0; i < TargetTsLs.Count; i++)
        {
            TargetList.Add(TargetTsLs[i].position);
        }
        OridPos = transform.position;
        for (int i = 0; i < TargetList.Count; i++)
        {
            TargetList[i]= transform.position;
        }

    }
    void JumpStartTest()
    {
        float tmepDistance = Vector3.Distance(transform.position, TargetList[mCurJumpIndex]);
        float tempTime = tmepDistance / speed;
        float riseTime, downTime;
        riseTime = downTime = tempTime / 2;
        verticalSpeed = g * riseTime * angleSpeed;
        //transform.LookAt(target.transform.position);

        float tempTan = verticalSpeed / speed;
        double hu = Math.Atan(tempTan);
        angle = (float)(180 / Math.PI * hu);
        //transform.eulerAngles = new Vector3(-angle, transform.eulerAngles.y, transform.eulerAngles.z);
        //angleSpeed = angle / riseTime;

        moveDirection = TargetList[mCurJumpIndex] - transform.position;
    }
    void JumpIngTest()
    {
        if (transform.position.y < TargetList[mCurJumpIndex].y)
        {
            transform.position = TargetList[mCurJumpIndex];
            mCurJumpIndex++;
            if (mCurJumpIndex== JumpCount)
            {
                Jumping = false;
                time = 0;
                return;
            }
            JumpStartTest();
            //finish
            time = 0;
            return;
        }
        time += Time.deltaTime;
        float test = verticalSpeed - g * time* angleSpeed;
        //if (test>0)
        //{
        //    //说明向上
        //    test= test
        //}
        Debug.Log($"速度:{test}");
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
        transform.Translate(Vector3.up * test * Time.deltaTime, Space.World);
    }
    async UniTask DemoAsync()
    {
        var cts = new CancellationTokenSource();
        while (true)
        {
            Debug.Log($"haizai");
            await UniTask.Yield(cts.Token);
            if (!Jumping)
            {
                cts.Cancel();
            }
            else
            {
                JumpIngTest();
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.position = OridPos;
            textMesh.characterSize = 0.1F;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            mCurJumpIndex = 0;
            Jump = true;
            DobleJump = false;
            //DOTween.To(() => textMesh.characterSize, x => textMesh.characterSize = x, 0.3F, 1.2F);
            JumpStartTest();
            _ = DemoAsync();
            //StartCoroutine(Test());
            Jumping = true;
        }
        //if (Jumping)
        //{
        //    JumpIngTest();
        //}
    }
}