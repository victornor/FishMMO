﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FishMMO_DB.Entities
{
	[Table("character_achievements", Schema = "fish_mmo_postgresql")]
	[Index(nameof(CharacterId))]
	public class CharacterAchievementEntity
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }
		public long CharacterId { get; set; }
		public CharacterEntity Character { get; set; }
		public int TemplateID { get; set; }
		public byte Tier { get; set; }
		public uint Value { get; set; }
	}
}