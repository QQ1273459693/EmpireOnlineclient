using System;
using Cysharp.Threading.Tasks;
using TEngine;

namespace GameMain
{
    public class ProcedureFightSence : ProcedureBase
    {
        public override bool UseNativeDialog { get; }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Debug("正式进入战斗场景");
            GameModule.Resource.LoadSceneAsync("Fight");
        }
    }
}