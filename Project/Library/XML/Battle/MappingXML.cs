using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

/* 
    [PORTUGUESE BRAZIL]
    Este código-fonte é de autoria da <BR.UZ/>, MomzGames, ExploitTeam.

    BR.UZ é um software privado: você não pode usar,
    redistribuir e/ou modificar sem autorização.

    Você deveria ter recebido uma autorização com esse software/codigo-fonte
    para ter cópias, se você não recebeu, encerre/exclua imediatamente 
    todos os vestígios do mesmo para não ter futuros problemas.

    Atenciosamente, <BR.UZ/>, MomzGames, ExploitTeam.
*/

/*
    [INGLISH USA]
    This source code is written by <BR.UZ/>, MomzGames, ExploitTeam.

    BR.UZ a private software: you will not be able to use it,
    redistribute it and / or modify without authorization.

    You should have received an authorization with this software/source code
    to have copies, if you did not receive it, immediately shutdown/exclude
    all traces of it so that you have no future problems.

    Sincerely, <BR.UZ/>, MomzGames, ExploitTeam.
*/

namespace PointBlank
{
    public class MappingXML
    {
        public static List<MapModel> maps = new List<MapModel>();
        private static readonly string path = "Data/Battle/Mapping.xml";
        public static MapModel GetMapById(int mapId)
        {
            //for (int i = 0; i < maps.Count; i++)
            //{
            //    MapModel map = maps[i];
            //    if (map.id == mapId)
            //    {
            //        return map;
            //    }
            //}
            //return null;

            return maps.Where(x => x.id == mapId).FirstOrDefault();
        }

        public static void ReGenerateList()
        {
            maps.Clear();
            Load();
        }

        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [MappingXML] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [MappingXML] Loaded {maps.Count} informations maps.");
        }

        private static void GenerateList()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                for (XmlNode xmlNode1 = document.FirstChild; xmlNode1 != null; xmlNode1 = xmlNode1.NextSibling)
                {
                    if ("list".Equals(xmlNode1.Name))
                    {
                        for (XmlNode xmlNode2 = xmlNode1.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
                        {
                            if ("Map".Equals(xmlNode2.Name))
                            {
                                XmlNamedNodeMap xml = xmlNode2.Attributes;
                                MapModel map = new MapModel
                                {
                                    id = int.Parse(xml.GetNamedItem("Id").Value)
                                };
                                BombsXML(xmlNode2, map);
                                ObjectsXML(xmlNode2, map);
                                maps.Add(map);
                            }
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                Logger.Warning(ex.ToString());
            }
        }
        private static void BombsXML(XmlNode xmlNode, MapModel map)
        {
            for (XmlNode xmlNode3 = xmlNode.FirstChild; xmlNode3 != null; xmlNode3 = xmlNode3.NextSibling)
            {
                if ("BombPositions".Equals(xmlNode3.Name))
                {
                    for (XmlNode xmlNode4 = xmlNode3.FirstChild; xmlNode4 != null; xmlNode4 = xmlNode4.NextSibling)
                    {
                        if ("Bomb".Equals(xmlNode4.Name))
                        {
                            XmlNamedNodeMap xml4 = xmlNode4.Attributes;
                            BombPosition bomb = new BombPosition
                            {
                                X = float.Parse(xml4.GetNamedItem("X").Value),
                                Y = float.Parse(xml4.GetNamedItem("Y").Value),
                                Z = float.Parse(xml4.GetNamedItem("Z").Value)
                            };
                            bomb.Position = new Half3(bomb.X, bomb.Y, bomb.Z);
                            if (bomb.X == 0 && bomb.Y == 0 && bomb.Z == 0)
                            {
                                bomb.Everywhere = true;
                            }
                            map.bombs.Add(bomb);
                        }
                    }
                }
            }
        }
        private static void ObjectsXML(XmlNode xmlNode, MapModel map)
        {
            for (XmlNode xmlNode3 = xmlNode.FirstChild; xmlNode3 != null; xmlNode3 = xmlNode3.NextSibling)
            {
                if ("objects".Equals(xmlNode3.Name))
                {
                    for (XmlNode xmlNode4 = xmlNode3.FirstChild; xmlNode4 != null; xmlNode4 = xmlNode4.NextSibling)
                    {
                        if ("Obj".Equals(xmlNode4.Name))
                        {
                            XmlNamedNodeMap xml4 = xmlNode4.Attributes;
                            ObjModel obj = new ObjModel(bool.Parse(xml4.GetNamedItem("NeedSync").Value))
                            {
                                Id = int.Parse(xml4.GetNamedItem("Id").Value),
                                Life = int.Parse(xml4.GetNamedItem("Life").Value),
                                Anim1 = int.Parse(xml4.GetNamedItem("Anim1").Value)
                            };
                            if (obj.Life > -1)
                            {
                                obj.IsDestroyable = true;
                            }
                            if (obj.Anim1 > 255)
                            {
                                if (obj.Anim1 == 256)
                                {
                                    obj.UltraSYNC = 1;
                                }
                                else if (obj.Anim1 == 257)
                                {
                                    obj.UltraSYNC = 2;
                                }
                                else if (obj.Anim1 == 258)
                                {
                                    obj.UltraSYNC = 3;
                                }
                                else if (obj.Anim1 == 259)
                                {
                                    obj.UltraSYNC = 4;
                                }
                                obj.Anim1 = 255;
                            }
                            AnimsXML(xmlNode4, obj);
                            DEffectsXML(xmlNode4, obj);
                            map.objects.Add(obj);
                        }
                    }
                }
            }
        }
        private static void AnimsXML(XmlNode xmlNode, ObjModel obj)
        {
            for (XmlNode xmlNode5 = xmlNode.FirstChild; xmlNode5 != null; xmlNode5 = xmlNode5.NextSibling)
            {
                if ("Anims".Equals(xmlNode5.Name))
                {
                    for (XmlNode xmlNode6 = xmlNode5.FirstChild; xmlNode6 != null; xmlNode6 = xmlNode6.NextSibling)
                    {
                        if ("Sync".Equals(xmlNode6.Name))
                        {
                            XmlNamedNodeMap xml6 = xmlNode6.Attributes;
                            AnimModel anim = new AnimModel
                            {
                                Id = int.Parse(xml6.GetNamedItem("Id").Value),
                                Duration = float.Parse(xml6.GetNamedItem("Date").Value),
                                NextAnim = int.Parse(xml6.GetNamedItem("Next").Value),
                                OtherObj = int.Parse(xml6.GetNamedItem("OtherOBJ").Value),
                                OtherAnim = int.Parse(xml6.GetNamedItem("OtherANIM").Value)
                            };
                            if (anim.Id == 0)
                            {
                                obj.NoInstaSync = true;
                            }
                            if (anim.Id != 255)
                            {
                                obj.UpdateId = 3;
                            }
                            obj.Anims.Add(anim);
                        }
                    }
                }
            }
        }
        private static void DEffectsXML(XmlNode xmlNode, ObjModel obj)
        {
            for (XmlNode xmlNode5 = xmlNode.FirstChild; xmlNode5 != null; xmlNode5 = xmlNode5.NextSibling)
            {
                if ("DestroyEffects".Equals(xmlNode5.Name))
                {
                    for (XmlNode xmlNode6 = xmlNode5.FirstChild; xmlNode6 != null; xmlNode6 = xmlNode6.NextSibling)
                    {
                        if ("Effect".Equals(xmlNode6.Name))
                        {
                            XmlNamedNodeMap xml6 = xmlNode6.Attributes;
                            DEffectModel anim = new DEffectModel
                            {
                                Id = int.Parse(xml6.GetNamedItem("Id").Value),
                                Life = uint.Parse(xml6.GetNamedItem("Percent").Value)
                            };
                            obj.Effects.Add(anim);
                        }
                    }
                }
            }
        }
    }
    public class MapModel
    {
        public int id;
        public List<ObjModel> objects = new List<ObjModel>();
        public List<BombPosition> bombs = new List<BombPosition>();
        public BombPosition GetBomb(int bombId)
        {
            try
            {
                return bombs[bombId];
            }
            catch
            {
                return null;
            }
        }
    }
    public class BombPosition
    {
        public float X, Y, Z;
        public Half3 Position;
        public bool Everywhere; //Em toda parte
    }
    public class ObjModel
    {
        public int Id;
        public int Life;
        public int Anim1;
        public int UltraSYNC;
        public int UpdateId = 1;
        public bool NeedSync; //Precisa de sincronização
        public bool IsDestroyable;
        public bool NoInstaSync;
        public List<AnimModel> Anims;
        public List<DEffectModel> Effects;
        public ObjModel(bool needSYNC)
        {
            NeedSync = needSYNC;
            if (needSYNC)
            {
                Anims = new List<AnimModel>();
            }
            Effects = new List<DEffectModel>();
        }

        public int CheckDestroyState(int life)
        {
            for (int i = Effects.Count - 1; i > -1; i--)
            {
                DEffectModel eff = Effects[i];
                if (eff.Life >= life)
                {
                    return eff.Id;
                }
            }
            return 0;
        }
        public int GetARandomAnim(Room room, ObjectInfo obj)
        {
            if (Anims != null && Anims.Count > 0)
            {
                int idx = new Random().Next(Anims.Count);
                AnimModel anim = Anims[idx];
                obj.animation = anim;
                obj.useDate = DateTime.Now;
                if (anim.OtherObj > 0)
                {
                    ObjectInfo obj2 = room.Objects[anim.OtherObj];
                    GetAnim(anim.OtherAnim, 0, 0, obj2);
                }
                return anim.Id;
            }
            obj.animation = null;
            return 255;
        }
        public void GetAnim(int animId, float time, float duration, ObjectInfo obj)
        {
            if (animId == 255 || obj == null || obj.info == null || obj.info.Anims == null || obj.info.Anims.Count == 0)
            {
                return;
            }
            ObjModel objModel = obj.info;
            for (int i = 0; i < objModel.Anims.Count; i++)
            {
                AnimModel anim = objModel.Anims[i];
                if (anim.Id == animId)
                {
                    obj.animation = anim;
                    time -= duration;
                    obj.useDate = DateTime.Now.AddSeconds(time * -1);
                    break;
                }
            }
        }
    }
    public class AnimModel
    {
        public int Id;
        public int NextAnim;
        public int OtherObj;
        public int OtherAnim;
        public float Duration;
    }
    public class DEffectModel
    {
        public int Id;
        public uint Life;
    }
}