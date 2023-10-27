//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;


namespace GameConfig.swordSkillBase
{
   
public partial class TbSwordSkillBase
{
    private readonly Dictionary<int, swordSkillBase.SwordSkill> _dataMap;
    private readonly List<swordSkillBase.SwordSkill> _dataList;
    
    public TbSwordSkillBase(ByteBuf _buf)
    {
        _dataMap = new Dictionary<int, swordSkillBase.SwordSkill>();
        _dataList = new List<swordSkillBase.SwordSkill>();
        
        for(int n = _buf.ReadSize() ; n > 0 ; --n)
        {
            swordSkillBase.SwordSkill _v;
            _v = swordSkillBase.SwordSkill.DeserializeSwordSkill(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<int, swordSkillBase.SwordSkill> DataMap => _dataMap;
    public List<swordSkillBase.SwordSkill> DataList => _dataList;

    public swordSkillBase.SwordSkill GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public swordSkillBase.SwordSkill Get(int key) => _dataMap[key];
    public swordSkillBase.SwordSkill this[int key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}