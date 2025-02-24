//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;


namespace GameConfig.itemBackGround1
{
   
public partial class TbItemBgGround1
{
    private readonly Dictionary<int, itemBackGround1.ItemBgGround> _dataMap;
    private readonly List<itemBackGround1.ItemBgGround> _dataList;
    
    public TbItemBgGround1(ByteBuf _buf)
    {
        _dataMap = new Dictionary<int, itemBackGround1.ItemBgGround>();
        _dataList = new List<itemBackGround1.ItemBgGround>();
        
        for(int n = _buf.ReadSize() ; n > 0 ; --n)
        {
            itemBackGround1.ItemBgGround _v;
            _v = itemBackGround1.ItemBgGround.DeserializeItemBgGround(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<int, itemBackGround1.ItemBgGround> DataMap => _dataMap;
    public List<itemBackGround1.ItemBgGround> DataList => _dataList;

    public itemBackGround1.ItemBgGround GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public itemBackGround1.ItemBgGround Get(int key) => _dataMap[key];
    public itemBackGround1.ItemBgGround this[int key] => _dataMap[key];

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