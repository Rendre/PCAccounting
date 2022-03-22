using DB.Entities;

namespace WebClient.Models;
//todo: T
public class ResultModel
{
    public byte Success { get; set; }

    public Computer? Computer { get; set; }
}