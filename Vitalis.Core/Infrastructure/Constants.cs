namespace Vitalis.Core.Infrastructure
{
    public static class Constants
    {
        public const string GetPossibleReactantsPrompt =
            "output all the possible reactions for the given SMILES compound: ";

        public const int TestTitleMinLength = 5;
        public const int TestTitleMaxLength = 30;
        public const int DescriptionMaxLength = 1000;
        public const string TestsCacheKey = "Tests";
        public const char SeparationCharacter = '&';
        public const string NotFoundResponse = "Not found";
        public const string SuccessResponse = "success";
    }
}
