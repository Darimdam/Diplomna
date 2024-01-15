using System.Windows.Controls;

namespace Diplomna.Resources.Validations
{
    public class TextLengthValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value.ToString().Length > 2)
                return new ValidationResult(true, null);

            return new ValidationResult(false, "Попълнете полето с повече от 2 символа");
        }
    }
}
