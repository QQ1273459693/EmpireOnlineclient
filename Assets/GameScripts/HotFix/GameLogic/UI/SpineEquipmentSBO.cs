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
        [LabelText("骨骼数据")]
        public SkeletonDataAsset skeletonDataAsset;
        public Material sourceMaterial;
        SkeletonDataAsset IHasSkeletonDataAsset.SkeletonDataAsset { get { return this.skeletonDataAsset; } }
        [LabelText("人物换装部位")]
        public List<EquipHook> equippables=new List<EquipHook>();//骨骼需要换装的部位
    }

    [Serializable]
    public class EquipHook
    {
        [LabelText("装备部位")]
        public SpineEquipType type;
        [SpineSkin]
        [LabelText("目标皮肤")]
        public string templateSkin;
        [SpineSlot]
        [LabelText("装备插槽")]
        public List<string> slot;
        
        [SpineAttachment(skinField: "templateSkin")]
        [LabelText("装备附件")]
        public List<string> templateAttachment;
        //[Header("多部位同时换装列表")]
        //[SpineSlot]
        //public List<string> ListSlot;
        //[SpineAttachment(skinField: "templateSkin")]
        //public List<string> ListTemplateAttachment;
    }
}
