using ProductGroup.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class GRandom
    {
        private static GRandom self = new GRandom();
        /// <summary>
        /// 生成指定范围的随机数
        /// </summary>
        /// <param name="max">最大值(不包含最大值)</param>
        /// <param name="min">最小值(包含最小值)</param>
        /// <returns></returns>
        public static int Next(int max, int min = 0)
        {
            return self.GetNext(min, max);
        }
        private Random rdm = new Random((int)DateTime.Now.Ticks);
        private int GetNext(int min, int max)
        {
            return rdm.Next(min, max);
        }

        public int forcount = 0;
        public List<Product> getArrayItems(List<Product> arr, int num)
        {
            var temp_array = arr;
            List<Product> return_array = new List<Product>();
            var i = 0;
            while (i < num&&forcount<15000)
            {
                forcount++;
                var arrIndex = Next(temp_array.Count);
                var s = return_array.Where(w => w.ShopName == temp_array[arrIndex].ShopName).ToList();
                if (!string.IsNullOrEmpty(temp_array[arrIndex].ShopKeeper))
                {
                    s = return_array.Where(w => w.ShopName == temp_array[arrIndex].ShopName||w.ShopKeeper== temp_array[arrIndex].ShopKeeper).ToList();
                }
                if (s.Count < 1)
                {
                    return_array.Add(temp_array[arrIndex]);
                    temp_array.Remove(temp_array[arrIndex]);
                    i++;
                }
            }
            return return_array;
        }
        public int getcount(List<Product> arr)
        {
            var s = arr.Where((x, i) => arr.FindIndex(z => z.Shop == x.Shop) == i).ToList();
            return s.Count;
        }
        public void Group(int count)
        {
            ProductGroupEntities pg = new ProductGroupEntities();
            DateTime dtToday = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));//今天
            DateTime dtNexDay = Convert.ToDateTime(DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"));//明天
            pg.Groups.RemoveRange(pg.Groups.Where(w => w.Date>dtToday&&w.Date<dtNexDay));
            pg.SaveChanges();
            var list = new List<Product>();
            var alllist = new List<Product>();
            var toady = DateTime.Now.AddDays(1);
            list = pg.Products.Where(w => w.CreateDate.Value.Month == toady.Month && w.CreateDate.Value.Day == toady.Day).ToList();
            foreach (var a in list)
            {
                for (int i = 0; i < a.OrderCount; i++)
                {
                    alllist.Add(a);
                }
            }
            List<List<Product>> zh=new List<List<Product>>();
            var task = 0;
            List<Group> te = new List<Group>();
            while (getcount(alllist) > count-1 && forcount < 15000)
            {
                forcount++;
                var arritem = getArrayItems(alllist, count);
                zh.Add(arritem);
                string proid = "";
                string name = "";
                foreach (var p in arritem)
                {
                    proid += p.Id+"|";
                }
                task++;
                name += ""+ DateTime.Now.AddDays(1).ToString("MMdd")+"Q";
                var l=4 - task.ToString().Length;
                for (var p = 0; p < l; p++)
                {
                    name += "0";
                }
                name += task ;
                pg.Groups.Add(new Group { TaskID=name,ProId=proid,Date=DateTime.Now.AddDays(1)});
                te.Add(new Group { TaskID = name, ProId = proid, Date = DateTime.Now.AddDays(1) });
            }
            pg.SaveChanges();
            foreach (var a in alllist)
            {
                var gro = pg.Groups.FirstOrDefault(w =>w.Date>dtToday&&w.Date<dtNexDay &&w.ProId == a.Id.ToString());
                if (gro!=null)
                {
                    gro.SurplusCount += 1;
                }
                else {
                    pg.Groups.Add(new Group { TaskID = "0", ProId = a.Id.ToString(), SurplusCount = 1, Date = DateTime.Now.AddDays(1) });
                }
                pg.SaveChanges();

            }
        }

    }
}
