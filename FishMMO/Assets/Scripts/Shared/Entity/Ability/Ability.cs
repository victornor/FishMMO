﻿using System.Collections.Generic;
using System.Text;

public class Ability
{
	public int AbilityID;
	public float ActivationTime;
	public float ActiveTime;
	public float Cooldown;
	public float Range;
	public float Speed;

	public AbilityTemplate Template { get; private set; }
	public string Name { get; set; }
	public AbilityResourceDictionary Resources { get; private set; }
	public AbilityResourceDictionary Requirements { get; private set; }
	// cache of all ability events
	public Dictionary<int, AbilityEvent> AbilityEvents { get; private set; }
	public Dictionary<int, SpawnEvent> PreSpawnEvents { get; private set; }
	public Dictionary<int, SpawnEvent> SpawnEvents { get; private set; }
	public Dictionary<int, MoveEvent> MoveEvents { get; private set; }
	public Dictionary<int, HitEvent> HitEvents { get; private set; }

	/// <summary>
	/// Cache of all active ability Objects. <ContainerID, <AbilityObjectID, AbilityObject>>
	/// </summary>
	public Dictionary<int, Dictionary<int, AbilityObject>> Objects { get; set; }

	public int TotalResourceCost
	{
		get
		{
			int totalCost = 0;
			foreach (int cost in Resources.Values)
			{
				totalCost += cost;
			}
			return totalCost;
		}
	}

	public Ability(int abilityID, int templateID) : this(abilityID, templateID, null)
	{
	}

	public Ability(int abilityID, int templateID, List<AbilityEvent> events)
	{
		AbilityID = abilityID;
		Template = AbilityTemplate.Get<AbilityTemplate>(templateID);
		Name = Template.Name;

		InternalAddTemplateModifiers(Template);

		if (events != null)
		{
			for (int i = 0; i < events.Count; ++i)
			{
				AddAbilityEvent(events[i]);
			}
		}
	}

	internal void InternalAddTemplateModifiers(AbilityTemplate template)
	{
		ActivationTime += template.ActivationTime;
		ActiveTime += template.ActiveTime;
		Cooldown += template.Cooldown;
		Range += template.Range;
		Speed += template.Speed;

		foreach (KeyValuePair<CharacterAttributeTemplate, int> pair in template.Resources)
		{
			if (!Resources.ContainsKey(pair.Key))
			{
				Resources.Add(pair.Key, pair.Value);

			}
			else
			{
				Resources[pair.Key] += pair.Value;
			}
		}

		foreach (KeyValuePair<CharacterAttributeTemplate, int> pair in template.Requirements)
		{
			if (!Requirements.ContainsKey(pair.Key))
			{
				Requirements.Add(pair.Key, pair.Value);

			}
			else
			{
				Requirements[pair.Key] += pair.Value;
			}
		}
	}

	public bool TryGetAbilityEvent<T>(int templateID, out T modifier) where T : AbilityEvent
	{
		if (AbilityEvents.TryGetValue(templateID, out AbilityEvent result))
		{
			if ((modifier = result as T) != null)
			{
				return true;
			}
		}
		modifier = null;
		return false;
	}

	public bool HasAbilityEvent(int templateID)
	{
		return AbilityEvents.ContainsKey(templateID);
	}

	public void AddAbilityEvent(AbilityEvent abilityEvent)
	{
		if (!AbilityEvents.ContainsKey(abilityEvent.ID))
		{
			AbilityEvents.Add(abilityEvent.ID, abilityEvent);

			SpawnEvent spawnEvent = abilityEvent as SpawnEvent;
			if (spawnEvent != null)
			{
				switch (spawnEvent.SpawnEventType)
				{
					case SpawnEventType.OnPreSpawn:
						if (!PreSpawnEvents.ContainsKey(spawnEvent.ID))
						{
							PreSpawnEvents.Add(spawnEvent.ID, spawnEvent);
						}
						break;
					case SpawnEventType.OnSpawn:
						if (!SpawnEvents.ContainsKey(spawnEvent.ID))
						{
							SpawnEvents.Add(spawnEvent.ID, spawnEvent);
						}
						break;
					default:
						break;
				}
			}
			else
			{
				HitEvent hitEvent = abilityEvent as HitEvent;
				if (hitEvent != null)
				{
					HitEvents.Add(abilityEvent.ID, hitEvent);
				}
				else
				{
					MoveEvent moveEvent = abilityEvent as MoveEvent;
					if (moveEvent != null)
					{
						MoveEvents.Add(abilityEvent.ID, moveEvent);
					}
				}
			}

			ActivationTime += abilityEvent.ActivationTime;
			ActiveTime += abilityEvent.ActiveTime;
			Cooldown += abilityEvent.Cooldown;
			Range += abilityEvent.Range;
			Speed += abilityEvent.Speed;
			foreach (KeyValuePair<CharacterAttributeTemplate, int> pair in abilityEvent.Resources)
			{
				if (!Resources.ContainsKey(pair.Key))
				{
					Resources.Add(pair.Key, pair.Value);

				}
				else
				{
					Resources[pair.Key] += pair.Value;
				}
			}
			foreach (KeyValuePair<CharacterAttributeTemplate, int> pair in abilityEvent.Requirements)
			{
				if (!Requirements.ContainsKey(pair.Key))
				{
					Requirements.Add(pair.Key, pair.Value);
				}
				else
				{
					Requirements[pair.Key] += pair.Value;
				}
			}
		}
	}

	public void RemoveAbilityEvent(AbilityEvent abilityEvent)
	{
		if (AbilityEvents.ContainsKey(abilityEvent.ID))
		{
			AbilityEvents.Remove(abilityEvent.ID);

			SpawnEvent spawnEvent = abilityEvent as SpawnEvent;
			if (spawnEvent != null)
			{
				switch (spawnEvent.SpawnEventType)
				{
					case SpawnEventType.OnPreSpawn:
						PreSpawnEvents.Remove(spawnEvent.ID);
						break;
					case SpawnEventType.OnSpawn:
						SpawnEvents.Remove(spawnEvent.ID);
						break;
					default:
						break;
				}
			}
			else
			{
				HitEvent hitEvent = abilityEvent as HitEvent;
				if (hitEvent != null)
				{
					HitEvents.Remove(abilityEvent.ID);
				}
				else
				{
					MoveEvent moveEvent = abilityEvent as MoveEvent;
					if (moveEvent != null)
					{
						MoveEvents.Remove(abilityEvent.ID);
					}
				}
			}

			ActivationTime -= abilityEvent.ActivationTime;
			ActiveTime -= abilityEvent.ActiveTime;
			Cooldown -= abilityEvent.Cooldown;
			Range -= abilityEvent.Range;
			Speed -= abilityEvent.Speed;
			foreach (KeyValuePair<CharacterAttributeTemplate, int> pair in abilityEvent.Resources)
			{
				if (Resources.ContainsKey(pair.Key))
				{
					Resources[pair.Key] -= pair.Value;
				}
			}
			foreach (KeyValuePair<CharacterAttributeTemplate, int> pair in abilityEvent.Requirements)
			{
				if (Requirements.ContainsKey(pair.Key))
				{
					Requirements[pair.Key] += pair.Value;
				}
			}
		}
	}

	public bool MeetsRequirements(Character character)
	{
		foreach (KeyValuePair<CharacterAttributeTemplate, int> pair in Requirements)
		{
			if (!character.AttributeController.TryGetResourceAttribute(pair.Key.ID, out CharacterResourceAttribute requirement) ||
				requirement.CurrentValue < pair.Value)
			{
				return false;
			}
		}
		return true;
	}

	public bool HasResource(Character character, AbilityEvent bloodResourceConversion, CharacterAttributeTemplate bloodResource)
	{
		if (AbilityEvents.ContainsKey(bloodResourceConversion.ID))
		{
			int totalCost = TotalResourceCost;

			CharacterResourceAttribute resource;
			if (!character.AttributeController.TryGetResourceAttribute(bloodResource.ID, out resource) ||
				resource.CurrentValue < totalCost)
			{
				return false;
			}
		}
		else
		{
			foreach (KeyValuePair<CharacterAttributeTemplate, int> pair in Resources)
			{
				CharacterResourceAttribute resource;
				if (!character.AttributeController.TryGetResourceAttribute(pair.Key.ID, out resource) ||
					resource.CurrentValue < pair.Value)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void ConsumeResources(CharacterAttributeController attributeController, AbilityEvent bloodResourceConversion, CharacterAttributeTemplate bloodResource)
	{
		if (bloodResourceConversion != null && AbilityEvents.ContainsKey(bloodResourceConversion.ID))
		{
			int totalCost = TotalResourceCost;

			CharacterResourceAttribute resource;
			if (bloodResource != null && attributeController.TryGetResourceAttribute(bloodResource.ID, out resource) &&
				resource.CurrentValue >= totalCost)
			{
				resource.Consume(totalCost);
			}
		}
		else
		{
			foreach (KeyValuePair<CharacterAttributeTemplate, int> pair in Resources)
			{
				CharacterResourceAttribute resource;
				if (attributeController.TryGetResourceAttribute(pair.Key.ID, out resource) &&
					resource.CurrentValue < pair.Value)
				{
					resource.Consume(pair.Value);
				}
			}
		}
	}

	public void RemoveAbilityObject(int containerID, int objectID)
	{
		if (Objects.TryGetValue(containerID, out Dictionary<int, AbilityObject> container))
		{
			container.Remove(objectID);
		}
	}

	public string Tooltip()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("<size=120%><color=#f5ad6e>");
		sb.Append(Template.Name);
		sb.Append("</color></size>");
		sb.AppendLine();
		sb.Append("<color=#a66ef5>AbilityID: ");
		sb.Append(AbilityID);
		sb.Append("</color>");
		sb.AppendLine();
		sb.Append("<color=#a66ef5>TemplateID: ");
		sb.Append(Template.ID);
		sb.Append("</color>");
		sb.AppendLine();
		sb.Append("<color=#a66ef5>Activation Time: ");
		sb.Append(ActivationTime);
		sb.Append("</color>");
		sb.AppendLine();
		sb.Append("<color=#a66ef5>Active Time: ");
		sb.Append(ActiveTime);
		sb.Append("</color>");
		sb.AppendLine();
		sb.Append("<color=#a66ef5>Cooldown: ");
		sb.Append(Cooldown);
		sb.Append("</color>");
		sb.AppendLine();
		sb.Append("<color=#a66ef5>Range: ");
		sb.Append(Range);
		sb.Append("</color>");
		sb.AppendLine();
		sb.Append("<color=#a66ef5>Speed: ");
		sb.Append(Speed);
		sb.Append("</color>");
		return sb.ToString();
	}
}