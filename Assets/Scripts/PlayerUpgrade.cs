using UnityEngine;

public interface PlayerUpgrade
{
    public virtual void Apply(PlayerComponents playerComponents){}
    public virtual void Detach(PlayerComponents playerComponents){}

}
