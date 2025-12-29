using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "GameplayAbility", menuName = "GAS/GameplayAbility")]
public class GameplayAbility : ScriptableObject
{
    public string ability_name;

    public List<GameplayTag> ability_tags;
    public List<GameplayTag> cancel_abilities_with_tags;
    public List<GameplayTag> block_abilitiesç_with_tags;
    public List<GameplayTag> owned_abilities_with_tags;
    public List<GameplayTag> blocked_tags;
    public List<GameplayTag> required_tags;

    public GameplayTag gameplay_event_tag;
    public GameplayTag send_gameplay_event_tag;

    public System.Action<GameplayData> event_action;
    public System.Action<GameplayAbility> on_ability_end;

    public bool is_active = false;

    private void OnEnable()
    {
        is_active = false;
    }

    public virtual bool CanActivate(AbilitySystem owner_)
    {
        foreach(var tag in blocked_tags)
        {
            if(owner_.HasTag(tag))
            {
                return false;
            }
        }

        if(required_tags.Count > 0)
        {
            foreach (var tag in required_tags)
            {
                if (!owner_.HasTag(tag))
                {
                    return false;
                }
            }
        }

        if(is_active)
        {
            return false;
        }

        return true;
    }

    public virtual void StartAbility(AbilitySystem owner_)
    {
        if(is_active)
        {
            return;
        }

        Debug.Log("Activation de " + ability_name + " sans gameplay data");

        is_active = true;

        if(gameplay_event_tag != null)
        {
            owner_.AddEventListener(gameplay_event_tag, event_action);
        }

        if(send_gameplay_event_tag != null)
        {
            owner_.SendGameplayEvent(send_gameplay_event_tag, null);
        }

    }

    public virtual void StartAbilityWithData(AbilitySystem owner_,GameplayData gameplay_data_)
    {
        Debug.Log("Activation de " + ability_name + " avec gameplay data");
        is_active = true;
        if (gameplay_event_tag != null)
        {
            owner_.AddEventListener(gameplay_event_tag, GetGameplayEvent);
        }

        if (send_gameplay_event_tag != null)
        {
            owner_.SendGameplayEvent(send_gameplay_event_tag, gameplay_data_);
        }

    }

    public  virtual IEnumerator TimerAbilityCoroutine(AbilitySystem owner_)
    {
        yield return new WaitForSeconds(5.0f); //50 SECONDES !

        if(is_active)
        {
            EndAbility(false);
        }
    }

    public virtual void EndAbility(bool cancel_)
    {
        if (!is_active)
        {
            return;
        }

        Debug.Log("EndAbility de " + ability_name);
        is_active = false;
        if(on_ability_end != null)
        {
            on_ability_end.Invoke(this);
        }

    }

    public virtual void GetGameplayEvent(GameplayData payload_)
    {
        if(!is_active)
        {
            return;
        }

        Debug.Log("Get gameplay event ");
        EndAbility(false);
    }
}
