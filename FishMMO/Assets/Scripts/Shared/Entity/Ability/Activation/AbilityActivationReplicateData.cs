﻿using FishNet.Object.Prediction;
using UnityEngine;

public struct AbilityActivationReplicateData : IReplicateData
{
	public bool InterruptQueued;
	public int QueuedAbilityID;
	public KeyCode HeldKey;

	public AbilityActivationReplicateData(bool interruptQueued, int queuedAbilityID, KeyCode heldKey)
	{
		InterruptQueued = interruptQueued;
		QueuedAbilityID = queuedAbilityID;
		HeldKey = heldKey;
		_tick = 0;
	}

	private uint _tick;
	public void Dispose() { }
	public uint GetTick() => _tick;
	public void SetTick(uint value) => _tick = value;
}