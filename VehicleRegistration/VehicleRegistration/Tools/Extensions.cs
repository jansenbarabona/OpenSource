using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace VehicleRegistration.Tools
{
    public static class Extensions
    {
        public const int ImageMinimumBytes = 512;
        public static string Encrypt(this string input, string privateKey)
        {
            using (var algorithm = SHA256.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var saltedInput = new Byte[privateKey.Length + inputBytes.Length];

                Encoding.UTF8.GetBytes(privateKey).CopyTo(saltedInput, 0);
                inputBytes.CopyTo(saltedInput, privateKey.Length);

                var hashedBytes = algorithm.ComputeHash(saltedInput);

                return BitConverter.ToString(hashedBytes).Replace("-", "");
            }
        }
        public static byte[] ToByte(this HttpPostedFileBase File)
        {
            byte[] ByteArray = null;
            if (File != null)
            {
                using (Stream inputStream = File.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    ByteArray = memoryStream.ToArray();
                }
            }
            return ByteArray;
        }

        public static E ToModel<T, E>(this T model)
        {
            E entity = Activator.CreateInstance<E>();

            entity.GetType().GetProperties()
                .Where(o => typeof(T).GetProperties()
                    .Select(p => p.Name)
                    .Contains(o.Name))
                .ToList()
                .ForEach(o => o.SetValue(entity, typeof(T).GetProperty(o.Name).GetValue(model))
            );

            return entity;
        }

        public static DataTable ToDataTable<T>(this IList<T> data, string tableName = "")
        {
            System.ComponentModel.PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = string.IsNullOrEmpty(tableName) ? new DataTable() : new DataTable(tableName);
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public static bool IsValidFileSize(this HttpPostedFileBase file)
        {
            if (file == null)
                return false;
            try
            {
                if (file.ContentLength <= (1 * 1024 * 1024))
                {
                    return true;
                }
                else if (file != null)
                {
                    return false;
                }
            }
            catch
            {
            }
            return false;
        }
        public static bool IsAllowedContentType(this HttpPostedFileBase file)
        {
            if (file == null)
                return false;
            try
            {
                string[] extensions = new string[] { "image/jpg", "image/jpeg", "image/png", "application/pdf" };
                if (extensions.Contains(file.ContentType) && file != null)
                {
                    return true;
                }
                else if (file != null)
                {
                    return false;
                }
            }
            catch
            {
            }
            return false;
        }
        public static bool IsImage(this HttpPostedFileBase postedFile)
        {
            if (postedFile == null)
                return false;
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (!string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".pdf", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }
                //------------------------------------------
                //   Check whether the image size exceeding the limit or not
                //------------------------------------------ 
                if (postedFile.ContentLength < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                postedFile.InputStream.Read(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            //try
            //{
            //    using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
            //    {
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            finally
            {
                postedFile.InputStream.Position = 0;
            }

            return true;
        }

        public static DataTable FormatDataTable(this DataTable TableToFormat)
        {
            DataTable NewDataTable = new DataTable();

            for (int i = 0; i < TableToFormat.Columns.Count; i++)
            {
                NewDataTable.Columns.Add(TableToFormat.Rows[0][i].ToString());
            }

            int rowcounter = 0;
            for (int row_ = 1; row_ < TableToFormat.Rows.Count; row_++)
            {
                var row = NewDataTable.NewRow();

                for (int col = 0; col < TableToFormat.Columns.Count; col++)
                {
                    row[col] = TableToFormat.Rows[row_][col].ToString();
                    rowcounter++;
                }
                NewDataTable.Rows.Add(row);
            }

            return NewDataTable;
        }

        public static string convertContentTypeToExtention(this string ContentType)
        {
            if (ContentType == null)
                return "";
            try
            {
                switch (ContentType.Trim())
                {
                    case "image/jpeg":
                        return ".jpeg";
                    case "image/jpg":
                        return ".jpg";
                    case "image/png":
                        return ".png";
                    case "application/pdf":
                        return ".pdf";
                    default:
                        return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public static int BusinessDaysUntil(this DateTime firstDay, DateTime lastDay, params DateTime[] bankHolidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            foreach (DateTime bankHoliday in bankHolidays)
            {
                DateTime bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }
    }
}