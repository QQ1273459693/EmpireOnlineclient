using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameLogic.EnumeRation;

namespace GameLogic
{
    [CreateAssetMenu(fileName = "SpineAnimaConfig", menuName = "SpineAnimaConfig", order = 0)]
    public class SpineEquipmentSBO : ScriptableObject, IHasSkeletonDataAsset
    {
        [LabelText("��������")]
        public SkeletonDataAsset skeletonDataAsset;
        public Material sourceMaterial;
        SkeletonDataAsset IHasSkeletonDataAsset.SkeletonDataAsset { get { return this.skeletonDataAsset; } }
        [LabelText("���ﻻװ��λ")]
        public List<EquipHook> equippables=new List<EquipHook>();//������Ҫ��װ�Ĳ�λ
    }

    [Serializable]
    public class EquipHook
    {
        [LabelText("װ����λ")]
        public SpineEquipType type;
        [SpineSkin]
        [LabelText("Ŀ��Ƥ��")]
        public string templateSkin;
        [SpineSlot]
        [LabelText("װ�����")]
        public List<string> slot;
        
        [SpineAttachment(skinField: "templateSkin")]
        [LabelText("װ������")]
        public List<string> templateAttachment;
        //[Header("�ಿλͬʱ��װ�б�")]
        //[SpineSlot]
        //public List<string> ListSlot;
        //[SpineAttachment(skinField: "templateSkin")]
        //public List<string> ListTemplateAttachment;
    }
}
