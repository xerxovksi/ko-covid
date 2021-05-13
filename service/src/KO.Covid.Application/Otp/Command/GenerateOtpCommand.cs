﻿namespace KO.Covid.Application.Otp
{
    using KO.Covid.Application.Models;
    using MediatR;

    public class GenerateOtpCommand : IRequest<bool>
    {
        public string Mobile { get; set; }
    }
}
