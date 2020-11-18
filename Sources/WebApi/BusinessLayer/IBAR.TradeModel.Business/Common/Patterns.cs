namespace IBAR.ServiceLayer.Common
{
    public static class Patterns
    {
        public const string EmailPattern = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$";
        public const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W+).{9,15}$";
        public const string PhonePattern = @"^\+[0-9]+$";
        public const string NamePattern = @"^[a-zA-Z]+$";
        public const string AccountName = @"^[A-Za-z]{1}\d{5,7}$";
        public const string PhoneCodePattern = @"^\d{6}$";
    }
}