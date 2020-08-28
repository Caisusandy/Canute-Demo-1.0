using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    public static class Extensions
    {
        public static T Exist<T>(this T instance) where T : UnityEngine.Object
        {
            return instance ? instance : null;
        }

        public static string GenerateName(int num)
        {
            int number;
            char code;
            string checkCode = string.Empty;
            System.Random random = new System.Random();
            for (int i = 0; i < num; i++)
            {
                number = random.Next();
                if (number % 2 == 0)
                    code = (char)('0' + (char)(number % 10));
                else
                    code = (char)('A' + (char)(number % 26));
                checkCode += code.ToString();
            }

            return checkCode;
        }

        //public static Sprite GetSprite(this BuildingType buildingType)
        //{
        //    switch (buildingType)
        //    {
        //        case BuildingType.Bridge:
        //            return Resources.Load("Textures/Map/Entity/Buildings/Camp", typeof(Sprite)) as Sprite;
        //    }

        //    return Resources.Load("Textures/Map/Entity/Buildings/" + Enum.GetName(typeof(BuildingType), buildingType), typeof(Sprite)) as Sprite;
        //}

        //public static Sprite GetSprite(this EffectType effect)
        //{
        //    return Resources.Load("Textures/Map/Entity/Cards/" + Enum.GetName(typeof(EffectType), effect), typeof(Sprite)) as Sprite;
        //}

        /// <summary>
        /// 获取手牌的列表
        /// </summary>
        /// <param name="CardEntities">手牌的Entity</param>
        /// <returns></returns>
        public static List<Card> GetData(this List<CardEntity> CardEntities)
        {
            List<Card> Cards = new List<Card>();
            foreach (CardEntity CardEntity in CardEntities)
            {
                Cards.Add(CardEntity.data);
            }

            return Cards;
        }

        public static List<T> Clone<T>(this List<T> ts) where T : ICloneable
        {
            if (ts is null)
            {
                return null;
            }
            List<T> newList = new List<T>();
            foreach (T item in ts)
            {
                newList.Add((T)item.Clone());
            }
            return newList;
        }


        public static List<T> ShallowClone<T>(this List<T> ts)
        {
            if (ts is null)
            {
                return null;
            }
            List<T> newList = new List<T>();
            foreach (T item in ts)
            {
                newList.Add(item);
            }
            return newList;
        }

        public static IEnumerable<T> Clone<T>(this IEnumerable<T> ts) where T : ICloneable
        {
            List<T> newList = new List<T>();
            foreach (T item in ts)
            {
                newList.Add((T)item.Clone());
            }
            return newList;
        }

        public static DataList<T> ToDataList<T>(this IEnumerable<T> ts) where T : class, INameable
        {
            return new DataList<T>(ts);
        }
        public static List<UUID> ToUUIDList<T>(this List<T> ts) where T : class, IUUIDLabeled
        {
            List<UUID> uUIDs = new List<UUID>();
            foreach (T item in ts)
            {
                uUIDs.Add(item.UUID);
            }
            return uUIDs;
        }


        public static void Switch<T>(this T item1, T item2)
        {
            ref T var1 = ref item1;
            ref T var2 = ref item2;

            T tmp = var1;
            var1 = var2;
            var2 = tmp;
        }

        public static List<T1> Finds<T1, T2>(this IEnumerable<T1> list, T2[] findList) where T1 : INameable where T2 : INameable
        {
            List<T1> find = new List<T1>();
            foreach (T2 item in findList)
            {
                foreach (T1 listItem in list)
                {
                    if (listItem.Name == item.Name)
                    {
                        find.Add(listItem);
                    }
                }
            }
            return find;
        }

        public static T Parse<T>(this string str, bool ignoreCase = true) where T : struct
        {
            if (str is null)
            {
                return default;
            }

            try
            {
                return (T)Enum.Parse(typeof(T), str, ignoreCase);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static StatusList ToStatList(this IEnumerable<Status> stats)
        {
            return new StatusList(stats);
        }


        public static string Color(this string message, Career career)
        {
            string colorName;
            switch (career)
            {
                case Career.scholar:
                    colorName = "blue";
                    break;
                case Career.politician:
                    colorName = "red";
                    break;
                case Career.general:
                    colorName = "yellow";
                    break;
                case Career.merchant:
                    colorName = "green";
                    break;
                default:
                    colorName = "white";
                    break;
            }
            return message.Color(colorName);
        }
        public static string Color(this string message, string colorName)
        {
            return "<color=" + colorName + ">" + message + "</color>";
        }
        public static string Color(this string message, int color)
        {
            return "<color=" + Convert.ToString(color, 16) + ">" + message + "</color>";
        }
        public static string AutoColor(this string message)
        {
            int lastIndex = 0;
            List<string> section = new List<string>();
            List<int> numbers = new List<int>();
            for (int i = 0; i < message.Length; i++)
            {
                char cha = message[i];
                if (int.TryParse(cha.ToString(), out int integer))
                {
                    int number = integer;
                    string before = message.Remove(i);
                    section.Add(before);
                    lastIndex = i;
                    i++;
                    for (; i < message.Length; i++)
                    {
                        if (int.TryParse(message[i].ToString(), out integer))
                        {
                            number = (number * 10) + integer;
                            lastIndex = i;
                        }
                        else
                        {
                            numbers.Add(number);
                        }
                    }

                }
            }
            string remaining = message.Remove(0, lastIndex + 1);
            string ret = string.Empty;
            for (int i = 0; i < section.Count; i++)
            {
                ret += section[i] + numbers[i].ToString().Color("yellow");
            }
            return ret + remaining;
        }

        public static Color GetColor(this Career career)
        {
            switch (career)
            {
                case Career.scholar:
                    return UnityEngine.Color.blue;
                case Career.politician:
                    return UnityEngine.Color.red;
                case Career.general:
                    return UnityEngine.Color.yellow;
                case Career.merchant:
                    return UnityEngine.Color.green;
                default:
                    return UnityEngine.Color.white;
            }
        }
        public static Color GetColor(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.common:
                    return UnityEngine.Color.white;
                case Rarity.rare:
                    return UnityEngine.Color.green;
                case Rarity.epic:
                    return UnityEngine.Color.blue;
                case Rarity.legendary:
                    return UnityEngine.Color.yellow;
                default:
                    return UnityEngine.Color.red;
            }
        }

        public static T[] GetAllValue<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)) as T[];

        }

        public static Vector3 ToVector3(this string sVector)
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }

        public static bool IsVector2(this string sVector)
        {
            try
            {
                sVector.ToVector2();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Vector2 ToVector2(this string sVector)
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector2 result = new Vector2(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]));

            return result;
        }
        public static Vector3Int ToVector3Int(this string sVector)
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector3Int result = new Vector3Int(
                int.Parse(sArray[0]),
                int.Parse(sArray[1]),
                int.Parse(sArray[2]));

            return result;
        }

        public static bool IsVector3(this string sVector)
        {
            try
            {
                sVector.ToVector3();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Vector2Int ToVector2Int(this string sVector)
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector2Int result = new Vector2Int(
                int.Parse(sArray[0]),
                int.Parse(sArray[1]));

            return result;
        }
    }

    public static class Extensions2
    {
        public static List<T> Clone<T>(this List<T> ts) where T : struct
        {
            if (ts is null)
            {
                return null;
            }
            List<T> newList = new List<T>();
            foreach (T item in ts)
            {
                T a = item;
                newList.Add(a);
            }
            return newList;
        }
    }
}