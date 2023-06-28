using System;
using Cysharp.Threading.Tasks;
using TEngine;

namespace GameMain
{
    public class ProcedureMainSence : ProcedureBase
    {
        public override bool UseNativeDialog { get; }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Debug("正式进入主场景");
            GameModule.Resource.LoadSceneAsync("MainCity");
            GameEvent.Send(GameLogic.GameProcedureEvent.LoadMainCityUIEvent.EventId);
        }
    }
}