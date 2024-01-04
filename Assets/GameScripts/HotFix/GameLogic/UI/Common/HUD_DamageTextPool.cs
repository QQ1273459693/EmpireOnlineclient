using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TEngine
{
    class HUD_DamageTextPool
    {
        public float g = 12f;
        public float speed = 1;
        private float verticalSpeed;
        private Vector3 moveDirection;
        public float angleSpeed = 2.5F;
        private float angle;

        bool Jumping;
        private float time;
        public List<Vector3> TargetList;
        public int JumpCount;
        public int mCurJumpIndex;
        public TextMesh textMesh;
        static Color RecoverColor = new Color(0.2980392F, 0.8117647F, 0, 1);
        static Color DamageColor = new Color(0.8117647F, 0.2392157F, 0, 1);
        Transform transform;
        public HUD_DamageTextPool(GameObject obj)
        {
            transform = obj.transform;
            textMesh = transform.Find("DamageText").GetComponent<TextMesh>();
        }
        public bool isCanShow()
        {
            Log.Debug($"{transform.name}的状态是:{transform.gameObject.activeSelf}");
            return transform.gameObject.activeSelf;
        }
        public void TextJumpStart(Vector2 StartPos,List<Vector3> Target, int Count, int DamageNum,bool isGetPool=false)
        {
            if (isGetPool)
            {
                transform.gameObject.SetActive(true);
            }
            transform.position = StartPos;
            TargetList = Target;
            JumpCount = Count;
            textMesh.text = DamageNum.ToString();
            if (DamageNum>0)
            {
                //恢复
                textMesh.color = RecoverColor;
            }
            else
            {
                //损伤
                textMesh.color = DamageColor;
            }
            textMesh.characterSize = 0.1F;
            mCurJumpIndex = 0;
            DOTween.To(() => textMesh.characterSize, x => textMesh.characterSize = x, 0.2F, 0.45F);
            JumpStartTest();
            _ = DemoAsync();
            Jumping = true;

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
                if (mCurJumpIndex == JumpCount)
                {
                    Jumping = false;
                    time = 0;
                    Color color = textMesh.color;
                    DOTween.To(() => textMesh.color.a, x => textMesh.color = color, 0.3F, 0.6F).OnComplete(() =>
                    {
                        transform.gameObject.SetActive(false);
                    });
                    return;
                }
                JumpStartTest();
                //finish
                time = 0;
                return;
            }
            time += Time.deltaTime;
            float test = verticalSpeed - g * time * angleSpeed;
            //if (test>0)
            //{
            //    //说明向上
            //    test= test
            //}
            transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
            transform.Translate(Vector3.up * test * Time.deltaTime, Space.World);
        }
        async UniTask DemoAsync()
        {
            var cts = new CancellationTokenSource();
            while (true)
            {
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
    }
}
