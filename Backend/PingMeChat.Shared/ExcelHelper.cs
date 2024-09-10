using Microsoft.VisualBasic;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared
{
    public static class ExcelHelper
    {
        public static DataTable ReadExcelFile(Stream stream, string fileType)
        {
            var excelToDataTable = new DataTable();
            try
            {
                //The Workbook object represents a workbook, first define an Excel workbook
                IWorkbook workbook;

                //XSSFWorkbook is suitable for XLSX format, HSSFWorkbook is suitable for XLS format
                #region Determine the Excel version
                switch (fileType)
                {
                    case ".xlsx":
                        workbook = new XSSFWorkbook(stream);
                        break;
                    case ".xls":
                        workbook = new XSSFWorkbook(stream);
                        break;
                    default:
                        throw new Exception("Excel not valid");
                }
                #endregion

                //Check if sheet content is null
                for (int numberSheet = 0; numberSheet < workbook.NumberOfSheets; numberSheet++)
                {
                    if (excelToDataTable.Rows.Count <= 0)
                    {
                        var sheet = workbook.GetSheetAt(numberSheet);
                        var rows = sheet.GetRowEnumerator();

                        excelToDataTable.Rows.Clear();
                        excelToDataTable.Columns.Clear();

                        var headerRow = sheet.GetRow(0);
                        int cellCount = headerRow != null ? headerRow.LastCellNum : 0;//The number of columns in the last row (that is, the total number of columns)

                        //Get the first row header column data source, and convert it to the table header name of the dataTable data source

                        for (var j = 0; j < cellCount; j++)
                        {
                            var cell = headerRow.GetCell(j);

                            excelToDataTable.Columns.Add(cell.ToString());
                        }

                        //Get all the data sources in the Excel table except the title, and convert them to the table data sources in the dataTable
                        for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                        {
                            var dataRow = excelToDataTable.NewRow();

                            var row = sheet.GetRow(i);

                            if (row == null || row.Cells.Count <= 0) continue; //Rows without data are null by default

                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null)//Cell content is not empty verification
                                {
                                    #region NPOI gets different types of data in Excel cells
                                    //Get the specified cell information
                                    var cell = row.GetCell(j);
                                    switch (cell.CellType)
                                    {
                                        //Numeric type
                                        case CellType.Numeric when DateUtil.IsCellDateFormatted(cell):
                                            dataRow[j] = cell.DateCellValue;
                                            break;
                                        case CellType.Numeric:
                                            //Other number types
                                            dataRow[j] = cell.NumericCellValue;
                                            break;
                                        //Null data type
                                        case CellType.Blank:
                                            dataRow[j] = string.Empty;
                                            break;
                                        //Formula type
                                        case CellType.Formula:
                                            {
                                                cell.SetCellType(CellType.String);
                                                dataRow[j] = cell.StringCellValue;
                                                break;
                                            }
                                        //Boolean type
                                        case CellType.Boolean:
                                            dataRow[j] = row.GetCell(j).BooleanCellValue;
                                            break;
                                        //Other types are handled as string types (unknown type CellType.Unknown, string type CellType.String
                                        default:
                                            dataRow[j] = cell.StringCellValue;
                                            break;
                                    }
                                    #endregion
                                }
                            }
                            excelToDataTable.Rows.Add(dataRow);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return TrimEmptyRecords(excelToDataTable);
        }

        private static DataTable TrimEmptyRecords(DataTable dataTable)
        {
            DataTable dtClean = dataTable.Clone();
            try
            {
                for (int j = 0; j < dataTable.Rows.Count; j++)
                {

                    bool blankRecord = false;
                    int emptyCount = 0;
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        if (dataTable.Rows[j][i].ToString() == string.Empty)
                        {
                            emptyCount = emptyCount + 1;
                            if (emptyCount == dataTable.Columns.Count)
                            {
                                blankRecord = true;
                            }
                        }
                        else
                        {
                            break;
                        }

                    }
                    if (!blankRecord)
                    {
                        dtClean.ImportRow(dataTable.Rows[j]);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtClean;
        }

        public static DataTable ConvertToDataTable(Stream stream)
        {
            DataTable tbl = new DataTable();
            tbl.TableName = "UploadFileText";
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);

            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            string[] stringSeparators = new string[] { "\r\n" };
            string[] lines = text.Split(stringSeparators, StringSplitOptions.None);
            DataRow dr;
            tbl.Columns.Add("Record");
            foreach (string line in lines)
            {
                dr = tbl.NewRow();
                dr[0] = line;
                tbl.Rows.Add(dr);
            }

            return tbl;
        }
    }
}
