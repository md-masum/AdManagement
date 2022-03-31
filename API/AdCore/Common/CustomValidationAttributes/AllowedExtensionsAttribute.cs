using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AdCore.Common.CustomValidationAttributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"File extension is not allowed!";
        }
    }

    public class AllowedExtensionsListAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsListAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if (value is List<IFormFile> files)
                foreach (var file in files)
                {
                    
                    var extension = Path.GetExtension(file.FileName);
                    if (!_extensions.Contains(extension.ToLower()))
                    {
                        return new ValidationResult(GetErrorMessage());
                    }
                }


            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"File extension is not allowed!";
        }
    }
}
