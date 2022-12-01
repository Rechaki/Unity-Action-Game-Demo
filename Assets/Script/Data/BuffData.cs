using System.Collections;
using System.Collections.Generic;

public class BuffData
{
    public string id;                                                                       //ID
    public string name;                                                                     //Buffの名前
    public float durationTime;                                                              //持続時間
    public UnitData owner;                                                                  //所有者
    public BuffAttributes attributes = BuffAttributes.None;                                 //Buffのタイプ
    public List<PropertyData> modifiedProperties = new List<PropertyData>();                //
}

public class PropertyData
{
    public ModifiedProperty property = ModifiedProperty.None;                               //変更属性
    public ValueSource source = ValueSource.Set;                                            //数値出所
    public ModifiedProperty sourceType = ModifiedProperty.None;                             //数値出所の属性
    public float value;                                                                     //数値
    public int actualValue;                                                               //計算した数値
}