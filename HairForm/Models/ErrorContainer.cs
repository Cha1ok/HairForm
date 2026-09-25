namespace HairForm.Models
{
    public class ErrorContainer
    {
        public string ErrorText { get; set; }
        public ErrorContainer() { };
        public ErrorContainer(string errorText)
        {
            ErrorText = errorText;
        }
    }
}
