using DB.Entities;

namespace WebClient.Models;
//todo:
public class ResultModelBuilder
{
    private readonly ResultModel _resultModel;

    public ResultModelBuilder(bool isSuccess)
    {
        _resultModel = new ResultModel
        {
            Success = isSuccess ? (byte)1 : (byte)0
        };
    }

    //public ResultModelBuilder WithSuccess(bool isSuccess)
    //{
    //    _resultModel.Success = isSuccess ? (byte)1 : (byte)0;
    //    return this;
    //}

    public ResultModelBuilder WithComputer(Computer computer)
    {
        _resultModel.Computer = computer;
        return this;
    }

    public ResultModel Build()
    {
        return _resultModel;
    }
}