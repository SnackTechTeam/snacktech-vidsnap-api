namespace Vidsnap.Domain.Guards
{
    public static class CommonGuards
    {
        public static Guid AgainstEmptyGuid(Guid value, string paramName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{paramName} não pode ser um GUID vazio.", paramName);

            return value;
        }

        public static string AgainstNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} não pode ser nulo ou vazio.", paramName);

            return value;
        }

        public static string AgainstInvalidEmail(string email, string paramName)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException($"{paramName} não é um e-mail válido.", paramName);

            return email;
        }

        public static int AgainstNegativeOrZero(int value, string paramName)
        {
            if (value <= 0)
                throw new ArgumentException($"{paramName} deve ser maior que zero.", paramName);

            return value;
        }

        public static TEnum AgainstEnumOutOfRange<TEnum>(TEnum value, string paramName) where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new ArgumentException($"{paramName} contém um valor inválido para o enum {typeof(TEnum).Name}.", paramName);

            return value;
        }

        public static object AgainstNull(object value, string paramName)
        {
            if (value is null)
                throw new ArgumentNullException($"{paramName} não pode ser nulo.", paramName);

            return value;
        }

        public static string AgainstInvalidUrl(string url, string paramName)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
                uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException($"{paramName} não é uma URL válida.", paramName);
            }

            return url;
        }
    }
}
