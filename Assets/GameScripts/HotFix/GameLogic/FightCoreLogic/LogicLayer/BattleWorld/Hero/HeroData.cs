
using System.Collections.Generic;

public class HeroData
{
    public HeroData()
    {
        skillidArr = new List<int>();
    }
    public int id;
    public string Name;
    public int type;
    public List<int> skillidArr;//技能数组id
    public int hp;//血量
    public int atk;//攻击力
    public int def;//防御力
    public int agle;//敏捷值
    public int atkRange;//攻击回复怒气值
    public int takeDamageRange;//受击回复怒气值
    public int maxRage;//最大怒气值
    public int seatid;//作为位置id

}
