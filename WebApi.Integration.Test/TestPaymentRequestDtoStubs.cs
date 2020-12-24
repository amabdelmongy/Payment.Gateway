﻿using System;
using WebApi.Controllers.v1;
using WebApi.dto;

namespace WebApi.Integration.Test
{
    public static class PaymentDtoTests
    {
        public static readonly Guid TestAcquiringBankId = Guid.Parse("fc05a938-ac01-4090-aa1c-34721c1e3346");

        public static readonly PaymentRequestDto TestPaymentRequestDto = new PaymentRequestDto
        {
            Card = new CardDto
            {
                Number = "5105105105105100",
                Expiry = "8/22",
                Cvv = "123"
            },
            MerchantId = Guid.Parse("77d17eb6-a996-4375-bf1c-fb9808d95801"),
            Amount = new MoneyDto
            {
                Currency = "Euro",
                Value = 10.30
            }
        };
    }
}
