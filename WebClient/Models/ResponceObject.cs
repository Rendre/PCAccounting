namespace WebClient.Models;

public class ResponceObject<T>
{
    public byte Success { get; set; } = 0;

    public byte? Access { get; set; } = null;

    public T? Data { get; set; }

    public List<T>? DataList {get; set; }
}

//если все ок - то access = null