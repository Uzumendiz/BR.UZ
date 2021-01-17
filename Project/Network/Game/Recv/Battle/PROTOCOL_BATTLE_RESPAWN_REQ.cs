using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_RESPAWN_REQ : GamePacketReader
    {
        private PlayerEquipedItems equipment;
        private int WeaponsFlag;
        public override void ReadImplement()
        {
            equipment = new PlayerEquipedItems
            {
                primary = ReadInt(),
                secondary = ReadInt(),
                melee = ReadInt(),
                grenade = ReadInt(),
                special = ReadInt()
            };
            ReadInt(); //slot do equipment?
            equipment.red = ReadInt();
            equipment.blue = ReadInt();
            equipment.helmet = ReadInt();
            equipment.beret = ReadInt();
            equipment.dino = ReadInt();
            WeaponsFlag = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room == null || room.state != RoomStateEnum.Battle)
                {
                    return;
                }
                Slot slot = room.GetSlot(player.slotId);
                if (slot == null || slot.state != SlotStateEnum.BATTLE)
                {
                    return;
                }
                bool NoChangeEquipment = false;
                if (slot.deathState.HasFlag(DeadEnum.isDead) || slot.deathState.HasFlag(DeadEnum.useChat))
                {
                    slot.deathState = DeadEnum.isAlive;
                }
                player.CheckEquipedItems(equipment, true); //Verifica se os equipamentos enviados pelo pacote existem no inventario.
                string roomName = room.roomName.ToUpper();
                if (TournamentRulesManager.CheckRoomRule(roomName))
                {
                    if ((WeaponsFlag & 8) > 0)
                    {
                        if (!TournamentRulesManager.IsBlocked(roomName, equipment.primary))
                        {
                            slot.equipment.primary = equipment.primary;
                            InsertItem(equipment.primary, slot);
                        }
                        else if (slot.equipment.primary != equipment.primary)
                        {
                            NoChangeEquipment = true;
                        }
                    }
                    if ((WeaponsFlag & 4) > 0)
                    {
                        if (!TournamentRulesManager.IsBlocked(roomName, equipment.secondary))
                        {
                            slot.equipment.secondary = equipment.secondary;
                            InsertItem(equipment.secondary, slot);
                        }
                        else if (slot.equipment.secondary != equipment.secondary)
                        {
                            NoChangeEquipment = true;
                        }
                    }
                    if ((WeaponsFlag & 2) > 0)
                    {
                        if (!TournamentRulesManager.IsBlocked(roomName, equipment.melee))
                        {
                            slot.equipment.melee = equipment.melee;
                            InsertItem(equipment.melee, slot);
                        }
                        else if (slot.equipment.melee != equipment.melee)
                        {
                            NoChangeEquipment = true;
                        }
                    }
                    if ((WeaponsFlag & 1) > 0)
                    {
                        if (!TournamentRulesManager.IsBlocked(roomName, equipment.grenade))
                        {
                            slot.equipment.grenade = equipment.grenade;
                            InsertItem(equipment.grenade, slot);
                        }
                        else if(slot.equipment.grenade != equipment.grenade)
                        {
                            NoChangeEquipment = true;
                        }
                    }
                    if (!TournamentRulesManager.IsBlocked(roomName, equipment.special))
                    {
                        slot.equipment.special = equipment.special;
                        InsertItem(equipment.special, slot);
                    }
                    else if (slot.equipment.special != equipment.special)
                    {
                        NoChangeEquipment = true;
                    }
                }
                else
                {
                    slot.equipment = equipment;
                    if ((WeaponsFlag & 8) > 0)
                    {
                        InsertItem(equipment.primary, slot);
                    }
                    if ((WeaponsFlag & 4) > 0)
                    {
                        InsertItem(equipment.secondary, slot);
                    }
                    if ((WeaponsFlag & 2) > 0)
                    {
                        InsertItem(equipment.melee, slot);
                    }
                    if ((WeaponsFlag & 1) > 0)
                    {
                        InsertItem(equipment.grenade, slot);
                    }
                    InsertItem(equipment.special, slot);
                }
                if (slot.firstRespawn)
                {
                    if (slot.teamId == 0)
                    {
                        InsertItem(equipment.red, slot);
                    }
                    else
                    {
                        InsertItem(equipment.blue, slot);
                    }
                    InsertItem(equipment.helmet, slot);
                    InsertItem(equipment.beret, slot);
                }
                InsertItem(equipment.dino, slot);
                using (BATTLE_RESPAWN_PAK packet = new BATTLE_RESPAWN_PAK(room, slot))
                {
                    room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                }
                if (NoChangeEquipment)
                {
                    client.SendCompletePacket(PackageDataManager.SERVER_MESSAGE_ANNOUNCE_BLOCK_RULE_PAK);
                }
                room.UpdatePlayerInfoInBattle(slot, player.effects, slot.firstRespawn ? 0 : 2);
                slot.firstRespawn = false;
                if (room.weaponsFlag != WeaponsFlag)
                {
                    Logger.Warning($" [BATTLE_RESPAWN_REQ] [WeaponsFlag] Room: " + room.weaponsFlag + " Player: " + WeaponsFlag);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }

        private void InsertItem(int id, Slot slot)
        {
            lock (slot.EquipmentsUsed)
            {
                if (!slot.EquipmentsUsed.Contains(id))
                {
                    slot.EquipmentsUsed.Add(id);
                }
            }
        }
    }
}