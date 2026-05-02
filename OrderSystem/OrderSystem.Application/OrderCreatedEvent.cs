using System;
using System.Collections.Generic;
using System.Text;

namespace OrderSystem.Application
{
    public record OrderCreatedEvent(Guid OrderId);
}
