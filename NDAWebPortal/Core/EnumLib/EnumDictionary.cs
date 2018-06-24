using System;
using System.Collections.Generic;
using System.Linq;

namespace NDAWebPortal.Core.EnumLib
{
    public static class EnumDictionary
    {
        public static List<EnumList> GetList<T>() where T : struct
        {
            return (from int e in Enum.GetValues(typeof (T))
                select new EnumList
                {
                    ItemId = e, ItemName = ((Enum) Enum.Parse(typeof (T), e.ToString())).DisplayName()
                }).ToList();
        }
    }

    public class EnumList
    {
        public int ItemId { get;  set; }
        public string ItemName { get; set; }
    }
}
