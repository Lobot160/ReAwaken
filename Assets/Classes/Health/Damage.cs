
using System.Collections.Generic;
using System.Linq;


//Holds damage types and values
//Can also be used for type multiplyers
public struct Damage : IEnumerable<KeyValuePair<string, float>>
{
    public Damage(Dictionary<string, float> types)
    {
        amount = types;
    }

    public Damage(string type, float amount)
    {
        this.amount = new Dictionary<string, float> { { type, amount } };
    }

    public Damage(float amount)
    {
        this.amount = new Dictionary<string, float> { { "basic", amount } };
    }

    private Dictionary<string, float> amount;
    
    public float Total{ get {
        return amount.Sum(kv => kv.Value);
    }}

    public float this[string type]
    {
        get => amount.GetValueOrDefault(type, 0);
        set => amount[type] = value;
    }

    public IEnumerator<KeyValuePair<string, float>> GetEnumerator()
    {
        return amount.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    

    public static Damage operator *(Damage damage, Damage mult)
    {
        return new Damage(damage.Select(d => new {d.Key, Value = d.Value * mult.amount.GetValueOrDefault(d.Key, 1)}).ToDictionary(x => x.Key, x => x.Value));
    }

    public static Damage operator *(Damage damage, float mult)
    {
        return new Damage(damage.ToDictionary(kv => kv.Key, kv => kv.Value * mult));
    }

    public static Damage operator +(Damage damage, Damage add)
    {
        var newAmount = new Dictionary<string, float>(damage.amount);
        foreach (var kv in add.amount)
        {
            if (newAmount.ContainsKey(kv.Key))
                newAmount[kv.Key] += kv.Value;
            else
                newAmount[kv.Key] = kv.Value;
        }
        return new Damage(newAmount);
    }

}