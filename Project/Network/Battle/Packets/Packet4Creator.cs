using System;
using System.Collections.Generic;

namespace PointBlank
{
    public class Packet4Creator
    {
        public static float GetTime(DateTime date)
        {
            return (float)(DateTime.Now - date).TotalSeconds;
        }
        public static byte[] Encrypt(byte[] data, int shift)
        {
            try
            {
                int length = data.Length - 1;
                byte first = data[0];
                for (int i = 0; i < length; i++)
                {
                    data[i] = (byte)(data[i + 1] >> (8 - shift) | (data[i] << shift));
                }
                data[length] = (byte)(first >> (8 - shift) | (data[length] << shift));
                return data;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return new byte[0];
        }
        /// <summary>
        /// Gera um código do protocolo 4 com os dados encriptados.
        /// </summary>
        /// <param name="actionsBuffer">Ações (Encriptadas)</param>
        /// <param name="date">Data da sala</param>
        /// <param name="round">Rodada da partida</param>
        /// <param name="slot">Slot do jogador; 255 (Todos)</param>
        /// <returns></returns>
        public static byte[] GetCode4(byte[] actions, DateTime date, int round, int slot)
        {
            try
            {
                byte[] actionsBuffer = Encrypt(actions, (13 + actions.Length) % 6 + 1);
                using (BattlePacketWriter send = new BattlePacketWriter())
                {
                    send.WriteC(4);
                    send.WriteC((byte)slot);
                    send.WriteT(GetTime(date));
                    send.WriteC((byte)round);
                    send.WriteH((ushort)(13 + actionsBuffer.Length));
                    send.WriteD(0);
                    send.WriteB(actionsBuffer);
                    return send.memory.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return new byte[0];
        }

        public static byte[] GetCode4SyncData(List<ObjectHitInfo> objects)
        {
            try
            {
                using (BattlePacketWriter send = new BattlePacketWriter())
                {
                    for (int i = 0; i < objects.Count; i++)
                    {
                        ObjectHitInfo HitInfo = objects[i];
                        if (HitInfo.SyncType == 1)
                        {
                            if (HitInfo.ObjSyncId == 0)
                            {
                                send.WriteC((byte)P2PSubHeadEnum.OBJECT_STATIC); //Action Type
                                send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                                send.WriteH(8); //Action Length

                                send.WriteH((ushort)HitInfo.ObjectLife); //ObjectLife
                                send.WriteC((byte)HitInfo.killerId); //DestroyedBySlot (Slot de quem destruiu o objeto).
                            }
                            else
                            {
                                send.WriteC((byte)P2PSubHeadEnum.OBJECT_ANIM); //Action Type
                                send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                                send.WriteH(13); //Action Length

                                send.WriteH((ushort)HitInfo.ObjectLife);
                                send.WriteC((byte)HitInfo.AnimationId1);
                                send.WriteC((byte)HitInfo.AnimationId2);
                                send.WriteT(HitInfo.SpecialUse);
                            }
                        }
                        else if (HitInfo.SyncType == 2)
                        {
                            EventsEnum events = EventsEnum.LifeSync;
                            ushort length = 11;
                            if (HitInfo.ObjectLife == 0)
                            {
                                events |= EventsEnum.Death;
                                length += 12;
                            }
                            send.WriteC((byte)P2PSubHeadEnum.USER); //Action Type
                            send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                            send.WriteH(length); //Action Length

                            send.WriteD((uint)events); //Action Events Flags
                            send.WriteH((ushort)HitInfo.ObjectLife);

                            if (events.HasFlag(EventsEnum.Death))
                            {
                                send.WriteC((byte)(HitInfo.DeathType + (HitInfo.ObjId * 16)));
                                send.WriteC((byte)HitInfo.HitPart);
                                send.WriteH(HitInfo.Position.X.RawValue);
                                send.WriteH(HitInfo.Position.Y.RawValue);
                                send.WriteH(HitInfo.Position.Z.RawValue);
                                send.WriteD(HitInfo.WeaponId);
                            }
                        }
                        else if (HitInfo.SyncType == 3)
                        {
                            if (HitInfo.ObjSyncId == 0)
                            {
                                send.WriteC((byte)P2PSubHeadEnum.STAGEINFO_OBJ_STATIC); //Action Type
                                send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                                send.WriteH(6); //Action Length

                                send.WriteC(HitInfo.ObjectLife == 0); //isDestroyed
                            }
                            else
                            {
                                send.WriteC((byte)P2PSubHeadEnum.STAGEINFO_OBJ_ANIM); //Action Type
                                send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                                send.WriteH(14); //Action Length

                                send.WriteC((byte)HitInfo.DestroyState);
                                send.WriteH((ushort)HitInfo.ObjectLife);
                                send.WriteT(HitInfo.SpecialUse);
                                send.WriteC((byte)HitInfo.AnimationId1);
                                send.WriteC((byte)HitInfo.AnimationId2);
                            }
                        }
                        else if (HitInfo.SyncType == 4)
                        {
                            send.WriteC((byte)P2PSubHeadEnum.STAGEINFO_CHARA); //Action Type
                            send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                            send.WriteH(11); //Action Length

                            send.WriteD((uint)EventsEnum.LifeSync);
                            send.WriteH((ushort)HitInfo.ObjectLife);
                        }
                        else if (HitInfo.SyncType == 5)
                        {
                            send.WriteC((byte)P2PSubHeadEnum.USER); //Action Type
                            send.WriteH((short)HitInfo.ObjId); //Action Slot
                            send.WriteH(11); //Action Length

                            send.WriteD((uint)EventsEnum.SufferingDamage); //Dano Sofrido
                            send.WriteC((byte)(HitInfo.killerId + ((byte)HitInfo.DeathType * 16)));
                            send.WriteC((byte)HitInfo.ObjectLife);
                        }
                    }
                    return send.memory.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return new byte[0];
        }
    }
}