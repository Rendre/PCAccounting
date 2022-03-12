namespace DB;

public enum StatusEnum
{
    Null = 0,
    UnderRepair = 1,
    Properly = 2,
    Defective = 3,
}

public enum TaskType
{
    None,
    Auth = 1,
}

public enum TaskStatus
{
    None,
    InActive = 1,
    Active = 2,
    Done = 3,
}