namespace _2024_2C_SushiPOP_G6.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public const string CaracteresMaximos = "{0} debe tener como maximo {1} caracteres.";
        public const string CaracteresMinimos = "{0} debe tener al menos {1} caracteres.";
        public const string ValorMinMax = "{0} debe tener un valor entre {1} y {2}.";
        public const string CampoRequerido = "{0} es un campo obligatorio.";
    }
}
