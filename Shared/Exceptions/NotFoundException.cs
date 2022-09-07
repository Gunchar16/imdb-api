namespace Shared.Exceptions
{
    public class NotFoundException : Exception
    {
         public NotFoundException() : base() { }

         public NotFoundException(string obj) : base(obj + " Not Found"){
         
         }   
    }
}
