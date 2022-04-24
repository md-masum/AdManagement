using AdCore.Entity;
using AdCore.Enums;
using AdCore.Exceptions;

namespace AdCore.Extensions
{
    public static class ExtensionMethod
    {
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static void UpdateObjectWithoutId<TFirst, TSecond>(this TFirst obj1, TSecond obj2)
        {
            foreach (var property in obj2?.GetType().GetProperties()!)
            {
                if (property.Name == nameof(BaseEntity.Id)) continue;
                obj1?.GetType().GetProperty(property.Name)?.SetValue(obj1, property.GetValue(obj2));
            }

        }
        public static void UpdateObject<TFirst, TSecond>(this TFirst obj1, TSecond obj2)
        {
            foreach (var property in obj2?.GetType().GetProperties()!)
                obj1?.GetType().GetProperty(property.Name)?.SetValue(obj1, property.GetValue(obj2));
        }

        public static FileTypes GetFileType(this string fileType)
        {
            return fileType.ToUpper() switch
            {
                ".GIF" => FileTypes.Image,
                ".JPG" => FileTypes.Image,
                ".JPEG" => FileTypes.Image,
                ".SVG" => FileTypes.Image,
                ".PNG" => FileTypes.Image,
                ".PDF" => FileTypes.Pdf,
                ".DOC" => FileTypes.Document,
                ".DOCX" => FileTypes.Document,
                ".XLS" => FileTypes.Document,
                ".XLSX" => FileTypes.Document,
                ".PPT" => FileTypes.Document,
                ".PPTX" => FileTypes.Document,
                ".TXT" => FileTypes.Document,
                ".MKV" => FileTypes.Video,
                ".AVI" => FileTypes.Video,
                ".WMV" => FileTypes.Video,
                ".MP4" => FileTypes.Video,
                ".3GP" => FileTypes.Video,
                ".FLV" => FileTypes.Video,
                _ => throw new CustomException("Invalid file format.")
            };
        }

        public static string GetContainerName(this string fileType)
        {
            return fileType.ToUpper() switch
            {
                ".GIF" => "image-container",
                ".JPG" => "image-container",
                ".JPEG" => "image-container",
                ".SVG" => "image-container",
                ".PNG" => "image-container",
                ".PDF" => "pdf-container",
                ".DOC" => "file-container",
                ".DOCX" => "file-container",
                ".XLS" => "file-container",
                ".XLSX" => "file-container",
                ".PPT" => "file-container",
                ".PPTX" => "file-container",
                ".TXT" => "file-container",
                ".MKV" => "video-container",
                ".AVI" => "video-container",
                ".WMV" => "video-container",
                ".MP4" => "video-container",
                ".3GP" => "video-container",
                ".FLV" => "video-container",
                _ => throw new CustomException("Invalid file format.")
            };
        }

        public static string GetContainerName(this FileTypes fileType)
        {
            return fileType switch
            {
                FileTypes.Image => "image-container",
                FileTypes.Pdf => "pdf-container",
                FileTypes.Document => "file-container",
                FileTypes.Video => "video-container",
                _ => throw new CustomException("Invalid file type.")
            };
        }

        public static string GetFileExtension(this string fileName)
        {
            return "." + fileName.Split('.')[fileName.Split('.').Length - 1];
        }
    }
}
