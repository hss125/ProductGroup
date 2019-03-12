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
using NPOI.XSSF.UserModel;
using System.Drawing;
using ProductGroup.Models.EF;

namespace Utility
{
    public static class DataImport 
    {
        public static List<Product> ImpotrExcel(string path)
        {
            IWorkbook workbook = null;  //新建IWorkbook对象  
            string fileName = @"D:\Excel\导入\import.xls";
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本  
            {
                return new List<Product>();
                workbook = new XSSFWorkbook(fileStream);  //xlsx数据读入workbook  
            }
            else if (fileName.IndexOf(".xls") > 0) // 2003版本  
            {
                workbook = new HSSFWorkbook(fileStream);  //xls数据读入workbook  
            }
            ISheet sheet = workbook.GetSheetAt(0);  //获取第一个工作表  
            IRow row;// = sheet.GetRow(0);            //新建当前工作表行数据  
            Dictionary<int, string> imglist = new Dictionary<int, string>();
            imglist=ExcelToImage(fileName, path+ "Upload/");
            //return new List<Product>();
            List<Product> pros = new List<Product>();
            for (int i = 1; i < sheet.LastRowNum; i++)  //对工作表每一行  
            {
                row = sheet.GetRow(i);   //row读入第i行数据 
                if (row.GetCell(0) == null)
                {
                    break;
                }
                string imgurl = "";
                try
                {
                    imglist.TryGetValue(i, out imgurl);
                    string word = row.GetCell(4).ToString();
                    string[] keys = word.Split('#');
                    var totalcount = 0;
                    foreach (var k in keys)
                    {
                        var kw = k.Split('=');
                        totalcount += Convert.ToInt32(kw[1]);
                        pros.Add(new Product
                        {
                            Shop = row.GetCell(0).ToString().Replace("\n", ""),
                            ShopName = row.GetCell(1)?.ToString().Replace("\n", ""),
                            Price = Convert.ToDecimal(row.GetCell(2)?.ToString()),
                            PriceMark = row.GetCell(3)?.ToString(),
                            Screen = row.GetCell(5)?.ToString(),
                            ImgUrl = imgurl,
                            KeyWord = kw[0],
                            OrderCount = Convert.ToInt32(kw[1]),
                            ShopKeeper = row.GetCell(8)?.ToString().Replace("\n", ""),
                            ServiceCost = Convert.ToDecimal(string.IsNullOrEmpty(row.GetCell(9)?.ToString())?"0": row.GetCell(9)?.ToString())
                        });
                    }
                    if (totalcount != Convert.ToInt32(row.GetCell(7).ToString()))
                    {
                        pros = new List<Product>();
                        pros.Add(new Product { Shop = "-1", ShopName = row.GetCell(1)?.ToString() + "[行号:" + (i + 1) + "][单数不匹配]" });
                        return pros;
                    }
                }
                catch (Exception ex)
                {
                    pros = new List<Product>();
                    pros.Add(new Product { Shop = "-1", ShopName = row.GetCell(1)?.ToString() + "[行号:" + (i + 1) + "]" });
                    return pros;
                }
                
            }
            //Console.ReadLine();
            fileStream.Close();
            workbook.Close();
            return pros;
        }      
        public static Dictionary<int, string> ExcelToImage(string filepath, string savepath)
        {
            Dictionary<int, string> pictarr = new Dictionary<int, string>();
            try
            {
                if (filepath.IndexOf(".xls") > 0) // 2007版本  
                {
                    IWorkbook workbook = null;  //新建IWorkbook对象  
                    string fileName = filepath;
                    FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    workbook = new HSSFWorkbook(fileStream);
                    var dr = workbook.GetSheetAt(0).DrawingPatriarch;
                    HSSFPatriarch pat = (HSSFPatriarch)dr;
                    var shape = pat.Children;
                    //List<HSSFPicture> pictarr = new List<HSSFPicture>();
                    var j = 0;
                    foreach (var s in shape)
                    {
                        string patType = s.GetType().ToString();
                        switch (patType)
                        {
                            case "NPOI.HSSF.UserModel.HSSFSimpleShape":
                                {
                                    var simpleshape = (HSSFSimpleShape)s;

                                    /*Save Shape*/

                                    break;
                                }
                            case "NPOI.HSSF.UserModel.HSSFPicture":
                                {
                                    j++;
                                    var pic = (HSSFPicture)s;
                                    byte[] data = pic.PictureData.Data;
                                    //pictarr.Add(pic);
                                    string ext = pic.PictureData.SuggestFileExtension();//获取扩展名
                                    string path = string.Empty;
                                    var row1 = pic.ClientAnchor.Row1;
                                    if (pic.ClientAnchor.Dy1 > 100)
                                    {
                                        row1 = pic.ClientAnchor.Row1 + 1;
                                    }
                                    string imgurl = "Product/" + DateTime.Now.ToString("yyyyMMddHHmmssfff")+j;
                                    if (ext.Equals("jpg") || ext.Equals("jpeg"))
                                    {
                                        Image jpg = Image.FromStream(new MemoryStream(pic.PictureData.Data));//从pic.Data数据流创建图片
                                        imgurl += ".jpg";
                                        path = Path.Combine(savepath, imgurl);
                                        jpg.Save(path);//保存
                                    }
                                    else if (ext.Equals("png"))
                                    {
                                        Image png = Image.FromStream(new MemoryStream(pic.PictureData.Data));
                                        imgurl += ".png";
                                        path = Path.Combine(savepath, imgurl);
                                        png.Save(path);
                                    }
                                    try
                                    {
                                        pictarr.Add(row1, imgurl);
                                    }
                                    catch (Exception ex2)
                                    {
                                        
                                    }
                                    break;
                                }
                            default: break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                

            }
            return pictarr;
        }

        public static List<CollectHistory> ImportCollectHistory(string path)
        {
            IWorkbook workbook = null;  //新建IWorkbook对象  
            string fileName = @"D:\Excel\导入\history.xls";
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本  
            {
                return new List<CollectHistory>();
                workbook = new XSSFWorkbook(fileStream);  //xlsx数据读入workbook  
            }
            else if (fileName.IndexOf(".xls") > 0) // 2003版本  
            {
                workbook = new HSSFWorkbook(fileStream);  //xls数据读入workbook  
            }
            ISheet sheet = workbook.GetSheetAt(0);  //获取第一个工作表  
            IRow row;// = sheet.GetRow(0);            //新建当前工作表行数据 
            List<CollectHistory> chs = new List<CollectHistory>();
            for (int i = 1; i < sheet.LastRowNum; i++)  //对工作表每一行  
            {
                row = sheet.GetRow(i);   //row读入第i行数据 
                if (row.GetCell(0) == null)
                {
                    break;
                }
                try
                {
                    chs.Add(new CollectHistory
                    {
                        SubmitDate = DateTime.Parse(row.GetCell(1).ToString()),
                        TaskId= row.GetCell(6).ToString(),
                        MemberName= row.GetCell(7).ToString(),
                        Clerk= row.GetCell(11).ToString(),
                        CityPartner= row.GetCell(10).ToString()
                    });
                }
                catch (Exception ex)
                {
                    chs = new List<CollectHistory>();
                    return chs;
                }

            }
            fileStream.Close();
            workbook.Close();
            return chs;
        }

    }
}
