using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

[System.Serializable]
public struct AbilityMapping
{
    public GameplayTag input_tag;
    public GameplayAbility ability;
    public GameplayData data;
}

public class AbilitySystem : MonoBehaviour
{
    public List<GameplayTag> activate_tags = new List<GameplayTag>();
    public List<AbilityMapping> boundAbilities;
    public Dictionary<GameplayTag, System.Action<GameplayData>> event_listener = new Dictionary<GameplayTag, System.Action<GameplayData>>();

    public void TryActivateAbilityByTag(GameplayTag input_tag_)
    {
        var mapping = boundAbilities.FirstOrDefault(m => m.input_tag == input_tag_);

        if(mapping.ability != null)
        {
            if(mapping.ability.CanActivate(this))
            {
                foreach (GameplayTag tag in mapping.ability.cancel_abilities_with_tags)
                {
                    var cancel_abilities = boundAbilities.FirstOrDefault(m => m.input_tag == tag);
                    if(cancel_abilities.ability != null)
                    {
                        cancel_abilities.ability.EndAbility(true);
                    }
                }

                foreach (GameplayTag tag in mapping.ability.block_abilitiesç_with_tags)
                {
                    var block_abilities = boundAbilities.FirstOrDefault(m => m.input_tag == tag);
                    if (block_abilities.ability != null)
                    {
                        block_abilities.ability.EndAbility(true);
                    }
                }
                mapping.ability.on_ability_end += OnHandleEnd;

                if (mapping.data != null)
                {
                    mapping.ability.StartAbilityWithData(this,mapping.data);

                }
                else
                {
                    mapping.ability.StartAbility(this);
                }

                StartCoroutine(mapping.ability.TimerAbilityCoroutine(this));
                foreach (var tag in mapping.ability.owned_abilities_with_tags)
                {
                    activate_tags.Add(tag);
                }

            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasTag(GameplayTag gameplay_tag_) => activate_tags.Contains(gameplay_tag_);

    public void AddTag(GameplayTag gameplay_tag_)
    {
        if(activate_tags.Contains(gameplay_tag_))
        {
            activate_tags.Add(gameplay_tag_);
        }
    }

    public void RemoveTag(GameplayTag gameplay_tag_)
    {
        activate_tags.Remove(gameplay_tag_);
    }

    public void RemoveTags(List<GameplayTag> tags_to_remove_)
    {
        foreach(var tag in tags_to_remove_)
        {
            RemoveTag(tag);
        }
    }

    public void SendGameplayEvent(GameplayTag event_tag_, GameplayData payload_)
    {
        if(event_listener.TryGetValue(event_tag_, out var action))
        {
            action.Invoke(payload_);
        }
    }

    public void AddEventListener(GameplayTag event_tag_, System.Action<GameplayData> listener)
    {
        if(!event_listener.ContainsKey(event_tag_))
        {
            event_listener[event_tag_] = delegate { };
        }

        event_listener[event_tag_] += listener;
    }

    public void RemoveEventListener(GameplayTag eventTag, System.Action<GameplayData> listener)
    {
        if (event_listener.ContainsKey(eventTag))
            event_listener[eventTag] -= listener;
    }

    public void OnHandleEnd(GameplayAbility ability_)
    {
        ability_.on_ability_end -= OnHandleEnd;
        RemoveTags(ability_.owned_abilities_with_tags);
    }

}
