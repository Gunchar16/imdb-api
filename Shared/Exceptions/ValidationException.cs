namespace Shared.Exceptions
{
    public class ValidationException:Exception
    { 
            public ValidationException() : base() { }

            public ValidationException(string obj) : base(obj + " Failed Validation"){
            
            }
        
    }
}
