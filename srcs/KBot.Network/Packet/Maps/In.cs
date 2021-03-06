﻿using System.Linq;
using KBot.Common.Extension;
using KBot.Game;
using KBot.Game.Enum;

namespace KBot.Network.Packet.Maps
{
    public class In : IPacket
    {
        public EntityType EntityType { get; set; }
        public string Name { get; set; }
        public int ModelId { get; set; }
        public long EntityId { get; set; }
        public Position Position { get; set; }
        public int Direction { get; set; }
        
        public InNpc Npc { get; set; }
        public InPlayer Player { get; set; }
        public InMapObject MapObject { get; set; }
    }

    public class InNpc
    {
        public int HpPercentage { get; set; }
        public int MpPercentage { get; set; }
        public long? Owner { get; set; }
        public string Name { get; set; }
    }

    public class InPlayer
    {
        public Gender Gender { get; set; }
        public Job Job { get; set; }
        public int HpPercentage { get; set; }
        public int MpPercentage { get; set; }
        
        public int Level { get; set; }
        public int HeroLevel { get; set; }
    }

    public class InMapObject
    {
        public int Amount { get; set; }
        public bool IsQuestRelative { get; set; }
        public long Owner { get; set; }
    }

    public class InCreator : IPacketCreator
    {
        public string Header { get; } = "in";
        public PacketType PacketType { get; } = PacketType.Received;
        
        public IPacket Create(string[] content)
        {
            EntityType entityType = content[0].ToEnum<EntityType>();
            int startIndex = entityType == EntityType.Player ? 3 : 2;

            var packet = new In
            {
                EntityType = entityType,
                Name = entityType == EntityType.Player ? content[1] : string.Empty,
                ModelId = entityType != EntityType.Player ? content[1].ToInt() : 0,
                EntityId = content[startIndex].ToLong(),
                Position = new Position(content[startIndex + 1].ToShort(), content[startIndex + 2].ToShort()),
                Direction = entityType != EntityType.MapObject ? content[startIndex + 3].ToInt() : 0
            };

            string[] specialInfo = content.Skip(startIndex + (entityType == EntityType.MapObject ? 3 : 4)).ToArray();
            switch (entityType)
            {
                case EntityType.Monster:
                case EntityType.Npc:
                    packet.Npc = new InNpc
                    {
                        HpPercentage = specialInfo[0].ToInt(),
                        MpPercentage = specialInfo[1].ToInt(),
                        Owner = specialInfo[5].ToNullableLong(),
                        Name = specialInfo[9]
                    };
                    break;
                case EntityType.Player:
                    packet.Player = new InPlayer
                    {
                        Gender = specialInfo[1].ToEnum<Gender>(),
                        Job = specialInfo[4].ToEnum<Job>(),
                        HpPercentage = specialInfo[6].ToInt(),
                        MpPercentage = specialInfo[7].ToInt(),
                        Level = specialInfo[25].ToInt(),
                        HeroLevel = specialInfo[31].ToInt()
                    };
                    break;
                case EntityType.MapObject:
                    packet.MapObject = new InMapObject
                    {
                        Amount = specialInfo[0].ToInt(),
                        IsQuestRelative = specialInfo[1].ToBool(),
                        Owner = specialInfo[2].ToLong()
                    };
                    break;
            }

            return packet;
        }
    }
}