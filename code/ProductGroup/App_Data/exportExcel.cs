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
    public class ExportExcel 
    {
        public void exportExcel(string mappath, GroupItem gi)
        {
            var name = gi.group.TaskID;
            var shopall = "_";
            decimal pri = 0;
            foreach (var p in gi.proList)
            {
                shopall += p.Shop;
                pri += (decimal)p.Price;
            }
            name += shopall+"_"+pri.ToString();
            //name += "_" + DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            //创建工作薄
            var workbook = new HSSFWorkbook();
            //创建表
            var table = workbook.CreateSheet(name);
            // 添加表头
            var row1 = table.CreateRow(0);
            string[] head = {"店铺","任务编号","店铺名称", "主图","客单价(元)", "客单价备注", "搜索关键词", "筛选条件" };
            for (int j = 0; j < head.Count(); j++)
            {
                var cell = row1.CreateCell(j);
                setCellStyle(workbook, cell);
                cell.SetCellValue(head[j]);
            }
            table.SetColumnWidth(0, 1500);
            table.SetColumnWidth(1, 2200);
            table.SetColumnWidth(2, 5000);
            table.SetColumnWidth(3, 8000);
            table.SetColumnWidth(4, 5000);
            table.SetColumnWidth(5, 9000);
            table.SetColumnWidth(6, 9000);
            table.SetColumnWidth(7, 4000);
            for (var i = 1; i < gi.proList.Count()+1; i++)
            {
                var row = table.CreateRow(i);
                row.Height = 2800;

                var cell = row.CreateCell(0);
                cell.SetCellValue(gi.proList[i-1].Shop);
                var cell2 = row.CreateCell(1);
                cell2.SetCellValue(gi.group.TaskID);
                var cell3 = row.CreateCell(2);
                cell3.SetCellValue(gi.proList[i-1].ShopName);
                var cell4 = row.CreateCell(4);
                cell4.SetCellValue(gi.proList[i-1].Price.ToString());
                var cell5 = row.CreateCell(5);
                cell5.SetCellValue(gi.proList[i-1].PriceMark);
                var cell6 = row.CreateCell(6);
                cell6.SetCellValue(gi.proList[i-1].KeyWord);
                var cell7 = row.CreateCell(7);
                cell7.SetCellValue(gi.proList[i-1].Screen);

                ICell[] cells = { cell,cell2,cell3,cell5,cell7 };
                setCellStyle2(workbook, cells);
                setCellStyle4(workbook, cell4);
                setCellStyle3(workbook, cell6);

                string picurl = "/Upload/"+gi.proList[i-1].ImgUrl;  //图片存储路径   
                AddPieChart(table, workbook, picurl, i, 3,mappath);
            }
            // 写入 
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            workbook = null;
            var zh = @"D:\Excel\分组报表\";
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
        /// <summary>
        /// 设置单元格样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="cell"></param>
        private void setCellStyle2(HSSFWorkbook workbook, ICell[] cells)
        {
            HSSFCellStyle fCellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ffont = (HSSFFont)workbook.CreateFont();
            ffont.FontHeightInPoints = 13;
            ffont.IsBold = true;
            fCellStyle.SetFont(ffont);

            fCellStyle.VerticalAlignment = VerticalAlignment.Center;//垂直对齐
            fCellStyle.Alignment = HorizontalAlignment.Center;//水平对齐
            fCellStyle.WrapText = true;
            foreach (var cell in cells)
            {
                cell.CellStyle = fCellStyle;
            }            
        }
        private void setCellStyle3(HSSFWorkbook workbook, ICell cell)
        {
            HSSFCellStyle fCellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ffont = (HSSFFont)workbook.CreateFont();
            ffont.FontHeightInPoints = 13;
            ffont.Color = HSSFColor.Red.Index;
            ffont.IsBold =true;
            fCellStyle.SetFont(ffont);

            fCellStyle.VerticalAlignment = VerticalAlignment.Center;//垂直对齐
            fCellStyle.Alignment = HorizontalAlignment.Center;//水平对齐
            fCellStyle.WrapText = true;
            cell.CellStyle = fCellStyle;
        }
        private void setCellStyle4(HSSFWorkbook workbook, ICell cell)
        {
            HSSFCellStyle fCellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ffont = (HSSFFont)workbook.CreateFont();
            ffont.FontHeightInPoints = 24;
            ffont.Color= HSSFColor.Red.Index;
            ffont.IsBold = true;
            fCellStyle.SetFont(ffont);

            fCellStyle.VerticalAlignment = VerticalAlignment.Center;//垂直对齐
            fCellStyle.Alignment = HorizontalAlignment.Center;//水平对齐
            fCellStyle.WrapText = true;
            cell.CellStyle = fCellStyle;
        }
        /// 向sheet插入图片
        ///
        ///
        ///
        private void AddPieChart(ISheet sheet, HSSFWorkbook workbook, string fileurl, int row, int col,string mappath)
        {
            try
            {
                if (string.IsNullOrEmpty(fileurl)) { return; }
                string path= mappath+fileurl.Replace("/",@"\");
                string FileName = path;
                if (!File.Exists(FileName))
                {
                    return;
                }
                byte[] bytes = File.ReadAllBytes(FileName);

                if (!string.IsNullOrEmpty(FileName))
                {
                    int pictureIdx = workbook.AddPicture(bytes, PictureType.JPEG);
                    HSSFPatriarch patriarch = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
                    HSSFClientAnchor anchor = new HSSFClientAnchor(70, 10, 0, 0, col, row, col + 1, row + 1);
                    HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
                    // pict.Resize();这句话一定不要，这是用图片原始大小来显示
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //信息采集报表
        public void ExcelHand(string mappath, List<ExcelhandModel> GroupItems)
        {
            //创建工作薄
            var workbook = new HSSFWorkbook();
            //创建表
            var table = workbook.CreateSheet(GroupItems[0].ShopName + "_" + DateTime.Now.ToString("yyyyMMdd"));
            // 添加表头
            var row1 = table.CreateRow(0);
            string[] head = { "任务编号", "店铺名称", "淘宝账号","时间" };
            for (int j = 0; j < head.Count(); j++)
            {
                var cell = row1.CreateCell(j);
                setCellStyle(workbook, cell);
                cell.SetCellValue(head[j]);
            }
            table.SetColumnWidth(0, 3000);
            table.SetColumnWidth(1, 4000);
            table.SetColumnWidth(2, 9000);
            table.SetColumnWidth(3, 4000);
            var pindex = 1;
            var GroupItems1 = GroupItems.Where(w => w.group.WWInputDate != null);
            var GroupItems2 = GroupItems.Where(w => w.group.WWInputDate == null);
            foreach (var gi in GroupItems1)
            {
                var row = table.CreateRow(pindex);

                var cell = row.CreateCell(0);
                cell.SetCellValue(gi.group.TaskID);
                var cell2 = row.CreateCell(1);
                cell2.SetCellValue(gi.Shop + " " + gi.ShopName);
                var cell3 = row.CreateCell(2);
                cell3.SetCellValue(gi.group.WWId);
                var cell4 = row.CreateCell(3);
                cell4.SetCellValue(gi.group.WWInputDate?.ToString("MM/dd HH:mm"));
                pindex++;

            }
            foreach (var gi in GroupItems2)
            {
                var row = table.CreateRow(pindex);

                var cell = row.CreateCell(0);
                cell.SetCellValue(gi.group.TaskID);
                var cell2 = row.CreateCell(1);
                cell2.SetCellValue(gi.Shop + " " + gi.ShopName);
                var cell3 = row.CreateCell(2);
                cell3.SetCellValue(gi.group.WWId);
                var cell4 = row.CreateCell(3);
                cell4.SetCellValue(gi.group.WWInputDate?.ToString("MM/dd HH:mm"));
                pindex++;

            }
            // 写入 
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            workbook = null;
            var xxcj = @"D:\Excel\信息采集\";
            CreatFolder(xxcj);
            using (FileStream fs = new FileStream(xxcj + GroupItems[0].Shop + "_" + GroupItems[0].ShopName.Replace("\n", "") + "_"+DateTime.Now.ToString("yyyyMMdd") + ".xls", FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            ms.Close();
            ms.Dispose();
        }
        public void ExcelTask(string mappath, List<GroupItem> GroupItems)
        {
            //创建工作薄
            var workbook = new HSSFWorkbook();
            //创建表
            var table = workbook.CreateSheet("任务明细");
            // 添加表头
            var row1 = table.CreateRow(0);
            string[] head = { "任务编号", "店铺1", "店铺2", "店铺3", "店铺4" ,"总金额"};
            for (int j = 0; j < head.Count(); j++)
            {
                var cell = row1.CreateCell(j);
                setCellStyle(workbook, cell);
                cell.SetCellValue(head[j]);
            }
            table.SetColumnWidth(0, 3000);
            table.SetColumnWidth(1, 4000);
            table.SetColumnWidth(2, 4000);
            table.SetColumnWidth(3, 4000);
            table.SetColumnWidth(4, 4000);
            table.SetColumnWidth(5, 4000);
            var pindex = 1;
            foreach (var gi in GroupItems)
            {
                var row = table.CreateRow(pindex);

                var cell = row.CreateCell(0);
                cell.SetCellValue(gi.group.TaskID);
                var cell2 = row.CreateCell(1);
                cell2.SetCellValue(gi.proList[0].Shop);
                var cell3 = row.CreateCell(2);
                cell3.SetCellValue(gi.proList[1].Shop);
                if (gi.proList.Count > 2)
                {
                    var cell4 = row.CreateCell(3);
                    cell4.SetCellValue(gi.proList[2].Shop);
                }
                
                if (gi.proList.Count>3)
                {
                    var cell5 = row.CreateCell(4);
                    cell5.SetCellValue(gi.proList[3].Shop);
                }
               
                var cell6 = row.CreateCell(5);
                decimal pri = 0;
                foreach (var p in gi.proList)
                {
                    pri += (decimal)p.Price;
                }
                cell6.SetCellValue((double)pri);
                pindex++;
            }
            // 写入 
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            workbook = null;
            var xxcj = @"D:\Excel\任务明细\";
            CreatFolder(xxcj);
            using (FileStream fs = new FileStream(xxcj + "任务明细.xls", FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            ms.Close();
            ms.Dispose();
        }
        public void CreatFolder(string url)
        {
            if (!Directory.Exists(url))
            {
                Directory.CreateDirectory(url);
            }
        }
        public void exportSource(string mappath, List<Product> proarr)
        {
            var name = "源数据_"+DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            //name += "_" + DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            //创建工作薄
            var workbook = new HSSFWorkbook();
            //创建表
            var table = workbook.CreateSheet(name);
            // 添加表头
            var row1 = table.CreateRow(0);
            string[] head = { "店铺", "店铺名称", "单数", "客单价(元)", "客单价备注", "关键词", "筛选条件", "主图" };
            for (int j = 0; j < head.Count(); j++)
            {
                var cell = row1.CreateCell(j);
                setCellStyle(workbook, cell);
                cell.SetCellValue(head[j]);
            }
            table.SetColumnWidth(0, 3000);
            table.SetColumnWidth(1, 9000);
            table.SetColumnWidth(2, 3000);
            table.SetColumnWidth(3, 3000);
            table.SetColumnWidth(4, 8000);
            table.SetColumnWidth(5, 9000);
            table.SetColumnWidth(6, 9000);
            table.SetColumnWidth(7, 9000);
            for (var i = 0; i < proarr.Count; i++)
            {
                var row = table.CreateRow(i+1);
                row.Height = 2800;

                var cell = row.CreateCell(0);
                cell.SetCellValue(proarr[i].Shop);
                var cell2 = row.CreateCell(1);
                cell2.SetCellValue(proarr[i].ShopName);
                var cell3 = row.CreateCell(2);
                cell3.SetCellValue(proarr[i].OrderCount.ToString());
                var cell4 = row.CreateCell(3);
                cell4.SetCellValue((double)proarr[i].Price);
                var cell5 = row.CreateCell(4);
                cell5.SetCellValue(proarr[i].PriceMark);
                var cell6 = row.CreateCell(5);
                cell6.SetCellValue(proarr[i].KeyWord);
                var cell7 = row.CreateCell(6);
                cell7.SetCellValue(proarr[i].Screen);

                ICell[] cells = { cell, cell2, cell3, cell4, cell5, cell6};
                setCellStyle2(workbook, cells);
                setCellStyle3(workbook, cell7);

                string picurl = "/Upload" + proarr[i].ImgUrl;  //图片存储路径   
                AddPieChart(table, workbook, picurl, i+1, 7, mappath);
            }
            // 写入 
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            workbook = null;
            var zh = @"D:\Excel\源数据报表\";
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
        //导出单个产品的excel
        //public void exportOneExcel(string mappath, Product pro)
        //{
        //    var name = DateTime.Now.AddDays(1).ToString("MMdd")+"_"+pro.Shop + "_" + pro.KeyWord;
        //   // name += pro.Price + "_" + pri.ToString();
        //    //name += "_" + DateTime.Now.AddDays(1).ToString("yyyyMMdd");
        //    //创建工作薄
        //    var workbook = new HSSFWorkbook();
        //    //创建表
        //    var table = workbook.CreateSheet(name);
        //    // 添加表头
        //    var row1 = table.CreateRow(0);
        //    string[] head = { "店铺", "任务编号", "店铺名称", "主图", "客单价(元)", "客单价备注", "搜索关键词", "筛选条件" };
        //    for (int j = 0; j < head.Count(); j++)
        //    {
        //        var cell1 = row1.CreateCell(j);
        //        setCellStyle(workbook, cell1);
        //        cell1.SetCellValue(head[j]);
        //    }
        //    table.SetColumnWidth(0, 1500);
        //    table.SetColumnWidth(1, 2200);
        //    table.SetColumnWidth(2, 5000);
        //    table.SetColumnWidth(3, 8000);
        //    table.SetColumnWidth(4, 5000);
        //    table.SetColumnWidth(5, 9000);
        //    table.SetColumnWidth(6, 9000);
        //    table.SetColumnWidth(7, 4000);
        //    var row = table.CreateRow(1);
        //    row.Height = 2800;

        //    var cell = row.CreateCell(0);
        //    cell.SetCellValue(pro.Shop);
        //    var cell2 = row.CreateCell(1);
        //    cell2.SetCellValue("a");
        //    var cell3 = row.CreateCell(2);
        //    cell3.SetCellValue(pro.ShopName);
        //    var cell4 = row.CreateCell(4);
        //    cell4.SetCellValue(pro.Price.ToString());
        //    var cell5 = row.CreateCell(5);
        //    cell5.SetCellValue(pro.PriceMark);
        //    var cell6 = row.CreateCell(6);
        //    cell6.SetCellValue(pro.KeyWord);
        //    var cell7 = row.CreateCell(7);
        //    cell7.SetCellValue(pro.Screen);

        //    ICell[] cells = { cell, cell2, cell3, cell5, cell7 };
        //    setCellStyle2(workbook, cells);
        //    setCellStyle4(workbook, cell4);
        //    setCellStyle3(workbook, cell6);

        //    string picurl = "/Upload/" + pro.ImgUrl;  //图片存储路径   
        //    AddPieChart(table, workbook, picurl, 1, 3, mappath);

        //    // 写入 
        //    MemoryStream ms = new MemoryStream();
        //    workbook.Write(ms);
        //    workbook = null;
        //    var zh = @"D:\Excel\分组报表\";
        //    CreatFolder(zh);
        //    for (var i = 1; i < pro.OrderCount+1; i++)
        //    {
        //        using (FileStream fs = new FileStream(zh + name+"_"+i + ".xls", FileMode.Create, FileAccess.Write))
        //        {
        //            byte[] data = ms.ToArray();
        //            fs.Write(data, 0, data.Length);
        //            fs.Flush();
        //        }
        //    }            
        //    ms.Close();
        //    ms.Dispose();
        //}
    }
}
