using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using System.Web;
using ProductGroup.Models;
using ProductGroup.Models.EF;

namespace Utility
{
    public class ExportPro
    {
        public void exportToayProData(string mappath, List<IGrouping<string,Product>> gi)
        {
            var name = DateTime.Now.ToString("yyyyMMdd");
            //创建工作薄
            var workbook = new HSSFWorkbook();
            //创建表
            var table = workbook.CreateSheet(name);
            // 添加表头
            var row1 = table.CreateRow(0);
            string[] head = {"日期","店主","店铺编号", "店铺名称","排单数", "客单价(元)", "服务费"};
            for (int j = 0; j < head.Count(); j++)
            {
                var cell = row1.CreateCell(j);
                setCellStyle(workbook, cell);
                cell.SetCellValue(head[j]);
            }
            table.SetColumnWidth(0, 3000);
            table.SetColumnWidth(1, 4000);
            table.SetColumnWidth(2, 2200);
            table.SetColumnWidth(3, 8000);
            table.SetColumnWidth(4, 3000);
            table.SetColumnWidth(5, 3000);
            table.SetColumnWidth(6, 3000);
            for (var i = 1; i < gi.Count()+1; i++)
            {
                var row = table.CreateRow(i);
                var pro = gi[i - 1].ToList();
                var count = 0;
                for(var j=0;j<pro.Count;j++)
                {
                    count+= (int)pro[j].OrderCount;
                }
                var cell = row.CreateCell(0);
                cell.SetCellValue(pro[0].CreateDate?.ToString("yyyy-MM-dd"));
                var cell2 = row.CreateCell(1);
                cell2.SetCellValue(pro[0].ShopKeeper);
                var cell3 = row.CreateCell(2);
                cell3.SetCellValue(pro[0].Shop);
                var cell4 = row.CreateCell(3);
                cell4.SetCellValue(pro[0].ShopName);
                var cell5 = row.CreateCell(4);
                cell5.SetCellValue(count);
                var cell6 = row.CreateCell(5);
                cell6.SetCellValue(pro[0].Price.ToString());
                var cell7 = row.CreateCell(6);
                cell7.SetCellValue(pro[0].ServiceCost.ToString());
            }
            // 写入 
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            workbook = null;
            var zh = @"D:\Excel\今日源数据\";
            CreatFolder(zh);
            using (FileStream fs = new FileStream(zh + name + ".xls", FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            ms.Close();
            ms.Dispose();
        }
        public void exportSameProData(string mappath, IGrouping<string, Product> pros)
        {
            var name = DateTime.Now.ToString("yyyyMM");
            //创建工作薄
            var workbook = new HSSFWorkbook();
            //创建表
            var table = workbook.CreateSheet(name);
            // 添加表头
            var row1 = table.CreateRow(0);
            string[] head = { "日期", "店主", "店铺编号", "店铺名称", "排单数", "客单价(元)", "服务费" };
            for (int j = 0; j < head.Count(); j++)
            {
                var cell = row1.CreateCell(j);
                setCellStyle(workbook, cell);
                cell.SetCellValue(head[j]);
            }
            table.SetColumnWidth(0, 3000);
            table.SetColumnWidth(1, 4000);
            table.SetColumnWidth(2, 2200);
            table.SetColumnWidth(3, 8000);
            table.SetColumnWidth(4, 3000);
            table.SetColumnWidth(5, 3000);
            table.SetColumnWidth(6, 3000);
            var gi2 = pros.GroupBy(g => g.CreateDate.Value.Year+ g.CreateDate.Value.Month+ g.CreateDate.Value.Day).OrderBy(o=>o.FirstOrDefault().CreateDate).ToList();
            name = gi2[0].ToList()[0].ShopKeeper + name;
            int rownum = 0;
            for (var x = 0; x < gi2.Count(); x++)
            {
                var gi = gi2[x].GroupBy(g => g.Shop).ToList();
                for (var i = 1; i < gi.Count() + 1; i++)
                {
                    rownum++;
                    var row = table.CreateRow(rownum);
                    var pro = gi[i - 1].ToList();
                    var count = 0;
                    for (var j = 0; j < pro.Count; j++)
                    {
                        count += (int)pro[j].OrderCount;
                    }
                    var cell = row.CreateCell(0);
                    cell.SetCellValue(pro[0].CreateDate?.ToString("yyyy-MM-dd"));
                    var cell2 = row.CreateCell(1);
                    cell2.SetCellValue(pro[0].ShopKeeper);
                    var cell3 = row.CreateCell(2);
                    cell3.SetCellValue(pro[0].Shop);
                    var cell4 = row.CreateCell(3);
                    cell4.SetCellValue(pro[0].ShopName);
                    var cell5 = row.CreateCell(4);
                    cell5.SetCellValue(count);
                    var cell6 = row.CreateCell(5);
                    cell6.SetCellValue(pro[0].Price.ToString());
                    var cell7 = row.CreateCell(6);
                    cell7.SetCellValue(pro[0].ServiceCost.ToString());
                }
            }
                
            // 写入 
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            workbook = null;
            var zh = @"D:\Excel\当月源数据\";
            CreatFolder(zh);
            using (FileStream fs = new FileStream(zh + name + ".xls", FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            ms.Close();
            ms.Dispose();
        }
        /// <summary>
        /// 设置单元格样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="cell"></param>
        private void setCellStyle(HSSFWorkbook workbook, ICell cell)
        {
            HSSFCellStyle fCellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ffont = (HSSFFont)workbook.CreateFont();
            //ffont.FontHeight = 80 * 20;
            ffont.IsBold = true;
            //ffont.FontName = "宋体";
            ffont.FontHeightInPoints=14;
            
            fCellStyle.SetFont(ffont);

            fCellStyle.VerticalAlignment = VerticalAlignment.Center;//垂直对齐
            fCellStyle.Alignment = HorizontalAlignment.Center;//水平对齐
            fCellStyle.WrapText = true;
            cell.CellStyle = fCellStyle;
        }
        public void CreatFolder(string url)
        {
            if (!Directory.Exists(url))
            {
                Directory.CreateDirectory(url);
            }
        }
    }
}
