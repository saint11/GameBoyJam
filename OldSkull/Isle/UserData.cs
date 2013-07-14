using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle
{
    public class UserData
    {
        private static List<ItemStats> ItemsColected = new List<ItemStats>();

        public struct ItemStats
        {
            public ItemStats(string Id, string CurrentLevel, Drop Drop, bool Valid) 
            {
                this.Id = Id;
                this.Drop = Drop;
                this.Valid = Valid;
                this.CurrentLevel = CurrentLevel;
            }

            public string Id;
            public string CurrentLevel;
            public Drop Drop;
            public bool Valid;
        }

        public static void AffectItem(string ItemId, string CurrentLevel, Drop Drop)
        {
            if (ItemId == "") return;
            if (ItemsColected == null)
            {
                ItemsColected = new List<ItemStats>();
            }

            bool exists = false;
            for (int i = 0; i < ItemsColected.Count; i++)
            {
                //Modify an existing item vallue
                ItemStats item = ItemsColected[i];
                if (item.Id == ItemId)
                {
                    ItemsColected[i] = new ItemStats(ItemId, CurrentLevel, Drop, true);
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                //Create a new Item;
                ItemStats item = new ItemStats(ItemId, CurrentLevel, Drop, true);
                ItemsColected.Add(item);
            }
        }

        public static ItemStats GetItemStats(string id)
        {
            ItemStats item = new ItemStats("", "", null, false);
            
            if (ItemsColected==null) return item;

            for (int i = 0; i < ItemsColected.Count; i++)
            {
                ItemStats itemCompare = ItemsColected[i];
                if (itemCompare.Id == id)
                {
                    return itemCompare;
                }
            }

            return item;
        }


        internal static List<Drop> GetItemsHere(string isleLevel)
        {
            List<Drop> ret = new List<Drop>();

            foreach (ItemStats item in ItemsColected)
            {
                if (item.CurrentLevel == isleLevel) ret.Add(item.Drop);
            }

            return ret;
        }
    }

}
