namespace Model;

public class Customer
{
    public required long Id { get; set; }
    public required string Company { get; set; }
    public required bool Active { get; set; }

    public override string ToString()
    {
        return $"{{\n\tId:{Id},\n\tCompany:{Company}\n\tActive:{Active}\n}}";
    }
}