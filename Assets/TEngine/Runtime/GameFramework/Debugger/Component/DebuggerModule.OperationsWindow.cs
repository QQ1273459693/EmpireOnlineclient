﻿using UnityEngine;

namespace TEngine
{
    public sealed partial class DebuggerModule : GameFrameworkModuleBase
    {
        private sealed class OperationsWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Operations</b>");
                GUILayout.BeginVertical("box");
                {
                    ObjectPoolModule objectPoolModule = GameEntry.GetModule<ObjectPoolModule>();
                    if (objectPoolModule != null)
                    {
                        if (GUILayout.Button("Object Pool Release", GUILayout.Height(30f)))
                        {
                            //objectPoolModule.Release();
                        }

                        if (GUILayout.Button("Object Pool Release All Unused", GUILayout.Height(30f)))
                        {
                            //objectPoolModule.ReleaseAllUnused();
                        }
                    }

                    ResourceModule resourceCompoent = GameEntry.GetModule<ResourceModule>();
                    if (resourceCompoent != null)
                    {
                        if (GUILayout.Button("Unload Unused Assets", GUILayout.Height(30f)))
                        {
                            resourceCompoent.ForceUnloadUnusedAssets(false);
                        }

                        if (GUILayout.Button("Unload Unused Assets and Garbage Collect", GUILayout.Height(30f)))
                        {
                            resourceCompoent.ForceUnloadUnusedAssets(true);
                        }
                    }

                    if (GUILayout.Button("Shutdown Game Framework (None)", GUILayout.Height(30f)))
                    {
                        GameEntry.Shutdown(ShutdownType.None);
                    }
                    if (GUILayout.Button("Shutdown Game Framework (Restart)", GUILayout.Height(30f)))
                    {
                        GameEntry.Shutdown(ShutdownType.Restart);
                    }
                    if (GUILayout.Button("Shutdown Game Framework (Quit)", GUILayout.Height(30f)))
                    {
                        GameEntry.Shutdown(ShutdownType.Quit);
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
