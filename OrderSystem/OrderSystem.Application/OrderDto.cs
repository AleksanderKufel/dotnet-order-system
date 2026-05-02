using System;
using System.Collections.Generic;
using System.Text;

namespace OrderSystem.Application
{
    public record OrderDto(Guid Id, string CustomerEmail, decimal Amount, string Status);
}
