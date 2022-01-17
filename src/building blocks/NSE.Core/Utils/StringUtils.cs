namespace NSE.Core.Utils;

public static class StringUtils
{
    //Método de extensão para retornar somente número
    public static string ApenasNumeros(this string str, string input)
    {
        return new string(input.Where(char.IsDigit).ToArray());
    }
}