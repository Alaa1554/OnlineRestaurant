
using System.ComponentModel.DataAnnotations;

namespace OnlineRestaurant.Helpers
{
    public class ValidateHelper<T> where T : class
    {
        public static string Validate(T model)
        {
            var modelValidationContext = new ValidationContext(model);
            var validationResult = new List<ValidationResult>();
            bool IsValid = Validator.TryValidateObject(model, modelValidationContext,validationResult,true);
             
            if (!IsValid)
            {
                var errors = validationResult.Select(x => x.ErrorMessage).ToList();
                var errormessage = string.Join(",",errors);
                return errormessage;

            }
            return null;
        }
       
    }
}
