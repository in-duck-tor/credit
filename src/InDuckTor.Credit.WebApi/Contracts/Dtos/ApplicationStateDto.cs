using System.Runtime.Serialization;

namespace InDuckTor.Credit.WebApi.Contracts.Dtos;

public enum ApplicationStateDto
{
    [EnumMember] Rejected,
    [EnumMember] Pending,
    [EnumMember] Approved
}