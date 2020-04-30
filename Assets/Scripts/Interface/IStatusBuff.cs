using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusBuff
{
    CharactorStatus AddAmount { get; }
    CharactorStatus MultiAmount { get; }

}
