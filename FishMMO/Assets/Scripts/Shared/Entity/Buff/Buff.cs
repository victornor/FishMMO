﻿using System.Collections.Generic;
using System.Text;

public class Buff
{
	public int templateID;
	public float tickTime;
	public float remainingTime;
	private List<BuffAttribute> attributeBonuses = new List<BuffAttribute>();
	private List<Buff> stacks = new List<Buff>();

	public BuffTemplate Template { get { return BuffTemplate.Cache[templateID]; } }
	public List<BuffAttribute> AttributeBonuses { get { return attributeBonuses; } }
	public List<Buff> Stacks { get { return stacks; } }

	public Buff(int templateID)
	{
		this.templateID = templateID;
		tickTime = Template.TickRate;
		remainingTime = Template.Duration;
	}

	public void SubtractTime(float time)
	{
		remainingTime -= time;
	}

	public void AddTime(float time)
	{
		remainingTime += time;
	}

	public void SubtractTickTime(float time)
	{
		tickTime -= time;
	}

	public void AddTickTime(float time)
	{
		tickTime += time;
	}

	public void TryTick(Character target)
	{
		if (tickTime <= 0.0f)
		{
			Template.OnTick(this, target);
			ResetTickTime();
		}
	}

	public void ResetDuration()
	{
		remainingTime = Template.Duration;
	}

	public void ResetTickTime()
	{
		tickTime = Template.TickRate;
	}

	private void Reset()
	{
		attributeBonuses.Clear();
	}

	public void AddAttributeBonus(BuffAttribute buffAttributeInstance)
	{
		attributeBonuses.Add(buffAttributeInstance);
	}

	public void Apply(Character target)
	{
		Template.OnApply(this, target);
	}

	public void Remove(Character target)
	{
		Template.OnRemove(this, target);
		Reset();
	}

	public void AddStack(Buff stack, Character target)
	{
		Template.OnApplyStack(stack, target);
		stacks.Add(stack);
	}

	public void RemoveStack(Character target)
	{
		Template.OnRemoveStack(this, target);
	}

	public string Tooltip()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("<size=120%><color=#f5ad6e>");
		sb.Append(Template.Name);
		sb.Append("</color></size>");
		sb.AppendLine();
		sb.Append("<color=#a66ef5>Remaining Time: ");
		sb.Append(remainingTime);
		sb.Append("</color>");
		if (attributeBonuses != null)
		{
			foreach (BuffAttribute attribute in attributeBonuses)
			{
				sb.AppendLine();
				sb.Append(attribute.Tooltip());
			}
		}
		return sb.ToString();
	}
}