using System.Runtime.Serialization;

namespace InDuckTor.Credit.Domain.LoanManagement.State;

/// <summary>
/// <b>Статус Кредита</b>
/// </summary>
public enum LoanState
{
    /// <summary>
    /// Кредит одобрен, но деньги не переведены на счёт заёмщика
    /// </summary>
    [EnumMember(Value = "approved")] Approved,

    /// <summary>
    /// Кредит одобрен и деньги переведены на счёт заёмщика
    /// </summary>
    [EnumMember(Value = "active")] Active,

    /// <summary>
    /// Кредит погашен
    /// </summary>
    [EnumMember(Value = "closed")] Closed,

    /// <summary>
    /// Клиент ахуел. Он очень сильно ахуел. Нельзя наёбывать банк, но эта дрянь попыталась это сделать.
    /// Теперь с ним будут разбираться коллекторы. И поверьте, он пожалеет о том, что решил не платить вовремя...
    /// </summary>
    [EnumMember(Value = "ahuel")] Sold
}