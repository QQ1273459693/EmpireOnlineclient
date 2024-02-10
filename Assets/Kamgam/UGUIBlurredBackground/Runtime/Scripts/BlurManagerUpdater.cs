using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using Kamgam.UGUIBlurredBackground.EditorSingleton;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.UGUIBlurredBackground
{
    /// <summary>
    /// This class ensure update is called in play and in edit mode.
    /// </summary>
    public class BlurManagerUpdater : EditorMonoBehaviourSingleton<BlurManagerUpdater>
    {
        protected override string getMonoBehaviourName()
        {
            return "UGUI Blur Manager Updater";
        }
    }
}

