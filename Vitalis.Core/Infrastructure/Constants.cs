namespace Vitalis.Core.Infrastructure
{
    public static class Constants
    {
        public const string GetPossibleReactantsPrompt =
            "output all the possible reactions for the given SMILES compound: ";

        public const int TestTitleMinLength = 4;
        public const int TestTitleMaxLength = 30;
        public const int DescriptionMaxLength = 1000;
        public const int EmailMaxLength = 60;
        public const int EmailMinLength = 5;
        public const int PasswordMaxLength = 30;
        public const int PasswordMinLength = 8;
        public const int NameMaxLength = 50;
        public const int NameMinLength = 3;

        public const string TestsCacheKey = "Tests";
        public const char SeparationCharacter = '&';
        public const string NotFoundResponse = "Not found";
        public const string SuccessResponse = "success";
    }
}
