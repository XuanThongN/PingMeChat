using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PingMeChat.CMS.Application.Common.Attributes
{
    public class EmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string email = value.ToString();
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (Regex.IsMatch(email, pattern))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Định dạng email không hợp lệ.");
        }
    }

    public class VietnamPhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string phoneNumber = value.ToString();
                string pattern = @"^0\d{9}$";
                if (Regex.IsMatch(phoneNumber, pattern))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Định dạng số điện thoại không hợp lệ.");
        }
    }

    public class UsernameAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public UsernameAttribute(int minLength = 8, int maxLength = 225)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string username = value.ToString();

                // Kiểm tra độ dài của tên người dùng
                if (username.Length < _minLength || username.Length > _maxLength)
                {
                    return new ValidationResult($"Tên người dùng phải có độ dài từ {_minLength} đến {_maxLength} ký tự.");
                }

                // Kiểm tra tên người dùng chỉ chứa các ký tự chữ cái và số
                string pattern = @"^[a-zA-Z0-9]+$";
                if (Regex.IsMatch(username, pattern))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult("Tên người dùng phải có ít nhất 6 ký tự và không chứa ký tự đặc biệt.");
        }
    }

    public class PasswordAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public PasswordAttribute(int minLength = 8, int maxLength = 225)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string password = value.ToString();

                if (password.Length < _minLength || password.Length > _maxLength)
                {
                    return new ValidationResult($"Password must be between {_minLength} and {_maxLength} characters long.");
                }

                bool hasUpperCase = password.Any(char.IsUpper);
                bool hasLowerCase = password.Any(char.IsLower);
                bool hasDigit = password.Any(char.IsDigit);
                bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

                if (!hasUpperCase)
                    return new ValidationResult("Password must contain at least one uppercase letter.");
                if (!hasLowerCase)
                    return new ValidationResult("Password must contain at least one lowercase letter.");
                if (!hasDigit)
                    return new ValidationResult("Password must contain at least one digit.");
                if (!hasSpecialChar)
                    return new ValidationResult("Password must contain at least one special character.");

                return ValidationResult.Success;
            }
            return new ValidationResult("Password is required.");
        }
    }

    public class ConfirmPasswordAttribute : ValidationAttribute
    {
        private readonly string _passwordPropertyName;

        public ConfirmPasswordAttribute(string passwordPropertyName)
        {
            _passwordPropertyName = passwordPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var passwordProperty = validationContext.ObjectType.GetProperty(_passwordPropertyName);

            if (passwordProperty == null)
            {
                return new ValidationResult($"Không tìm thấy thuộc tính '{_passwordPropertyName}'.");
            }

            var passwordValue = passwordProperty.GetValue(validationContext.ObjectInstance, null) as string;
            var confirmPasswordValue = value as string;

            if (passwordValue != confirmPasswordValue)
            {
                return new ValidationResult("Mật khẩu và xác nhận mật khẩu không khớp.");
            }

            return ValidationResult.Success;
        }
    }

    // Trường linh hoạt
    public class UsernameOrEmailAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public UsernameOrEmailAttribute(int minLength = 8, int maxLength = 225)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string input = value.ToString();

                // Kiểm tra nếu là email hợp lệ
                if (IsValidEmail(input))
                {
                    return ValidationResult.Success;
                }

                // Kiểm tra nếu là username hợp lệ
                if (IsValidUsername(input))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("Giá trị phải là một tên người dùng hợp lệ hoặc một địa chỉ email hợp lệ.");
            }

            return new ValidationResult("Trường này không được để trống.");
        }

        // Kiểm tra email hợp lệ
        private bool IsValidEmail(string email)
        {
            try
            {
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Email pattern cơ bản
                return Regex.IsMatch(email, emailPattern);
            }
            catch
            {
                return false;
            }
        }

        // Kiểm tra username hợp lệ
        private bool IsValidUsername(string username)
        {
            if (username.Length < _minLength || username.Length > _maxLength)
            {
                return false;
            }

            // Username chỉ chứa chữ và số
            string pattern = @"^[a-zA-Z0-9]+$";
            return Regex.IsMatch(username, pattern);
        }
    }
}
