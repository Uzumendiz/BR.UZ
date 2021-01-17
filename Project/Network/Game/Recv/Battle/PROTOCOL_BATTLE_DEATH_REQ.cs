using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_DEATH_REQ : GamePacketReader
    {
        private FragInfos kills = new FragInfos();
        private bool isSuicide;
        public override void ReadImplement()
        {
            kills.killingType = (CharaKillTypeEnum)ReadByte();
            kills.killsCount = ReadByte();
            kills.killerIdx = ReadByte();
            kills.weaponId = ReadInt();
            kills.x = ReadFloat();
            kills.y = ReadFloat();
            kills.z = ReadFloat();
            kills.flag = ReadByte();
            for (int i = 0; i < kills.killsCount; i++)
            {
                Frag frag = new Frag
                {
                    victimWeaponClass = ReadByte()
                };
                frag.SetHitspotInfo(ReadByte());
                ReadShort();
                frag.flag = ReadByte();
                frag.x = ReadFloat();
                frag.y = ReadFloat();
                frag.z = ReadFloat();
                kills.frags.Add(frag);
                if (frag.VictimSlot == kills.killerIdx)
                {
                    isSuicide = true;
                }
            }
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room == null || room.round.Timer != null || room.state < RoomStateEnum.Battle)
                {
                    return;
                }
                bool isBotMode = room.IsBotMode();
                Slot killer = room.GetSlot(kills.killerIdx);
                if (/*!isBotMode && Settings.UdpType > UdpStateEnum.CLIENT || */killer == null || (!isBotMode && (killer.state < SlotStateEnum.BATTLE || killer.Id != player.slotId)))
                {
                    return;
                }
                ushort score = 0;
                ItemClassEnum weaponClass = (ItemClassEnum)Utilities.GetIdStatics(kills.weaponId, 1);
                for (int i = 0; i < kills.frags.Count; i++)
                {
                    Frag frag = kills.frags[i];
                    CharaDeathEnum deathType = (CharaDeathEnum)(frag.hitspotInfo >> 4);
                    if (kills.killsCount - (isSuicide ? 1 : 0) > 1)
                    {
                        frag.killFlag |= deathType == CharaDeathEnum.BOOM || deathType == CharaDeathEnum.OBJECT_EXPLOSION || deathType == CharaDeathEnum.POISON || deathType == CharaDeathEnum.HOWL || deathType == CharaDeathEnum.TRAMPLED || weaponClass == ItemClassEnum.SHOTGUN ? KillingMessageEnum.MassKill : KillingMessageEnum.PiercingShot;
                    }
                    else
                    {
                        int type = 0;
                        if (deathType == CharaDeathEnum.HEADSHOT)
                        {
                            type = 4;
                        }
                        else if (deathType == CharaDeathEnum.DEFAULT && weaponClass == ItemClassEnum.KNIFE)
                        {
                            type = 6;
                        }
                        if (type > 0)
                        {
                            int lastState = killer.lastKillState >> 12;
                            if (type == 4)
                            {
                                if (lastState != 4)
                                {
                                    killer.repeatLastState = false;
                                }
                                killer.lastKillState = type << 12 | (killer.killsOnLife + 1);
                                if (killer.repeatLastState)
                                {
                                    frag.killFlag |= (killer.lastKillState & 16383) <= 1 ? KillingMessageEnum.Headshot : KillingMessageEnum.ChainHeadshot;
                                }
                                else
                                {
                                    frag.killFlag |= KillingMessageEnum.Headshot;
                                    killer.repeatLastState = true;
                                }
                            }
                            else if (type == 6)
                            {
                                if (lastState != 6)
                                {
                                    killer.repeatLastState = false;
                                }
                                killer.lastKillState = type << 12 | (killer.killsOnLife + 1);
                                if (killer.repeatLastState && (killer.lastKillState & 16383) > 1)
                                {
                                    frag.killFlag |= KillingMessageEnum.ChainSlugger;
                                }
                                else
                                {
                                    killer.repeatLastState = true;
                                }
                            }
                        }
                        else
                        {
                            killer.lastKillState = 0;
                            killer.repeatLastState = false;
                        }
                    }
                    int victimId = frag.VictimSlot;
                    Slot victimSlot = room.slots[victimId];
                    if (victimSlot.killsOnLife > 3)
                    {
                        frag.killFlag |= KillingMessageEnum.ChainStopper;
                    }
                    if (!((kills.weaponId == 19016 || kills.weaponId == 19022) && kills.killerIdx == victimId) || !victimSlot.specGM)
                    {
                        victimSlot.allDeaths++; //explosao acid
                    }
                    if (killer.teamId != victimSlot.teamId)
                    {
                        score += GetKillScore(frag.killFlag);
                        killer.allKills++;
                        if (killer.deathState == DeadEnum.isAlive)
                        {
                            killer.killsOnLife++;
                        }
                        if (victimSlot.teamId == 0)
                        {
                            room.redDeaths++;
                            room.blueKills++;
                        }
                        else
                        {
                            room.blueDeaths++;
                            room.redKills++;
                        }
                        if (room.mode == RoomTypeEnum.Dino)
                        {
                            if (killer.teamId == 0)
                            {
                                room.redDino += 4;
                            }
                            else
                            {
                                room.blueDino += 4;
                            }
                        }
                    }
                    victimSlot.lastKillState = 0;
                    victimSlot.killsOnLife = 0;
                    victimSlot.repeatLastState = false;
                    victimSlot.passSequence = 0;
                    victimSlot.deathState = DeadEnum.isDead;
                    if (!isBotMode)
                    {
                        room.MissionCompleteBase(room.GetPlayerBySlot(victimSlot), victimSlot, MissionTypeEnum.DEATH, 0);
                    }
                    if (deathType == CharaDeathEnum.HEADSHOT)
                    {
                        killer.headshots++;
                    }
                }
                if (isBotMode)
                {
                    killer.score += (ushort)(killer.killsOnLife + room.IngameAiLevel + score);
                    if (killer.score > 65535)
                    {
                        killer.score = 65535;
                        Logger.Warning($" [GAME] [BATTLE_DEATH_REQ] Foi atingido o limite de pontuação no Modo Challenge. Nick: {player.nickname} PlayerId: {player.playerId}");
                    }
                    kills.Score = killer.score;
                }
                else
                {
                    killer.score += score;
                    room.MissionCompleteBase(player, killer, kills, MissionTypeEnum.NA, 0);
                    kills.Score = score;
                }
                using (BATTLE_DEATH_PAK packet = new BATTLE_DEATH_PAK(room, kills, killer, isBotMode))
                {
                    room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                }
                if ((room.mode == RoomTypeEnum.DeathMatch || room.mode == RoomTypeEnum.HeadHunter || room.mode == RoomTypeEnum.Chaos) && !isBotMode)
                {
                    room.BattleEndKills(isBotMode);
                }
                else if (!killer.specGM && (room.mode == RoomTypeEnum.Destruction || room.mode == RoomTypeEnum.Suppression)) //Destruição e Supressão
                {
                    room.GetPlayingPlayers(true, out int allRed, out int allBlue, out int redDeaths, out int blueDeaths);
                    int winner = 0;
                    if (redDeaths == allRed && killer.teamId == 0 && isSuicide && !room.PlantedBombC4)
                    {
                        winner = 1;
                        room.blueRounds++;
                        room.BattleEndRound(winner, true);
                    }
                    else if (blueDeaths == allBlue && killer.teamId == 1)
                    {
                        room.redRounds++;
                        room.BattleEndRound(winner, true);
                    }
                    else if (redDeaths == allRed && killer.teamId == 1)
                    {
                        if (!room.PlantedBombC4)
                        {
                            winner = 1;
                            room.blueRounds++;
                        }
                        else if (isSuicide)
                        {
                            room.redRounds++;
                        }
                        room.BattleEndRound(winner, false);
                    }
                    else if (blueDeaths == allBlue && killer.teamId == 0)
                    {
                        if (!isSuicide || !room.PlantedBombC4)
                        {
                            room.redRounds++;
                        }
                        else
                        {
                            winner = 1;
                            room.blueRounds++;
                        }
                        room.BattleEndRound(winner, true);
                    }
                }
                kills = null;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }

        public ushort GetKillScore(KillingMessageEnum killingMessage)
        {
            ushort score = 0;
            if (killingMessage == KillingMessageEnum.MassKill || killingMessage == KillingMessageEnum.PiercingShot)
            {
                score += 6;
            }
            else if (killingMessage == KillingMessageEnum.ChainStopper)
            {
                score += 8;
            }
            else if (killingMessage == KillingMessageEnum.Headshot)
            {
                score += 10;
            }
            else if (killingMessage == KillingMessageEnum.ChainHeadshot)
            {
                score += 14;
            }
            else if (killingMessage == KillingMessageEnum.ChainSlugger)
            {
                score += 6;
            }
            else if (killingMessage == KillingMessageEnum.ObjectDefense)
            {
                score += 7;
            }
            else if (killingMessage != KillingMessageEnum.Suicide)
            {
                score += 5;
            }
            return score;
        }
    }
}