using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.Validations;

public class CurrentDateAttribute : ValidationAttribute
{
	public override bool IsValid(object? value)
	{
		if (value != null)
		{
			var date = (DateTime)value;
			return date >= DateTime.Now;
		}

		return false;
	}
}

