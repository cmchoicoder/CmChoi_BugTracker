using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace CmChoi_BugTracker.Helpers
{
    public class ImageHelpers
    {
        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file == null)
                return false;
            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                return false;

            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                    ImageFormat.Png.Equals(img.RawFormat) ||
                    ImageFormat.Gif.Equals(img.RawFormat) ||
                    ImageFormat.Icon.Equals(img.RawFormat) ||
                    ImageFormat.Bmp.Equals(img.RawFormat);
                }
            }
            catch
            {
                return false;
            }
        }


        public static bool IsValidAttachment(HttpPostedFileBase file)
        {
            try
            {
                if (file == null)
                    return false;
                if (file.ContentLength > 5 * 1024 * 1024 || file.ContentLength < 1024)
                    return false;

                var extValid = false;
                foreach (var ext in WebConfigurationManager.AppSettings["AllowedAttachmentExtensions"].Split(','))
                {
                    if (Path.GetExtension(file.FileName) == ext)
                    {
                        extValid = true;
                        break;

                    }
                }

                return IsWebFriendlyImage(file) || extValid;
            }
            catch
            {
                return false;
            }
        }

        public static string GetIconPath(string filePath)
        {
            switch (Path.GetExtension(filePath))
            {
                case ".tif":
                case ".ico":
                    return filePath;
                    
                case ".pdf":
                    return "/Images/pdf.png";
                case ".bmp":
                    return "/Images/bmp.png";
                case ".docx":
                    return "/Images/docx.png";
                case ".gif":
                    return "/Images/gif.png";
                case ".jpeg":
                    return "/Images/jpeg.png";
                case ".jpg":
                    return "/Images/jpg.png";
                case ".png":
                    return "/Images/png.png";
                case ".pptx":
                    return "/Images/pptx.png";
                case ".rar":
                    return "/Images/rar.png";
                case ".zip":
                    return "/Images/zip.png";
                case ".text":
                    return "/Images/text.png";
                case ".xls":
                    return "/Images/xls.png";
                default:
                    return "/Images/other.png";
            }
        }
    }
}