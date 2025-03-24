using MunModel;

public class RegisterViewModel
{
    // Campos que o formulário precisa
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Cpf { get; set; }
    public string? MunPR { get; set; }
    public string? Occupation { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    // Lista de municípios apenas para exibir no <select>
    public List<MunicipiosList> Municipios { get; set; } = new();
}