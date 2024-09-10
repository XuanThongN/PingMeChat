using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OfficeOpenXml;
using PingMeChat.CMS.Application.Common.ExcelModel.Exports;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;

namespace PingMeChat.CMS.Application.Common.Helpers
{
    public static class ExcelHandleHelper
    {
        /// <summary>
        /// chuyển đổi 1 danh sách đổi tượng thành 1 danh sách dạng BaseExcelExportModel
        /// điều này giúp ta không cần biết dữ liệu liệu ban đầu truyền vào là 1 danh sách đối tượng cụ thể nào
        /// và khi chuyển đổi nó thành List<BaseExcelExportModel>
        /// chúng ta có thể dễ dàng xử lý dữ liệu để trả về cho người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<BaseExcelExportModel> ConvertToListExcelExportModel(Object model)
        {
            try
            {
                Type type = model.GetType();
                PropertyInfo[] properties = type.GetProperties();

                List<BaseExcelExportModel> list = new List<BaseExcelExportModel>();
                foreach (PropertyInfo property in properties)
                {
                    var excelExportModel = new BaseExcelExportModel();

                    var displayAttribute = property.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;

                    excelExportModel.HeaderName = displayAttribute != null ? displayAttribute.Name : property.Name;
                    excelExportModel.VariableName = property.Name;
                    excelExportModel.VariableValue = property.GetValue(model, null);

                    list.Add(excelExportModel);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T ReadFromExcel<T>(IFormFile file, Object model, bool hasHeader = true, int startColumn = 1, int startRow = 1)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPack = new ExcelPackage())
            {
                //Load excel stream
                using (var stream = file.OpenReadStream())
                {
                    excelPack.Load(stream);
                }

                //Lets Deal with first worksheet.(You may iterate here if dealing with multiple sheets)
                var ws = excelPack.Workbook.Worksheets[0];

                //Get all details as DataTable -because Datatable make life easy :)
                DataTable excelasTable = new DataTable();
                // startRow, startColumn : ô bắt đầu
                // startRow, ws.Dimension.End.Column: ô kết thúc
                // truy cập vào ô 1 hàng 1 đến ô cuối cùng của hàng 1
                // đoạn mã này đang gán giá trị của phần header thành dạng key và phối hợp với đoạn for bên dưới để biến đối nó thành 1 dạng json hoàn chỉnh
                // vd: { "họ và tên" : "dương văn huy"}
                Type type = model.GetType();
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    excelasTable.Columns.Add(property.Name);
                }
                //    foreach (var firstRowCell in ws.Cells[startRow, startColumn, startRow, ws.Dimension.End.Column])
                //{
                //    //Get colummn details
                //    if (!string.IsNullOrEmpty(firstRowCell.Text))
                //    {
                //        string firstColumn = string.Format("Column {0}", firstRowCell.Start.Column);
                //        excelasTable.Columns.Add(hasHeader ? firstRowCell.Text : firstColumn);
                //    }
                //}
                var startIndexRow = hasHeader ? startRow + 1 : startRow;
                //Get row details
                for (int rowNum = startIndexRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, startColumn, rowNum, excelasTable.Columns.Count];
                    DataRow row = excelasTable.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    // index hàng 
                    row[excelasTable.Columns.Count - 1] = rowNum - startRow + 1;
                    //Get everything as generics and let end user decides on casting to required type

                }
                // Convert DataTable to the specified generic type using JSON serialization
                var data = JsonConvert.SerializeObject(excelasTable);
                var generatedType = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(excelasTable));
                return (T)Convert.ChangeType(generatedType, typeof(T));
            }
        }

        public static T ReadFromExcel<T>(IFormFile file, bool hasHeader = true, int startColumn = 1, int startRow = 1)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPack = new ExcelPackage())
            {
                //Load excel stream
                using (var stream = file.OpenReadStream())
                {
                    excelPack.Load(stream);
                }

                //Lets Deal with first worksheet.(You may iterate here if dealing with multiple sheets)
                var ws = excelPack.Workbook.Worksheets[0];

                //Get all details as DataTable -because Datatable make life easy :)
                DataTable excelasTable = new DataTable();
                // startRow, startColumn : ô bắt đầu
                // startRow, ws.Dimension.End.Column: ô kết thúc
                // truy cập vào ô 1 hàng 1 đến ô cuối cùng của hàng 1
                foreach (var firstRowCell in ws.Cells[startRow, startColumn, startRow, ws.Dimension.End.Column])
                {
                    //Get colummn details
                    if (!string.IsNullOrEmpty(firstRowCell.Text))
                    {
                        string firstColumn = string.Format("Column {0}", firstRowCell.Start.Column);
                        excelasTable.Columns.Add(hasHeader ? firstRowCell.Text : firstColumn);
                    }
                }
                var startIndexRow = hasHeader ? startRow +1 : startRow;
                //Get row details
                for (int rowNum = startIndexRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, startColumn, rowNum, excelasTable.Columns.Count];
                    DataRow row = excelasTable.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    // index hàng 
                    row[excelasTable.Columns.Count - 1] = rowNum - startRow + 1;
                    //Get everything as generics and let end user decides on casting to required type
                    
                }
                // Convert DataTable to the specified generic type using JSON serialization
                var data = JsonConvert.SerializeObject(excelasTable);
                var generatedType = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(excelasTable));
                return (T)Convert.ChangeType(generatedType, typeof(T));
            }
        }

        public static string ReadFromExcel(IFormFile file, bool hasHeader = true, int startColumn = 1, int startRow = 1)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPack = new ExcelPackage())
            {
                //Load excel stream
                using (var stream = file.OpenReadStream())
                {
                    excelPack.Load(stream);
                }

                //Lets Deal with first worksheet.(You may iterate here if dealing with multiple sheets)
                var ws = excelPack.Workbook.Worksheets[0];

                //Get all details as DataTable -because Datatable make life easy :)
                DataTable excelasTable = new DataTable();
                // startRow, startColumn : ô bắt đầu
                // startRow, ws.Dimension.End.Column: ô kết thúc
                // truy cập vào ô 1 hàng 1 đến ô cuối cùng của hàng 1
                foreach (var firstRowCell in ws.Cells[startRow, startColumn, startRow, ws.Dimension.End.Column])
                {
                    //Get colummn details
                    if (!string.IsNullOrEmpty(firstRowCell.Text))
                    {
                        string firstColumn = string.Format("Column {0}", firstRowCell.Start.Column);
                        excelasTable.Columns.Add(hasHeader ? firstRowCell.Text : firstColumn);
                    }
                }
                var startIndexRow = hasHeader ? startRow + 1 : startRow;
                //Get row details
                for (int rowNum = startIndexRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, startColumn, rowNum, excelasTable.Columns.Count];
                    DataRow row = excelasTable.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    // index hàng 
                    row[excelasTable.Columns.Count - 1] = rowNum - startRow + 1;
                    //Get everything as generics and let end user decides on casting to required type

                }
                return JsonConvert.SerializeObject(excelasTable);
            }
        }
    }
}
