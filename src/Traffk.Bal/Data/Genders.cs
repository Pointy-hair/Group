namespace Traffk.Bal.Data
{
    public static class Genders
    {
        public const string Male = "Male";
        private static readonly string MaleLower = Male.ToLower();
        public const string Female = "Female";
        private static readonly string FemaleLower = Female.ToLower();

        public static bool IsMale(string gender)
        {
            gender = (gender ?? "").ToLower();
            return gender == "m" || gender == MaleLower;
        }

        public static bool IsFemale(string gender)
        {
            gender = (gender ?? "").ToLower();
            return gender == "f" || gender == FemaleLower;
        }
    }
}
