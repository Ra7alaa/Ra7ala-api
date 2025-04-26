using System.Collections.Generic;

namespace Presentation.Models
{
    public class ApiValidationErrorResponse : ApiResponse
	{
        public IEnumerable<string> Errors { get; set; }

        public ApiValidationErrorResponse() 
			: base(400)
		{
			Errors = new List<string>();
		}
	}
}