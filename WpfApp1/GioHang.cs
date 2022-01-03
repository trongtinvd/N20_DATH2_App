using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1
{
    public class GioHang
    {
        public GioHang()
        {
        }

        private Dictionary<int, GioHang.Item> dict = new Dictionary<int, Item>();

        public List<GioHang.Item> Items => dict.Values.ToList();

        internal void AddItem(SanPham sp, int soLuong)
        {
            if (dict.ContainsKey(sp.ID_SP))
            {
                dict.Remove(sp.ID_SP);
            }

            dict.Add(sp.ID_SP, new Item()
            {
                SP = sp,
                SoLuong = soLuong
            });
        }

        public class Item
        {
            public SanPham SP { get; internal set; }
            public int SoLuong { get; internal set; }
            public decimal TongGia => SP.Gia * SoLuong;
        }

        internal void Remove(SanPham sP)
        {
            if (dict.ContainsKey(sP.ID_SP))
            {
                dict.Remove(sP.ID_SP);
            }
        }

        internal void Clear()
        {
            dict.Clear();
        }
    }

}