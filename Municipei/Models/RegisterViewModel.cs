using MunModel;

public class RegisterViewModel
{
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Cpf { get; set; }
    public string? MunPR { get; set; }
    public string? Occupation { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public List<MunicipiosList> Municipios { get; set; } = new();
}