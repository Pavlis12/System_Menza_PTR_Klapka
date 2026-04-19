using System;
using System.Collections.Generic;
using System.Text;

namespace UTB.Minute.Contracts
{
    public enum OrderStatus
    {
        Preparing,
        ReadyToPickUp,
        PickedUp,
        Canceled
    }
}
