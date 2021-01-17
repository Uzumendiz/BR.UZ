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
    public class WeaponInfo
    {
        public int number;
        public int classType;
        public int usageType; //0=PRIMARY 1=SECONDARY 2=MEELE 3=GRENADE 4=SPECIAL (MEDICAL KIT) 5=ONLY C4 & C4 ShuttleX 6=DominationObject, SupplyBase 7=Dummy, Dummy_Lv2 8=SentryGun, SentryGun_Lv2

        public int damage;

        public float recoilHorzMax;
        public float recoilHorzAngle;
        public float recoilVertMax;
        public float recoilVertAngle;

        public float attackDamageA1;
        public float attackDamageA2;
        public float attackDamageB1;
        public float attackDamageB2;
        public float attackDamageA1ForDino;
        public float attackDamageB1ForDino;

        public float range;

        public string name;
        public bool CheckWeapon(HitDataNormalDamage hit, out string result)
        {
            if (usageType != 2 && hit.Range > range) //TODAS MENOS MEELE
            {
                result = "RANGE OTHERS";
                return false;
            }
            else if (usageType == 2 && hit.Range > range) //MEELE
            {
                result = "RANGE MEELE";
                return false;
            }
            if (hit.DeathType != CharaDeathEnum.HEADSHOT)
            {
                int PercentualDamage = damage + (damage * 30 / 100);
                if (usageType != 2 && hit.WeaponDamage > PercentualDamage) //TODAS MENOS MEELE
                {
                    result = "DAMAGE";
                    return false;
                }
                if (usageType == 2) //MEELE
                {
                    result = $"hit.WeaponDamage {hit.WeaponDamage} AttackDamageA1: {attackDamageA1} AttackDamageA2: {attackDamageA2} AttackDamageB1: {attackDamageB1} AttackDamageB2: {attackDamageB2} AttackDamageA1ForDino: {attackDamageA1ForDino} AttackDamageA1ForDino: {attackDamageB1ForDino}";
                    if (hit.WeaponDamage > attackDamageA1)
                    {
                        result = "AttackDamageA1";
                        return false;
                    }
                    if (hit.WeaponDamage > attackDamageA2)
                    {
                        result = "AttackDamageA2";
                        return false;
                    }
                    if (hit.WeaponDamage > attackDamageB1)
                    {
                        result = "AttackDamageB1";
                        return false;
                    }
                    if (hit.WeaponDamage > attackDamageB2)
                    {
                        result = "AttackDamageB2";
                        return false;
                    }
                    if (hit.WeaponDamage > attackDamageA1ForDino)
                    {
                        result = "AttackDamageA1ForDino";
                        return false;
                    }
                    if (hit.WeaponDamage > attackDamageB1ForDino)
                    {
                        result = "AttackDamageB1ForDino";
                        return false;
                    }
                }
            }
            result = "";
            return true;
        }
    }
    public class WeaponsXML
    {
        public static List<WeaponInfo> weapons = new List<WeaponInfo>();
        private static readonly string path = "Data/Battle/Weapons.xml";
        public static WeaponInfo GetWeapon(int number, int classType, int usageType)
        {
            try
            {
                return weapons.Where(x => x.number == number && x.classType == classType && x.usageType == usageType).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return null;
        }

        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [CharaXML] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [WeaponsXML] Loaded {weapons.Count} weapons.");
        }

        private static void GenerateList()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                for (XmlNode PrimaryNode = document.FirstChild; PrimaryNode != null; PrimaryNode = PrimaryNode.NextSibling)
                {
                    if ("list".Equals(PrimaryNode.Name))
                    {
                        for (XmlNode SecundaryNode = PrimaryNode.FirstChild; SecundaryNode != null; SecundaryNode = SecundaryNode.NextSibling)
                        {
                            XmlNamedNodeMap itemMap = SecundaryNode.Attributes;
                            if ("item".Equals(SecundaryNode.Name))
                            {
                                WeaponInfo item = new WeaponInfo
                                {
                                    number = int.Parse(itemMap.GetNamedItem("number").Value),
                                    classType = int.Parse(itemMap.GetNamedItem("class_type").Value),
                                    usageType = int.Parse(itemMap.GetNamedItem("usage_type").Value),

                                    damage = int.Parse(itemMap.GetNamedItem("damage").Value),

                                    recoilHorzAngle = float.Parse(itemMap.GetNamedItem("recoil_horz_angle").Value),
                                    recoilHorzMax = float.Parse(itemMap.GetNamedItem("recoil_horz_max").Value),
                                    recoilVertAngle = float.Parse(itemMap.GetNamedItem("recoil_vert_angle").Value),
                                    recoilVertMax = float.Parse(itemMap.GetNamedItem("recoil_vert_max").Value),

                                    attackDamageA1 = float.Parse(itemMap.GetNamedItem("attack_damage_a1").Value),
                                    attackDamageA1ForDino = float.Parse(itemMap.GetNamedItem("attack_damage_a1_for_dino").Value),
                                    attackDamageA2 = float.Parse(itemMap.GetNamedItem("attack_damage_a2").Value),
                                    attackDamageB1 = float.Parse(itemMap.GetNamedItem("attack_damage_b1").Value),
                                    attackDamageB1ForDino = float.Parse(itemMap.GetNamedItem("attack_damage_b1_for_dino").Value),
                                    attackDamageB2 = float.Parse(itemMap.GetNamedItem("attack_damage_b2").Value),

                                    range = float.Parse(itemMap.GetNamedItem("range").Value),

                                    name = itemMap.GetNamedItem("name").Value,
                                };
                                weapons.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}